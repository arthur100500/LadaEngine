using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;

namespace LadaEngine
{
    [Obsolete("Class Quad is kinda depricated, use Sprite or Tilemap instead")]
    internal class Quad : IRenderable
    {
        private bool is_initialised = false;

        private int _vertexArrayObject;
        private int _elementBufferObject;
        private int _vertexBufferObject;


        private float[] _vertices { get; set; }
        public Shader _shader;
        public Texture _texture;
        public bool supportsNormalMap = false;
        public Texture _normal_map = null;

        public int textureloc = -1;

        private float[] rel_angles = new float[] { 0f, (float)Math.PI / 2, (float)Math.PI, 3 * (float)Math.PI / 2 };
        private float rotation_angle = 0;
        public FPos centre = new FPos(0f, 0f);
        private float rad = 1.41421356237f;

        public static readonly uint[] _indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        public Quad(float[] coordinates, Shader shader, Texture texture)
        {
            _vertices = coordinates;
            _shader = shader;
            _texture = texture;
            ReshapeWithCoords(-coordinates[0], coordinates[1], -coordinates[10], coordinates[11]);
        }

        public Quad(float[] coordinates, Shader shader, Texture texture, Texture normalMap)
        {
            _vertices = coordinates;
            _shader = shader;
            _texture = texture;
            _normal_map = normalMap;
            supportsNormalMap = true;
            ReshapeWithCoords(-coordinates[0], coordinates[1], -coordinates[10], coordinates[11]);
        }
        public void ReshapeWithCoords(float top_x, float top_y, float bottom_x, float bottom_y)
        {
            _vertices = new float[]
            {
                -top_x, top_y, 0f, 1.0f, 1.0f, // top right
                -top_x, bottom_y, 0f, 1.0f, 0.0f, // bottom right
                -bottom_x, bottom_y, 0f, 0.0f, 0.0f, // bottom left
                -bottom_x, top_y, 0f, 0.0f, 1.0f // top left
            };
            // Rotation constants
            centre = new FPos((bottom_x + top_x) / 2, (bottom_y + top_y) / 2);
            rad = Misc.Len(centre, new FPos(bottom_x, bottom_y)); // This Rad should match all other rads because plane is rectangle shaped
                                                                  // AB (bottom x top y) - centre
                                                                  // vector starting from (0,0) presented with a pos
            FPos AB = new FPos(bottom_x - centre.X, top_y - centre.Y);
            // AC (bottom x bottom y) - centre
            // vector starting from (0,0) presented with a pos
            FPos AC = new FPos(bottom_x - centre.X, bottom_y - centre.Y);
            rel_angles[0] = (float)Math.Atan2(AB.X, AB.Y);
            rel_angles[1] = (float)Math.Atan2(AC.X, AC.Y);
            rel_angles[2] = (float)Math.Atan2(AB.X, AB.Y) + (float)Math.PI;
            rel_angles[3] = (float)Math.Atan2(AC.X, AC.Y) + (float)Math.PI;

        }

        public void Render()
        {
            // Uninitialized quad has unpredictible render
            if (!is_initialised)
            {
                is_initialised = true;
                Misc.Log("QUAD " + Convert.ToString(_texture.Handle) + " WAS NOT INITALISED");
            }
            // No render if quad is off screen
            if (!CheckBounds())
                return;

            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices,
                BufferUsageHint.DynamicDraw);

            if (supportsNormalMap)
                _normal_map.Use(TextureUnit.Texture1);
            _texture.Use(TextureUnit.Texture0);
            _shader.Use();

            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        private bool CheckBounds()
        {
            if (_vertices[0] < -1 && _vertices[10] < -1)
                return false;
            if (_vertices[0] > 1 && _vertices[10] > 1)
                return false;
            if (_vertices[1] < -1 && _vertices[11] < -1)
                return false;
            if (_vertices[1] > 1 && _vertices[11] > 1)
                return false;
            return true;
        }

        public void Render(FPos cam)
        {

            for (int i = 0; i < 4; i++)
            {
                _vertices[5 * i + 1] = _vertices[5 * i + 1] - cam.Y;
                _vertices[5 * i] = _vertices[5 * i] - cam.X;
            }

            Render();

            for (int i = 0; i < 4; i++)
            {
                _vertices[5 * i + 1] = _vertices[5 * i + 1] + cam.Y;
                _vertices[5 * i] = _vertices[5 * i] + cam.X;
            }
        }

        public void Load()
        {
            is_initialised = true;

            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices,
                BufferUsageHint.DynamicDraw);

            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices,
                BufferUsageHint.StaticDraw);

            _shader.Use();
            _shader.SetInt("texture0", 0);
            if (supportsNormalMap)
                _shader.SetInt("texture1", 1);
            var vertexLocation = _shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float),
                3 * sizeof(float));
            if (textureloc < 0)
                _texture.Use(TextureUnit.Texture0);
            else
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, textureloc);
            }
        }

        public void Unload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteBuffer(_elementBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);
        }

        public float[] GetRelativeCursorPosition(Window window, int x, int y)
        {
            //position relative to window
            float xpos = -1 + (-window.Location.X + x - 8) / (float)window.Size.X * 2;
            float ypos = 1 - (-window.Location.Y + y - 30) / (float)window.Size.Y * 2;

            //position relative to Quad
            xpos = 1f - (_vertices[0] - xpos) / (_vertices[0] - _vertices[15]);
            ypos = 1f - (_vertices[1] - ypos) / (_vertices[1] - _vertices[6]);
            return new float[] { xpos, ypos };
        }

        // Moves Quad directly via coordinates
        public void Move(float x_amount, float y_amount)
        {
            _vertices[0] += x_amount;
            _vertices[5] += x_amount;
            _vertices[10] += x_amount;
            _vertices[15] += x_amount;

            _vertices[1] += y_amount;
            _vertices[6] += y_amount;
            _vertices[11] += y_amount;
            _vertices[16] += y_amount;

            centre.X += x_amount;
            centre.Y += y_amount;
        }

        public void rotate(float angle)
        {
            angle = (float)Math.PI - angle;
            _vertices[0] = centre.X + rad * (float)Math.Cos(angle + rel_angles[2]);
            _vertices[1] = centre.Y + rad * (float)Math.Sin(angle + rel_angles[2]);
            _vertices[5] = centre.X + rad * (float)Math.Cos(angle + rel_angles[1]);
            _vertices[6] = centre.Y + rad * (float)Math.Sin(angle + rel_angles[1]);
            _vertices[10] = centre.X + rad * (float)Math.Cos(angle + rel_angles[0]);
            _vertices[11] = centre.Y + rad * (float)Math.Sin(angle + rel_angles[0]);
            _vertices[15] = centre.X + rad * (float)Math.Cos(angle + rel_angles[3]);
            _vertices[16] = centre.Y + rad * (float)Math.Sin(angle + rel_angles[3]);

            rotation_angle = angle;
        }
        public void SetLightSources(float[] positions, float[] colors)
        {
            for (int i = 0; i < positions.Length; i += 4)
            {
                FPos corrected_centre = new FPos((centre.X + 1) / 2, (centre.Y + 1) / 2);
                float dist = Misc.Len(corrected_centre, new FPos(positions[i + 0], positions[i + 1]));
                float angle = (float)Math.Atan((corrected_centre.X - positions[i + 0]) / (corrected_centre.Y - positions[i + 1]));
                if (corrected_centre.Y < positions[i + 1])
                    angle = 3.1415926535f + angle;
                angle += rotation_angle;
                angle = (float)Math.PI - angle;

                positions[i + 0] = (float)Math.Sin(angle) * dist + corrected_centre.X;
                positions[i + 1] = 1 - ((float)Math.Cos(angle) * dist + corrected_centre.Y);
            }
            _shader.SetVector4Group(_shader.GetUniformLocation("light_sources_colors[0]"), colors.Length, colors);
            _shader.SetVector4Group(_shader.GetUniformLocation("light_sources[0]"), positions.Length, positions);
        }
        public void ReshapeVertexArray(BaseObject obj, FPos cam)
        {
            centre = obj.centre;

            rad = (float)Math.Sqrt(obj.width * obj.width + obj.height * obj.height);

            FPos AB = new FPos(obj.height, obj.width);
            FPos AC = new FPos(obj.height, -obj.width);
            this.rel_angles[0] = (float)Math.Atan2(AB.X, AB.Y);
            this.rel_angles[1] = (float)Math.Atan2(AC.X, AC.Y);
            this.rel_angles[2] = (float)Math.Atan2(AB.X, AB.Y) + (float)Math.PI;
            this.rel_angles[3] = (float)Math.Atan2(AC.X, AC.Y) + (float)Math.PI;

            rotate(rotation_angle);
        }
    }
}
