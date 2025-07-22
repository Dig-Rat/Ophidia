using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Ophidia.Models;
using Ophidia.Services;

namespace Ophidia.Middleware
{
    /// <summary>
    /// Middleware to capture and log basic, non-identifiable metadata about each HTTP request.
    /// This includes route, method, user-agent, referrer, and inferred device type.
    /// </summary>
    public class VisitorLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Constructs the middleware with the next delegate in the pipeline.
        /// </summary>
        /// <param name="next">The next RequestDelegate to call after this middleware executes.</param>
        public VisitorLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Intercepts each request, extracts relevant metadata, and passes it to the MetadataLogger.
        /// Continues request processing afterward.
        /// </summary>
        /// <param name="context">The current HTTP request context.</param>
        /// <param name="logger">A MetadataLogger instance used to save metadata to the database.</param>
        public async Task InvokeAsync(HttpContext context, MetadataLogger logger)
        {
            VisitorMetadata metadata = new VisitorMetadata
            {
                Path = context.Request.Path,
                Method = context.Request.Method,
                UserAgent = context.Request.Headers["User-Agent"].ToString(),
                Referrer = context.Request.Headers["Referer"].ToString() ?? "None",
                DeviceType = ParseDeviceType(context.Request.Headers["User-Agent"].ToString()),
                ScreenResolution = "Unknown", // JavaScript needed to fill this in later
                Timestamp = DateTime.UtcNow
            };

            logger.Log(metadata);

            await _next(context);
        }

        /// <summary>
        /// Parses the user-agent string to infer the type of device (Desktop, Mobile, or Tablet).
        /// </summary>
        /// <param name="userAgent">The user-agent header string from the HTTP request.</param>
        /// <returns>A string representing the inferred device type.</returns>
        private string ParseDeviceType(string userAgent)
        {
            string device = "Desktop";

            if (!string.IsNullOrEmpty(userAgent))
            {
                if (userAgent.Contains("Mobi"))
                {
                    device = "Mobile";
                }
                else if (userAgent.Contains("Tablet"))
                {
                    device = "Tablet";
                }
            }

            return device;
        }
    }
}
