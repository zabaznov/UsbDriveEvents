using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PInvoke;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.ComponentModel;

namespace UI
{
    public class CustomNativeWindow
    {
        public CustomNativeWindow() { }

        public IntPtr CreateWindow()
        {
            // IntPtr hinstance = Marshal.GetHINSTANCE(System.Reflection.Assembly.GetExecutingAssembly().GetModule("UI.exe"));
            // hInstance = Marshal.GetHINSTANCE(this.GetType().Module);
            // Or returned by PInvoked LoadLibrary
            IntPtr hInstance = Process.GetCurrentProcess().Handle;

            if (hInstance == new IntPtr(-1))
                throw new Win32Exception("Couldn't get modules instance");

            Winuser.WNDCLASSEX wnd_class_ex = new Winuser.WNDCLASSEX()
            {
                cbSize = (UInt32)Marshal.SizeOf(typeof(Winuser.WNDCLASSEX)),
                style = 0x4000, // CS_GLOBALCLASS  (int)ClassStyles.CS_GLOBALCLASS,
                lpfnWndProc = CustomWndProc,
                cbClsExtra = 0,
                cbWndExtra = 0,
                hInstance = IntPtr.Zero,//hInstance, // NULL = application's HINSTANCE
                hIcon = IntPtr.Zero,
                hCursor = IntPtr.Zero,
                hbrBackground = IntPtr.Zero,
                lpszMenuName = null,
                lpszClassName = "TestClass",
                hIconSm = IntPtr.Zero
            };

            UInt16 class_atom = Winuser.RegisterClassEx(ref wnd_class_ex);

            if (class_atom == 0)
                throw new Win32Exception("Unable to register Window Class");

            IntPtr hWnd = Winuser.CreateWindowEx
            (
                dwExStyle: 0,
                lpClassName: "TestClass",
                lpWindowName: "Window",
                dwStyle: 0,
                x: 0,
                y: 0,
                nWidth: 0,
                nHeight: 0,
                hWndParent: IntPtr.Zero,
                hMenu: IntPtr.Zero,
                hInstance: hInstance,
                lpParam: IntPtr.Zero
            );

            if (hWnd == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            return hWnd;
        }

        public static IntPtr CustomWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            int i = 5;
            return Winuser.DefWindowProc(hWnd, msg, wParam, lParam);
        }

        
    }
}
