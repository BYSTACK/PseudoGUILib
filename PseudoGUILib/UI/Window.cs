using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudoGUILib.UI
{
    public class Window : UIElement
    {
        public delegate void UpdateHandler();
        public event UpdateHandler Update;

        private Renderer renderer;
        public Window(Renderer renderer)
        {
            this.renderer = renderer;
            RecalculatePositionSize();
        }

        protected override void ProcessNewChild(UIElement element)
        {
            if (children.Count >= 1)
                throw new Exception("Window must have one child at most");
        }

        internal override void Draw(Renderer renderer, Rectangle screenPortion)
        {
            Update?.Invoke();
            foreach (var child in children)
                child.Draw(renderer, new Rectangle(X, Y, width, height));
        }

        protected override void RecalculatePositionSize()
        {
            X = 0;
            Y = 0;
            width = renderer.Width;
            height = renderer.Height;
            OnPositionSizeChanged(this);
        }
    }
}
