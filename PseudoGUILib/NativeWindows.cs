using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PseudoGUILib
{
    public class NativeWindows
    {
        [StructLayout(LayoutKind.Sequential)]
        struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();
        [DllImport("User32.dll")]
        private static extern int SetWindowLongA(IntPtr handle, int nIndex, int dwNewLong);
        [DllImport("User32.dll")]
        private static extern int GetWindowLongA(IntPtr handle, int nIndex);
        [DllImport("User32.dll")]
        private static extern int GetWindowRect(IntPtr handle, ref RECT rect);

        public static bool DisableConsoleResize()
        {
            IntPtr handle = GetConsoleWindow();
            return SetWindowLongA(handle, -16, GetWindowLongA(handle, -16) & ~(0x00010000) & ~(0x00040000)) != 0;
        }

        public static Rectangle GetConsoleWindowRectangle()
        {
            RECT rect = new RECT();
            IntPtr handle = GetConsoleWindow();
            GetWindowRect(handle, ref rect);
            return new Rectangle() { x = rect.left, y = rect.top, width = rect.right - rect.left, height = rect.bottom - rect.top };
        }
    }
}
