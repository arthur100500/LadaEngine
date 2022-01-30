using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using OpenTK;

namespace LadaEngine
{
    // Tilemap used to draw plane that contains of textures given
    public class Tilemap : BaseObject
    {
        public int tm_width;
        public int tm_height;
        public int[] map;

        private Quad quad;
        public Shader shader;
        public Texture textures;
        public Texture normalMap;

        private BakedLight lightManager;

        public int grid_length = 10;
        public int grid_width = 10;
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
            normalMap = Texture.LoadFromFile(normalMapLocation);
            lightManager = new BakedLight();
            Load();
        }
        public void Load()
        {
            if (normalMap != null)
            {
                quad = new Quad(Misc.fullscreenverticies, shader, textures, normalMap);
                quad.supportsNormalMap = true;
            }
            else{
                quad = new Quad(Misc.fullscreenverticies, shader, textures);
            }
            lightManager.Load(StandartShaders.tm_light_gen);
            quad.Load();
            UpdateMap();
        }


        // Sends needed info to shader as uniform
        public void UpdateMap()
        {
            // Set map array to shader
            // 50x50 max grid
            try
            {
                shader.SetIntGroup(shader.GetUniformLocation("map_array[0]"), map.Length, map);
                shader.SetInt("width", tm_width);
                shader.SetInt("height", tm_height);
                shader.SetInt("texture_length", grid_length);
                shader.SetInt("texture_width", grid_width);
                shader.SetInt("static_light", 2);
            }
            catch (Exception ex)
            {
                shader.PrintUniformLocations();
                Console.WriteLine(ex);
            }


        }
        public override void Render()
        {
            if (lightManager != null)
            {
                lightManager.light_map.Use(OpenTK.Graphics.OpenGL.TextureUnit.Texture2);
            }
            quad.Render();
        }
        /// <summary>
        /// Adds static light to a pre-rendered texture
        /// </summary>
        public void AddStaticLight(LightSource light)
        {
            lightManager.AddLightTM(new float[] { light.X, light.Y, light.Z, light.Density }, light.Color, this);
        }
        /// <summary>
        /// Adds static light to a pre-rendered texture
        /// </summary>
        public void AddStaticLight(float[] positions, float[] colors)
        {
            lightManager.AddLightTM(positions, colors, this);
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
            quad.rotate(angle);
        }
    }
}