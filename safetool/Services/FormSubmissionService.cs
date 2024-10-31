using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using safetool.Data;
using safetool.Models;
using System.Diagnostics.Metrics;

namespace safetool.Services
{
    public class FormSubmissionService
    {
        private readonly SafetoolContext _context;
        private readonly IEmailService _emailService;
        private readonly AppSettings _appSettings;

        public FormSubmissionService(SafetoolContext context, IEmailService emailService, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _emailService = emailService;
            _appSettings = appSettings.Value;
        }

        public async Task CheckAndNotifyExpiredRegistrations()
        {
            //Acceder a la variable MonthsSubmissionsValidity
            int MonthsSubmissionsValidity = _appSettings.MonthsSubmissionsValidity;

            //Acceder a la variable SystemUrl
            string SystemUrl = _appSettings.SystemUrl;

            // Obtener los registros que tienen más de 6 meses y que aún no han sido notificados
            var expiredSubmissions = await _context.FormSubmissions
                .Include(f => f.Device)
                .Where(f => f.CreatedAt.AddMonths(MonthsSubmissionsValidity) <= DateTime.Now)
                .ToListAsync();

            foreach (var submission in expiredSubmissions)
            {
                // Enviar el correo notificando al usuario
                var userName = submission.EmployeeName;
                var userEmail = submission.EmployeeEmail;
                var device = submission.Device.Name;
                var deviceID = submission.Device.ID;

                string subject = "Safetool - Tu registro ha vencido";
                string body = $@"
                <!DOCTYPE html>
                <html lang='es'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>Notificación de Registro Vencido</title>
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
                        a {{
                            color: #007BFF;
                            text-decoration: none;
                        }}
                        a:hover {{
                            text-decoration: underline;
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
                        <h1>Estimado {userName},</h1>
                        <p>
                            Tu registro para el equipo <strong>'{device}'</strong> ha vencido.
                        </p>
                        <p>
                            Te invitamos a registrarte nuevamente en el siguiente enlace: 
                            <a href='{SystemUrl}Devices/Details?id={deviceID}'>{device}</a>.
                        </p>
                        <p>Gracias por su atención.</p>
                        <div class='footer'>
                            <p>Saludos cordiales,<br>Equipo de ESH</p>
                        </div>
                    </div>
                </body>
                </html>";

                // Enviar correo
                await _emailService.SendEmailAsync(userEmail, subject, body);
            }

        }
    }
}
