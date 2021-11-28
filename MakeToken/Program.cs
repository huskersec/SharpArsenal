using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Security.Principal;

namespace MakeToken
{
    class Program
    {

        static void Usage()
        {

            Console.WriteLine(".\\MakeToken.exe username=dvader domain=deathstar.local password=dvaderpass");

        }

        //https://stackoverflow.com/questions/3600322/check-if-the-current-user-is-administrator
        public static bool IsAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        static void Main(string[] args)
        {

            Console.WriteLine("Making token...");

            //per https://docs.microsoft.com/en-us/windows/win32/api/winbase/nf-winbase-createprocesswithtokenw,
            //the "process that calls CreateProcessWithTokenW must have SeImpersonatePrivilege"

            bool isAdmin = IsAdministrator();
            if (isAdmin.Equals(true))
            {

                if (args.Length < 1)
                {
                    Usage();
                    return;
                }

                var arguments = new Dictionary<string, string>();
                foreach (string argument in args)
                {
                    int idx = argument.IndexOf('=');
                    if (idx > 0)
                        arguments[argument.Substring(0, idx)] = argument.Substring(idx + 1);
                }


                if (arguments.ContainsKey("username"))
                {

                    if (arguments.ContainsKey("domain"))
                    {

                        if (arguments.ContainsKey("password"))
                        {

                            string username = arguments["username"];
                            string domain = arguments["domain"];
                            string password = arguments["password"];

                            Token.TokenOps.TokenCreate(username, domain, password);

                        }

                        else
                        {
                            Usage();
                        }
                    }

                    else
                    {
                        Usage();
                    }
                }

                else
                {
                    Usage();
                }
            }

            else
            {
                Console.WriteLine("You need to be admin! Exiting...");
                return;
            }

        }
    }
}
