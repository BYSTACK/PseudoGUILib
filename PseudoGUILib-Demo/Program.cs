using PseudoGUILib;
using PseudoGUILib.UI;
using PseudoGUILib.UI.Attributes;
using System;

namespace PseudoGUILib_Demo
{
    class Program
    {
        static string longtext = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. \nярусский\nEtiam consectetur leo eu lectus dictum, id aliquet est convallis. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Morbi eget faucibus diam. Maecenas in tincidunt felis, sed porta magna. Cras in ligula id nulla sollicitudin ornare fermentum et purus. Vivamus interdum urna eget felis viverra, vitae venenatis nisl feugiat. Aliquam sit amet scelerisque massa, vitae volutpat sem. Vivamus sit amet vehicula est. Sed nec neque et lacus ultrices imperdiet a placerat lacus. Maecenas metus est, imperdiet ut ullamcorper sed, convallis quis turpis. Etiam commodo nisi urna, tempus egestas quam ultrices eu. Proin aliquam lorem non augue venenatis efficitur. Sed maximus efficitur risus non feugiat.";
        static string text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. \nярусский\nEtiam consectetur leo eu lectus dictum, id aliquet est convallis.";

        public static void Main()
        {

            Application.Build += Build;

            Application.Initialize(200, 50, 24, true);

            while (true)
            {
                Application.StartUI();
                Console.WriteLine("hi hi");
                Console.ReadKey();
                //Application.Build -= Build;
                //Application.Initialize(100, 100, 4, true);
            }
        }

        private static void Build(Window window)
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
            right.Padding = new ThicknessRectangle(1);

            TextBlock block = new TextBlock();
            block.Text = text;
            block.Parent = left;
            block.Id = "left";

            TextBlock block2 = new TextBlock();
            block2.Text = longtext;
            block2.Parent = middle;
            block2.Id = "mid";

            Button but = new Button();
            but.Parent = right;
            but.Text = "ClickMe!";
            but.Width = 8;
            but.Click += (sender) =>
            {
                Application.StopUI();
            };
        }
    }
}
