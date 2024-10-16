using safetool.Data;
using safetool.Models;
using System.Linq;

namespace safetool.Services
{
    public class GeneralParameterService
    {
        private readonly SafetoolContext _context;

        public GeneralParameterService(SafetoolContext context)
        {
            _context = context;
        }

        public GeneralParameter GetEmailSettings()
        {
            // Aquí obtienes los parámetros de correo electrónico almacenados en la base de datos.
            return _context.GeneralParameters.FirstOrDefault();
        }

        public void UpdateEmailSettings(GeneralParameter updatedSettings)
        {
            var existingSettings = _context.GeneralParameters.FirstOrDefault();
            if (existingSettings != null)
            {
                existingSettings.EmailAccount = updatedSettings.EmailAccount;
                existingSettings.EmailAccountDisplayName = updatedSettings.EmailAccountDisplayName;
                existingSettings.EmailAccountPassword = updatedSettings.EmailAccountPassword;
                existingSettings.EmailAccountUser = updatedSettings.EmailAccountUser;
                existingSettings.EmailPort = updatedSettings.EmailPort;
                existingSettings.EmailServer = updatedSettings.EmailServer;
                existingSettings.EmailSsl = updatedSettings.EmailSsl;

                _context.GeneralParameters.Update(existingSettings);
                _context.SaveChanges();
            }
            else
            {
                _context.GeneralParameters.Add(updatedSettings);
                _context.SaveChanges();
            }
        }
    }

}
