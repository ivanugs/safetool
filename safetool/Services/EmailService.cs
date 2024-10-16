using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using safetool.Models;
using System.Threading.Tasks;

namespace safetool.Services
{
    public class EmailService : IEmailService
    {
        private readonly GeneralParameterService _parameterService;

        public EmailService(GeneralParameterService parameterService)
        {
            _parameterService = parameterService;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var _emailSettings = _parameterService.GetEmailSettings();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.EmailAccountDisplayName, _emailSettings.EmailAccount));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;
;
            message.Body = new TextPart("html") { Text = body};

            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_emailSettings.EmailServer, int.Parse(_emailSettings.EmailPort), SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_emailSettings.EmailAccountUser, _emailSettings.EmailAccountPassword);
                    await client.SendAsync(message);
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }
    }


}
