using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK;
using OpenTK.Mathematics;

namespace LadaEngine
{
    // A simple class meant to help create shaders.
    public class Shader
    {

        /// <summary>
        /// GL Handle for the shader
        /// <summary>
        public readonly int Handle;

        /// <summary>
        /// Dictionary of string uniforms
        /// </summary>
        public Dictionary<string, int> _uniformLocations { get; internal set; }
        internal Shader(int programHandle)
        {
            Handle = programHandle;

            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

            _uniformLocations = new Dictionary<string, int>();

            for (var i = 0; i < numberOfUniforms; i++)
            {
                var key = GL.GetActiveUniform(Handle, i, out _, out _);
                var location = GL.GetUniformLocation(Handle, key);
                _uniformLocations.Add(key, location);
            }

            if (GlobalOptions.full_debug)
            {
                Misc.Log("Shader '" + Convert.ToString(Handle) + "' created(from int)");
                Misc.Log("Uniforms");
                PrintUniformLocations();
            }
        }

        /// <summary>
        /// Shader constructor from file strings
        /// </summary>
        /// <param name="vertPath">Path to the vertext shader file</param>
        /// <param name="fragPath">Path to the fragment shader file</param>
        public Shader(string vertPath, string fragPath)
        {
            var shaderSource = File.ReadAllText(vertPath);

            var vertexShader = GL.CreateShader(ShaderType.VertexShader);

            GL.ShaderSource(vertexShader, shaderSource);

            CompileShader(vertexShader, shaderSource);

            shaderSource = File.ReadAllText(fragPath);
            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, shaderSource);
            CompileShader(fragmentShader, shaderSource);

            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, vertexShader);
            GL.AttachShader(Handle, fragmentShader);

            LinkProgram(Handle);

            GL.DetachShader(Handle, vertexShader);
            GL.DetachShader(Handle, fragmentShader);
            GL.DeleteShader(fragmentShader);
            GL.DeleteShader(vertexShader);

            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

            _uniformLocations = new Dictionary<string, int>();

            for (var i = 0; i < numberOfUniforms; i++)
            {
                var key = GL.GetActiveUniform(Handle, i, out _, out _);
                var location = GL.GetUniformLocation(Handle, key);
                _uniformLocations.Add(key, location);
            }


            if (GlobalOptions.full_debug)
            {
                Misc.Log("Shader '" + Convert.ToString(Handle) + "' created");
                Misc.Log("Uniforms");
                PrintUniformLocations();
            }
        }
        /// <summary>
        /// Shader constructor from code strings
        /// </summary>
        /// <param name="shaderSourceVert">Code for vertex shader</param>
        /// <param name="shaderSourceFrag">Code for fragment shader</param>
        /// <param name="type">Use 0</param>
        /// <exception cref="NotImplementedException"></exception>
        public Shader(string shaderSourceVert, string shaderSourceFrag, int type)
        {
            if (type != 0)
                throw new NotImplementedException();

            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, shaderSourceVert);
            CompileShader(vertexShader, shaderSourceVert);

            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, shaderSourceFrag);
            CompileShader(fragmentShader, shaderSourceFrag);

            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, vertexShader);
            GL.AttachShader(Handle, fragmentShader);

            LinkProgram(Handle);

            GL.DetachShader(Handle, vertexShader);
            GL.DetachShader(Handle, fragmentShader);
            GL.DeleteShader(fragmentShader);
            GL.DeleteShader(vertexShader);

            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

            _uniformLocations = new Dictionary<string, int>();

            for (var i = 0; i < numberOfUniforms; i++)
            {
                var key = GL.GetActiveUniform(Handle, i, out _, out _);
                var location = GL.GetUniformLocation(Handle, key);
                _uniformLocations.Add(key, location);
            }

            if (GlobalOptions.full_debug)
            {
                Misc.Log("Shader '" + Convert.ToString(Handle) + "' created");
                Misc.Log("Uniforms");
                PrintUniformLocations();
            }
        }

        private static void CompileShader(int shader, string text)
        {
            GL.CompileShader(shader);

            GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
            if (code != (int)All.True)
            {
                var infoLog = GL.GetShaderInfoLog(shader);
                Misc.PrintShaderError(text, infoLog);
                throw new Exception($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}");
            }
        }

        internal int GetUniformLocation(string name)
        {
            try
            {
                return _uniformLocations[name];
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        private static void LinkProgram(int program)
        {
            GL.LinkProgram(program);

            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
            if (code != (int)All.True)
            {
                throw new Exception($"Error occurred whilst linking Program({program})");
            }
        }

        /// <summary>
        /// Uses the shader program
        /// </summary>
        public void Use()
        {
            if (Handle != GlobalOptions.lastShaderUsed)
            {
                if (GlobalOptions.full_debug)
                    Misc.Log("Shader '" + Convert.ToString(Handle) + "' used");
                GL.UseProgram(Handle);
                GlobalOptions.lastShaderUsed =  Handle;
            }
        }


        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(Handle, attribName);
        }

        /// <summary>
        /// Set a uniform int on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetInt(string name, int data)
        {
            try
            {
                Use();
                GL.Uniform1(_uniformLocations[name], data);
                if (GlobalOptions.full_debug)
                    Misc.Log("Shader '" + Convert.ToString(Handle) + "' set int '" + name + "' to " + Convert.ToString(data));
            }
            catch (Exception e)
            {
                Misc.Log(e.Message);
            }
        }
        /// <summary>
        /// Set an array of integers using uniform name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="count"></param>
        /// <param name="data"></param>
        public void SetIntGroup(string name, int count, int[] data)
        {
            try
            {
                Use();
                GL.Uniform1(_uniformLocations[name], count, data);
                if (GlobalOptions.full_debug)
                    Misc.Log("Shader '" + Convert.ToString(Handle) + "' set int[] '" + name + "' to " + Convert.ToString(data));
                

            }
            catch (Exception e)
            {
                Misc.Log(e.Message);
            }
        }
        /// <summary>
        /// Set an array of integer using uniform location
        /// </summary>
        /// <param name="location"></param>
        /// <param name="count"></param>
        /// <param name="data"></param>
        public void SetIntGroup(int location, int count, int[] data)
        {
            try
            {
                Use();
                GL.Uniform1(location, count, data);
                if (GlobalOptions.full_debug)
                    Misc.Log("Shader '" + Convert.ToString(Handle) + "' set int loc(" + Convert.ToString(location) + ") to " + Convert.ToString(data));
            }
            catch (Exception e)
            {
                Misc.Log(e.Message);
            }
        }

        /// <summary>
        /// Set a uniform float on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetFloat(string name, float data)
        {
            try
            {
                Use();
                GL.Uniform1(_uniformLocations[name], data);
                if (GlobalOptions.full_debug)
                    Misc.Log("Shader '" + Convert.ToString(Handle) + "' set float '" + name + "' to " + Convert.ToString(data));
            }
            catch (Exception e)
            {
                Misc.Log(e.Message);
            }
        }

        /// <summary>
        /// Set a uniform Matrix4 on this shader
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        /// <remarks>
        ///   <para>
        ///   The matrix is transposed before being sent to the shader.
        ///   </para>
        /// </remarks>
        public void SetMatrix4(string name, Matrix4 data)
        {
            try
            {
                Use();
                GL.UniformMatrix4(_uniformLocations[name], true, ref data);
            }
            catch (Exception e)
            {
                Misc.Log(e.Message);
            }
        }

        /// <summary>
        /// Set a uniform Vector3 on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetVector3(string name, Vector3 data)
        {
            try
            {
                Use();
                GL.Uniform3(_uniformLocations[name], data);
            }
            catch (Exception e)
            {
                Misc.Log(e.Message);
            }
        }

        /// <summary>
        /// Set a group of uniforms Vector4 on this shader.
        /// </summary>
        /// <param name="position">The integer position in shader of the uniform</param>
        /// <param name="count">The number of Vector4 group</param>
        /// <param name="data">The data to set</param>
        public void SetVector4Group(int position, int count, float[] data)
        {
            try
            {
                Use();
                GL.Uniform4(position, count, data);
                if (GlobalOptions.full_debug)
                    Misc.Log("Shader '" + Convert.ToString(Handle) + "' set vec4[] loc(" + Convert.ToString(position) + ")");
            }
            catch (Exception e)
            {
                Misc.Log(e.Message);
            }
        }

        /// <summary>
        /// Set a uniform Vector2 on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public virtual void SetVector2(string name, Vector2 data)
        {
            try
            {
                Use();
                GL.Uniform2(_uniformLocations[name], data);
                if (GlobalOptions.full_debug)
                    Misc.Log("Shader '" + Convert.ToString(Handle) + "' set vec2 '" + name + "' to vec2(" + Convert.ToString(data.X) + " " + Convert.ToString(data.Y) + ")");
            }
            catch (Exception e)
            {
                Misc.Log(e.Message);
            }
        }

        /// <summary>
        /// Set a uniform Vector4 on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public virtual void SetVector4(string name, Vector4 data)
        {
            try
            {
                Use();
                GL.Uniform4(_uniformLocations[name], data);

                if (GlobalOptions.full_debug)
                    Misc.Log("Shader '" + Convert.ToString(Handle) + "' set vec4 '" + name + "' to vec4(" + Convert.ToString(data.X) + " " + Convert.ToString(data.Y) + " " + Convert.ToString(data.Z) + " " + Convert.ToString(data.W) + ")");
            }
            catch (Exception e)
            {
                Misc.Log(e.Message);
            }
        }
        /// <summary>
        /// Get all uniform locations
        /// </summary>
        public void PrintUniformLocations()
        {
            foreach (var key in _uniformLocations.Keys)
            {
                Misc.Log(Convert.ToString(_uniformLocations[key]) + " " + key);
            }
        }
    }
}
