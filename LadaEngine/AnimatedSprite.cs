using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using OpenTK;
using OpenTK.Mathematics;


namespace LadaEngine
{
    public class AnimatedSprite : BaseObject
    {
        private Quad quad;
        public Shader shader;
        public Texture textures;
        public Texture normal_map;

        public int state;
        public int grid_length = 10;
        public int grid_width = 10;

        public AnimatedSprite(Texture textureLocation, int gridLength, int gridRowLength)
        {

            // shader is a default tilemap shader
            shader = StandartShaders.ANIMATED_SHADER;
            grid_length = gridLength;
            grid_width = gridRowLength;
            textures = textureLocation;
             
            Load();
        }
        public void Load()
        {
            quad = new Quad(Misc.fullscreenverticies, shader, textures);
            quad.Load();

            if (GlobalOptions.full_debug)
                Misc.Log("Tilemap loaded");
        }

        public override void Render()
        {
            if (GlobalOptions.full_debug)
                Misc.Log("--- Tilemap render begin ---");

            shader.SetInt("texture_length", grid_length);
            shader.SetInt("texture_width", grid_width);
            shader.SetInt("state", state);
            quad.Render();
            if (GlobalOptions.full_debug)
                Misc.Log(" --- Tilemap render end ---");
        }


        public override void Render(FPos cam)
        {
            if (GlobalOptions.full_debug)
                Misc.Log("--- Tilemap render begin ---");

            shader.SetInt("texture_length", grid_length);
            shader.SetInt("texture_width", grid_width);
            shader.SetInt("state", state);
            quad.Render(cam);
            if (GlobalOptions.full_debug)
                Misc.Log(" --- Tilemap render end ---");
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
        public void Rotate(float angle)
        {
            rotation = angle;
            quad.rotate(angle);
        }
    }
}
