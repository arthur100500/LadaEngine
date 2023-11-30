using LadaEngine.Engine.Global;
using OpenTK.Graphics.OpenGL4;

namespace LadaEngine.Engine.Renderables.GroupRendering;

/// <summary>
///     A texture atlas class
/// </summary>
public class TextureAtlas : ITextureAtlas
{
    private readonly Dictionary<string, float[]> _imgCoords;

    /// <summary>
    ///     Create texture atlas from path list
    /// </summary>
    /// <param name="fileNames">Paths to the images</param>
    public TextureAtlas(List<string> fileNames)
    {
        _imgCoords = new Dictionary<string, float[]>();
        var atlas = GenImage(fileNames);

        var handle = GL.GenTexture();
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, handle);

        var pixelBytes = new byte[atlas.Width * atlas.Height * 4];
        atlas.CopyPixelDataTo(pixelBytes);

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, atlas.Width,
            atlas.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixelBytes);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

        Handle = handle;
    }

    /// <summary>
    ///     OpenGL Texture handle
    /// </summary>
    public int Handle { get; }

    /// <summary>
    ///     Uses the texture placing it to the unit slot
    /// </summary>
    /// <param name="unit">OpenGL Texture unit to be loaded to</param>
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

    /// <summary>
    ///     Get the vertex data for one image
    /// </summary>
    /// <param name="name">name of the image</param>
    /// <returns>20 elements array with vertex data for a Sprite</returns>
    public float[] GetCoords(string name)
    {
        return _imgCoords[name];
    }

    private Image<Rgba32> GenImage(List<string> fns)
    {
        var images = new List<Image<Rgba32>>();
        var height = 0;
        var width = 0;
        foreach (var fn in fns)
        {
            var i = Image.Load<Rgba32>(fn);
            images.Add(i);
            height = Math.Max(height, i.Height);
            width += i.Width;
        }

        var result = new byte[width * height * 4];

        var cp = 0;
        var imgindex = 0;
        foreach (var image in images)
        {
            var pixelBytes = new byte[image.Width * image.Height * 4];
            image.CopyPixelDataTo(pixelBytes);

            for (var y = 0; y < image.Height; y++)
            for (var x = 0; x < image.Width; x++)
            for (var c = 0; c < 4; c++)
            {
                var t = pixelBytes[y * image.Width * 4 + x * 4 + c];
                result[y * width * 4 + (cp + x) * 4 + c] = t;
            }

            _imgCoords.Add(fns[imgindex++], new[]
            {
                ((float)cp + image.Width) / width, 0f,
                ((float)cp + image.Width) / width, (float)image.Height / height,
                (float)cp / width, (float)image.Height / height,
                (float)cp / width, 0f
            });
            cp += image.Width;
        }

        return Image.LoadPixelData<Rgba32>(result, width, height);
    }
}