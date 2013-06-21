/*
 * Общие примечания.
 * 1. В функциях типа CreateWindowEx использую целочисленный тип для полей-флагов (dwStyle),
 *    а не ввожу тип-перечисление, так как при создании окна предопределённого класса, флаги
 *    dwStyle будут дополняться флагами, специфическими данному классу. 
 */

/*
 *  Полезные ссылки:
 *  Соответствие типов данных:
 *      Windows Data Types: http://msdn.microsoft.com/en-us/library/windows/desktop/aa383751(v=vs.85).aspx
 *      Fundamental Types (C++): http://msdn.microsoft.com/ru-ru/library/vstudio/cc953fe1(v=vs.100).aspx
 *      C# <-> .NET Types Table: http://msdn.microsoft.com/en-us/library/ya5y69ds.aspx
 */

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


        public const int ERROR_CLASS_ALREADY_EXISTS = 1410;


        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms633587(v=vs.85).aspx
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.U2)]
        public static extern UInt16 RegisterClassEx([In] ref WNDCLASSEX lpwcx);

        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms644899(v=vs.85).aspx
        [DllImport("user32.dll", SetLastError = true)]
        static extern Boolean UnregisterClass(String lpClassName, IntPtr hInstance);

        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms632680(v=vs.85).aspx
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr CreateWindowEx(
           UInt32 dwExStyle,
           String lpClassName,
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

        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms633572(v=vs.85).aspx
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr DefWindowProc(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);

        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms633571(v=vs.85).aspx
        [DllImport("user32.dll")]
        static extern IntPtr CallWindowProc(WndProc lpPrevWndFunc, IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        // http://msdn.microsoft.com/en-us/library/windows/desktop/ms632682(v=vs.85).aspx
        [DllImport("user32.dll", SetLastError = true)]
        static extern Boolean DestroyWindow(IntPtr hWnd);


        // SetWindowLong ?
    }


}
