using PseudoGUILib;
using PseudoGUILib.UI;
using PseudoGUILib.UI.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace PseudoGUILib_Demo
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        public int X;
        public int Y;
    }

    class Program
    {
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point lpPoint);

        public static Point GetCursorPosition()
        {
            Point lpPoint;
            GetCursorPos(out lpPoint);
            return lpPoint;
        }


        public static void Main()
        {
            string longtext = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. \nярусский\nEtiam consectetur leo eu lectus dictum, id aliquet est convallis. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Morbi eget faucibus diam. Maecenas in tincidunt felis, sed porta magna. Cras in ligula id nulla sollicitudin ornare fermentum et purus. Vivamus interdum urna eget felis viverra, vitae venenatis nisl feugiat. Aliquam sit amet scelerisque massa, vitae volutpat sem. Vivamus sit amet vehicula est. Sed nec neque et lacus ultrices imperdiet a placerat lacus. Maecenas metus est, imperdiet ut ullamcorper sed, convallis quis turpis. Etiam commodo nisi urna, tempus egestas quam ultrices eu. Proin aliquam lorem non augue venenatis efficitur. Sed maximus efficitur risus non feugiat.";
            string text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. \nярусский\nEtiam consectetur leo eu lectus dictum, id aliquet est convallis.";

            Manager.CreateWindow(200, 50, 24, (window) =>
            {
                Border border = new Border();
                window.AppendChild(border);

                StackPanel panel = new StackPanel();
                border.AppendChild(panel);
                panel.Orientation = Orientation.Vertical;
                panel.Overlap = true;

                Border header = new Border();
                header.Height = 10;
                panel.AppendChild(header);

                StackPanel menu = new StackPanel();
                menu.Id = "menu";
                menu.Orientation = Orientation.Horizontal;
                menu.Overlap = true;
                panel.AppendChild(menu);

                Border left = new Border();
                left.Width = 30;
                left.Padding = new ThicknessRectangle(1);
                menu.AppendChild(left);

                Border middle = new Border();
                middle.Parent = menu;
                middle.Padding = new ThicknessRectangle(5);

                Border right = new Border();
                right.Width = 20;
                right.Parent = menu;

                TextBlock block = new TextBlock();
                block.Text = text;
                block.Parent = left;

                TextBlock block2 = new TextBlock();
                block2.Text = longtext;
                block2.Parent = middle;

                int framec = 0;

                window.Update += () =>
                {
                    framec++;
                    panel.Width = GetCursorPosition().X / 8;
                    panel.Height = GetCursorPosition().Y / 16;

                    if (framec % 24 == 0)
                    {
                        /*if (panel.Orientation == Orientation.Horizontal)
                            panel.Orientation = Orientation.Vertical;
                        else
                            panel.Orientation = Orientation.Horizontal;*/
                    }
                    if (framec % 48 == 0)
                    {
                        //panel.Overlap = !panel.Overlap;
                    }
                };
            });


            Console.ReadKey();
        }
    }
}
