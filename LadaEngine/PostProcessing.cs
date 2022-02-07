using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadaEngine
{
    public static class PostProcessing
    {
        static int FBO;
        static int RBO;
        static Texture texture;
        public static Sprite sprite;
        public static void Load(Pos screen_resolution, Shader shader)
        {
            FBO = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBO);

            texture = CreateTexture(screen_resolution);

            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, texture.Handle, 0);

            RBO = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, RBO);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8, screen_resolution.X, screen_resolution.Y);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);

            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, RBO);

            sprite = new Sprite(texture, new Texture(0), shader);
            sprite.centre = new FPos(0, 0);
            sprite.width = 1;
            sprite.height = 1;
            sprite.ReshapeVertexArray(new FPos(0, 0));
        }

        public static void Start()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBO);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }

        public static void Stop()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        public static void Draw()
        {
            sprite.Render();
        }
        static Texture CreateTexture(Pos screen_resolution)
        {
            // Generate handle
            int handle = GL.GenTexture();

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
                    IntPtr.Zero);
            
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            return new Texture(handle);
        }
    }
}
