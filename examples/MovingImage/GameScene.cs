using LadaEngine.Engine.Base;
using LadaEngine.Engine.Common;
using LadaEngine.Engine.Common.SpriteGroup;
using LadaEngine.Engine.Renderables;
using LadaEngine.Engine.Renderables.GroupRendering;
using LadaEngine.Engine.Scene;
using SpriteGroup = LadaEngine.Engine.SpriteGroup.SpriteGroup;
using Window = LadaEngine.Engine.Global.Window;

namespace MovingImage
{
    public class GameScene : IScene
    {
        private readonly Camera _camera;
        private readonly ITextureAtlas _atlas;
        private SpriteGroup _world;
        private Sprite _sprite;
        private Pos _velocity;

        public string GetName()
        {
            return "Moving Image";
        }

        public GameScene(Window window, Game game)
        {
            // Create atlas and camera
            _atlas = new TextureAtlas(new List<string> {
                "Textures/image.png",
            });
            _camera = new();
            
            // Create velocity vector
            Random random = new();
            _velocity = new(random.NextSingle() * 0.01f, random.NextSingle() * 0.01f);
        }

        public void Load()
        {
            // Load sprite group
            _world = new SpriteGroup(_atlas);
            
            // Create sprite
            _sprite = new Sprite(Pos.Zero, _atlas, "Textures/image.png");
            _sprite.Width = _sprite.Height = 0.3f;
            
            // Add sprite to sprite group
            _world.AddSprite(_sprite);
        }

        public void Render()
        {
            // Render the world with the sprite
            _world.Render(_camera);
            
            // Update sprite position (it will automatically update in the world as it is a reference)
            _sprite.Position += _velocity;

            // Make object not go offscreen
            if (MathF.Abs(_sprite.Position.X) > 1 - _sprite.Width / 2) _velocity.X *= -1;
            if (MathF.Abs(_sprite.Position.Y) > 1 - _sprite.Height / 2) _velocity.Y *= -1;
        }

        public void Update()
        {
            // Recalculate vertices of world. If the world is big and has a lot of sprites, this operation will be costly.
            // It is recommended to update it not often, and better give the coordinates via camera position to shader
            _world.Update(_camera);
        }

        public void FixedUpdate()
        {

        }

        public void Resize()
        {

        }
    }
}