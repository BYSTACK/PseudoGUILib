using System;

namespace PseudoGUILib.UI
{
    public class Button : UIElement
    {
        private bool pressInside = false;
        public delegate void ClickHandler(UIElement sender);
        public event ClickHandler Click;

        public ConsoleColor TextColor { get; set; } = ConsoleColor.White;
        public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.DarkBlue;

        public string Text { get; set; } = "";
        public Button()
        {
            Width = 8;
            Height = 1;
        }
        internal override void Draw(Renderer renderer, Rectangle screenPortion)
        {
            renderer.DrawText(new Rectangle(X, Y, width, height), screenPortion, Text, TextColor, BackgroundColor);
        }

        internal override void MouseEvent(MouseArgs args)
        {
            if (args.type == MouseEventType.Press)
                pressInside = args.inside;
            else if (args.type == MouseEventType.Release && pressInside && args.inside)
            {
                Click?.Invoke(this);
                pressInside = false;
            }
            else if (args.type == MouseEventType.Release)
            {
                pressInside = false;
            }
        }
    }
}
