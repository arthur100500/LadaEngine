using OpenTK.Graphics.OpenGL4;

namespace LadaEngine.Engine.Renderables;

public interface ITextureAtlas
{
    public void Use(TextureUnit unit);
    public float[] GetCoords(string name);
}