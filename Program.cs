using System;
using McMaster.Extensions.CommandLineUtils;
using Novell.Directory.Ldap;

namespace AligaAuth
{
    [Command(Name = "AligaAuth", Description = "LDAP Tester")]
    [HelpOption("-?")]
    class Program
    {
        private const string UsuariError = "Credencials invàlides\n";
        private const string UsuariCorrecte = " OK\n";
        private const string LdapCNUser = "cn=readonly,dc=aliga,dc=cat";
        private const string LdapCNPassword = "passwr0rd!";

        static void Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        [Argument(0, Description = "Usuari")]
        public string Username { get; }

        [Argument(1, Description = "Contrasenya")]
        public string Password { get; }

        private void OnExecute()
        {
            var username = Username ?? "helena";
            var password = Password ?? "";

            username = username.ToLower();
            System.Console.WriteLine($"... Provant usuari: {username} i contrasenya {password}\n");

            using (var cn = new LdapConnection())
            {
                try
                {
                    // connect
                    cn.Connect("localhost", 389);
                    // ... Bind CN
                    cn.Bind(LdapCNUser, LdapCNPassword);

                    // Buscar entre els usuaris
                    var filtre = $"(uid={username})";
                    var resutatcerca = cn.Search("ou=People,dc=aliga,dc=cat",
                                                  LdapConnection.SCOPE_SUB,
                                                  filtre,
                                                  null,
                                                  typesOnly: false);

                    // O hi és o no hi és
                    var user = resutatcerca.hasMore() ? resutatcerca.next() : null;
                    cn.Bind(user.DN, password);
                    if (cn.Bound)
                    {
                        Console.WriteLine(username + UsuariCorrecte);
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
