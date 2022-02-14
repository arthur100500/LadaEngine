using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;

namespace LadaEngine
{
    public class Window : GameWindow
    {
        // Events
        // Update event
        public delegate void UpdateFrameDelegate();
        public event UpdateFrameDelegate Update;

        // Fixed Update event
        public delegate void FixedUpdateFrameDelegate();
        public event UpdateFrameDelegate FixedUpdate;

        // Render event
        public delegate void RenderFrameDelegate();
        public event RenderFrameDelegate Render;

        // Load event
        public delegate void OnLoadDelegate();
        public event OnLoadDelegate Load;

        // Resize event
        public delegate void OnResizeDelegate();
        public event OnResizeDelegate Resize;

        private double dt = 0;
        // Refresh rate of FixedUpdate func (every N ms) (Standart 250hz)
        public double fixed_time_update_rate = 0.004;
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
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);

            // Load Delegate
            Load?.Invoke();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            if (GlobalOptions.full_debug)
                Misc.Log("---- Frame begin ----");
            GL.Clear(ClearBufferMask.ColorBufferBit);
            // Render delegate
            Render?.Invoke();

            dt += e.Time;
            while (dt > fixed_time_update_rate) 
            {
                FixedUpdate?.Invoke();
                dt -= fixed_time_update_rate;
            }

            SwapBuffers();
            
            if (GlobalOptions.full_debug)
                Misc.Log("---- Frame end ----");
        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            Controls.mouse = MouseState;
            Controls.keyboard = KeyboardState;

            Controls.cursor_position.X = 2 * (Controls.mouse.X) / (float)Misc.window.Size.X;
            Controls.cursor_position.Y = 2 * (Controls.mouse.Y) / (float)Misc.window.Size.Y;

            Controls.control_direction.X = (Controls.keyboard.IsKeyDown(Keys.D) ? 1 : 0) - (Controls.keyboard.IsKeyDown(Keys.A) ? 1 : 0);
            Controls.control_direction.Y = (Controls.keyboard.IsKeyDown(Keys.W) ? 1 : 0) - (Controls.keyboard.IsKeyDown(Keys.S) ? 1 : 0);

            Controls.control_direction_f.X = (Controls.keyboard.IsKeyDown(Keys.D) ? 1 : 0) - (Controls.keyboard.IsKeyDown(Keys.A) ? 1 : 0);
            Controls.control_direction_f.Y = (Controls.keyboard.IsKeyDown(Keys.W) ? 1 : 0) - (Controls.keyboard.IsKeyDown(Keys.S) ? 1 : 0);

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
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);


            Misc.screen_ratio = Size.X / (float)Size.Y;

            float x_dim = 2 * 1920 / (float)Size.X - 1;
            float y_dim = 2 * 1080 / (float)Size.Y - 1;
            Misc.fbo_sprite_coords = new FPos(x_dim, y_dim);
            

            // Resize delegate
            Resize?.Invoke();
        }
    }
}