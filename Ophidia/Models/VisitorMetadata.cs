using System;

namespace Ophidia.Models
{
    public class VisitorMetadata
    {
        public int Id { get; set; }
        public string Path { get; set; } = "";
        public string Method { get; set; } = "";
        public string UserAgent { get; set; } = "";
        public string Referrer { get; set; } = "";        
        public string DeviceType { get; set; } = "";        
        public string ScreenResolution { get; set; } = "";
        public int StatusCode { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
