using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace PInvoke
{
    class Winbase
    {
        [DllImport("kernel32.dll", SetLastError = true/*, CharSet = CharSet.Auto*/)]
        public static extern UInt16 GlobalFindAtom([MarshalAs(UnmanagedType.LPTStr)] string lpString);
        
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [ResourceExposure(ResourceScope.Process)]
        public static extern IntPtr GetModuleHandle([MarshalAs(UnmanagedType.LPTStr)] string lpModuleName);
    }
}
