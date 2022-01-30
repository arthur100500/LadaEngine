using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;

namespace LadaEngine
{
    public class Controls
    {
        private static List<Key> keys_pressed = new List<Key>();
        private static List<MouseButton> mouse_pressed = new List<MouseButton>();

        /// <summary>
        /// Global object MouseState mouse
        /// </summary>
        public static MouseState mouse;

        /// <summary>
        /// Global object KeyboardState keyboard
        /// </summary>
        public static KeyboardState keyboard;

        /// <summary>
        /// Position of cursor
        /// </summary>
        public static FPos cursor_position = new FPos(0, 0);

        /// <summary>
        /// Vector of player movement tracked with WASD keys
        /// </summary>
        public static Pos control_direction = new Pos(0, 0);

        /// <summary>
        /// Check if the button was only pressed once
        /// </summary>
        /// <param name="key"></param>
        /// <returns>If button was pressed once</returns>
        public static bool ButtonPressedOnce(Key key)
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
        /// Check if the button was only pressed once
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
}
