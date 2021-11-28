using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace MakeToken.Token
{
    class TokenOps
    {
        public enum LOGON_TYPE
        {
            LOGON32_LOGON_INTERACTIVE = 2,
            LOGON32_LOGON_NETWORK = 3,
            LOGON32_LOGON_BATCH = 4,
            LOGON32_LOGON_SERVICE = 5,
            LOGON32_LOGON_UNLOCK = 7,
            LOGON32_LOGON_NETWORK_CLEARTEXT = 8,
            LOGON32_LOGON_NEW_CREDENTIALS = 9
        }

        public enum LOGON_PROVIDER
        {
            LOGON32_PROVIDER_DEFAULT = 0,
            LOGON32_PROVIDER_WINNT35 = 1,
            LOGON32_PROVIDER_WINNT40 = 2,
            LOGON32_PROVIDER_WINNT50 = 3
        }

        [DllImport("advapi32.dll", SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool LogonUser(
          [MarshalAs(UnmanagedType.LPStr)] string pszUserName,
          [MarshalAs(UnmanagedType.LPStr)] string pszDomain,
          [MarshalAs(UnmanagedType.LPStr)] string pszPassword,
          LOGON_TYPE dwLogonType,
          LOGON_PROVIDER dwLogonProvider,
          ref IntPtr phToken);

        [DllImport("advapi32.dll", SetLastError = true)]
        static extern bool ImpersonateLoggedOnUser(IntPtr hToken);

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
            NetCredentialsOnly = 2
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


        public static void TokenCreate(string username, string domain, string password)
        {

            Console.WriteLine("from TokenCreate()");

            //https://blog.cobaltstrike.com/2015/12/16/windows-access-tokens-and-alternate-credentials/

            IntPtr phToken = IntPtr.Zero;
            
            bool logonRes = LogonUser(username, domain, password, LOGON_TYPE.LOGON32_LOGON_NEW_CREDENTIALS, LOGON_PROVIDER.LOGON32_PROVIDER_DEFAULT, ref phToken);

            bool impersonateRes = ImpersonateLoggedOnUser(phToken);

            IntPtr env = IntPtr.Zero;
            STARTUPINFO si = new STARTUPINFO();
            si.wShowWindow = 3;
            si.cb = Marshal.SizeOf(si);
            PROCESS_INFORMATION pi = new PROCESS_INFORMATION();

            bool cRes = CreateProcessWithTokenW(phToken, LogonFlags.NetCredentialsOnly, null, @"C:\Windows\system32\WindowsPowerShell\v1.0\powershell.exe", CreationFlags.NewConsole, env, null, ref si, out pi);
           
        }



    }
}
