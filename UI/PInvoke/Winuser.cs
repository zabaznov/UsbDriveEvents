/*
 * Общие примечания.
 * 1. В функциях типа CreateWindowEx использую целочисленный тип для полей-флагов (dwStyle),
 *    а не ввожу тип-перечисление, так как при создании окна предопределённого класса, флаги
 *    dwStyle будут дополняться флагами, специфическими данному классу.
 * 2. Строки .NET по умолчанию маршаллятся как LPTSTR. Если строка (особенно в аргументе функции)
 *    имеет другой тип (пусть даже LPСTSTR), нужно явно указывать тип строки для .NET, а именно,
 *    [MarshalAs(UnmanagedType.LPTStr)].
 * 3. CharSet в DllImport в первую очередь определяет, какая из функций будет вызываться — A или W.
 *    Но также определяет, как будут трактоваться строки-аргументы. 
 */

/*
 *  Полезные ссылки:
 *  Соответствие типов данных:
 *      Windows Data Types: http://msdn.microsoft.com/en-us/library/windows/desktop/aa383751(v=vs.85).aspx
 *      Fundamental Types (C++): http://msdn.microsoft.com/ru-ru/library/vstudio/cc953fe1(v=vs.100).aspx
 *      C# <-> .NET Types Table: http://msdn.microsoft.com/en-us/library/ya5y69ds.aspx
 *      System Error Codes: http://msdn.microsoft.com/en-us/library/windows/desktop/ms681381(v=vs.85).aspx
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;

namespace PInvoke
{
    using HWND = System.IntPtr;

    class Winuser
    {
        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms633573(v=vs.85).aspx
        public delegate IntPtr WndProc(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);
        
        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms633577(v=vs.85).aspx
        [StructLayout(LayoutKind.Sequential)]
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
            [MarshalAs(UnmanagedType.LPTStr)]
            public String lpszMenuName;
            [MarshalAs(UnmanagedType.LPTStr)] // CAUTION: Works without it. On PInvoke it is said that this cause errors.
            public String lpszClassName;
            public IntPtr hIconSm;
        }

        public const int ERROR_CLASS_ALREADY_EXISTS = 1410;

        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms633587(v=vs.85).aspx
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.U2)]
        public static extern UInt16 RegisterClassEx([In] ref WNDCLASSEX lpwcx);

        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms644899(v=vs.85).aspx
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern Boolean UnregisterClass(
            [MarshalAs(UnmanagedType.LPTStr)]
            String lpClassName,
            IntPtr hInstance);

        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms632680(v=vs.85).aspx
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateWindowEx(
           UInt32 dwExStyle,
           [MarshalAs(UnmanagedType.LPTStr)] // Otherwise — GLE 1407 after.
           String lpClassName,    
           [MarshalAs(UnmanagedType.LPTStr)]
           String lpWindowName,
           UInt32 dwStyle,
           Int32 x,
           Int32 y,
           Int32 nWidth,
           Int32 nHeight,
           IntPtr hWndParent,
           IntPtr hMenu,
           IntPtr hInstance,
           IntPtr lpParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CreateWindowEx(
           UInt32 dwExStyle,
           UInt16 lpClassName,
           [MarshalAs(UnmanagedType.LPTStr)]
           String lpWindowName,
           UInt32 dwStyle,
           Int32 x,
           Int32 y,
           Int32 nWidth,
           Int32 nHeight,
           IntPtr hWndParent,
           IntPtr hMenu,
           IntPtr hInstance,
           IntPtr lpParam);

        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms632682(v=vs.85).aspx
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern Boolean DestroyWindow(IntPtr hWnd);

        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms633572(v=vs.85).aspx
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);

        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms633571(v=vs.85).aspx
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr CallWindowProc(WndProc lpPrevWndFunc, IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        // SetWindowLongPtr ?
        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms633591(v=vs.85).aspx
        /* Note that on 32 bit systems the procedure SetWindowLongPtr is not actually implemented
         * in Windows (USER32.DLL) -- the function is defined in the headers to be SetWindowLong
         * under 32 bit. Therefore if you are calling this function though a mechanism that
         * does not inherit this redirection (using PInvoke from .NET / VB or directly calling
         * GetProcAddress into USER32 e.g.) you can't simply switch to using SetWindowLongPtr,
         * you will need to redirect to SetWindowLong on 32 bit yourself.
         * Function will block on windows which do not respond */
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, UInt32 nIndex, Int32 dwNewLong);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, Int32 nIndex, WndProc dwNewLong);

        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms633584(v=vs.85).aspx
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern Int32 GetWindowLong(IntPtr hWnd, Int32 nIndex);

        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms633529(v=vs.85).aspx
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern Boolean IsWindowUnicode([In] HWND hWnd);

        // http://msdn.microsoft.com/en-us/library/ms633579(v=VS.85).aspx
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern Boolean GetClassInfoEx(
            IntPtr hinst,
            [MarshalAs(UnmanagedType.LPTStr)]String lpszClass,
            ref WNDCLASSEX lpwcx);


        // http://msdn.microsoft.com/en-us/library/windows/desktop/aa363431(v=vs.85).aspx
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr NotificationFilter, Int32 Flags);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, Dbt.DEV_BROADCAST_DEVICEINTERFACE NotificationFilter, Int32 Flags);


        // http://msdn.microsoft.com/en-us/library/windows/desktop/aa363475(v=vs.85).aspx
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern Boolean UnregisterDeviceNotification(IntPtr Handle);
    }


}
