using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using OpenTK;
using OpenTK.Mathematics;


namespace LadaEngine
{
    // Tilemap used to draw plane that contains of textures given
    public class Tilemap : BaseObject
    {
        public int tm_width;
        public int tm_height;
        public int[] map;

        private int SSBO;

        private Quad quad;
        public Shader shader;
        public Texture textures;
        public Texture normal_map;

        public BakedLight lightManager;

        public int grid_length = 10;
        public int grid_width = 10;
        public void SetLightMapResolution(Pos resolution)
        {
            lightManager.resolution = resolution;
        }
        public Tilemap(Texture textureLocation, int[] map, int height, int width, int gridLength, int gridRowLength)
        {
            this.tm_height = height;
            this.tm_width = width;
            this.map = (int[])map.Clone();
            // shader is a default tilemap shader
            shader = StandartShaders.GenTilemapShader();
            grid_length = gridLength;
            grid_width = gridRowLength;
            textures = textureLocation;

            SSBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, SSBO);
            Load();
        }
        public Tilemap(string textureLocation, int[] map, int height, int width, int gridLength, int gridRowLength)
        {
            this.tm_height = height;
            this.tm_width = width;
            this.map = (int[])map.Clone();
            // shader is a default tilemap shader
            shader = StandartShaders.GenTilemapShader();
            grid_length = gridLength;
            grid_width = gridRowLength;
            textures = Texture.LoadFromFile(textureLocation);

            SSBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, SSBO);
            Load();
        }
        // With normal map
        public Tilemap(string textureLocation, string normalMapLocation, int[] map, int height, int width, int gridLength, int gridRowLength)
        {
            this.tm_height = height;
            this.tm_width = width;
            this.map = (int[])map.Clone();
            // shader is a default tilemap shader
            shader = StandartShaders.GenTilemapShaderNMSL();
            grid_length = gridLength;
            grid_width = gridRowLength;
            textures = Texture.LoadFromFile(textureLocation);
            normal_map = Texture.LoadFromFile(normalMapLocation);
            lightManager = new BakedLight();

            SSBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ShaderStorageBuffer, SSBO);
            Load();
        }
        public void Load()
        {
            if (normal_map != null)
            {
                quad = new Quad(Misc.fullscreenverticies, shader, textures, normal_map);
                quad.supportsNormalMap = true;
            }   
            else{
                quad = new Quad(Misc.fullscreenverticies, shader, textures);
            }
            if (lightManager != null)
                lightManager.Load(StandartShaders.tm_light_gen);
            quad.Load();
            UpdateMap();

            if (GlobalOptions.full_debug)
                Misc.Log("Tilemap loaded");
        }


        // Sends needed info to shader as uniform
        public void UpdateMap()
        {
            // Set map array to shader
            // 50x50 max grid
            try
            {
                GL.BindBuffer(BufferTarget.ShaderStorageBuffer, SSBO);
                GL.BufferData(BufferTarget.ShaderStorageBuffer, map.Length * 4, map, BufferUsageHint.DynamicDraw);
                
                shader.SetInt("width", tm_width);
                shader.SetInt("height", tm_height);
                shader.SetInt("texture_length", grid_length);
                shader.SetInt("texture_width", grid_width);
                if (lightManager != null)
                {
                    shader.SetInt("static_light", 2);
                    shader.SetVector2("texture_size", new Vector2(width, height));
                    shader.SetFloat("texture_rotation", rotation);
                }
            }
            catch (Exception ex)
            {
                shader.PrintUniformLocations();
                Console.WriteLine(ex);
            }


        }
        public override void Render()
        {
            if (GlobalOptions.full_debug)
                Misc.Log("--- Tilemap render begin ---");
            if (lightManager != null && quad.CheckBounds())
            {
                lightManager.light_map.Use(TextureUnit.Texture2);
            }

            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 1, SSBO);
            quad.Render();
            if (GlobalOptions.full_debug)
                Misc.Log(" --- Tilemap render end ---");
        }


        public override void Render(FPos cam)
        {
            if (GlobalOptions.full_debug)
                Misc.Log("--- Tilemap render begin ---");
            if (lightManager != null && quad.CheckBounds(cam))
            {
                lightManager.light_map.Use(TextureUnit.Texture2);
            }
            GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, 1, SSBO);
            quad.Render(cam);
            if (GlobalOptions.full_debug)
                Misc.Log(" --- Tilemap render end ---");
        }
        /// <summary>
        /// Adds static light to a pre-rendered texture
        /// </summary>
        public void AddStaticLight(LightSource light)
        {
            normal_map.Use(TextureUnit.Texture1);
            light.X -= centre.X;
            light.Y -= centre.Y;

            light.X /= width;
            light.Y /= height;

            light.X = (light.X + 1) / 2;
            light.Y = (light.Y + 1) / 2;
            lightManager.AddLight(new float[] { light.X, light.Y, light.Z, light.Density }, light.Color, this);
            light.X = (light.X * 2) - 1;
            light.Y = (light.Y * 2) - 1;

            light.X *= width;
            light.Y *= height;

            light.X += centre.X;
            light.Y += centre.Y;
        }
        /// <summary>
        /// Adds static light to a pre-rendered texture
        /// </summary>
        public void AddStaticLight(float[] positions, float[] colors)
        {
            normal_map.Use(TextureUnit.Texture1);
            // Transforming
            positions[0] -= centre.X;
            positions[1] -= centre.Y;

            positions[0] /= width;
            positions[1] /= height;

            positions[0] = (positions[0] + 1) / 2;
            positions[1] = (positions[1] + 1) / 2;
            lightManager.AddLight(positions, colors, this);
            // Restoring to original transformation
            positions[0] = (positions[0] * 2) - 1;
            positions[1] = (positions[1] * 2) - 1;

            positions[0] *= width;
            positions[1] *= height;

            positions[0] += centre.X;
            positions[1] += centre.Y;
        }
        public void SetLightSources(float[] positions, float[] colors)
        {
            quad.SetLightSources(positions, colors);
        }
        public void SetAmbient(float[] color)
        {
            quad._shader.SetVector4("ambient", new Vector4(color[0], color[1], color[2], color[3]));
        }

        public override void ReshapeVertexArray(FPos camera_position)
        {
            quad.ReshapeVertexArray(this, camera_position);
        }
        public void ClearStaticLight()
        {
            lightManager.ClearLights();
        }
        public void Rotate(float angle)
        {
            rotation = angle;
            quad.rotate(angle);
        }
    }
}