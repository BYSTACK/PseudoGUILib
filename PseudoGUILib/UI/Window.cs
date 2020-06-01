using System;

namespace PseudoGUILib.UI
{
    public class Window : UIElement
    {
        public Window(int width, int height)
        {
            SetSize(width, height);
        }

        internal void SetSize(int width, int height)
        {
            this.width = width;
            this.height = height;
            OnPositionSizeChanged(this);
        }

        protected override void ProcessNewChild(UIElement element)
        {
            if (children.Count >= 1)
                throw new Exception("Window must have one child at most");
        }

        internal override void Draw(Renderer renderer, Rectangle screenPortion)
        {
            foreach (var child in children)
                child.Draw(renderer, new Rectangle(X, Y, width, height));
        }

        protected override void RecalculatePositionSize()
        {
        }
    }
}
