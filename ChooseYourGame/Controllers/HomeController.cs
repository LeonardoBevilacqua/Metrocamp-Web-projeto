using Microsoft.AspNetCore.Mvc;

namespace ChooseYourGame.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Main()
        {
            return View();
        }
    }
}