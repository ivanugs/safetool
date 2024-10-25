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

        [HttpPost]
        public async Task<IActionResult> SendEmailSuccess(string subject, int deviceID)
        {
            var device = await _context.Devices.FindAsync(deviceID);
            var fullName = @User.Claims.FirstOrDefault(c => c.Type == "FullName")?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            var deviceName = device.Name;

            string body = $@"
            <!DOCTYPE html>
                <html lang='es'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Registro Exitoso</title>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            margin: 0;
                            padding: 20px;
                        }}
                        .container {{
                            max-width: 600px;
                            margin: 0 auto;
                            padding: 20px;
                            border-radius: 8px;
                            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
                        }}
                        h1 {{
                            color: #333;
                            font-size: 20px;
                        }}
                        p {{
                            color: #555;
                            font-size: 16px;
                            line-height: 1.5;
                        }}
                        strong {{
                            color: #333;
                        }}
                        .footer {{
                            margin-top: 20px;
                            font-size: 14px;
                            color: #777;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <h1>{fullName},</h1>
                        <p>
                            Usted ha realizado exitosamente el registro del equipo: <strong>{deviceName}</strong>. <br> Su registro cuenta con una vigencia de 6 meses.
                        </p>
                        <p>Gracias por su atención.</p>
                        <div class='footer'>
                            <p>Saludos cordiales,<br>Equipo de ESH</p>
                        </div>
                    </div>
                </body>
                </html>";

            await _emailService.SendEmailAsync(email, subject, body);

            return RedirectToAction("Details", "Devices", new { id = deviceID });
        }
    }
}
