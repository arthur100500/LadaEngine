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
        public override void Render(FPos cam)
        {
            if (GlobalOptions.full_debug)
                Misc.Log("--- Anim render begin ---");

            quad.texture = textures;
            shader.SetInt("texture_length", grid_length);
            shader.SetInt("texture_width", grid_width);
            shader.SetInt("state", state);
            quad.Render(cam);
            if (GlobalOptions.full_debug)
                Misc.Log(" --- Anim render end ---");
        }

        public override void ReshapeVertexArray()
        {
            quad.ReshapeVertexArray(this);
        }
        public void Rotate(float angle)
        {
            rotation = angle;
            quad.rotate(angle);
        }

        public void FlipX()
        {
            quad.FlipX();
        }
        public void FlipY()
        {
            quad.FlipY();
        }
    }
}
