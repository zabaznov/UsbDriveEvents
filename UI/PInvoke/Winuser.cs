using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace UI.PInvoke
{
    class Winuser
    {
        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms633573(v=vs.85).aspx
        public delegate IntPtr WndProc(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);
        
        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms633577(v=vs.85).aspx
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct WNDCLASSEX
        {
            public UInt32 cbSize;
            public UInt32 style;
            public WndProc lpfnWndProc; 
            public Int32 cbClsExtra;
            public Int32 cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            public String lpszMenuName;
            public String lpszClassName;
            public IntPtr hIconSm;
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.U2)]
        static extern UInt16 RegisterClassEx([In] ref WNDCLASSEX lpwcx);
    }
}
