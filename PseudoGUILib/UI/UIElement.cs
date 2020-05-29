using PseudoGUILib.UI.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PseudoGUILib.UI
{
    abstract public class UIElement
    {
        public IReadOnlyList<UIElement> Children { get => children; }

        private UIElement parent;
        public UIElement Parent
        {
            get => parent;
            set
            {
                if (value != null)
                    value.AppendChild(this);
            }
        }

        public string Id { get; set; }

        protected int width = 0;
        public int Width
        {
            get => width;
            set
            {
                if (width != value || widthMode != SizeMode.Fixed)
                {
                    width = value;
                    if (widthMode != SizeMode.Fixed)
                        WidthMode = SizeMode.Fixed;
                    else
                        RecalculatePositionSize();
                }
            }
        }

        protected int height = 0;
        public int Height
        {
            get => height;
            set
            {
                if (height != value || heightMode != SizeMode.Fixed)
                {
                    height = value;
                    if (heightMode != SizeMode.Fixed)
                        HeightMode = SizeMode.Fixed;
                    else
                        RecalculatePositionSize();
                }
            }
        }

        private ThicknessRectangle margin = new ThicknessRectangle(0, 0, 0, 0);
        public ThicknessRectangle Margin
        {
            get => margin; set
            {
                if (margin != value)
                {
                    margin = value;
                    RecalculatePositionSize();
                }
            }
        }

        private ThicknessRectangle padding = new ThicknessRectangle(0, 0, 0, 0);
        public ThicknessRectangle Padding
        {
            get => padding; set
            {
                if (padding != value)
                {
                    padding = value;
                    OnPositionSizeChanged(this);
                }
            }
        }

        private SizeMode widthMode = SizeMode.Auto;
        public SizeMode WidthMode { get => widthMode; set { widthMode = value; RecalculatePositionSize(); } }

        private SizeMode heightMode = SizeMode.Auto;
        public SizeMode HeightMode { get => heightMode; set { heightMode = value; RecalculatePositionSize(); } }

        public int X { get; protected set; }
        public int Y { get; protected set; }

        public delegate void UIEventHandler(UIElement sender);
        internal event UIEventHandler PositionSizeChanged;

        protected List<UIElement> children = new List<UIElement>();

        public void AppendChild(UIElement element)
        {
            ProcessNewChild(element);

            if (element.Parent != null)
                element.Parent.RemoveChild(element);
            element.parent = this;
            children.Add(element);

            PositionSizeChanged += element.ParentResizedOrMoved;
            element.ParentResizedOrMoved(this);

        }

        public void RemoveChild(UIElement element)
        {
            ProcessRemovedChild(element);
            element.parent = null;
            PositionSizeChanged -= element.ParentResizedOrMoved;
            children.Remove(element);
        }

        private void ParentResizedOrMoved(UIElement sender)
        {
            RecalculatePositionSize();
        }

        protected virtual Rectangle GetChildContainer(UIElement child)
        {
            Rectangle rect = new Rectangle();
            rect.x = X + Padding.left;
            rect.y = Y + Padding.top;
            rect.width = Width - Padding.left - Padding.right;
            rect.height = Height - Padding.top - Padding.bottom;
            return rect;
        }

        protected virtual void RecalculatePositionSize()
        {
            if (parent == null)
                return;
            Rectangle container = parent.GetChildContainer(this);
            int newX = container.x + Margin.left;
            int newY = container.y + Margin.top;
            int newWidth = widthMode == SizeMode.Auto ? container.width - Margin.left - Margin.right : width;
            int newHeight = heightMode == SizeMode.Auto ? container.height - Margin.top - Margin.bottom : height;
            if (newWidth < 0)
                newWidth = 0;
            if (newHeight < 0)
                newHeight = 0;

            width = newWidth;
            height = newHeight;
            X = newX;
            Y = newY;

            OnPositionSizeChanged(this);

        }

        protected void OnPositionSizeChanged(UIElement sender)
        {
            PositionSizeChanged?.Invoke(sender);
        }

        protected virtual void ProcessRemovedChild(UIElement element) { }

        protected virtual void ProcessNewChild(UIElement element) { }

        internal abstract void Draw(Renderer renderer, Rectangle screenPortion);

        public void ClearChildren()
        {
            children.Clear();
        }

        public override string ToString()
        {
            return (Id != null ? " " : "") + Id;
        }
    }
}
