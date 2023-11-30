using OpenTK.Graphics.OpenGL4;

namespace LadaEngine.Engine.Renderables;

/// <summary>
///     Grants ability to store multiple images in one texture
/// </summary>
public interface ITextureAtlas
{
    /// <summary>
    ///     Use ITextureAtlas as a texture for OpenGL
    /// </summary>
    /// <param name="unit">OpenGL Texture unit to be loaded to</param>
    public void Use(TextureUnit unit);

    /// <summary>
    ///     GetCoords of specific texture via name
    /// </summary>
    /// <param name="name">Name of the texture</param>
    /// <returns>20 elements array with vertex data for a Sprite</returns>
    public float[] GetCoords(string name);
}