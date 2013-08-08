using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PInvoke;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;

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
        private IntPtr windowHandle;

        private Boolean isListening = false;

        private IntPtr deviceNotificationHandle = IntPtr.Zero;

        public MyNativeWindow(IntPtr hParent)
        {            
            // Register window class if miss one
            if (!WindowClass.IsRegistered("NetEventListenerClass"))
                WindowClass.Register("NetEventListenerClass");
            
            // Set parameters of the window
            CreateParams cp = new CreateParams();            
                cp.ClassName = "NetEventListenerClass";
                cp.Caption = "NetEventListenerWindow";
                cp.Style = 0;
                cp.X = 0;
                cp.Y = 0;
                cp.Width = 0;
                cp.Height = 0;
                cp.Parent = hParent;
                cp.Param = null;

            // Create the actual window
            this.CreateHandle(cp);

        }

        ~MyNativeWindow()
        {
            if (isListening)
                StopListen();
            this.DestroyHandle();
        }

        // Listen to when the handle changes to keep the variable in sync
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        protected override void OnHandleChange()
        {
            windowHandle = this.Handle;
        }

        public Boolean SubscribeDeviceNotification(IntPtr hRecipient, string interfaceGuid)
        {            
            Dbt.DEV_BROADCAST_DEVICEINTERFACE devBroadcastDeviceInterface = new Dbt.DEV_BROADCAST_DEVICEINTERFACE(interfaceGuid);
            this.deviceNotificationHandle = Winuser.RegisterDeviceNotification(hRecipient, devBroadcastDeviceInterface, Dbt.DEVICE_NOTIFY_WINDOW_HANDLE);
            return (this.deviceNotificationHandle == IntPtr.Zero) ? false : true;
        }

        public Boolean UnsubscribeDeviceNotification()
        {
            return Winuser.UnregisterDeviceNotification(this.deviceNotificationHandle);
        }

        public Boolean StartListen()
        {
            Boolean result = SubscribeDeviceNotification(windowHandle, "3af3f480-41b5-4c24-b2a9-6aacf7de3d01"); 
            isListening = result;
            return result;
        }

        public Boolean StopListen()
        {
            Boolean result = UnsubscribeDeviceNotification();
            isListening = !result;
            return result;
        }

        public delegate void DevicePresenceDelegate();

        public event EventHandler<DeviceArrivalArgs> OnDeviceArrival;

        public DevicePresenceDelegate OnDeviceRemoval;

        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message m)
        {
            // Listen for messages that are sent to the window. 
            switch (m.Msg)
            {
                case Dbt.WM_DEVICECHANGE:
                    if (m.WParam.ToInt32() == Dbt.DBT_DEVICEARRIVAL)
                    {
                        //bool b = DeviceNameMatch(hWnd, Msg, wParam, lParam, "{524cc09a-0a72-4d06-980e-afee3131196e}");
                        if (OnDeviceArrival != null) OnDeviceArrival(this, new DeviceArrivalArgs("sadfsafd"));
                    }
                    else if (m.WParam.ToInt32() == Dbt.DBT_DEVICEREMOVECOMPLETE)
                    {
                        //bool b = DeviceNameMatch(hWnd, Msg, wParam, lParam, "{524cc09a-0a72-4d06-980e-afee3131196e}");
                        if (OnDeviceRemoval != null) OnDeviceRemoval();
                    }
                    break; 
            }

            base.WndProc(ref m);
        }
    }
}


