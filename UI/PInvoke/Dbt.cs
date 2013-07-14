using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;

namespace PInvoke
{
    class Dbt
    {
        public const Int32 DBT_DEVICEARRIVAL = 0X8000;
        public const Int32 DBT_DEVICEREMOVECOMPLETE = 0X8004;
        public const Int32 DBT_DEVTYP_DEVICEINTERFACE = 5;
        public const Int32 DBT_DEVTYP_HANDLE = 6;
        public const Int32 DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 4;
        public const Int32 DEVICE_NOTIFY_SERVICE_HANDLE = 1;
        public const Int32 DEVICE_NOTIFY_WINDOW_HANDLE = 0;
        public const Int32 WM_DEVICECHANGE = 0x219;


        /*public enum dbcc_devicetype : int
        {
            DBT_DEVTYP_DEVICEINTERFACE = 0x00000005,
            DBT_DEVTYP_HANDLE = 0x00000006,
            DBT_DEVTYP_OEM = 0x00000000,
            DBT_DEVTYP_PORT = 0x00000003,
            DBT_DEVTYP_VOLUME = 0x00000002
        }*/

        // http://msdn.microsoft.com/en-us/library/windows/desktop/aa363246(v=vs.85).aspx
        // Is usually used for receiving data about receiving data.
        [StructLayout(LayoutKind.Sequential)]
        public class DEV_BROADCAST_HDR
        {
            public Int32 dbch_size;
            public Int32 dbch_devicetype;
            public Int32 dbch_reserved;
        }
        
        // http://msdn.microsoft.com/en-us/library/windows/desktop/aa363244(v=vs.85).aspx
        // Maximum device pathname is 255. So in dbcc_name there is only (dbcc_size - "all other field") bytes of unicodode string.
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class DEV_BROADCAST_DEVICEINTERFACE
        {
            public Int32 dbcc_size;
            public Int32 dbcc_devicetype;
            public Int32 dbcc_reserved = 0;
            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
            public Byte[] dbcc_classguid;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)]
            public Char[] dbcc_name = new Char[255];

            // Useful for sending data by mean of the structure.
            public DEV_BROADCAST_DEVICEINTERFACE()
            {
                this.dbcc_size = Marshal.SizeOf(this);
                this.dbcc_devicetype = DBT_DEVTYP_DEVICEINTERFACE;
            }

            // Useful for sending data by mean of the structure.
            public DEV_BROADCAST_DEVICEINTERFACE(string InterfaceGuid)
                : this()
            {
                this.dbcc_classguid = new Guid(InterfaceGuid).ToByteArray();
            }

            // Useful for receiving data through the structure.
            public DEV_BROADCAST_DEVICEINTERFACE(DEV_BROADCAST_HDR DevBroadcastHdr)
            {
                this.dbcc_size = DevBroadcastHdr.dbch_size;
                this.dbcc_devicetype = DevBroadcastHdr.dbch_devicetype;
            }

            internal int dbcc_name_ulength
            {
                get
                {
                    // (4 + 4 + 4 + 16) — all fields before dbcc_name
                    // (dbcc_size - (4 + 4 + 4 + 16)) — number of 1 byte ANSI chars in Unicode string (including returned null terminate)
                    // (dbcc_size - (4 + 4 + 4 + 16))/2 — number of 2 byte Unicode chars in Unicode string (including returned null terminate)
                    // ( (dbcc_size - (4 + 4 + 4 + 16))/2 - 1 ) — lenght of Unicode string without returned null terminate
                    return ((this.dbcc_size - (4 + 4 + 4 + 16)) / 2 - 1);
                }
            }

            public string dbcc_name_string
            {
                get
                {
                    return new String(dbcc_name, 0, this.dbcc_name_ulength);
                }
            }

        }/**/
    }
}
