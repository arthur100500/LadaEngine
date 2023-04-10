using LadaEngine.Engine.Base;
using LadaEngine.Engine.Common;
using LadaEngine.Engine.Common.SpriteGroup;
using LadaEngine.Engine.Global;
using LadaEngine.Engine.Physics.Colliders;

namespace Pong.Elements;

public class Ball
{
    private Sprite _sprite;
    private Pos _speed;
    public BoxCollider _collider;
    private Board leftBoard;
    private Board rightBoard;

    public Ball(SpriteGroup group, Board leftBoard, Board rightBoard)
    {
        // Add boards
        this.leftBoard = leftBoard;
        this.rightBoard = rightBoard;
        
        // Create sprite
        _sprite = new Sprite(new Pos(0, 0), group.TextureAtlas, "Textures/ball.png");
        _sprite.Width = _sprite.Height = 0.1f;
        group.AddSprite(_sprite);
        
        // Create collider
        _collider = new BoxCollider(_sprite.Position, _sprite.Width, _sprite.Height);
        
        // Generate speed with random angle
        Random random = new();
        var angle = random.NextSingle() * 0.1f;
        _speed = new(0.005f * MathF.Cos(angle), 0.005f * MathF.Sin(angle));
    }

    public void SetPosition(Pos position)
    {
        _collider.Position = _sprite.Position = position;
    }
    
    public void FixedUpdate()
    {
        // Move collider with sprite
        _collider.Position = _sprite.Position;
        
        // Move sprite
        _sprite.Position += _speed;
        
        // Change Y if lands on ceiling
        if (MathF.Abs(_sprite.Position.Y) > 1 - _sprite.Width / 2) _speed.Y *= -1;
        
        // Check for collisions and change bounce
        if (leftBoard.Collider.CheckCollision(_collider))
        {
            // Make speed positive
            _speed.X = MathF.Abs(_speed.X);
            
            // Get and increase speed slightly
            var speed = Misc.Len(_speed, Pos.Zero);
            
            // Change angle according to hit
            var prevAngle = MathF.Atan2(_speed.X, _speed.Y);
            prevAngle += (leftBoard.Collider.Position.Y - _collider.Position.Y) * 1f;
            _speed = new Pos(speed * MathF.Sin(prevAngle), speed * MathF.Cos(prevAngle));
        }
        
        // Check for collisions and change bounce
        if (rightBoard.Collider.CheckCollision(_collider))
        {
            // Make speed negative
            _speed.X = -MathF.Abs(_speed.X);
            
            // Get and increase speed slightly
            var speed = Misc.Len(_speed, Pos.Zero);
            
            // Change angle according to hit
            var prevAngle = MathF.Atan2(_speed.X, _speed.Y);
            prevAngle += (leftBoard.Collider.Position.Y - _collider.Position.Y) * 0.1f;
            _speed = new Pos(speed * MathF.Sin(prevAngle), speed * MathF.Cos(prevAngle));
        }
    }
}