using System;
using System.Collections.Generic;

namespace GetSystem
{
    class Program
    {
        
        static void Usage()
        {

            //https://blog.cobaltstrike.com/2014/04/02/what-happens-when-i-type-getsystem/
            //https://github.com/rapid7/meterpreter/blob/master/source/extensions/priv/server/elevate/
            //try one of the techniques: pipe_cmd, pipe_dll, token_theft
            Console.WriteLine(".\\GetSystem.exe technique=pipe_cmd");
            Console.WriteLine(".\\GetSystem.exe technique=pipe_dll");
            Console.WriteLine(".\\GetSystem.exe technique=token_theft");
            
        }


        static void Main(string[] args)
        {

            Console.WriteLine("Let's GetSystem...");

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

            //String technique = "";

            //pipe_cmd, pipe_dll, token_theft
            if (arguments.ContainsKey("technique"))
            {

                if (arguments["technique"] == "pipe_cmd")
                {

                    Technique.ElevateTechniques.ServicePipeCmd();

                }

                else if (arguments["technique"] == "pipe_dll")
                {

                    Technique.ElevateTechniques.ServicePipeDLL();

                }

                else if (arguments["technique"] == "token_theft")
                {

                    Technique.ElevateTechniques.TokenTheft();

                }

                else
                {

                    Console.WriteLine("technique not implemented");
                    Usage();
                    return;
                
                }
            }

        }
    }
}
