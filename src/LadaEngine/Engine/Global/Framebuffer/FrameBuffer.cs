using LadaEngine.Engine.Base;
using LadaEngine.Engine.Common;
using OpenTK.Graphics.OpenGL4;

namespace LadaEngine.Engine.Global.Framebuffer;

public class FrameBuffer
{
    private readonly Camera _blancCam = new();
    private int FBO;
    public Sprite Sprite;
    public Texture Texture;

    public void Load(IntPos screen_resolution)
    {
        FBO = GL.GenFramebuffer();

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBO);

        Texture = CreateTexture(screen_resolution);

        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
            TextureTarget.Texture2D, Texture.Handle, 0);

        Sprite = new Sprite(Texture);
    }

    public void Start()
    {
        if (GlobalOptions.bfbo != FBO)
        {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, FBO);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        GlobalOptions.bfbo = FBO;
    }

    public void Stop()
    {
        if (GlobalOptions.bfbo != 0)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        GlobalOptions.bfbo = 0;
    }

    private Texture CreateTexture(IntPos screen_resolution)
    {
        // Generate handle
        var handle = GL.GenTexture();

        // Bind the handle
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, handle);


        GL.TexImage2D(TextureTarget.Texture2D,
            0,
            PixelInternalFormat.Rgba,
            screen_resolution.X,
            screen_resolution.Y,
            0,
            PixelFormat.Bgra,
            PixelType.UnsignedByte,
            nint.Zero);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
            (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
            (int)TextureMagFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.ClampToEdge, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.ClampToEdge, (int)TextureWrapMode.Repeat);

        return new Texture(handle);
    }

    public void ResizeToFullscreen()
    {
        if (Sprite is null) throw new Exception("Framebuffer must be loaded before resize");
        Sprite.quad.ReshapeWithCoords(-Misc.fbo_sprite_coords.X, Misc.fbo_sprite_coords.Y, 1, -1);
    }

    public void Resize(IntPos screen_resolution)
    {
        if (Texture is null) throw new Exception("Framebuffer must be loaded before resize");
        // Bind the handle
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, Texture.Handle);

        GL.TexImage2D(TextureTarget.Texture2D,
            0,
            PixelInternalFormat.Rgba,
            screen_resolution.X,
            screen_resolution.Y,
            0,
            PixelFormat.Bgra,
            PixelType.UnsignedByte,
            nint.Zero);
    }

    public void Render()
    {
        if (Sprite is null) throw new Exception("Framebuffer must be loaded before resize");
        Sprite.Render(_blancCam);
    }
}