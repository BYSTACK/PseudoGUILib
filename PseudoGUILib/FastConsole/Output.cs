using System;
using System.Runtime.InteropServices;

namespace FastConsole
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ConsChar
    {
        public char Character;
        public ConsoleColor BgColor;
        public ConsoleColor TextColor;

        public override bool Equals(object obj)
        {
            return obj is ConsChar @char &&
                   Character == @char.Character &&
                   BgColor == @char.BgColor &&
                   TextColor == @char.TextColor;
        }

        public static bool operator ==(ConsChar c1, ConsChar c2)
        {
            return c1.Character == c2.Character &&
                   c1.BgColor == c2.BgColor &&
                   c1.TextColor == c2.TextColor;
        }
        public static bool operator !=(ConsChar c1, ConsChar c2)
        {
            return c1.Character != c2.Character ||
                   c1.BgColor != c2.BgColor ||
                   c1.TextColor != c2.TextColor;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ConsolePos
    {
        public short x;
        public short y;
    }

    public class FConsole
    {
        private static int outputHandle = Native.GetStdHandle(Native.nStdHandle.STD_OUTPUT_HANDLE);


        private static Native.wAttribute ColorsToWAttribute(ConsoleColor bg, ConsoleColor text)
        {
            return (Native.wAttribute)((UInt16)bg | (UInt16)text << 4);
        }

        private static Native.COORD ConsPosToCOORDS(ConsolePos pos)
        {
            return new Native.COORD() { x = pos.x, y = pos.y };
        }


        public static void SetOutCodepage(uint id)
        {
            Console.WriteLine(Native.SetConsoleOutputCP(id));
        }


        public static ConsolePos GetWindowPosition()
        {
            Native.CONSOLE_SCREEN_BUFFER_INFO info = new Native.CONSOLE_SCREEN_BUFFER_INFO();
            Native.GetConsoleScreenBufferInfo(outputHandle, ref info);
            ConsolePos pos = new ConsolePos()
            {
                x = info.visibleSegment.Left,
                y = info.visibleSegment.Top
            };
            return pos;
        }

        public static ConsolePos GetWindowSize()
        {
            Native.CONSOLE_SCREEN_BUFFER_INFO info = new Native.CONSOLE_SCREEN_BUFFER_INFO();
            Native.GetConsoleScreenBufferInfo(outputHandle, ref info);
            ConsolePos size = new ConsolePos()
            {
                x = (short)(info.visibleSegment.Right - info.visibleSegment.Left),
                y = (short)(info.visibleSegment.Bottom - info.visibleSegment.Top)
            };
            return size;
        }

        public static ConsolePos GetBufferSize()
        {
            Native.CONSOLE_SCREEN_BUFFER_INFO info = new Native.CONSOLE_SCREEN_BUFFER_INFO();
            Native.GetConsoleScreenBufferInfo(outputHandle, ref info);
            ConsolePos size = new ConsolePos()
            {
                x = info.bufferSize.x,
                y = info.bufferSize.y
            };
            return size;
        }


        public static void CopyArrayToScreen(ConsChar[,] buffer, ConsolePos destPos)
        {
            short width = (short)buffer.GetLength(0);
            short height = (short)buffer.GetLength(1);

            Native.SMALL_RECT rect = new Native.SMALL_RECT()
            {
                Top = destPos.y,
                Left = destPos.x,
                Right = (short)(destPos.x + width - 1),
                Bottom = (short)(destPos.y + height - 1)
            };

            Native.CHAR_INFO[] outBuffer = new Native.CHAR_INFO[buffer.Length];
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    Native.wAttribute attribute = ColorsToWAttribute(buffer[i, j].TextColor, buffer[i, j].BgColor);

                    Native.CHAR_INFO info = new Native.CHAR_INFO()
                    {
                        c = (short)buffer[i, j].Character,
                        attr = attribute
                    };
                    outBuffer[i + buffer.GetLength(0) * j] = info;
                }
            }

            Native.COORD outBufferDim = new Native.COORD() { x = width, y = height };

            Native.WriteConsoleOutputW(outputHandle, outBuffer, outBufferDim, new Native.COORD() { x = 0, y = 0 }, ref rect);
        }

        public static void Write(string text)
        {
            int written = -1;
            Native.WriteConsole(outputHandle, text, text.Length, ref written, 0);
        }

        public static void Write(string text, ConsolePos pos)
        {
            int written = -1;
            Native.WriteConsoleOutputCharacter(outputHandle, text, text.Length, ConsPosToCOORDS(pos), ref written);
        }

        public static void Write(string text, ConsolePos pos, ConsoleColor textColor, ConsoleColor backgroundColor)
        {
            Write(text, pos);
            SetColor(text.Length, pos, textColor, backgroundColor);
        }

        public static void SetColor(int length, ConsolePos pos, ConsoleColor textColor, ConsoleColor backgroundColor)
        {
            int written = -1;
            Native.wAttribute attr = ColorsToWAttribute(textColor, backgroundColor);
            Native.FillConsoleOutputAttribute(outputHandle, attr, length, ConsPosToCOORDS(pos), ref written);
        }
    }
}
