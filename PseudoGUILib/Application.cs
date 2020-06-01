using PseudoGUILib.UI;
using System;
using System.Diagnostics;
using System.Threading;

namespace PseudoGUILib
{
    public static class Application
    {
        private static bool started = false;
        private static bool initialized = false;

        private static uint initialConsoleMode;
        private static int initialWindowWidth;
        private static int initialWindowHeight;
        private static int initialBufferWidth;
        private static int initialBufferHeight;

        private static Window window;
        private static Renderer renderer;

        private static int width;
        private static int height;
        private static int fps;
        private static bool resizable;
        private static int consoleHandle;
        private static NativeWin.INPUT_RECORD[] records = new NativeWin.INPUT_RECORD[100];
        private static uint prevButtonState = 0;

        public delegate void BuildEventHandler(Window window);
        public delegate void StartEventHandler(Window window);
        public delegate void StopEventHandler(Window window);
        public static event BuildEventHandler Build;
        public static event StartEventHandler Start;
        public static event StopEventHandler Stop;

        public delegate void UpdateHandler();
        public static event UpdateHandler Update;

        /// <summary>
        /// Set window params. Can be called multiple times when UI isn't running
        /// </summary>
        public static void Initialize(int width, int height, int fps, bool resizable)
        {
            if (started)
            {
                throw new Exception("Can't initialize while app is running");
            }
            initialized = true;
            Application.width = width;
            Application.height = height;
            Application.fps = fps;
            Application.resizable = resizable;
            renderer = new Renderer(width, height);
            window = new Window(width, height);
            Build?.Invoke(window);
        }
        /// <summary>
        /// Set up console, display UI and start main loop
        /// </summary>
        public static void StartUI()
        {
            if (!initialized)
                throw new Exception("App needs to be initialized before start");
            if (started)
                throw new Exception("App is already running");
            started = true;
            SetupConsole();
            Start?.Invoke(window);
            Loop();
            Stop?.Invoke(window);
            RestoreConsoleState();
        }

        /// <summary>
        /// Restore console state and stop main loop
        /// </summary>
        public static void StopUI()
        {
            started = false;
        }

        private static void Loop()
        {
            long msPerFrame = 1000 / fps;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            while (started)
            {
                Update?.Invoke();
                PollEvents();

                renderer.Clear();
                window.Draw(renderer, new Rectangle());
                renderer.Display();

                long currentTime = sw.ElapsedMilliseconds;
                long nextFrameTime = ((currentTime / msPerFrame) + 1) * msPerFrame;
                Thread.Sleep((int)(nextFrameTime - currentTime));
            }
        }

        private static void SetupConsole()
        {
            initialBufferWidth = Console.BufferWidth;
            initialBufferHeight = Console.BufferHeight;
            initialWindowWidth = Console.WindowWidth;
            initialWindowHeight = Console.WindowHeight;
            SetConsoleSize();

            consoleHandle = FastConsole.Native.GetStdHandle(FastConsole.Native.nStdHandle.STD_INPUT_HANDLE);
            NativeWin.GetConsoleMode((IntPtr)consoleHandle, ref initialConsoleMode);

            NativeWin.SetConsoleMode(consoleHandle,
                (uint)(NativeWin.ConsoleInputMode.ENABLE_MOUSE_INPUT | NativeWin.ConsoleInputMode.ENABLE_EXTENDED_FLAGS | NativeWin.ConsoleInputMode.ENABLE_WINDOW_INPUT));

            ResizeFitConsole();

            if (!resizable)
            {
                NativeWin.SetConsoleResizeEnabled(false);
            }
        }

        private static void RestoreConsoleState()
        {
            NativeWin.SetConsoleMode(consoleHandle, initialConsoleMode);
            Console.WindowWidth = initialWindowWidth;
            Console.WindowHeight = initialWindowHeight;
            Console.BufferWidth = initialBufferWidth;
            Console.BufferHeight = initialBufferHeight;
            Console.Clear();
            NativeWin.SetConsoleResizeEnabled(true);

        }

        private static void SetConsoleSize()
        {
            if (width > Console.LargestWindowWidth)
                width = Console.LargestWindowWidth;
            if (height > Console.LargestWindowHeight)
                height = Console.LargestWindowHeight;

            if (width <= 0)
                width = 1;
            if (height <= 0)
                height = 1;

            Console.WindowWidth = width;
            Console.WindowHeight = height;
            Console.CursorVisible = false;
        }
        private static void ResizeFitConsole()
        {
            int counter = 0;
            while (counter < 3)
            {
                try
                {
                    Console.SetCursorPosition(0, 0);
                    width = Console.WindowWidth;
                    height = Console.WindowHeight;
                    Console.BufferWidth = width;
                    Console.BufferHeight = height;
                    Console.CursorVisible = false;
                }
                catch (Exception)
                {
                    counter++;
                    continue;
                }
                break;
            }

            renderer.Resize(width, height);
            window.SetSize(width, height);
        }

        private static void PollEvents()
        {
            int read = 0;
            int count = 0;


            NativeWin.GetNumberOfConsoleInputEvents(consoleHandle, ref count);
            while (count > 0)
            {
                NativeWin.ReadConsoleInput(consoleHandle, records, 100, ref read);
                for (int i = 0; i < read; i++)
                {
                    NativeWin.INPUT_RECORD e = records[i];
                    if (e.eventType == NativeWin.ConsoleEventType.WINDOW_BUFFER_SIZE_EVENT)
                    {
                        ResizeFitConsole();
                    }
                    else if (e.eventType == NativeWin.ConsoleEventType.KEY_EVENT)
                    {

                    }
                    else if (e.eventType == NativeWin.ConsoleEventType.MOUSE_EVENT)
                    {
                        if ((prevButtonState & 1) == 0 && (e.mouseEvent.dwButtonState & 1) == 1)
                        {
                            window.MouseEvent(new MouseArgs()
                            {
                                X = e.mouseEvent.dwMousePosition.X,
                                Y = e.mouseEvent.dwMousePosition.Y,
                                button = MouseButton.Left,
                                type = MouseEventType.Press,
                                inside = true
                            });
                        }
                        if ((prevButtonState & 1) == 1 && (e.mouseEvent.dwButtonState & 1) == 0)
                        {
                            window.MouseEvent(new MouseArgs()
                            {
                                X = e.mouseEvent.dwMousePosition.X,
                                Y = e.mouseEvent.dwMousePosition.Y,
                                button = MouseButton.Left,
                                type = MouseEventType.Release,
                                inside = true
                            });
                        }
                        prevButtonState = e.mouseEvent.dwButtonState;
                    }
                }
                NativeWin.GetNumberOfConsoleInputEvents(consoleHandle, ref count);
            }


        }

    }
}
