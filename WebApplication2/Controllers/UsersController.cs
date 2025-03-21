using Microsoft.AspNetCore.Mvc;

namespace WebApplication2.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            return View("Users"); 
        }

        
        public IActionResult Users()
        {
            return View();
        }
        public IActionResult Profile()
        {
            return View();
        }
    }
}
