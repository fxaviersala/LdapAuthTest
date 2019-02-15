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
                    cn.Bind($"uid={username},ou=People,dc=aliga,dc=cat", password);
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
