using Microsoft.AspNetCore.Mvc;
using Ophidia.Services;
using Ophidia.Models;

namespace Ophidia.Controllers
{
    /// <summary>
    /// Controller responsible for managing user-related HTTP requests.
    /// </summary>
    public class UsersController : Controller
    {
        private readonly UserRepository _repo;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class.
        /// </summary>
        /// <param name="repo">An injected instance of <see cref="UserRepository"/> used to access user data.</param>
        public UsersController(UserRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Displays a list of all users.
        /// </summary>
        /// <returns>A view that renders the list of users.</returns>
        public IActionResult Index()
        {
            // Retrieve all users from the repository
            IEnumerable<User> users = _repo.GetAllUsers();

            // Pass the users to the view for rendering
            ViewResult vr = View(users);
            return vr;
        }

        /// <summary>
        /// Creates a new user with the specified username.
        /// </summary>
        /// <param name="username">The username of the user to add.</param>
        /// <returns>Redirects to the Index action to display the updated user list.</returns>
        [HttpPost]
        public IActionResult Create(string username)
        {
            // Add the user to the repository
            _repo.AddUser(username);

            // Redirect to the Index page to show updated list
            RedirectToActionResult rtar = RedirectToAction("Index");
            return rtar;
        }
    }
}
