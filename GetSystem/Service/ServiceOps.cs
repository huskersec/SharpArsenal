using System;
using System.Runtime.InteropServices;

namespace GetSystem.Service
{
    class ServiceOps
    {

        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true,
            CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr OpenSCManager(string machineName, string databaseName,
            uint dwAccess);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr CreateService(
            IntPtr hSCManager,
            string lpServiceName,
            string lpDisplayName,
            uint dwDesiredAccess,
            uint dwServiceType,
            uint dwStartType,
            uint dwErrorControl,
            string lpBinaryPathName,
            string lpLoadOrderGroup,
            string lpdwTagId,
            string lpDependencies,
            string lpServiceStartName,
            string lpPassword);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, uint
        dwDesiredAccess);

        [DllImport("advapi32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool StartService(IntPtr hService, int dwNumServiceArgs, string[]
        lpServiceArgVectors);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool DeleteService(IntPtr hService);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool CloseServiceHandle(IntPtr hSCObject);



        public static void CreateService(string binPathName)
        {

            IntPtr SCMHandle = OpenSCManager(null, null, 0xF003F);

            string svcName = "Getsystem";
            string displayName = "Getsystem";
            string svcPassword = "";

            IntPtr schService = CreateService(SCMHandle, svcName, displayName, 0xF01FF, 0x00000010, 0x00000003, 0x00000001, binPathName, null, null, null, null, svcPassword);

            CloseServiceHandle(SCMHandle);
            CloseServiceHandle(schService);
        }


        public static void StartService()
        {

            IntPtr SCMHandle = OpenSCManager(null, null, 0xF003F);

            string svcName = "Getsystem";
            IntPtr openService = OpenService(SCMHandle, svcName, 0xF01FF);

            var startService = StartService(openService, 0, null);

            CloseServiceHandle(SCMHandle);
            CloseServiceHandle(openService);

        }

        public static void DeleteService()
        {

            IntPtr SCMHandle = OpenSCManager(null, null, 0xF003F);

            string svcName = "Getsystem";
            IntPtr openService = OpenService(SCMHandle, svcName, 0xF01FF);

            DeleteService(openService);

            CloseServiceHandle(SCMHandle);
            CloseServiceHandle(openService);

        }
    }
}
