using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using OpenTK;

namespace LadaEngine
{
    // Tilemap used to draw plane that contains of textures given
    public class Tilemap : IRenderable
    {
        public int width;
        public int height;
        public int[] map;

        public bool supports_shadow_map;

        private Quad quad;
        public Shader shader;
        public Texture textures;
        public Texture normalMap;


        public int grid_length = 10;
        public int grid_width = 10;
        public Tilemap(Texture textureLocation, int[] map, int height, int width, int gridLength, int gridRowLength)
        {
            this.height = height;
            this.width = width;
            this.map = (int[])map.Clone();
            // shader is a default tilemap shader
            shader = StandartShaders.GenTilemapShader();
            grid_length = gridLength;
            grid_width = gridRowLength;
            textures = textureLocation;
        }
        public Tilemap(string textureLocation, int[] map, int height, int width, int gridLength, int gridRowLength)
        {
            this.height = height;
            this.width = width;
            this.map = (int[])map.Clone();
            // shader is a default tilemap shader
            shader = StandartShaders.GenTilemapShader();
            grid_length = gridLength;
            grid_width = gridRowLength;
            textures = Texture.LoadFromFile(textureLocation);

            supports_shadow_map = false;
        }
        // With normal map
        public Tilemap(string textureLocation, string normalMapLocation, int[] map, int height, int width, int gridLength, int gridRowLength)
        {
            this.height = height;
            this.width = width;
            this.map = (int[])map.Clone();
            // shader is a default tilemap shader
            shader = StandartShaders.GenTilemapShaderNM();
            grid_length = gridLength;
            grid_width = gridRowLength;
            textures = Texture.LoadFromFile(textureLocation);
            normalMap = Texture.LoadFromFile(normalMapLocation);

            supports_shadow_map = false;
        }
        // With normal and shadow map
        public Tilemap(string textureLocation, string normalMapLocation, int[] map, int[] shadow_map, int height, int width, int gridLength, int gridRowLength)
        {
            this.height = height;
            this.width = width;
            this.map = (int[])map.Clone();
            // shader is a default tilemap shader
            shader = StandartShaders.GenTilemapShaderNM();
            grid_length = gridLength;
            grid_width = gridRowLength;
            textures = Texture.LoadFromFile(textureLocation);
            normalMap = Texture.LoadFromFile(normalMapLocation);

            supports_shadow_map = true;
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
                shader.SetInt("width", width);
                shader.SetInt("height", height);
                shader.SetInt("texture_length", grid_length);
                shader.SetInt("texture_width", grid_width);
            }
            catch (Exception ex)
            {
                shader.PrintUniformLocations();
                Console.WriteLine(ex);
            }


        }
        public void Render()
        {
            quad.Render();
        }

        public void SetLightSources(float[] positions, float[] colors)
        {
            quad.SetLightSources(positions, colors);
        }
    }
}