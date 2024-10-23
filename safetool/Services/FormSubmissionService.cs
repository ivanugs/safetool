using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using safetool.Data;
using System.Diagnostics.Metrics;

namespace safetool.Services
{
    public class FormSubmissionService
    {
        private readonly SafetoolContext _context;
        private readonly IEmailService _emailService;

        public FormSubmissionService(SafetoolContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task CheckAndNotifyExpiredRegistrations()
        {
            // Obtener los registros que tienen más de 6 meses y que aún no han sido notificados
            var expiredSubmissions = await _context.FormSubmissions
                .Include(f => f.Device)
                .Where(f => f.CreatedAt.AddDays(6) <= DateTime.Now)
                .ToListAsync();

            foreach (var submission in expiredSubmissions)
            {
                // Enviar el correo notificando al usuario
                var userName = submission.EmployeeName;
                var userEmail = submission.EmployeeEmail;
                var device = submission.Device.Name;
                var deviceID = submission.Device.ID;

                string subject = "Safetool - Tu registro ha vencido";
                string body = $"Estimado {userName}, <br>" +
                    $" Tu registro para el equipo '{device}' ha vencido. <br>" +
                    $"Te invitamos a registrarte nuevamente en el siguiente enlace <a href='{deviceID}'>Safetool</a>." +
                    $"<br><br>Gracias por su atención.<br>Saludos cordiales,<br>Equipo de ESH";

                // Enviar correo
                await _emailService.SendEmailAsync(userEmail, subject, body);

            }
        }
    }
}
