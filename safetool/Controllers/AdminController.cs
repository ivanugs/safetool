using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace safetool.Controllers
{
    [Authorize(Roles = "Administrador, Operador")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
