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
    public class WindowClass
    {
        private string name = null;
        private UInt16 atom = 0;

        public string Name
        {
            get { return name; }
        }

        public UInt16 Atom
        {
            get { return atom; }
        }

        public static WindowClass Register(string className)
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
                hInstance = IntPtr.Zero, //hInstance, // NULL = application's HINSTANCE
                hIcon = IntPtr.Zero,
                hCursor = IntPtr.Zero,
                hbrBackground = IntPtr.Zero,
                lpszMenuName = null,
                lpszClassName = className,
                hIconSm = IntPtr.Zero
            };

            UInt16 class_atom = Winuser.RegisterClassEx(ref wnd_class_ex);

            if (class_atom == 0)
            {
                int err = Marshal.GetLastWin32Error();
                if (err == Winuser.ERROR_CLASS_ALREADY_EXISTS)
                {

                }
                else
                {
                    throw new Win32Exception("Unable to register Window Class");
                }
            }

            WindowClass wc = new WindowClass()
            {
                name = className
                //atom = class_atom
            };

            return wc;
        }

        public static Boolean Unregister(string className)
        {
            return Winuser.UnregisterClass(className, Process.GetCurrentProcess().Handle);
        }

        public static IntPtr CustomWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            int i = 5;
            return Winuser.DefWindowProc(hWnd, msg, wParam, lParam);
        }


    }

    
    public class SimpleNativeWindow
    {
        //private static int instancesCount = 0;

        private Winuser.WndProc windowWndProc = null;

        public IntPtr Handle = IntPtr.Zero;
                
        public SimpleNativeWindow() { }

        public IntPtr CreateWindow(string className, string windowName)
        {
            IntPtr hInstance = Process.GetCurrentProcess().Handle;


            // Пытаемся зарегистрировать класс и смотрим, что получится...

            Winuser.WNDCLASSEX wnd_class_ex = new Winuser.WNDCLASSEX()
            {
                cbSize = (UInt32)Marshal.SizeOf(typeof(Winuser.WNDCLASSEX)),
                style = 0x4000, // CS_GLOBALCLASS  (int)ClassStyles.CS_GLOBALCLASS,
                lpfnWndProc = CustomWndProc,
                cbClsExtra = 0,
                cbWndExtra = 0,
                hInstance = IntPtr.Zero, //hInstance, // NULL = application's HINSTANCE
                hIcon = IntPtr.Zero,
                hCursor = IntPtr.Zero,
                hbrBackground = IntPtr.Zero,
                lpszMenuName = null,
                lpszClassName = className,
                hIconSm = IntPtr.Zero
            };

            UInt16 class_atom = Winuser.RegisterClassEx(ref wnd_class_ex);

            if (class_atom == 0)
            {
                int err = Marshal.GetLastWin32Error();
                if (err != Winuser.ERROR_CLASS_ALREADY_EXISTS)
                {
                    // Что-то пошло _совсем_ не так.
                    throw new Win32Exception("Unable to register Window Class");
                }
                // Иначе — класс с этим именем уже зарегистрирован.
            }


            // Пытаемся создать окно

            IntPtr hWnd = Winuser.CreateWindowEx
            (
                dwExStyle: 0,
                lpClassName: className,
                lpWindowName: windowName,
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

            this.Handle = hWnd;
            //instancesCount++;

            return hWnd;
        }

        public Boolean DestoyWindow()
        {
            return Winuser.DestroyWindow(this.Handle);
        }

        public Boolean Subclass(Winuser.WndProc subclassWndProc)
        {
            this.windowWndProc = Winuser.SetWindowLong(this.Handle, Winuser.GWL_WNDPROC, 
        }

        public static IntPtr CustomWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            int i = 5;
            return Winuser.DefWindowProc(hWnd, msg, wParam, lParam);
        }
    }
}
