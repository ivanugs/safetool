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
                .Where(f => f.CreatedAt.AddMinutes(10) <= DateTime.Now)
                .ToListAsync();

            foreach (var submission in expiredSubmissions)
            {
                // Enviar el correo notificando al usuario
                var userName = submission.EmployeeName;
                var userEmail = submission.EmployeeEmail;
                var device = submission.Device.Name;

                string subject = "Tu registro ha vencido";
                string body = $"Hola {userName},\n\nTu registro para el equipo '{device}' ha vencido. Te invitamos a registrarte nuevamente.";

                // Enviar correo
                await _emailService.SendEmailAsync(userEmail, subject, body);

            }
        }
    }
}
