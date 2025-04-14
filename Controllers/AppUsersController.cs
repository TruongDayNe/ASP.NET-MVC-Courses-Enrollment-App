 using Microsoft.AspNetCore.Mvc;

namespace MVCWebApp.Controllers
{
    public class AppUsersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
