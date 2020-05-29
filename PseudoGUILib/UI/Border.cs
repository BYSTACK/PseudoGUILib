using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudoGUILib.UI
{
    public class Border : UIElement
    {
        internal override void Draw(Renderer renderer, Rectangle screenPortion)
        {
            renderer.DrawBox(new Rectangle(X, Y, Width, Height), screenPortion);
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
