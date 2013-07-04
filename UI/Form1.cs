using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PInvoke;

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
            Winuser.WNDCLASSEX wnd_class_ex = new Winuser.WNDCLASSEX();
            wnd_class_ex.lpszClassName = "sda";
            wnd_class_ex.lpfnWndProc = CustomWndProc;

            UInt16 class_atom = Winuser.RegisterClassEx(ref wnd_class_ex);

            int j = 6;
        }

        private static IntPtr CustomWndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            int i = 5;
            return Winuser.DefWindowProc(hWnd, msg, wParam, lParam);
        }
    }
}
