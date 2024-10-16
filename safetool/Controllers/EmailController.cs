using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using safetool.Data;
using safetool.Models;
using safetool.Services;
using System.Security.Claims;

namespace safetool.Controllers
{
    public class EmailController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly SafetoolContext _context;

        
        public EmailController(IEmailService emailService, SafetoolContext context)
        {
            _emailService = emailService;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail(string subject, string body, int deviceID)
        {
            var device = await _context.Devices.FindAsync(deviceID);

            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            await _emailService.SendEmailAsync(email, subject, body);

            return RedirectToAction("Details", "Devices", new { id = deviceID });
        }
    }
}
