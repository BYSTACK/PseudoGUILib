using System;
using System.Runtime.InteropServices;

namespace PseudoGUILib
{
    public class NativeWin
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

        public static bool SetConsoleResizeEnabled(bool enabled)
        {
            IntPtr handle = GetConsoleWindow();
            if (!enabled)
                return SetWindowLongA(handle, -16, GetWindowLongA(handle, -16) & ~(0x00010000) & ~(0x00040000)) != 0;
            else
                return SetWindowLongA(handle, -16, GetWindowLongA(handle, -16) | (0x00010000) | (0x00040000)) != 0;
        }

        public static Rectangle GetConsoleWindowRectangle()
        {
            RECT rect = new RECT();
            IntPtr handle = GetConsoleWindow();
            GetWindowRect(handle, ref rect);
            return new Rectangle() { x = rect.left, y = rect.top, width = rect.right - rect.left, height = rect.bottom - rect.top };
        }

        public enum ConsoleInputMode : uint
        {
            ENABLE_ECHO_INPUT = 0x0004,
            ENABLE_EXTENDED_FLAGS = 0x0080,
            ENABLE_INSERT_MODE = 0x0020,
            ENABLE_LINE_INPUT = 0x0002,
            ENABLE_MOUSE_INPUT = 0x0010,
            ENABLE_PROCESSED_INPUT = 0x0001,
            ENABLE_QUICK_EDIT_MODE = 0x0040,
            ENABLE_WINDOW_INPUT = 0x0008,
            ENABLE_VIRTUAL_TERMINAL_INPUT = 0x0200,
        }

        public enum ConsoleBufferMode : uint
        {
            ENABLE_PROCESSED_OUTPUT = 0x0001,
            ENABLE_WRAP_AT_EOL_OUTPUT = 0x0002,
            ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004,
            DISABLE_NEWLINE_AUTO_RETURN = 0x0008,
            ENABLE_LVB_GRID_WORLDWIDE = 0x0010,
        }

        public enum ConsoleEventType : ushort
        {
            FOCUS_EVENT = 0x0010,
            KEY_EVENT = 0x0001,
            MENU_EVENT = 0x0008,
            MOUSE_EVENT = 0x0002,
            WINDOW_BUFFER_SIZE_EVENT = 0x0004,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KEY_EVENT_RECORD
        {
            public bool bKeyDown;
            public ushort wRepeatCount;
            public ushort wVirtualKeyCode;
            public ushort wVirtualScanCode;
            public ushort unicodeOrAsciiChar;
            public uint dwControlKeyState;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSE_EVENT_RECORD
        {
            public COORD dwMousePosition;
            public uint dwButtonState;
            public uint dwControlKeyState;
            public uint dwEventFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct COORD
        {
            public short X;
            public short Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOW_BUFFER_SIZE_RECORD
        {
            public short columns;
            public short rows;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct INPUT_RECORD
        {
            [FieldOffset(0)] public ConsoleEventType eventType;
            [FieldOffset(4)] public KEY_EVENT_RECORD keyEvent;
            [FieldOffset(4)] public MOUSE_EVENT_RECORD mouseEvent;
            [FieldOffset(4)] public WINDOW_BUFFER_SIZE_RECORD windowBufferSizeEvent;
        }

        [DllImport("Kernel32.dll")]
        public static extern bool GetNumberOfConsoleInputEvents(int handle, ref int number);

        [DllImport("Kernel32.dll")]
        public static extern bool SetConsoleMode(int handle, uint mode);

        [DllImport("Kernel32.dll")]
        public static extern bool ReadConsoleInput(int handle, [Out] INPUT_RECORD[] buffer, int bufferArray, ref int readEvents);

        [DllImport("Kernel32.dll")]
        public static extern bool GetConsoleMode(IntPtr handle, ref uint lpMode);
    }
}
