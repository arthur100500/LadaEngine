using LadaEngine.Engine.Base;
using LadaEngine.Engine.Global;
using LadaEngine.Engine.Renderables;

namespace LadaEngine.Engine.Common.SpriteGroup;

/// <summary>
/// The object containing image
/// Should be in a SpriteGroup to work properly
/// An usage example: https://github.com/arthur100500/LadaEngine/tree/main/examples/MovingImage
/// </summary>
public sealed class Sprite : GameObject
{
    private readonly ITextureAtlas _atlas;
    private readonly float[] _relAngles = { 0f, (float)Math.PI / 2, (float)Math.PI, 3 * (float)Math.PI / 2 };
    private float[] _verts;
    
    /// <summary>
    /// Name of the texture
    /// </summary>
    public readonly string TextureName;

    /// <summary>
    /// Create a new sprite instance
    /// </summary>
    /// <param name="position">Position of new sprite</param>
    /// <param name="textureAtlas">Texture atlas for texture source</param>
    /// <param name="textureName">Name of texture in atlas</param>
    public Sprite(Pos position, ITextureAtlas textureAtlas, string textureName) : base(position, 1f, 1f)
    {
        _atlas = textureAtlas;
        TextureName = textureName;
        _verts = Misc.fullscreenverticies;
    }

    /// <summary>
    /// Adds coordinates of self to the vertices list via AddRange
    /// </summary>
    /// <param name="vertices"></param>
    public void AddToVerts(List<float> vertices)
    {
        InitializeVerts();
        RotateVerts(Rotation);
        vertices.AddRange(_verts);
    }

    private void InitializeVerts()
    {
        var coords = _atlas.GetCoords(TextureName);
        _verts = new[]
        {
            -0, 0, 1 / (1.1f + Level),
            coords[0],
            coords[1], // top right
            -0, 0, 1 / (1.1f + Level),
            coords[2],
            coords[3], // bottom right
            -0, 0, 1 / (1.1f + Level),
            coords[4],
            coords[5], // bottom left
            -0, 0, 1 / (1.1f + Level),
            coords[6],
            coords[7] // top left
        };
    }

    /// <summary>
    /// Creates copy of a sprite
    /// </summary>
    /// <returns>New sprite identical to self</returns>
    public Sprite Copy()
    {
        var t = new Sprite(Position, _atlas, TextureName)
        {
            Width = Width,
            Height = Height
        };

        return t;
    }

    private bool CheckBounds()
    {
        if (_verts[0] < -1 && _verts[10] < -1 && _verts[5] < -1 && _verts[15] < -1)
            return false;
        if (_verts[0] > 1 && _verts[10] > 1 && _verts[5] > 1 && _verts[15] > 1)
            return false;
        if (_verts[1] < -1 && _verts[11] < -1 && _verts[6] < -1 && _verts[16] < -1)
            return false;
        if (_verts[1] > 1 && _verts[11] > 1 && _verts[6] > 1 && _verts[16] > 1)
            return false;
        return true;
    }

    private void RotateVerts(float angle)
    {
        var rad = MathF.Sqrt(Width * Width + Height * Height) / 2;
        var ab = new Pos(Height, Width);
        var ac = new Pos(Height, -Width);

        _relAngles[0] = (float)Math.Atan2(ab.X, ab.Y);
        _relAngles[1] = (float)Math.Atan2(ac.X, ac.Y);
        _relAngles[2] = (float)Math.Atan2(ab.X, ab.Y) + (float)Math.PI;
        _relAngles[3] = (float)Math.Atan2(ac.X, ac.Y) + (float)Math.PI;

        var transposed = Position;
        
        angle = MathF.PI - angle;
        _verts[0] = transposed.X + rad * (float)Math.Cos(angle + _relAngles[2]);
        _verts[1] = transposed.Y + rad * (float)Math.Sin(angle + _relAngles[2]);
        _verts[5] = transposed.X + rad * (float)Math.Cos(angle + _relAngles[1]);
        _verts[6] = transposed.Y + rad * (float)Math.Sin(angle + _relAngles[1]);
        _verts[10] = transposed.X + rad * (float)Math.Cos(angle + _relAngles[0]);
        _verts[11] = transposed.Y + rad * (float)Math.Sin(angle + _relAngles[0]);
        _verts[15] = transposed.X + rad * (float)Math.Cos(angle + _relAngles[3]);
        _verts[16] = transposed.Y + rad * (float)Math.Sin(angle + _relAngles[3]);
    }
}