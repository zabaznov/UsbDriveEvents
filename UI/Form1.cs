using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PInvoke;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace UI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // IntPtr hinstance = Marshal.GetHINSTANCE(System.Reflection.Assembly.GetExecutingAssembly().GetModule("UI.exe"));
            
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

            Boolean unregiter = Winuser.UnregisterClass("TestClass", hInstance);

            int err = Marshal.GetLastWin32Error();

            int j = 6;

            /*if (class_atom == 0)
                throw new Win32Exception("Unable to register Window Class");

            IntPtr hWnd = Winuser.CreateWindowEx
            (
                dwExStyle : 0,
                lpClassName : "TestClass",
                lpWindowName : "Window",
                dwStyle : 0,
                x : 0,
                y : 0,
                nWidth : 0,
                nHeight : 0,
                hWndParent : IntPtr.Zero,
                hMenu : IntPtr.Zero,
                hInstance : hInstance,
                lpParam : IntPtr.Zero
            );*/

            /*if (hWnd == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }*/

        }

        public static IntPtr CustomWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            int i = 5;
            return Winuser.DefWindowProc(hWnd, msg, wParam, lParam);
        }
    }
}


// Полезности:
// http://social.msdn.microsoft.com/Forums/vstudio/en-US/8580a805-383b-4b17-8bd8-514da4a5f3a4/problems-with-pinvoke-createwindowex-and-registerclassex
// http://msdn.microsoft.com/en-us/library/windows/desktop/ms633575(v=vs.85).aspx
// http://bytes.com/topic/c-sharp/answers/428972-hinstance-hinstance-c
// http://msdn.microsoft.com/en-us/library/windows/desktop/ms633574(v=vs.85).aspx
// http://stackoverflow.com/questions/15462064/hinstance-in-createwindow
// http://stackoverflow.com/questions/1123598/what-does-getmodulehandle-do-in-this-code
// http://www.pinvoke.net/default.aspx/user32/createwindowex.html?diff=y

// http://www.codeproject.com/Articles/12121/Essential-P-Invoke

// I! http://stackoverflow.com/questions/9330517/how-to-listen-for-windows-broadcast-messages-in-net