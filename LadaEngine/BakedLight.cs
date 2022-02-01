using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadaEngine
{
    /// <summary>
    /// Adds light to a texture "light_map" once, then it can be used as a texture for baked light
    /// </summary>
    public class BakedLight
    {
        public Texture light_map;
        Shader light_generator;
        Dictionary<string, int> unilocs = new Dictionary<string, int>();

        // WARNING: This variable should not be very high, I am unsire if GPU can handle enourmous variables
        Pos resolution = new Pos(512, 512);
        /// <summary>
        /// Adds light to a texture (for tilemaps)
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="colors"></param>
        public void AddLightTM(float[] positions, float[] colors, Tilemap t)
        {
            if (GlobalOptions.full_debug)
                Misc.Log("\n\n--- Baked light counting (TileMap) ---");

            light_map.Use(TextureUnit.Texture0);
            light_generator.Use();

            light_generator.SetInt("light_map", 0);
            light_generator.SetInt("normal_map", 1);
            light_generator.SetInt("resolution_x", resolution.X);
            light_generator.SetInt("resolution_y", resolution.Y);
            light_generator.SetVector4("light_colors", new Vector4(colors[0], colors[1], colors[2], colors[3]));
            light_generator.SetVector4("light_position", new Vector4(positions[0], positions[1], positions[2], positions[3]));
            light_generator.SetVector2("texture_size", new Vector2(t.width, t.height));

            light_generator.SetInt("texture_length", t.grid_length);
            light_generator.SetInt("texture_width", t.grid_width);
            light_generator.SetIntGroup("map_array[0]", t.map.Length, t.map);

            light_generator.SetInt("height", t.tm_height);
            light_generator.SetInt("width", t.tm_width);


            //GL.Uniform1(unilocs["texture_rotation"], t.rotation);
            if (GlobalOptions.full_debug)
                Misc.Log("Shader + " + Convert.ToString(light_generator) + " computed");
            GL.DispatchCompute(resolution.X, resolution.Y, 1);
            GL.MemoryBarrier(MemoryBarrierFlags.ShaderImageAccessBarrierBit);

            if (GlobalOptions.full_debug)
                Misc.Log("--- End baked light counting ---");
        }
        /// <summary>
        /// Adds light to a texture
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="colors"></param>
        public void AddLight(float[] positions, float[] colors, Sprite s)
        {
            if (GlobalOptions.full_debug)
                Misc.Log("\n\n--- Baked light counting (Sprite) ---");
            light_map.Use(TextureUnit.Texture0);
            light_generator.Use();

            light_generator.SetInt("light_map", 0);
            light_generator.SetInt("normal_map", 1);
            light_generator.SetInt("resolution_x", resolution.X);
            light_generator.SetInt("resolution_y", resolution.Y);
            light_generator.SetVector4("light_colors", new Vector4(colors[0], colors[1], colors[2], colors[3]));
            light_generator.SetVector4("light_position", new Vector4(positions[0], positions[1], positions[2], positions[3]));
            light_generator.SetVector2("texture_size", new Vector2(s.width, s.height));
            //GL.Uniform1(unilocs["texture_rotation"], s.rotation);

            GL.DispatchCompute(resolution.X, resolution.Y, 1);
            GL.MemoryBarrier(MemoryBarrierFlags.ShaderImageAccessBarrierBit);
            if (GlobalOptions.full_debug)
                Misc.Log("Shader + " + Convert.ToString(light_generator) + " computed");
            if (GlobalOptions.full_debug)
                Misc.Log("--- End baked light counting ---");
        }

        public void ClearLights()
        {
            light_map.Use(TextureUnit.Texture2);
            light_generator.Use();

            light_generator.SetInt("light_map", 0);
            light_generator.SetInt("normal_map", 1);
            // This parameter triggers condition in a shader
            light_generator.SetInt("resolution_x", -1);
            light_generator.SetInt("resolution_x", -1);

            GL.DispatchCompute(resolution.X, resolution.Y, 1);
            GL.MemoryBarrier(MemoryBarrierFlags.ShaderImageAccessBarrierBit);
        }
        /// <summary>
        /// Prepare the class components to work
        /// </summary>
        public void Load(string shader_origin)
        {
            CreateTexture();
            CreateShader(shader_origin);
        }


        private void CreateTexture()
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

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba32f, resolution.X,
                 resolution.Y, 0, PixelFormat.Rgba, PixelType.Float, IntPtr.Zero);
            GL.BindImageTexture(0, glHandle, 0, false, 0, TextureAccess.ReadWrite, SizedInternalFormat.Rgba32f);

            light_map = new Texture(glHandle);
        }

        private void CreateShader(string shader_origin)
        {
            int light_generator_id;
            int light_generator_shader = GL.CreateShader(ShaderType.ComputeShader);
            GL.ShaderSource(light_generator_shader, shader_origin);

            GL.CompileShader(light_generator_shader);
            GL.GetShader(light_generator_shader, ShaderParameter.CompileStatus, out var code);
            if (code != (int)All.True)
            {
                var infoLog = GL.GetShaderInfoLog(light_generator_shader);
                Misc.PrintShaderError(StandartShaders.light_gen, infoLog);
                throw new Exception($"Error occurred whilst compiling Shader({light_generator_shader}).\n\n{infoLog}");
            }

            light_generator_id = GL.CreateProgram();
            GL.AttachShader(light_generator_id, light_generator_shader);
            GL.LinkProgram(light_generator_id);
            GL.GetProgram(light_generator_id, GetProgramParameterName.LinkStatus, out var c2ode);
            if (c2ode != (int)All.True) throw new Exception($"Error occurred whilst linking Program({light_generator_id})");
            GL.GetProgram(light_generator_id, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

            for (var i = 0; i < numberOfUniforms; i++)
            {
                var key = GL.GetActiveUniform(light_generator_id, i, out _, out _);
                var location = GL.GetUniformLocation(light_generator_id, key);
                unilocs.Add(key, location);
            }

            GL.DetachShader(light_generator_id, light_generator_shader);
            GL.DeleteShader(light_generator_shader);

            light_generator = new Shader(light_generator_id);
        }
    }
}
