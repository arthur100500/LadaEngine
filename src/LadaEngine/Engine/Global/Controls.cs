using LadaEngine.Engine.Base;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace LadaEngine.Engine.Global;

/// <summary>
///     Has all the controls, with available keyboard, mouse and some more features
/// </summary>
public class Controls
{
    private static readonly List<Keys> KeysPressed = new();
    private static readonly List<MouseButton> MousePressed = new();

    /// <summary>
    ///     Global object MouseState mouse
    /// </summary>
    public static MouseState Mouse;

    /// <summary>
    ///     Global object KeyboardState keyboard
    /// </summary>
    public static KeyboardState Keyboard;

    /// <summary>
    ///     Position of cursor
    /// </summary>
    public static Pos CursorPosition = new(0, 0);

    /// <summary>
    ///     Vector of player movement tracked with WASD keys
    /// </summary>
    public static IntPos ControlDirection = new(0, 0);

    /// <summary>
    ///     Vector of player movement tracked with WASD keys (but in float)
    /// </summary>
    public static Pos ControlDirectionF = new(0, 0);

    /// <summary>
    ///     Check if the button was only pressed once
    /// </summary>
    /// <param name="key"></param>
    /// <returns>If button was pressed once</returns>
    public static bool ButtonPressedOnce(Keys key)
    {
        return Keyboard.IsKeyPressed(key);
    }

    /// <summary>
    ///     Check if the button was only pressed once
    /// </summary>
    /// <param name="btn"></param>
    /// <returns>If button was pressed once</returns>
    public static bool MouseButtonPressedOnce(MouseButton btn)
    {
        return Mouse.IsButtonPressed(btn);
    }
}