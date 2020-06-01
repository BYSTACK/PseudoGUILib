using FastConsole;
using System;
using System.Collections.Generic;

namespace PseudoGUILib
{
    public class Renderer
    {

        private ConsChar[,] curScreenBuffer;
        private ConsChar[,] screenBuffer;
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
            Resize(width, height);
        }

        public void Resize(int width, int height)
        {
            Width = width;
            Height = height;
            screenBuffer = new ConsChar[Width, Height];
            curScreenBuffer = new ConsChar[Width, Height];
        }

        public void Display()
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (curScreenBuffer[i, j] != screenBuffer[i, j])
                    {
                        FConsole.Write(new string(screenBuffer[i, j].Character, 1), new ConsolePos() { x = (short)i, y = (short)j }, screenBuffer[i, j].TextColor, screenBuffer[i, j].BgColor);
                        curScreenBuffer[i, j] = screenBuffer[i, j];
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
                    screenBuffer[i, j].Character = ' ';
                    screenBuffer[i, j].BgColor = ConsoleColor.Black;
                    screenBuffer[i, j].TextColor = ConsoleColor.White;
                }
            }
        }


        private bool Inside(Rectangle screenPortion, int x, int y)
        {
            return !(x >= screenPortion.x + screenPortion.width || x < screenPortion.x || y >= screenPortion.y + screenPortion.height || y < screenPortion.y) &&
                    !(x < 0 || y < 0 || x >= Width || y >= Height);
        }

        public void DrawBox(Rectangle box, Rectangle screenPortion, ConsoleColor boxColor, ConsoleColor? fillColor)
        {
            if (box.width <= 1 || box.height <= 1)
                return;
            //draw corners
            if (Inside(screenPortion, box.x, box.y))
            {
                screenBuffer[box.x, box.y].Character = AddBox(screenBuffer[box.x, box.y].Character, Box.DOWN_RIGHT);
                screenBuffer[box.x, box.y].TextColor = boxColor;
            }
            if (Inside(screenPortion, box.x + box.width - 1, box.y))
            {
                screenBuffer[box.x + box.width - 1, box.y].Character = AddBox(screenBuffer[box.x + box.width - 1, box.y].Character, Box.DOWN_LEFT);
                screenBuffer[box.x + box.width - 1, box.y].TextColor = boxColor;
            }

            if (Inside(screenPortion, box.x, box.y + box.height - 1))
            {
                screenBuffer[box.x, box.y + box.height - 1].Character = AddBox(screenBuffer[box.x, box.y + box.height - 1].Character, Box.UP_RIGHT);
                screenBuffer[box.x, box.y + box.height - 1].TextColor = boxColor;
            }

            if (Inside(screenPortion, box.x + box.width - 1, box.y + box.height - 1))
            {
                screenBuffer[box.x + box.width - 1, box.y + box.height - 1].Character = AddBox(screenBuffer[box.x + box.width - 1, box.y + box.height - 1].Character, Box.UP_LEFT);
                screenBuffer[box.x + box.width - 1, box.y + box.height - 1].TextColor = boxColor;
            }

            //hor borders
            for (int i = 1; i < box.width - 1; i++)
            {
                if (Inside(screenPortion, box.x + i, box.y))
                {
                    screenBuffer[box.x + i, box.y].Character = AddBox(screenBuffer[box.x + i, box.y].Character, Box.HORIZONTAL);
                    screenBuffer[box.x + i, box.y].TextColor = boxColor;
                }
                if (Inside(screenPortion, box.x + i, box.y + box.height - 1))
                {
                    screenBuffer[box.x + i, box.y + box.height - 1].Character = AddBox(screenBuffer[box.x + i, box.y + box.height - 1].Character, Box.HORIZONTAL);
                    screenBuffer[box.x + i, box.y + box.height - 1].TextColor = boxColor;
                }
            }

            //ver borders
            for (int i = 1; i < box.height - 1; i++)
            {
                if (Inside(screenPortion, box.x, box.y + i))
                {
                    screenBuffer[box.x, box.y + i].Character = AddBox(screenBuffer[box.x, box.y + i].Character, Box.VERTICAL);
                    screenBuffer[box.x, box.y + i].TextColor = boxColor;
                }
                if (Inside(screenPortion, box.x + box.width - 1, box.y + i))
                {
                    screenBuffer[box.x + box.width - 1, box.y + i].Character = AddBox(screenBuffer[box.x + box.width - 1, box.y + i].Character, Box.VERTICAL);
                    screenBuffer[box.x + box.width - 1, box.y + i].TextColor = boxColor;
                }
            }

            //fill
            if (fillColor.HasValue)
            {
                for (int i = box.x; i < box.width + box.x && i < Width; i++)
                {
                    for (int j = box.y; j < box.height + box.y && j < Height; j++)
                    {
                        screenBuffer[i, j].BgColor = fillColor.Value;
                    }
                }
            }
        }
        public void DrawText(Rectangle rect, Rectangle screenPortion, string formattedText, ConsoleColor textColor, ConsoleColor? backgroundColor)
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
                    screenBuffer[rect.x + leftTrim + j, rect.y + topTrim + i].Character = lines[i][j];
                    screenBuffer[rect.x + leftTrim + j, rect.y + topTrim + i].TextColor = textColor;
                    if (backgroundColor.HasValue)
                        screenBuffer[rect.x + leftTrim + j, rect.y + topTrim + i].BgColor = backgroundColor.Value;
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
