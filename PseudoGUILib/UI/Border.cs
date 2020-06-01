using System;

namespace PseudoGUILib.UI
{
    public class Border : UIElement
    {
        public ConsoleColor BorderColor { get; set; } = ConsoleColor.White;
        public ConsoleColor FillColor { get; set; } = ConsoleColor.Gray;
        public bool EnableFill { get; set; } = true;

        internal override void Draw(Renderer renderer, Rectangle screenPortion)
        {
            if (EnableFill)
                renderer.DrawBox(new Rectangle(X, Y, Width, Height), screenPortion, BorderColor, FillColor);
            else
                renderer.DrawBox(new Rectangle(X, Y, Width, Height), screenPortion, BorderColor, null);

            foreach (var child in children)
                child.Draw(renderer, screenPortion.Intersection(GetChildContainer(child)));
        }

        protected override void ProcessNewChild(UIElement element)
        {
            if (children.Count >= 1)
                throw new Exception("Border must have one child at most");
        }
    }
}
