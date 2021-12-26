using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpawnJect.Payloads
{
    class Payload
    {

        public static byte[] Getx86()
        {
            /* x86 payload here */
            byte[] buf = new byte[1] { 0x00 };
            return buf;
        }

        public static byte[] Getx64()
        {
            /* x64 payload here */
            byte[] buf = new byte[1] { 0x00 };
            return buf;
        }


    }
}
