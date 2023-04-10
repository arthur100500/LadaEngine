using LadaEngine.Engine.Base;
using LadaEngine.Engine.Common;
using LadaEngine.Engine.Common.SpriteGroup;
using LadaEngine.Engine.Renderables.GroupRendering;
using OpenTK.Graphics.OpenGL4;

namespace LadaEngine.Engine.Global.Framebuffer;

/// <summary>
/// Class that represents framebuffer
/// You can draw stuff here instead of screen
/// To do so, create a FB object and load it, then call Start.
/// After that all you draw on screen will be drawn to the FBO texture.
/// To finish this call stop method.
/// Contents of framebuffer can be rendered to screen via Render method.
/// </summary>
public class FrameBuffer
{
    private readonly Camera _blancCam = new();
    private int _fbo;
    private Common.SpriteGroup.SpriteGroup _sg;
    
    /// <summary>
    /// Sprite of the framebuffer.
    /// Contained in it's private sprite group
    /// </summary>
    public Sprite Sprite;
    
    /// <summary>
    /// Texture of the framebuffer.
    /// Here stuff will be drawn.
    /// </summary>
    public Texture Texture;

    /// <summary>
    /// Load framebuffer object
    /// </summary>
    /// <param name="screenResolution">resolution of the window</param>
    public void Load(IntPos screenResolution)
    {
        _fbo = GL.GenFramebuffer();

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo);

        Texture = CreateTexture(screenResolution);

        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
            TextureTarget.Texture2D, Texture.Handle, 0);

        _sg = new Common.SpriteGroup.SpriteGroup(new SingleTextureAtlas(Texture));

        Sprite = new Sprite(new Pos(0, 0), _sg.TextureAtlas, "");
    }
    
    /// <summary>
    /// Start capture
    /// </summary>
    public void Start()
    {
        if (GlobalOptions.bfbo != _fbo)
        {
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, _fbo);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        GlobalOptions.bfbo = _fbo;
    }

    /// <summary>
    /// Stop capture
    /// </summary>
    public void Stop()
    {
        if (GlobalOptions.bfbo != 0)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        GlobalOptions.bfbo = 0;
    }
    
    private Texture CreateTexture(IntPos screenResolution)
    {
        // Generate handle
        var handle = GL.GenTexture();

        // Bind the handle
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, handle);


        GL.TexImage2D(TextureTarget.Texture2D,
            0,
            PixelInternalFormat.Rgba,
            screenResolution.X,
            screenResolution.Y,
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
    
    /// <summary>
    /// Resizes framebuffer
    /// </summary>
    /// <param name="screenResolution">New resolution</param>
    /// <exception cref="Exception"></exception>
    public void Resize(IntPos screenResolution)
    {
        if (Texture is null) throw new Exception("Framebuffer must be loaded before resize");
        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, Texture.Handle);

        GL.TexImage2D(TextureTarget.Texture2D,
            0,
            PixelInternalFormat.Rgba,
            screenResolution.X,
            screenResolution.Y,
            0,
            PixelFormat.Bgra,
            PixelType.UnsignedByte,
            nint.Zero);
    }

    /// <summary>
    /// Draw framebuffer on screen
    /// </summary>
    /// <exception cref="Exception"></exception>
    public void Render()
    {
        if (Sprite is null) throw new Exception("Framebuffer must be loaded before resize");
        _sg.Update();
        _sg.Render(_blancCam);
    }
}