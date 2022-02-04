using OpenTK;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace LadaEngine
{
    /// <summary>
    /// Wrapper clas to the Quad class
    /// </summary>
    public class Sprite : BaseObject
    {
        // Important
        private Quad quad;
        private Shader shader;

        // Not so important
        Texture texture = null;
        Texture normal_map = null;

        public BakedLight lightManager = null;

        /// <summary>
        /// Constructor for standart sprite, no dynamic light, no static light
        /// </summary>
        /// <param name="shader"></param>
        /// <param name="texture"></param>
        public Sprite(Texture texture)
        {
            this.texture = texture;
            quad = new Quad((float[])Misc.fullscreenverticies.Clone(), StandartShaders.GenStandartShader(), texture);
            quad.Load();
        }

        /// <summary>
        /// Constructor for standart sprite, shader can be chosen
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="normal_map"></param>
        /// <param name="shader"></param>
        public Sprite(Texture texture, Texture normal_map, Shader shader)
        {
            this.normal_map = normal_map;
            this.texture = texture;
            this.shader = shader;
            quad = new Quad((float[])Misc.fullscreenverticies.Clone(), shader, texture, normal_map);
            lightManager = new BakedLight();
            lightManager.Load(StandartShaders.light_gen);
            quad.Load();
            quad._shader.SetInt("static_light", 2);
        }
        /// <summary>
        /// Rotates sprite to an angle (in rad)
        /// </summary>
        /// <param name="angle"></param>
        public void Rotate(float angle)
        {
            rotation = angle;
            quad.rotate(angle);
        }
        /// <summary>
        /// Reshape the sprite (-1 -1 1 1 coordinate system)
        /// </summary>
        /// <param name="top_x"></param>
        /// <param name="top_y"></param>
        /// <param name="bottom_x"></param>
        /// <param name="bottom_y"></param>
        public void Reshape(float top_x, float top_y, float bottom_x, float bottom_y )
        {
            quad.ReshapeWithCoords(top_x, top_y, bottom_x, bottom_y);
        }
        /// <summary>
        /// Render the image (use load first)
        /// </summary>
        public override void Render()
        {
            if (GlobalOptions.full_debug)
                Misc.Log("\n\n --- Sprite render begin ---");
            if (lightManager != null)
                lightManager.light_map.Use(TextureUnit.Texture2);

            quad.Render();
            if (GlobalOptions.full_debug)
                Misc.Log("--- Sprite render end --- \n\n");
        }
        /// <summary>
        /// Free the resources
        /// </summary>
        public void Unload()
        {
            if (quad != null)
                quad.Unload();
        }

        /// <summary>
        /// Adds static light to a pre-rendered texture
        /// </summary>
        public void AddStaticLight(LightSource light)
        {
            normal_map.Use(TextureUnit.Texture1);
            lightManager.AddLight(new float[] { light.X, light.Y, light.Z, light.Density }, light.Color, this);
        }
        /// <summary>
        /// Adds static light to a pre-rendered texture
        /// </summary>
        public void AddStaticLight(float[] positions, float[]colors)
        {
            normal_map.Use(TextureUnit.Texture1);
            lightManager.AddLight(positions, colors, this);
        }
        /// <summary>
        /// Adds dynamic light (should be done once per frame)
        /// </summary>
        public void AddDynamicLight(float[] positions, float[] colors)
        {
            quad.SetLightSources(positions, colors);
        }
        /// <summary>
        /// Deletes all the light sources rendered to a sprite
        /// </summary>
        public void ClearStaticLight()
        {
            lightManager.ClearLights();
        }
        public void SetAmbient(float[] color)
        {
            quad._shader.SetVector4("ambient", new Vector4(color[0], color[1], color[2], color[3]));
        }
        public override void ReshapeVertexArray(FPos camera_position)
        {
            quad.ReshapeVertexArray(this, camera_position);
        }
    }
}
