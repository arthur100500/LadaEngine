using LadaEngine.Engine.Base;
using OpenTK.Graphics.OpenGL4;

namespace LadaEngine.Engine.Renderables.GroupRendering;

/// <summary>
///     Texture atlas that represents atlas with a single set texture.
///     All requests to GetCoords will return full size texture
/// </summary>
public class SingleTextureAtlas : ITextureAtlas
{
    private readonly Texture _texture;

    /// <summary>
    ///     Create SingleTextureAtlas object
    /// </summary>
    /// <param name="texture">Source texture</param>
    public SingleTextureAtlas(Texture texture)
    {
        _texture = texture;
        Handle = texture.Handle;
    }

    /// <summary>
    ///     Texture handle
    /// </summary>
    public int Handle { get; }

    /// <summary>
    ///     Use the texture of the atlas
    /// </summary>
    /// <param name="unit">Texture unit to be used</param>
    public void Use(TextureUnit unit)
    {
        _texture.Use(unit);
    }

    /// <summary>
    ///     Returns { 1, 1, 0, 1, 1, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1 }
    /// </summary>
    /// <param name="name">Any string</param>
    /// <returns>{ 1, 1, 0, 1, 1, 1, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1 }</returns>
    public float[] GetCoords(string name)
    {
        return new float[]
        {
             1, 1,
             1, 0,
             0, 0,
             0, 1
        };
    }
}