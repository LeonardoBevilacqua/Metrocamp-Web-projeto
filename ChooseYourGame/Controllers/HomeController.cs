using Microsoft.AspNetCore.Mvc;

namespace ChooseYourGame.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Content("Hello there!", "text/html");
        }
    }
}