using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace LadaEngine
{
    public class BakedLightManager
    {
        public int light_map;

        private int light_generator_shader;
        public int light_generator;

        Dictionary<string, int> light_generator_uniformLocations = new Dictionary<string, int>();

        Tilemap tm;

        public BakedLightManager(Tilemap self)
        {
            tm = self;
            light_map = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, light_map);

            GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureMagFilter,
                (int)TextureMagFilter.Nearest);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba32f, 128,
                 128, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
            GL.BindImageTexture(0, light_map, 0, false, 0, TextureAccess.WriteOnly, SizedInternalFormat.Rgba32f);




            string tool_shader_path = "shaders/tilemaps/light_gen.comp";
            string tool_shader = File.ReadAllText(tool_shader_path);
            light_generator_shader = GL.CreateShader(ShaderType.ComputeShader);
            GL.ShaderSource(light_generator_shader, tool_shader);

            GL.CompileShader(light_generator_shader);
            GL.GetShader(light_generator_shader, ShaderParameter.CompileStatus, out var code);
            if (code != (int)All.True)
            {
                Console.WriteLine(tool_shader);
                var infoLog = GL.GetShaderInfoLog(light_generator_shader);
                Misc.PrintShaderError(tool_shader, infoLog);
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
                light_generator_uniformLocations.Add(key, location);
            }

            GL.DetachShader(light_generator, light_generator_shader);
            GL.DeleteShader(light_generator_shader);



        }

        public void SetLight(LightSource lightSource)
        {
            Console.Write("LightMapID: ");
            Console.WriteLine(light_map);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, light_map);

            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, tm.normalMap.Handle);

            GL.UseProgram(light_generator);

            if(light_generator_uniformLocations.Keys.Contains("texture_length"))
            GL.Uniform1(light_generator_uniformLocations["texture_length"], 10);
            if (light_generator_uniformLocations.Keys.Contains("normal_map[0]"))
                GL.Uniform1(light_generator_uniformLocations["normal_map[0]"], tm.map.Length, tm.map);
            if (light_generator_uniformLocations.Keys.Contains("texture1"))
                GL.Uniform1(light_generator_uniformLocations["texture1"], 1);
            if (light_generator_uniformLocations.Keys.Contains("img_output"))
                GL.Uniform1(light_generator_uniformLocations["img_output"], 0);

            GL.DispatchCompute(128, 128, 1);
            GL.MemoryBarrier(MemoryBarrierFlags.ShaderImageAccessBarrierBit);
        }
    }
}
