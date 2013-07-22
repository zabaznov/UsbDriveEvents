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
        public static Boolean IsRegistered(string className)
        {
            IntPtr hinstance = Process.GetCurrentProcess().Handle;
            Winuser.WNDCLASSEX wnd_class_ex = new Winuser.WNDCLASSEX();
            wnd_class_ex.cbSize = (UInt32)Marshal.SizeOf(typeof(Winuser.WNDCLASSEX));

            if (Winuser.GetClassInfoEx(hinstance, className, ref wnd_class_ex))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public static Boolean Register(string className)
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
                if (err != Winuser.ERROR_CLASS_ALREADY_EXISTS)
                {                
                    throw new Win32Exception("Unable to register Window Class");
                }
            }

            return true;
        }

        public static Boolean Unregister(string className)
        {
            return Winuser.UnregisterClass(className, Process.GetCurrentProcess().Handle);
        }

        private static IntPtr CustomWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
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

        /*public Boolean Subclass(Winuser.WndProc subclassWndProc)
        {
            this.windowWndProc = Winuser.SetWindowLong(this.Handle, Winuser.GWL_WNDPROC, 
        }*/

        public static IntPtr CustomWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            int i = 5;
            return Winuser.DefWindowProc(hWnd, msg, wParam, lParam);
        }
    }

    // MyNativeWindow class to create a window given a class name.
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    public class MyNativeWindow : NativeWindow
    {

        // Constant values were found in the "windows.h" header file.
        private const int WS_CHILD = 0x40000000,
                          WS_VISIBLE = 0x10000000,
                          WM_ACTIVATEAPP = 0x001C;

        private int windowHandle;

        private Boolean IsListening = false;

        public MyNativeWindow(/*Object*/Form parent)
        {

            /*
            if !IsRegistered("WindowClassName")
                Register("WindowClassName");
            */

            CreateParams cp = new CreateParams();

            // Fill in the CreateParams details.
            cp.Caption = "Click here";
            cp.ClassName = "Button";

            // Set the position on the form
            cp.X = 100;
            cp.Y = 100;
            cp.Height = 100;
            cp.Width = 100;

            // Specify the form as the parent.
            cp.Parent = parent.Handle;

            // Create as a child of the specified parent
            cp.Style = WS_CHILD | WS_VISIBLE;

            // Create the actual window
            this.CreateHandle(cp);

        }

        public ~MyNativeWindow()
        {
            /*
                if IsListening
                    StopListen();
                this.DestroyHandle();
            */
        }

        public Boolean StartListen(/* VID&PID or Path*/)
        {
            /*
            RegisterForDeviceNotifications()
            */
        }

        public Boolean StopListen()
        {

        }

        public delegate void DevicePresenceDelegate();

        public DevicePresenceDelegate DeviceArrival;

        public DevicePresenceDelegate DeviceRemoval;

        // Listen to when the handle changes to keep the variable in sync
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        protected override void OnHandleChange()
        {
            windowHandle = (int)this.Handle;
        }

        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message m)
        {
            // Listen for messages that are sent to the button window. Some messages are sent
            // to the parent window instead of the button's window.

            switch (m.Msg)
            {
                case WM_ACTIVATEAPP:
                    // Do something here in response to messages
                    break;
            }
            base.WndProc(ref m);
        }
    }
}

}
