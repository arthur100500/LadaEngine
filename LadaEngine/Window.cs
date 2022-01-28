using OpenTK.Graphics.OpenGL;
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using OpenTK.Input;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;

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
        public Window(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.AlphaTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            Misc.window = this;
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(0.3f, 0.2f, 0.2f, 0.0f);

            // Load Delegate
            Load?.Invoke();

            base.OnLoad(e);
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
            Controls.mouse = Mouse.GetCursorState();
            Controls.keyboard = Keyboard.GetState();

            Controls.cursor_position.X = 2 * (-Location.X + Controls.mouse.X - 8) / (float)Misc.window.Width;
            Controls.cursor_position.Y = 2 * (-Location.Y + Controls.mouse.Y - 30) / (float)Misc.window.Height;

            Controls.control_direction.X = (Controls.keyboard.IsKeyDown(Key.D) ? 1 : 0) - (Controls.keyboard.IsKeyDown(Key.A) ? 1 : 0);
            Controls.control_direction.Y = (Controls.keyboard.IsKeyDown(Key.W) ? 1 : 0) - (Controls.keyboard.IsKeyDown(Key.S) ? 1 : 0);

            if (Controls.keyboard.IsKeyDown(Key.F11))
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

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);

            Misc.screen_ratio = Height / (float)Width;

            // Resize delegate
            Resize?.Invoke();


            base.OnResize(e);
        }
    }
}