using Microsoft.AspNetCore.Mvc;

namespace ChessPlatform.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}