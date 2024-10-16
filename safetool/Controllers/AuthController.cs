using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using safetool.Services;

namespace safetool.Controllers
{
    public class AuthController : Controller
    {
        private readonly LdapAuthentication _ldapAuth;
        private readonly RoleService _roleService;

        // Inyectamos el servicio LdapAuthentication y RoleService a través del constructor
        public AuthController(LdapAuthentication ldapAuth, RoleService roleService)
        {
            _ldapAuth = ldapAuth;
            _roleService = roleService;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(string uid, string password)
        {
            var user = _ldapAuth.AuthenticateAndGetUser(uid, password);

            if (user != null)
            {
                var roles = _roleService.GetRolesForUser(uid);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Uid),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("FullName", user.FullName),
                    new Claim("FirstName", user.FirstName),
                    new Claim("LastName", user.LastName)
                };

                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = "Autenticación fallida";
            return View();
        }



        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
