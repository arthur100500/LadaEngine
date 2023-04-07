using LadaEngine.Engine.Base;
using LadaEngine.Engine.Common;
using LadaEngine.Engine.Common.SpriteGroup;
using LadaEngine.Engine.Global;
using LadaEngine.Engine.Renderables;
using LadaEngine.Engine.Renderables.GroupRendering;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Pong.Elements;
using Window = LadaEngine.Engine.Global.Window;


namespace Pong
{
    public class Game
    {
        private Camera _camera;
        private ITextureAtlas _atlas;
        private SpriteGroup _world;
        private Window _window;

        private Ball _ball;
        private Board _leftBoard;
        private Board _rightBoard;

        public Game()
        {
            _window = Window.Create(800, 800, "");
            
            _window.Title = "Moving Image";
            
            _window.Render += RenderEvent;
            _window.Load += LoadEvent;
            _window.FixedUpdate += FixedUpdateEvent;

            _window.VSync = OpenTK.Windowing.Common.VSyncMode.On;
        }

        private void LoadEvent()
        {
            // Create atlas and camera
            _atlas = new TextureAtlas(new List<string> {
                "Textures/board.png",
                "Textures/ball.png"
            });
            _camera = new();
            // Load sprite group
            _world = new SpriteGroup(_atlas);
            
            // Create game elements
            _leftBoard = new Board(_world, new Pos(-0.9f, 0), Keys.W, Keys.S);
            _rightBoard = new Board(_world, new Pos(0.9f, 0), Keys.I, Keys.K);
            _ball = new Ball(_world, _leftBoard, _rightBoard);
        }
        
        private void RenderEvent()
        {
            // Render the world with the sprite
            _world.Render(_camera);
            
            // Recalculate vertices of world. If the world is big and has a lot of sprites, this operation will be costly.
            // It is recommended to update it not often, and better give the coordinates via camera position to shader
            _world.Update();
            
            // If player pressed R place ball in the center
            if (Controls.keyboard.IsKeyPressed(Keys.R))
                _ball.SetPosition(Pos.Zero);
        }

        private void FixedUpdateEvent()
        {
            _ball.FixedUpdate();
            _leftBoard.FixedUpdate();
            _rightBoard.FixedUpdate();
        }

        public void Run()
        {
            // Run the game
            _window.Run();
        }
    }
}