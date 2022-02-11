using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace LadaEngine
{
    public class SpotLight
    {
        private Texture resultRenderDiffuse;
        private Texture resultRenderNormal;
        public Vector4 position;
        public Sprite lightSprite;
        public Vector4 color;
        public Pos resolution = new Pos(1024, 1024);

        public SpotLight(Vector4 color, Vector4 position)
        {
            this.position = position;
            this.color = color;
            CreateTextures();
            lightSprite = new Sprite(resultRenderDiffuse, StandartShaders.STANDART_SHADER);
        }
        public void Render(FPos cam)
        {
            lightSprite.Render(cam);
        }
        private void CreateTextures()
        {
            resultRenderDiffuse = CreateTexture();
            GL.BindImageTexture(0, resultRenderDiffuse.Handle, 0, false, 0, TextureAccess.WriteOnly, SizedInternalFormat.Rgba32f);
            LightShaders.spotLightFiller.Use();
            LightShaders.spotLightFiller.SetInt("resolution_y", resolution.Y);
            LightShaders.spotLightFiller.SetInt("resolution_x", resolution.X);
            LightShaders.spotLightFiller.SetVector4("color", color);
            GL.DispatchCompute(resolution.X, resolution.Y, 1);

            resultRenderNormal = CreateTexture();
            GL.BindImageTexture(0, resultRenderNormal.Handle, 0, false, 0, TextureAccess.WriteOnly, SizedInternalFormat.Rgba32f);
            LightShaders.spotLightNormalFiller.Use();
            LightShaders.spotLightNormalFiller.SetInt("resolution_y", resolution.Y);
            LightShaders.spotLightNormalFiller.SetInt("resolution_x", resolution.X);

            GL.DispatchCompute(resolution.X, resolution.Y, 1);
        }

        private Texture CreateTexture()
        {
            int glHandle = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, glHandle);

            GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureMagFilter,
                (int)TextureMagFilter.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba32f, resolution.X,
                 resolution.Y, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
            return new Texture(glHandle);
        }

    }
}
