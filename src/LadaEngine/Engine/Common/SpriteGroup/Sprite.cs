using LadaEngine.Engine.Base;
using LadaEngine.Engine.Renderables;

namespace LadaEngine.Engine.Common.SpriteGroup;

public class Sprite : GameObject
{
    private readonly ITextureAtlas _atlas;
    private readonly float[] relAngles = { 0f, (float)Math.PI / 2, (float)Math.PI, 3 * (float)Math.PI / 2 };
    private float[] _verts;
    public string textureName;

    public Sprite(Pos position, ITextureAtlas textureAtlas, string textureName) : base(position, 1f, 1f)
    {
        _atlas = textureAtlas;
        this.textureName = textureName;
    }

    public virtual void AddToVerts(List<float> verticies, Camera camera)
    {
        InitializeVerts();
        RotateVerts(Rotation, camera);
        verticies.AddRange(_verts);
    }

    private void InitializeVerts()
    {
        var _coords = _atlas.GetCoords(textureName);
        _verts = new[]
        {
            -0, 0, 1 / (1.1f + Level),
            _coords[0],
            _coords[1], // top right
            -0, 0, 1 / (1.1f + Level),
            _coords[2],
            _coords[3], // bottom right
            -0, 0, 1 / (1.1f + Level),
            _coords[4],
            _coords[5], // bottom left
            -0, 0, 1 / (1.1f + Level),
            _coords[6],
            _coords[7] // top left
        };
    }

    internal Sprite Copy()
    {
        var t = new Sprite(Position, _atlas, textureName);
        t.Width = Width;
        t.Height = Height;

        return t;
    }

    protected bool CheckBounds()
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

    private void RotateVerts(float angle, Camera camera)
    {
        var rad = MathF.Sqrt(Width * Width + Height * Height) / camera.Zoom / 2;
        var AB = new Pos(Height, Width);
        var AC = new Pos(Height, -Width);

        relAngles[0] = (float)Math.Atan2(AB.X, AB.Y);
        relAngles[1] = (float)Math.Atan2(AC.X, AC.Y);
        relAngles[2] = (float)Math.Atan2(AB.X, AB.Y) + (float)Math.PI;
        relAngles[3] = (float)Math.Atan2(AC.X, AC.Y) + (float)Math.PI;

        var transposed = (Position - camera.Position) * (1 / camera.Zoom);
        angle = MathF.PI - angle;
        _verts[0] = transposed.X + rad * (float)Math.Cos(angle + relAngles[2]);
        _verts[1] = transposed.Y + rad * (float)Math.Sin(angle + relAngles[2]);
        _verts[5] = transposed.X + rad * (float)Math.Cos(angle + relAngles[1]);
        _verts[6] = transposed.Y + rad * (float)Math.Sin(angle + relAngles[1]);
        _verts[10] = transposed.X + rad * (float)Math.Cos(angle + relAngles[0]);
        _verts[11] = transposed.Y + rad * (float)Math.Sin(angle + relAngles[0]);
        _verts[15] = transposed.X + rad * (float)Math.Cos(angle + relAngles[3]);
        _verts[16] = transposed.Y + rad * (float)Math.Sin(angle + relAngles[3]);
    }
}