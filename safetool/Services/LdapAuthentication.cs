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

        public User AuthenticateAndGetUser(string username, string password)
        {
            try
            {
                string userPrincipalName = username + _ldapDomain;

                using (DirectoryEntry entry = new DirectoryEntry(_ldapPath, userPrincipalName, password))
                {
                    // Intenta acceder al objeto
                    object nativeObject = entry.NativeObject;

                    // Recuperar información del usuario
                    var user = new User { Uid = username };

                    // Realizar una búsqueda para obtener atributos
                    using (DirectorySearcher searcher = new DirectorySearcher(entry))
                    {
                        searcher.Filter = $"(sAMAccountName={username})"; // Filtro por el nombre de usuario
                        searcher.PropertiesToLoad.Add("mail");
                        searcher.PropertiesToLoad.Add("displayName");
                        searcher.PropertiesToLoad.Add("givenName"); // Nombre
                        searcher.PropertiesToLoad.Add("sn"); // Apellido
                        searcher.PropertiesToLoad.Add("st"); // Localidad

                        SearchResult result = searcher.FindOne();
                        if (result != null)
                        {
                            user.Email = result.Properties["mail"].Count > 0 ? result.Properties["mail"][0].ToString() : null;
                            user.FullName = result.Properties["displayName"].Count > 0 ? result.Properties["displayName"][0].ToString() : null;
                            user.FirstName = result.Properties["givenName"].Count > 0 ? result.Properties["givenName"][0].ToString() : null;
                            user.LastName = result.Properties["sn"].Count > 0 ? result.Properties["sn"][0].ToString() : null;
                        }
                    }

                    return user;
                }
            }
            catch (DirectoryServicesCOMException)
            {
                return null; // Credenciales inválidas
            }
            catch (Exception ex)
            {
                Console.WriteLine("LDAP error occurred: " + ex.Message);
                return null;
            }
        }
    }

    public class User
    {
        public string Uid { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}

