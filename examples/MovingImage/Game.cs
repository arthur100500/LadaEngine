using LadaEngine.Engine.Base;
using LadaEngine.Engine.Common;
using LadaEngine.Engine.Common.SpriteGroup;
using LadaEngine.Engine.Global;
using LadaEngine.Engine.Renderables;
using LadaEngine.Engine.Renderables.GroupRendering;
using LadaEngine.Engine.Scene;


namespace MovingImage
{
    public class Game
    {
        private Camera _camera;
        private ITextureAtlas _atlas;
        private SpriteGroup _world;
        private Sprite _sprite;
        private Pos _velocity;
        private Window _window;

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
                "Textures/image.png",
            });
            _camera = new();
            
            // Create velocity vector
            Random random = new();
            _velocity = new(random.NextSingle() * 0.01f, random.NextSingle() * 0.01f);
            
            // Load sprite group
            _world = new SpriteGroup(_atlas);
            
            // Create sprite
            _sprite = new Sprite(Pos.Zero, _atlas, "Textures/image.png");
            _sprite.Width = _sprite.Height = 0.3f;
            
            // Add sprite to sprite group
            _world.AddSprite(_sprite);
        }
        
        private void RenderEvent()
        {
            // Render the world with the sprite
            _world.Render(_camera);
            
            // Recalculate vertices of world. If the world is big and has a lot of sprites, this operation will be costly.
            // It is recommended to update it not often, and better give the coordinates via camera position to shader
            _world.Update(_camera);
        }

        private void FixedUpdateEvent()
        {
            // Update sprite position (it will automatically update in the world as it is a reference)
            _sprite.Position += _velocity;

            // Make object not go offscreen
            if (MathF.Abs(_sprite.Position.X) > 1 - _sprite.Width / 2) _velocity.X *= -1;
            if (MathF.Abs(_sprite.Position.Y) > 1 - _sprite.Height / 2) _velocity.Y *= -1;
        }

        public void Run()
        {
            // Run the game
            _window.Run();
        }
    }
}