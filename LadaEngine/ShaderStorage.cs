using System;
using System.Collections.Generic;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;


namespace LadaEngine
{
    internal static class ShaderStorage
    {
        public static Shader tilemap_shader = new Shader("shaders/shader.vert", "shaders/tilemaps/light_shadow_default.frag");
        private static int light_generator_shader;
        public static int light_generator;
        public static Dictionary<string, int> light_generator_uniformLocations = new Dictionary<string, int>();
        public static void Init()
        {
            InitLightGenerator("shaders/tilemaps/light_gen.comp");
        }


        private static void InitLightGenerator(string tool_shader_path)
        {

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
    }
}
