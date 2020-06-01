using System;

namespace PseudoGUILib
{
    public struct TwoPointRect
    {

        public int x;
        public int y;
        public int x2;
        public int y2;
        public TwoPointRect(int x, int y, int x2, int y2)
        {
            this.x = x;
            this.y = y;
            this.x2 = x2;
            this.y2 = y2;
        }
    }
    public struct Rectangle
    {
        public int x;
        public int y;
        public int width;
        public int height;

        public Rectangle(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public Rectangle Intersection(Rectangle other)
        {
            Rectangle result = new Rectangle();
            result.x = Math.Max(x, other.x);
            result.y = Math.Max(y, other.y);
            int x1 = Math.Min(x + width, other.x + other.width);
            int y1 = Math.Min(y + height, other.y + other.height);
            result.width = x1 - result.x;
            result.height = y1 - result.y;
            if (result.width < 0)
                result.width = 0;
            if (result.height < 0)
                result.height = 0;
            return result;
        }

    }
}
