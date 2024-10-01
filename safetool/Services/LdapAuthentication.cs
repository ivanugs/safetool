using System;
using System.DirectoryServices;

namespace safetool.Services
{
    public class LdapAuthentication
    {
        private readonly string _ldapPath;
        private readonly string _ldapDomain;

        public LdapAuthentication(string ldapPath, string ldapDomain)
        {
            _ldapPath = ldapPath;
            _ldapDomain = ldapDomain;
        }

        public bool Authenticate(string username, string password)
        {
            try
            {
                // Concatenar el UID con el dominio
                string userPrincipalName = username + _ldapDomain;

                using(DirectoryEntry  entry = new DirectoryEntry(_ldapPath, userPrincipalName, password))
                {
                    // Vincularse al servidor LDAP usando las credenciales proporcionadas
                    object nativeObject = entry.NativeObject;
                    return true;
                }
            }
            catch (DirectoryServicesCOMException)
            {
                // Credenciales invalidas
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("LDAP error ocurred:" + ex.Message);
                return false;
            }
        }
    }
}
