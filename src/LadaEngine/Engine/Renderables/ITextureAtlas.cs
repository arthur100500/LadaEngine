using OpenTK.Graphics.OpenGL4;

namespace LadaEngine.Engine.Renderables;

public interface ITextureAtlas
{
    public float[] Coordinates { get; }
    public int Handle { get; }
    public int Height { get; set; }
    public int Width { get; set; }
    public void Use(TextureUnit unit);
    public float[] GetCoords(string name);
}