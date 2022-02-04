using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using GL = OpenTK.Graphics.OpenGL4.GL;


namespace LadaEngine
{
    public class Window : GameWindow
    {
        // Events
        // Update event
        public delegate void UpdateFrameDelegate();
        public event UpdateFrameDelegate Update;

        // Render event
        public delegate void RenderFrameDelegate();
        public event RenderFrameDelegate Render;

        // Load event
        public delegate void OnLoadDelegate();
        public event OnLoadDelegate Load;

        // Resize event
        public delegate void OnResizeDelegate();
        public event OnResizeDelegate Resize;


        /// <summary>
        /// Window constructer
        /// </summary>
        /// <param name="width">Width of the window</param>
        /// <param name="height">Height of the window</param>
        /// <param name="title">Window title</param>
        public static Window Create(int width, int height, string title)
        {
            var nativeWindowSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(width, height),
                Title = title,
                // This is needed to run on macos
                Flags = ContextFlags.ForwardCompatible,
            };

            Window wnd = new Window(GameWindowSettings.Default, nativeWindowSettings);

            Misc.window = wnd;
            return wnd;
        }

        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(gameWindowSettings, nativeWindowSettings)
        {
            GL.Enable(EnableCap.Texture2D);
            //GL.Enable(EnableCap.AlphaTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        protected override void OnLoad()
        {
            GL.ClearColor(0.3f, 0.2f, 0.2f, 0.0f);

            // Load Delegate
            Load?.Invoke();

            base.OnLoad();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            // Render delegate
            Render?.Invoke();

            SwapBuffers();
            base.OnRenderFrame(e);
        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            Controls.mouse = MouseState;
            Controls.keyboard = KeyboardState;

            Controls.cursor_position.X = 2 * (Controls.mouse.X) / (float)Misc.window.Size.X;
            Controls.cursor_position.Y = 2 * (Controls.mouse.Y) / (float)Misc.window.Size.Y;

            Controls.control_direction.X = (Controls.keyboard.IsKeyDown(Keys.D) ? 1 : 0) - (Controls.keyboard.IsKeyDown(Keys.A) ? 1 : 0);
            Controls.control_direction.Y = (Controls.keyboard.IsKeyDown(Keys.W) ? 1 : 0) - (Controls.keyboard.IsKeyDown(Keys.S) ? 1 : 0);

            if (Controls.keyboard.IsKeyDown(Keys.F11))
            {
                if (this.WindowBorder != WindowBorder.Hidden)
                {
                    this.WindowBorder = WindowBorder.Hidden;
                    this.WindowState = WindowState.Fullscreen;
                }
                else
                {
                    this.WindowBorder = WindowBorder.Resizable;
                    this.WindowState = WindowState.Normal;
                }
            }

            // Update Frame delegate
            Update?.Invoke();

            base.OnUpdateFrame(e);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, Size.X, Size.Y);

            Misc.screen_ratio = Size.X / (float)Size.Y;

            // Resize delegate
            Resize?.Invoke();


            base.OnResize(e);
        }
    }
}