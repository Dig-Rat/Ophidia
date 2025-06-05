using Microsoft.AspNetCore.Mvc;
using Ophidia.Services;
using Ophidia.Models;

namespace Ophidia.Controllers
{
    public class UsersController : Controller
    {        
        private readonly UserRepository _repo;

        // --
        public UsersController(UserRepository repo)
        {
            _repo = repo;
        }

        // --
        public IActionResult Index()
        {
            IEnumerable<User> users = _repo.GetAllUsers();
            return View(users);
        }

        // --
        [HttpPost]
        public IActionResult Create(string username)
        {
            _repo.AddUser(username);
            return RedirectToAction("Index");
        }

    }
}
