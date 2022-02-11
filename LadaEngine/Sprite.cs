﻿using OpenTK;
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
        internal Quad quad;
        public Shader shader { get; internal set; }

        // Optional
        Texture texture = null;

        /// <summary>
        /// Constructor for standart sprite, no dynamic light, no static light
        /// </summary>
        /// <param name="shader"></param>
        /// <param name="texture"></param>
        public Sprite(Texture texture)
        {
            this.texture = texture;
            quad = new Quad((float[])Misc.fullscreenverticies.Clone(), StandartShaders.STANDART_SHADER, texture);
            quad.Load();
        }

        public void SetShader(Shader shader)
        {
            this.shader = shader;
            quad.shader = shader;
        }

        /// <summary>
        /// Constructor for standart sprite, shader can be chosen
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="shader"></param>
        public Sprite(Texture texture, Shader shader)
        {
            this.shader = shader;
            this.texture = texture;
            quad = new Quad((float[])Misc.fullscreenverticies.Clone(), shader, texture);
            quad.Load();
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
        public override void Render(FPos cam)
        {
            if (GlobalOptions.full_debug)
                Misc.Log("--- Sprite render begin ---");
            quad.Render(cam);
            if (GlobalOptions.full_debug)
                Misc.Log("--- Sprite render end ---");
        }

        /// <summary>
        /// Free the resources
        /// </summary>
        public void Unload()
        {
            if (quad != null)
                quad.Unload();
        }
        public override void ReshapeVertexArray(FPos camera_position)
        {
            quad.ReshapeVertexArray(this, camera_position);
        }
    }
}
