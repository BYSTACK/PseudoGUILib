using FastConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PseudoGUILib
{
    public class Renderer
    {
        private char[,] curScreenBuffer;
        private char[,] newScreenBuffer;
        public int Width { get; private set; }
        public int Height { get; private set; }

        private static class Box
        {
            public const char HORIZONTAL = '\u2500';
            public const char VERTICAL = '\u2502';
            public const char DOWN_RIGHT = '\u250c';
            public const char DOWN_LEFT = '\u2510';
            public const char UP_RIGHT = '\u2514';
            public const char UP_LEFT = '\u2518';
            public const char VERT_RIGHT = '\u251c';
            public const char VERT_LEFT = '\u2524';
            public const char HOR_DOWN = '\u252c';
            public const char HOR_UP = '\u2534';
            public const char CROSS = '\u253c';

            public static bool HasLeft(char c)
            {
                return c == HORIZONTAL || c == DOWN_LEFT || c == UP_LEFT || c == HOR_DOWN || c == HOR_UP || c == VERT_LEFT || c == CROSS;
            }

            public static bool HasRight(char c)
            {
                return c == HORIZONTAL || c == DOWN_RIGHT || c == UP_RIGHT || c == HOR_DOWN || c == HOR_UP || c == VERT_RIGHT || c == CROSS;
            }

            public static bool HasTop(char c)
            {
                return c == VERTICAL || c == UP_LEFT || c == UP_RIGHT || c == VERT_LEFT || c == VERT_RIGHT || c == HOR_UP || c == CROSS;
            }

            public static bool HasBottom(char c)
            {
                return c == VERTICAL || c == DOWN_LEFT || c == DOWN_RIGHT || c == VERT_LEFT || c == VERT_RIGHT || c == HOR_DOWN || c == CROSS;
            }
        }

        public Renderer(int width, int height)
        {
            if (width > Console.LargestWindowWidth)
                width = Console.LargestWindowWidth;
            if (height > Console.LargestWindowHeight)
                height = Console.LargestWindowHeight;
            Console.WindowWidth = width;
            Console.WindowHeight = height;
            Width = Console.WindowWidth;
            Height = Console.WindowHeight;
            curScreenBuffer = new char[Width, Height];
            newScreenBuffer = new char[Width, Height];
            Console.CursorVisible = false;
            Console.BufferWidth = Width;
            Console.BufferHeight = Height;
            //NativeWindows.DisableConsoleResize();
        }

        public void Display()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (newScreenBuffer[i, j] != curScreenBuffer[i, j])
                    {
                        curScreenBuffer[i, j] = newScreenBuffer[i, j];
                        FConsole.Write(new string(newScreenBuffer[i, j], 1), new ConsolePos() { x = (short)i, y = (short)j });
                    }
                }
            }
        }

        public void Clear()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    newScreenBuffer[i, j] = ' ';
                }
            }
        }

        public void DrawChar(char character, int x, int y)
        {
            if (x >= Width || x < 0 || y >= Height || y < 0)
                return;
            newScreenBuffer[x, y] = character;
        }

        private bool Inside(Rectangle screenPortion, int x, int y)
        {
            return !(x >= screenPortion.x + screenPortion.width || x < screenPortion.x || y >= screenPortion.y + screenPortion.height || y < screenPortion.y) &&
                    !(x < 0 || y < 0 || x >= Width || y >= Height);
        }

        public void DrawBox(Rectangle box, Rectangle screenPortion)
        {
            if (box.width <= 1 || box.height <= 1)
                return;
            if (Inside(screenPortion, box.x, box.y))
                newScreenBuffer[box.x, box.y] = AddBox(newScreenBuffer[box.x, box.y], Box.DOWN_RIGHT);
            if (Inside(screenPortion, box.x + box.width - 1, box.y))
                newScreenBuffer[box.x + box.width - 1, box.y] = AddBox(newScreenBuffer[box.x + box.width - 1, box.y], Box.DOWN_LEFT);
            if (Inside(screenPortion, box.x, box.y + box.height - 1))
                newScreenBuffer[box.x, box.y + box.height - 1] = AddBox(newScreenBuffer[box.x, box.y + box.height - 1], Box.UP_RIGHT);
            if (Inside(screenPortion, box.x + box.width - 1, box.y + box.height - 1))
                newScreenBuffer[box.x + box.width - 1, box.y + box.height - 1] = AddBox(newScreenBuffer[box.x + box.width - 1, box.y + box.height - 1], Box.UP_LEFT);

            for (int i = 1; i < box.width - 1; i++)
            {
                if (Inside(screenPortion, box.x + i, box.y))
                    newScreenBuffer[box.x + i, box.y] = AddBox(newScreenBuffer[box.x + i, box.y], Box.HORIZONTAL);
                if (Inside(screenPortion, box.x + i, box.y + box.height - 1))
                    newScreenBuffer[box.x + i, box.y + box.height - 1] = AddBox(newScreenBuffer[box.x + i, box.y + box.height - 1], Box.HORIZONTAL);
            }

            for (int i = 1; i < box.height - 1; i++)
            {
                if (Inside(screenPortion, box.x, box.y + i))
                    newScreenBuffer[box.x, box.y + i] = AddBox(newScreenBuffer[box.x, box.y + i], Box.VERTICAL);
                if (Inside(screenPortion, box.x + box.width - 1, box.y + i))
                    newScreenBuffer[box.x + box.width - 1, box.y + i] = AddBox(newScreenBuffer[box.x + box.width - 1, box.y + i], Box.VERTICAL);
            }
        }

        public void DrawText(Rectangle rect, Rectangle screenPortion, string formattedText)
        {
            if (formattedText.Length == 0 || rect.width <= 0 || rect.height <= 0 || screenPortion.width <= 0 || screenPortion.height <= 0)
                return;

            Rectangle trueArea = rect.Intersection(screenPortion).Intersection(new Rectangle(0, 0, Width, Height));

            int leftTrim = rect.x >= trueArea.x ? 0 : trueArea.x - rect.x;
            int topTrim = rect.y >= trueArea.y ? 0 : trueArea.y - rect.y;
            int rightTrim = (rect.x + rect.width - 1 < trueArea.x + trueArea.width) ? 0 : rect.x + rect.width - trueArea.width - trueArea.x;
            int bottomTrim = (rect.y + rect.height - 1 < trueArea.y + trueArea.height) ? 0 : rect.y + rect.height - trueArea.height - trueArea.y;

            if (leftTrim >= rect.width || rightTrim >= rect.width || topTrim >= rect.height || bottomTrim >= rect.height)
                return;

            List<string> lines = new List<string>();
            int actLinesCount = formattedText.Length / rect.width;
            for (int i = 0; i < rect.height && i < actLinesCount; i++)
            {
                lines.Add(formattedText.Substring(i * rect.width, rect.width));
            }

            //remove lines on top and bottom
            lines = lines.GetRange(topTrim, Math.Min(rect.height - bottomTrim - topTrim, lines.Count - topTrim));

            //remove character on left and right
            for (int i = 0; i < lines.Count; i++)
            {
                lines[i] = lines[i].Substring(leftTrim, Math.Min(rect.width - leftTrim - rightTrim, lines[i].Length - leftTrim));
            }

            //draw
            for (int i = 0; i < lines.Count; i++)
            {
                for (int j = 0; j < lines[i].Length; j++)
                {
                    newScreenBuffer[rect.x + leftTrim + j, rect.y + topTrim + i] = lines[i][j];
                }
            }
        }

        private char AddBox(char first, char second)
        {
            bool left = Box.HasLeft(first) || Box.HasLeft(second);
            bool right = Box.HasRight(first) || Box.HasRight(second);
            bool up = Box.HasTop(first) || Box.HasTop(second);
            bool down = Box.HasBottom(first) || Box.HasBottom(second);
            int value = 0;
            value |= up ? (1 << 3) : 0;
            value |= right ? (1 << 2) : 0;
            value |= down ? (1 << 1) : 0;
            value |= left ? 1 : 0;

            switch (value)
            {
                default:
                case 0b0000:
                case 0b0001:
                case 0b0010:
                case 0b0100:
                case 0b1000: return ' ';
                case 0b0011: return Box.DOWN_LEFT;
                case 0b0101: return Box.HORIZONTAL;
                case 0b1001: return Box.UP_LEFT;
                case 0b0110: return Box.DOWN_RIGHT;
                case 0b1010: return Box.VERTICAL;
                case 0b1100: return Box.UP_RIGHT;
                case 0b0111: return Box.HOR_DOWN;
                case 0b1011: return Box.VERT_LEFT;
                case 0b1101: return Box.HOR_UP;
                case 0b1110: return Box.VERT_RIGHT;
                case 0b1111: return Box.CROSS;
            }
        }
    }
}
