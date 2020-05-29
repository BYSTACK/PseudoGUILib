using PseudoGUILib.UI.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudoGUILib.UI
{
    public class StackPanel : UIElement
    {
        private Orientation orientation;
        public Orientation Orientation
        {
            get => orientation;
            set
            {
                if (orientation != value)
                {
                    orientation = value;
                    UpdateLayout(true);
                }
            }
        }

        private bool overlap;
        public bool Overlap
        {
            get => overlap;
            set
            {
                if (value != overlap)
                {
                    overlap = value;
                    UpdateLayout(true);
                }
            }
        }

        private List<int> offsets = new List<int>();

        internal override void Draw(Renderer renderer, Rectangle screenPortion)
        {
            foreach (var child in Children)
                child.Draw(renderer, screenPortion.Intersection(GetChildContainer(child)));
        }

        protected override void ProcessNewChild(UIElement element)
        {
            element.PositionSizeChanged += OnChildPositionSizeChanged;
        }

        protected override void ProcessRemovedChild(UIElement element)
        {
            element.PositionSizeChanged -= OnChildPositionSizeChanged;
        }

        protected override Rectangle GetChildContainer(UIElement child)
        {
            int offset = 0;
            int index = children.IndexOf(child);
            if (index >= 0 && index < offsets.Count)
                offset = offsets[index];
            int nextOffset = index < offsets.Count - 1 ? offsets[index + 1] : -1;
            Rectangle baseRect = base.GetChildContainer(child);
            Rectangle copy = baseRect;
            if (orientation == Orientation.Vertical)
            {
                if (nextOffset == -1)
                    nextOffset = Height - Padding.top - Padding.bottom;
                baseRect.height = nextOffset - offset + (overlap ? 1 : 0);
                if (baseRect.height < 0)
                    baseRect.height = 0;
                baseRect.y = Y + Padding.top + offset;
            }
            else
            {
                if (nextOffset == -1)
                    nextOffset = Width - Padding.left - Padding.right;
                baseRect.width = nextOffset - offset + (overlap ? 1 : 0);
                if (baseRect.width < 0)
                    baseRect.width = 0;
                baseRect.x = X + Padding.left + offset;
            }
            baseRect = baseRect.Intersection(copy);
            return baseRect;
        }

        private void OnChildPositionSizeChanged(UIElement child)
        {
            UpdateLayout();
        }

        private void UpdateLayout(bool force = false)
        {
            int cumulOffset = 0;
            if (offsets.Capacity < children.Count)
                offsets.Capacity = children.Count;
            while (offsets.Count < children.Count)
                offsets.Add(0);

            int totalLength = 0;
            int autoChildren = 0;
            foreach (var child in children)
            {
                if (orientation == Orientation.Vertical)
                {
                    if (child.HeightMode == SizeMode.Auto)
                    {
                        autoChildren++;
                        if (autoChildren >= 2)
                            throw new Exception("Vertical StackPanel cannot contain more than 1 element with HeightMode set to Auto");
                        continue;
                    }
                }
                else
                {
                    if (child.WidthMode == SizeMode.Auto)
                    {
                        autoChildren++;
                        if (autoChildren >= 2)
                            throw new Exception("Horizontal StackPanel cannot contain  more than 1 element with WidthMode set to Auto");
                        continue;
                    }
                }

                if (orientation == Orientation.Vertical)
                    totalLength += child.Height + child.Margin.top + child.Margin.bottom;
                else
                    totalLength += child.Width + child.Margin.left + child.Margin.right;
                if (overlap)
                    totalLength--;
            }
            int autoChildLength = Math.Max(2, (Orientation == Orientation.Vertical ? height - Padding.top - Padding.bottom : width - Padding.left - Padding.right) - totalLength);
            //int autoChildLength = 2;

            bool changed = false;
            for (int i = 0; i < children.Count; i++)
            {

                if (offsets[i] != cumulOffset)
                    changed = true;
                offsets[i] = cumulOffset;

                if (orientation == Orientation.Vertical)
                {
                    if (children[i].HeightMode == SizeMode.Auto)
                        cumulOffset += autoChildLength;
                    else
                        cumulOffset += children[i].Height + children[i].Margin.top + children[i].Margin.bottom;
                }
                else
                {
                    if (children[i].WidthMode == SizeMode.Auto)
                        cumulOffset += autoChildLength;
                    else
                        cumulOffset += children[i].Width + children[i].Margin.left + children[i].Margin.right;
                }
                if (overlap)
                    cumulOffset--;
            }

            if (changed || force)
                OnPositionSizeChanged(this);
        }
    }
}
