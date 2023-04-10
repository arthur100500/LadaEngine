using LadaEngine.Engine.Base;
using LadaEngine.Engine.Global;
using OpenTK.Graphics.OpenGL4;

namespace LadaEngine.Engine.Renderables.GroupRendering;

public class SingleTextureAtlas : ITextureAtlas
{
    private readonly Texture _texture;

    public SingleTextureAtlas(Texture texture)
    {
        _texture = texture;
        Handle = texture.Handle;
    }

    public int Handle { get; }

    public void Use(TextureUnit unit)
    {
        _texture.Use(unit);
    }

    public float[] GetCoords(string name)
    {
        return new float[] { 1, 1, 0, 1, 1, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1 };
    }
}