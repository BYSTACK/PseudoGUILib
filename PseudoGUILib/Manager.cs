using PseudoGUILib.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PseudoGUILib
{
    public static class Manager
    {
        private static bool started = false;
        private static Window window;
        private static Renderer renderer;
        private static Thread thread;

        private static int width;
        private static int height;
        private static int fps;
        private static Action<Window> startAction;

        public static void CreateWindow(int width, int height, int fps, Action<Window> startAction)
        {
            if (!started)
            {
                Manager.width = width;
                Manager.height = height;
                Manager.fps = fps;
                Manager.startAction = startAction;
                Start();
            }
        }

        private static void Start()
        {
            started = true;
            renderer = new Renderer(width, height);
            window = new Window(renderer);
            thread = new Thread(Loop);
            thread.Start();
        }

        public static void Stop()
        {
            started = false;
            thread.Join();
        }

        private static void Loop()
        {
            long msPerFrame = 1000 / fps;
            startAction(window);
            while (started)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                while (true)
                {
                    renderer.Clear();
                    window.Draw(renderer, new Rectangle());
                    renderer.Display();

                    long currentTime = sw.ElapsedMilliseconds;
                    long nextFrameTime = ((currentTime / msPerFrame) + 1) * msPerFrame;
                    Thread.Sleep((int)(nextFrameTime - currentTime));
                }
            }
        }

    }
}
