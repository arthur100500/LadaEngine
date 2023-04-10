using LadaEngine.Engine.Base;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace LadaEngine.Engine.Global;

/// <summary>
/// Game window class
/// </summary>
public class Window : GameWindow
{
    /// <summary>
    /// Delegate to be called on fixed update
    /// </summary>
    public delegate void FixedUpdateFrameDelegate();

    /// <summary>
    /// Delegate to be called on load 
    /// </summary>
    public delegate void OnLoadDelegate();

    /// <summary>
    /// Delegate to be called on resize 
    /// </summary>
    public delegate void OnResizeDelegate();

    /// <summary>
    /// Delegate to be called on render 
    /// </summary>
    public delegate void RenderFrameDelegate();
    
    /// <summary>
    /// Delegate to be called on frame update
    /// </summary>
    public delegate void UpdateFrameDelegate();

    
    private double _dt;

    /// <summary>
    /// Refresh rate of FixedUpdate (currently 250hz)
    /// </summary>
    private readonly double _fixedTimeUpdateRate = 0.004;

    /// <summary>
    /// Creates new window instance
    /// </summary>
    /// <param name="gameWindowSettings"></param>
    /// <param name="nativeWindowSettings"></param>
    public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings) : base(
        gameWindowSettings, nativeWindowSettings)
    {
        GL.Enable(EnableCap.Texture2D);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
    }

    /// <summary>
    /// Event to be invoked on frame update
    /// </summary>
    public event UpdateFrameDelegate Update;
    
    /// <summary>
    /// Event to be invoked on fixed update
    /// </summary>
    public event UpdateFrameDelegate FixedUpdate;
    
    /// <summary>
    /// Event to be invoked on redner
    /// </summary>
    public event RenderFrameDelegate Render;
    
    /// <summary>
    /// Event to be invoked on load
    /// </summary>
    public new event OnLoadDelegate Load;
    
    /// <summary>
    /// Event to be invoked on resize
    /// </summary>
    public new event OnResizeDelegate Resize;

    /// <summary>
    ///     Window constructer
    /// </summary>
    /// <param name="width">Width of the window</param>
    /// <param name="height">Height of the window</param>
    /// <param name="title">Window title</param>
    public static Window Create(int width, int height, string title)
    {
        var nativeWindowSettings = new NativeWindowSettings
        {
            Size = new Vector2i(width, height),
            Title = title,
            // This is needed to run on macos
            Flags = ContextFlags.ForwardCompatible
        };

        var wnd = new Window(GameWindowSettings.Default, nativeWindowSettings);

        Misc.window = wnd;
        return wnd;
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
        if (GlobalOptions.FullDebug)
            Misc.Log("---- Frame begin ----");
        GL.Clear(ClearBufferMask.ColorBufferBit);
        // Render delegate
        Render?.Invoke();

        _dt += e.Time;
        while (_dt > _fixedTimeUpdateRate)
        {
            FixedUpdate?.Invoke();
            _dt -= _fixedTimeUpdateRate;
            if (_dt > 30)
                _dt = 0;
        }

        SwapBuffers();

        if (GlobalOptions.FullDebug)
            Misc.Log("---- Frame end ----");
    }


    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        Controls.Mouse = MouseState;
        Controls.Keyboard = KeyboardState;

        Controls.CursorPosition.X = 2 * Controls.Mouse.X / Misc.window.Size.X;
        Controls.CursorPosition.Y = 2 * Controls.Mouse.Y / Misc.window.Size.Y;

        Controls.ControlDirection.X = (Controls.Keyboard.IsKeyDown(Keys.D) ? 1 : 0) -
                                       (Controls.Keyboard.IsKeyDown(Keys.A) ? 1 : 0);
        Controls.ControlDirection.Y = (Controls.Keyboard.IsKeyDown(Keys.W) ? 1 : 0) -
                                       (Controls.Keyboard.IsKeyDown(Keys.S) ? 1 : 0);

        Controls.ControlDirectionF.X = (Controls.Keyboard.IsKeyDown(Keys.D) ? 1 : 0) -
                                         (Controls.Keyboard.IsKeyDown(Keys.A) ? 1 : 0);
        Controls.ControlDirectionF.Y = (Controls.Keyboard.IsKeyDown(Keys.W) ? 1 : 0) -
                                         (Controls.Keyboard.IsKeyDown(Keys.S) ? 1 : 0);

        if (Controls.Keyboard.IsKeyPressed(Keys.F11))
        {
            if (WindowBorder != WindowBorder.Hidden)
            {
                WindowBorder = WindowBorder.Hidden;
                WindowState = WindowState.Fullscreen;
            }
            else
            {
                WindowBorder = WindowBorder.Resizable;
                WindowState = WindowState.Normal;
            }
        }

        // Update Frame delegate
        Update?.Invoke();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        GL.Viewport(0, 0, Size.X, Size.Y);


        Misc.ScreenRatio = Size.X / (float)Size.Y;

        var xDim = 2 * 1920 / (float)Size.X - 1;
        var yDim = 2 * 1080 / (float)Size.Y - 1;
        Misc.FboSpriteCoords = new Pos(xDim, yDim);


        // Resize delegate
        Resize?.Invoke();
    }
}