using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadaEngine
{
    public class BakedLight
    {
        public Texture light_map;
        int light_generator;
        Dictionary<string, int> unilocs = new Dictionary<string, int>();

        // WARNING: This variable should not be very high, I am unsire if GPU can handle enourmous variables
        Pos resolution = new Pos(1024, 1024);
        public void AddLight(float[] positions, float[] colors)
        {
            light_map.Use(TextureUnit.Texture0);
            GL.UseProgram(light_generator);

            GL.Uniform1(unilocs["normal_map"], 1);
            GL.Uniform1(unilocs["resolution_x"], resolution.X);
            GL.Uniform1(unilocs["resolution_y"], resolution.X);
            GL.Uniform4(unilocs["light_colors"], colors[0], colors[1], colors[2], colors[3]);
            GL.Uniform4(unilocs["light_position"], positions[0], positions[1], positions[2], positions[3]);

            GL.DispatchCompute(resolution.X, resolution.Y, 1);
            GL.MemoryBarrier(MemoryBarrierFlags.ShaderImageAccessBarrierBit);
        }
        public void Load()
        {
            CreateTexture();
            CreateShader();
        }


        private void CreateTexture()
        {
            int glHandle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, glHandle);

            GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureMagFilter,
                (int)TextureMagFilter.Nearest);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba32f, resolution.X,
                 resolution.Y, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
            GL.BindImageTexture(0, glHandle, 0, false, 0, TextureAccess.ReadWrite, SizedInternalFormat.Rgba32f);

            light_map = new Texture(glHandle);
        }

        private void CreateShader()
        {
            int light_generator_shader = GL.CreateShader(ShaderType.ComputeShader);
            GL.ShaderSource(light_generator_shader, StandartShaders.light_gen);

            GL.CompileShader(light_generator_shader);
            GL.GetShader(light_generator_shader, ShaderParameter.CompileStatus, out var code);
            if (code != (int)All.True)
            {
                var infoLog = GL.GetShaderInfoLog(light_generator_shader);
                Misc.PrintShaderError(StandartShaders.light_gen, infoLog);
                throw new Exception($"Error occurred whilst compiling Shader({light_generator_shader}).\n\n{infoLog}");
            }

            light_generator = GL.CreateProgram();
            GL.AttachShader(light_generator, light_generator_shader);
            GL.LinkProgram(light_generator);
            GL.GetProgram(light_generator, GetProgramParameterName.LinkStatus, out var c2ode);
            if (c2ode != (int)All.True) throw new Exception($"Error occurred whilst linking Program({light_generator})");
            GL.GetProgram(light_generator, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

            for (var i = 0; i < numberOfUniforms; i++)
            {
                var key = GL.GetActiveUniform(light_generator, i, out _, out _);
                var location = GL.GetUniformLocation(light_generator, key);
                unilocs.Add(key, location);
            }

            GL.DetachShader(light_generator, light_generator_shader);
            GL.DeleteShader(light_generator_shader);
        }
    }
}
