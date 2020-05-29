using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudoGUILib.UI.Attributes
{
    public struct ThicknessRectangle
    {
        public ThicknessRectangle(int hor, int vert) : this(hor, hor, vert, vert) { }
        public ThicknessRectangle(int thickness) : this(thickness, thickness, thickness, thickness) { }
        public ThicknessRectangle(int left, int right, int top, int bottom)
        {
            this.left = left;
            this.right = right;
            this.top = top;
            this.bottom = bottom;
        }
        public int left;
        public int right;
        public int top;
        public int bottom;

        public static bool operator ==(ThicknessRectangle p1, ThicknessRectangle p2)
        {
            return p1.left == p2.left && p1.right == p2.right && p1.top == p2.top && p1.bottom == p2.bottom;
        }

        public static bool operator !=(ThicknessRectangle p1, ThicknessRectangle p2)
        {
            return !(p1 == p2);
        }
    }
}
