using LadaEngine.Engine.Base;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Window = LadaEngine.Engine.Global.Window;

namespace LadaEngine.Engine.Global;

public class Controls
{
    private static readonly List<Keys> keys_pressed = new();
    private static readonly List<MouseButton> mouse_pressed = new();

    public static Window window;

    /// <summary>
    ///     Global object MouseState mouse
    /// </summary>
    public static MouseState mouse;

    /// <summary>
    ///     Global object KeyboardState keyboard
    /// </summary>
    public static KeyboardState keyboard;

    /// <summary>
    ///     Position of cursor
    /// </summary>
    public static Pos cursor_position = new(0, 0);

    /// <summary>
    ///     Vector of player movement tracked with WASD keys
    /// </summary>
    public static IntPos control_direction = new(0, 0);

    /// <summary>
    ///     Vector of player movement tracked with WASD keys (but in float)
    /// </summary>
    public static Pos control_direction_f = new(0, 0);

    /// <summary>
    ///     Check if the button was only pressed once
    /// </summary>
    /// <param name="key"></param>
    /// <returns>If button was pressed once</returns>
    public static bool ButtonPressedOnce(Keys key)
    {
        // first time
        if (!keys_pressed.Contains(key) && keyboard.IsKeyDown(key))
        {
            keys_pressed.Add(key);
            return true;
        }

        if (keys_pressed.Contains(key) && !keyboard.IsKeyDown(key))
            keys_pressed.Remove(key);

        return false;
    }

    /// <summary>
    ///     Check if the button was only pressed once
    /// </summary>
    /// <param name="key"></param>
    /// <returns>If button was pressed once</returns>
    public static bool MouseButtonPressedOnce(MouseButton btn)
    {
        // first time
        if (!mouse_pressed.Contains(btn) && mouse.IsButtonDown(btn))
        {
            mouse_pressed.Add(btn);
            return true;
        }

        if (mouse_pressed.Contains(btn) && !mouse.IsButtonDown(btn))
            mouse_pressed.Remove(btn);

        return false;
    }
}