using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using McMaster.Extensions.CommandLineUtils;

namespace AligaAuth
{
    class Program
    {
        const string LDAP_PATH = "127.0.0.1:389";
        const string LDAP_OU_USERS = "ou=People";
        const string LDAP_DOMAIN = "aliga.cat";
        const string LDAP_SERVICE_USER = @"uid=admin,ou=system";
        const string LDAP_SERVICE_PASSWORD = "X1nGuXunG1";

        static void Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        [Option(Description = "Usuari")]
        public string Username { get; }

        [Option(Description = "Contrasenya")]
        public string Password { get; }

        private void OnExecute()
        {
            var username = Username ?? "";
            var password = Password ?? "";

            using (var context = new PrincipalContext(ContextType.Domain, LDAP_DOMAIN, LDAP_OU_USERS, LDAP_SERVICE_USER, LDAP_SERVICE_PASSWORD))
            {
                if (context.ValidateCredentials(username, password))
                {
                    using (var de = new DirectoryEntry(LDAP_PATH))
                    using (var ds = new DirectorySearcher(de))
                    {

                        Console.WriteLine("Ok!");
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("KO");
                }
            }
        }





    }
}
