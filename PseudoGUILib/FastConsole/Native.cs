using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FastConsole
{
    public class Native
    {
        public enum nStdHandle : Int32
        {
            STD_INPUT_HANDLE = -10,
            STD_OUTPUT_HANDLE = -11,
            STD_ERROR_HANDLE = -12
        }

        public enum wAttribute : Int16
        {
            FOREGROUND_BLUE = 1,
            FOREGROUND_GREEN = 2,
            FOREGROUND_RED = 4,
            FOREGROUND_INTENSITY = 8,
            BACKGROUND_BLUE = 16,
            BACKGROUND_GREEN = 32,
            BACKGROUND_RED = 64,
            BACKGROUND_INTENSITY = 128
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CONSOLE_SCREEN_BUFFER_INFO
        {
            public COORD bufferSize;
            public COORD cursorPos;
            public wAttribute bufferAttributes;
            public SMALL_RECT visibleSegment;
            public COORD maxWindowSize;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CHAR_INFO
        {
            public short c;
            public wAttribute attr;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SMALL_RECT
        {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct COORD
        {
            public short x;
            public short y;
        }

        public static int GetLastError()
        {
            return Marshal.GetLastWin32Error();
        }

        [DllImport("kernel32.dll", EntryPoint = "GetStdHandle", SetLastError = true,
             CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetStdHandle(nStdHandle nStdHandle);


        /// <summary>
        /// Write text at the position of the cursor with buffer attributes
        /// </summary>
        [DllImport("kernel32.dll", EntryPoint = "WriteConsole", SetLastError = true,
            CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WriteConsole(int Handle, string text, int numOfCharsToWrite, ref int numOfCharsWritten, int mustBeNull);

        /// <summary>
        /// Copy an array to a position in the screen buffer
        /// </summary>
        [DllImport("kernel32.dll", EntryPoint = "WriteConsoleOutput", SetLastError = true,
            CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WriteConsoleOutput(int Handle, CHAR_INFO[] inputBuffer, COORD inputBufferSize, COORD inputBufferStart, ref SMALL_RECT copyDest);

        [DllImport("kernel32.dll", EntryPoint = "WriteConsoleOutputW", SetLastError = true,
            CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WriteConsoleOutputW(int Handle, CHAR_INFO[] inputBuffer, COORD inputBufferSize, COORD inputBufferStart, ref SMALL_RECT copyDest);

        /// <summary>
        /// Write text at a specified location with buffer attributes
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll", EntryPoint = "WriteConsoleOutputCharacter", SetLastError = true,
            CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WriteConsoleOutputCharacter(int Handle, string text, int length, COORD pos, ref int numOfCharsWritten);

        /// <summary>
        /// Sets the character attributes for a specified number of character cells, beginning at the specified coordinates in a screen buffer.
        /// </summary>
        [DllImport("kernel32.dll", EntryPoint = "FillConsoleOutputAttribute", SetLastError = true,
            CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool FillConsoleOutputAttribute(int Handle, wAttribute attributes, int length, COORD coords, ref int numOfCharsWritten);

        /// <summary>
        /// Writes a character to the screen buffer a specified number of times, beginning at the specified coordinates. Uses buffer attributes.
        /// </summary>
        [DllImport("kernel32.dll", EntryPoint = "FillConsoleOutputCharacter", SetLastError = true,
            CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool FillConsoleOutputCharacter(int Handle, char character, int length, COORD coords, ref int numOfCharsWritten);

        /*
        [DllImport("kernel32.dll", EntryPoint = "SetCurrentConsoleFontEx", SetLastError = true,
            CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool SetCurrentConsoleFontEx*/

        [DllImport("kernel32.dll", EntryPoint = "SetConsoleOutputCP", SetLastError = true,
        CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool SetConsoleOutputCP(uint id);


        /// <summary>
        /// Returns a struct which contains: buffer size, cursor position, attributes, view rectangle, max view size.
        /// </summary>
        [DllImport("kernel32.dll", EntryPoint = "GetConsoleScreenBufferInfo", SetLastError = true,
            CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool GetConsoleScreenBufferInfo(int Handle, ref CONSOLE_SCREEN_BUFFER_INFO info);

        
    }
}
