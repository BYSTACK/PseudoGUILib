using System;
using System.Text;

namespace PseudoGUILib.UI.Attributes
{
    public class TextBlock : UIElement
    {
        private string text = "";
        public string Text
        {
            get => text; set
            {
                if (text != value)
                {
                    text = value;
                    FormatText();
                }
            }
        }

        private string formattedTextCache;

        internal override void Draw(Renderer renderer, Rectangle screenPortion)
        {
            renderer.DrawText(new Rectangle(X, Y, width, height), screenPortion, formattedTextCache, ConsoleColor.Black, null);
        }

        protected override void ProcessNewChild(UIElement element)
        {
            throw new Exception("TextBlock cannot have children");
        }

        protected override void RecalculatePositionSize()
        {
            base.RecalculatePositionSize();
            FormatText();
        }

        private void FormatText()
        {
            if (Text.Length == 0 || width <= 0 || height <= 0)
            {
                formattedTextCache = "";
                return;
            }

            //account for newlines
            string[] logicalLines = text.Split('\n');
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < logicalLines.Length && i < height; i++)
            {
                builder.Append(logicalLines[i]).Append(new string(' ', (int)Math.Ceiling(logicalLines[i].Length / (double)width) * width - logicalLines[i].Length));
            }
            formattedTextCache = builder.ToString();
        }
    }
}
