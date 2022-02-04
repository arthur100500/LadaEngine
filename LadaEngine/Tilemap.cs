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

        private Quad quad;
        public Shader shader;
        public Texture textures;
        public Texture normalMap;

        public BakedLight lightManager;

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
                shader.SetIntGroup(shader.GetUniformLocation("map_array[0]"), map.Length, map);
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
                Misc.Log("\n\n --- Tilemap render begin ---");
            if (lightManager != null)
            {
                lightManager.light_map.Use(TextureUnit.Texture2);
            }
            quad.Render();
            if (GlobalOptions.full_debug)
                Misc.Log(" --- Tilemap render end ---");
        }
        /// <summary>
        /// Adds static light to a pre-rendered texture
        /// </summary>
        public void AddStaticLight(LightSource light)
        {
            normalMap.Use(TextureUnit.Texture1);
            lightManager.AddLightTM(new float[] { light.X, light.Y, light.Z, light.Density }, light.Color, this);
        }
        /// <summary>
        /// Adds static light to a pre-rendered texture
        /// </summary>
        public void AddStaticLight(float[] positions, float[] colors)
        {
            normalMap.Use(TextureUnit.Texture1);
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
            rotation = angle;
            quad.rotate(angle);
        }
    }
}