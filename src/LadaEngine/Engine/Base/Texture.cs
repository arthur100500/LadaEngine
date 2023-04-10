using LadaEngine.Engine.Global;
using OpenTK.Graphics.OpenGL4;

namespace LadaEngine.Engine.Base;

public class Texture
{
    /// <summary>
    ///     Create texture from GL Handle
    /// </summary>
    /// <param name="glHandle"></param>
    public Texture(int glHandle)
    {
        if (GlobalOptions.FullDebug)
            Misc.Log("Texture " + Convert.ToString(Handle) + " created");
        Handle = glHandle;
    }
    
    /// <summary>
    /// OpenGL Handle of a texture
    /// </summary>
    public int Handle { get; set; }

    /// <summary>
    ///     Create new Texture from bitmap class
    /// </summary>
    /// <param name="bmp"></param>
    /// <returns></returns>
    public static Texture LoadFromBitmap(Image<Rgba32> bmp)
    {
        var handle = GL.GenTexture();
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, handle);

        var pixelBytes = new byte[bmp.Width * bmp.Height * 4];
        bmp.Mutate(x => x.Flip(FlipMode.Vertical));
        bmp.CopyPixelDataTo(pixelBytes);
        bmp.Mutate(x => x.Flip(FlipMode.Vertical));

        GL.TexImage2D(TextureTarget.Texture2D,
            0,
            PixelInternalFormat.Rgba,
            bmp.Width,
            bmp.Height,
            0,
            PixelFormat.Rgba,
            PixelType.UnsignedByte,
            pixelBytes);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
            (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
            (int)TextureMagFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.ClampToEdge, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.ClampToEdge, (int)TextureWrapMode.Repeat);
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

        return new Texture(handle);
    }

    /// <summary>
    ///     Create texture with an image from file
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static Texture LoadFromFile(string path)
    {
        // Generate handle
        var handle = GL.GenTexture();

        // Bind the handle
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, handle);

        // For this example, we're going to use .NET's built-in System.Drawing library to load textures.

        // Load the image
        using (var bmp = Image.Load<Rgba32>(path))
        {
            var pixelBytes = new byte[bmp.Width * bmp.Height * 4];
            bmp.Mutate(x => x.Flip(FlipMode.Vertical));
            bmp.CopyPixelDataTo(pixelBytes);
            bmp.Mutate(x => x.Flip(FlipMode.Vertical));

            GL.TexImage2D(TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgba,
                bmp.Width,
                bmp.Height,
                0,
                PixelFormat.Rgba,
                PixelType.UnsignedByte,
                pixelBytes);
        }

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
            (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
            (int)TextureMagFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.ClampToEdge, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.ClampToEdge, (int)TextureWrapMode.Repeat);

        return new Texture(handle);
    }

    /// <summary>
    /// Updates data in a texture
    /// Maybe it doesn't work
    /// </summary>
    /// <param name="bmp">Image to set new texture to</param>
    public void UpdateData(Image<Rgba32> bmp)
    {
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, Handle);

        var pixelBytes = new byte[bmp.Width * bmp.Height * 4];

        bmp.Mutate(x => x.Flip(FlipMode.Vertical));
        bmp.CopyPixelDataTo(pixelBytes);
        bmp.Mutate(x => x.Flip(FlipMode.Vertical));

        GL.TexImage2D(TextureTarget.Texture2D,
            0,
            PixelInternalFormat.Rgba,
            bmp.Width,
            bmp.Height,
            0,
            PixelFormat.Rgba,
            PixelType.UnsignedByte,
            pixelBytes);
    }

    /// <summary>
    ///     Place texture in a slot
    /// </summary>
    /// <param name="unit"></param>
    public void Use(TextureUnit unit)
    {
        if (Handle != GlobalOptions.LastTextureUsed[unit - TextureUnit.Texture0])
        {
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
            if (GlobalOptions.FullDebug)
                Misc.Log("Texture " + Convert.ToString(Handle) + " loaded to slot " +
                         Convert.ToString((int)unit - 33984));

            GlobalOptions.LastTextureUsed[unit - TextureUnit.Texture0] = Handle;
        }
    }
}