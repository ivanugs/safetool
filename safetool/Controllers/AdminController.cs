using Microsoft.AspNetCore.Mvc;

namespace safetool.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
