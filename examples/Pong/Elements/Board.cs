using LadaEngine.Engine.Base;
using LadaEngine.Engine.Common;
using LadaEngine.Engine.Common.SpriteGroup;
using LadaEngine.Engine.Global;
using LadaEngine.Engine.Physics.Colliders;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Pong.Elements;

public class Board
{
    public readonly BoxCollider Collider;
    private readonly Sprite _sprite;
    private Keys _up;
    private Keys _down;

    public Board(SpriteGroup group, Pos initial, Keys up, Keys down)
    {
        // Create sprite
        _sprite = new Sprite(new Pos(0, 0), group.textureAtlas, "Textures/board.png");
        _sprite.Width = 0.06f;
        _sprite.Height = 0.3f;
        _sprite.Position = initial;
        group.AddSprite(_sprite);
        
        // Assign movement keys
        _up = up;
        _down = down;
        
        // Create collider
        Collider = new BoxCollider(_sprite.Position, _sprite.Width, _sprite.Height);
    }
    
    
    public void FixedUpdate()
    {
        Collider.Position = _sprite.Position;

        if (Controls.keyboard.IsKeyDown(_up))
            _sprite.Position.Y += 0.004f;
        
        if (Controls.keyboard.IsKeyDown(_down))
            _sprite.Position.Y -= 0.004f;
    }
}