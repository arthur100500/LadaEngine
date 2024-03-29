﻿using LadaEngine.Engine.Common;
using LadaEngine.Engine.Global;

namespace MovingCamera
{
    public class Game
    {
        private Camera _camera;
        private Window _window;
        private World _world;

        public Game()
        {
            _window = Window.Create(800, 800, "");
            
            _window.Title = "Moving Camera";
            
            _window.Render += RenderEvent;
            _window.Load += LoadEvent;
            _window.FixedUpdate += FixedUpdateEvent;

            _window.VSync = OpenTK.Windowing.Common.VSyncMode.On;
        }

        private void LoadEvent()
        {
            // Camera creation
            _camera = new();
            
            // Create velocity vector
            Random random = new();
            
            // Create the world
            _world = new();
        }
        
        private void RenderEvent()
        {
            // Render the world
            _world.Render(_camera);
        }

        private void FixedUpdateEvent()
        {
            // Move the camera with WASD keys
            _camera.Position += Controls.ControlDirectionF * 0.01f;
        }

        public void Run()
        {
            // Run the game
            _window.Run();
        }
    }
}
