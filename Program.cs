using System;
using McMaster.Extensions.CommandLineUtils;
using Novell.Directory.Ldap;

namespace AligaAuth
{
    [Command(Name = "AligaAuth", Description = "LDAP Tester")]
    [HelpOption("-?")]
    class Program
    {
        // Missatges
        private const string UsuariError = "Credencials invàlides\n";
        private const string UsuariCorrecte = " OK\n";

        // Constants LDAP que s'hauríen de passar com a configuració
        private const string LdapServer = "localhost";
        private const string LdapCNUser = "cn=readonly,dc=aliga,dc=cat";
        private const string LdapCNPassword = "passwr0rd!";
        private const string LdapUsers = "ou=People,dc=aliga,dc=cat";


        static void Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        [Argument(0, Description = "Usuari")]
        public string Username { get; }

        [Argument(1, Description = "Contrasenya")]
        public string Password { get; }

        private void OnExecute()
        {
            var username = Username ?? "";
            var password = Password ?? "";

            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
            {
                System.Console.WriteLine("\nAfegeix l'usuari i la contrasenya darrere de la comanda\n");
                System.Console.WriteLine(" ex. dotnet run Usuari Password\n");
                return;
            }

            username = username.ToLower();
            System.Console.WriteLine($"... Provant usuari: {username} i contrasenya {password}\n");

            using (var cn = new LdapConnection())
            {
                try
                {
                    // connect
                    cn.Connect(LdapServer, 389);
                    // ... Bind CN amb l'usuari readonly
                    cn.Bind(LdapCNUser, LdapCNPassword);

                    // Buscar entre els usuaris
                    var filtre = $"(uid={username})";
                    var resutatcerca = cn.Search(LdapUsers,
                                                  LdapConnection.SCOPE_SUB,
                                                  filtre,
                                                  null,
                                                  typesOnly: false);

                    // O hi és o no hi és
                    var user = resutatcerca.hasMore() ? resutatcerca.next() : null;
                    if (user != null)
                    {
                        cn.Bind(user.DN, password);
                        if (cn.Bound)
                        {
                            Console.WriteLine(username + UsuariCorrecte);
                        }
                    }
                    else
                    {
                        Console.WriteLine(UsuariError);
                    }
                }
                catch (LdapException)
                {
                    Console.WriteLine(UsuariError);
                }
            }
        }
    }
}
