using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace LadaEngine
{
    [Obsolete("Class Quad is kinda depricated, use Sprite or Tilemap instead")]
    internal class Quad : IRenderable
    {
        private bool is_initialised = false;

        private int _vertexArrayObject;
        private int _elementBufferObject;
        private int _vertexBufferObject;

        public float[] zoom_info;
        public float zoom_coeff = 0.2f;
        public float[] zoom_coord;
        private float[] _vertices { get; set; }
        public Shader _shader;
        public Texture _texture;
        public bool supportsNormalMap = false;
        public Texture _normal_map = null;

        public int textureloc = -1;
        public bool zoomable = true;

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
            zoom_info = new float[] { _vertices[0], _vertices[15], _vertices[1], _vertices[6] };
            ReshapeWithCoords(-coordinates[0], coordinates[1], -coordinates[10], coordinates[11]);
        }

        public Quad(float[] coordinates, Shader shader, Texture texture, Texture normalMap)
        {
            _vertices = coordinates;
            _shader = shader;
            _texture = texture;
            _normal_map = normalMap;
            supportsNormalMap = true;
            zoom_info = new float[] { _vertices[0], _vertices[15], _vertices[1], _vertices[6] };
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
            this.rel_angles[0] = (float)Math.Atan2(AB.X, AB.Y);
            this.rel_angles[1] = (float)Math.Atan2(AC.X, AC.Y);
            this.rel_angles[2] = (float)Math.Atan2(AB.X, AB.Y) + (float)Math.PI;
            this.rel_angles[3] = (float)Math.Atan2(AC.X, AC.Y) + (float)Math.PI;

            zoom_info = new float[] { _vertices[0], _vertices[15], _vertices[1], _vertices[6] };
        }

        public void Render()
        {
            // Uninitialized quad has unpredictible render
            if (!is_initialised)
            {
                is_initialised = true;
                Misc.Log("QUAD " + Convert.ToString(_texture.Handle) + " WAS NOT INITALISED");
            }
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices,
                BufferUsageHint.DynamicDraw);

            if (supportsNormalMap)
                _normal_map.Use(TextureUnit.Texture1);
            _texture.Use(TextureUnit.Texture0);

            _shader.Use();
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
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

            GL.DeleteProgram(_shader.Handle);
            if (textureloc < 0)
            {
                GL.DeleteTexture(_texture.Handle);
            }
            else
            {
                GL.DeleteTexture(textureloc);
            }
        }

        public float[] GetRelativeCursorPosition(Window window, int x, int y)
        {
            //position relative to window
            float xpos = -1 + (-window.Location.X + x - 8) / (float)window.Width * 2;
            float ypos = 1 - (-window.Location.Y + y - 30) / (float)window.Height * 2;
            //position relative toQuad
            //zoom_info = new float[] { _vertices[0], _vertices[15], _vertices[1], _vertices[6] };
            xpos = 1f - (_vertices[0] - xpos) / (_vertices[0] - _vertices[15]);
            ypos = 1f - (_vertices[1] - ypos) / (_vertices[1] - _vertices[6]);
            return new float[] { xpos, ypos };
        }

        public float[] GetZoomRelativeCursorPosition(Window window, int x, int y)
        {
            float xpos = -1 + (-window.Location.X + x - 8) / (float)window.Width * 2;
            float ypos = 1 - (-window.Location.Y + y - 30) / (float)window.Height * 2;
            xpos = 1f - (_vertices[0] - xpos) / (_vertices[0] - _vertices[15]);
            xpos = _vertices[13] - xpos * (_vertices[13] - _vertices[3]);
            ypos = 1f - (_vertices[1] - ypos) / (_vertices[1] - _vertices[6]);
            ypos = _vertices[9] - ypos * (_vertices[9] - _vertices[4]);
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

        public void ZoomIn(float[] cursor_coords)
        {
            zoom_coeff *= 0.99f;
            _vertices = new float[]
            {
                _vertices[0], _vertices[1], _vertices[2],
                Zoom_Normalize((_vertices[3] + cursor_coords[0] * zoom_coeff) / (1f + zoom_coeff)),
                Zoom_Normalize((_vertices[4] + cursor_coords[1] * zoom_coeff) / (1f + zoom_coeff)),
                _vertices[5], _vertices[6], _vertices[7],
                Zoom_Normalize((_vertices[8] + cursor_coords[0] * zoom_coeff) / (1f + zoom_coeff)),
                Zoom_Normalize((_vertices[9] + cursor_coords[1] * zoom_coeff) / (1f + zoom_coeff)),
                _vertices[10], _vertices[11], _vertices[12],
                Zoom_Normalize((_vertices[13] + cursor_coords[0] * zoom_coeff) / (1f + zoom_coeff)),
                Zoom_Normalize((_vertices[14] + cursor_coords[1] * zoom_coeff) / (1f + zoom_coeff)),
                _vertices[15], _vertices[16], _vertices[17],
                Zoom_Normalize((_vertices[18] + cursor_coords[0] * zoom_coeff) / (1f + zoom_coeff)),
                Zoom_Normalize((_vertices[19] + cursor_coords[1] * zoom_coeff) / (1f + zoom_coeff))
            };
            zoom_info = new float[]
            {
                (zoom_info[0] + cursor_coords[0] * zoom_coeff) / (1f + zoom_coeff),
                (zoom_info[1] + cursor_coords[0] * zoom_coeff) / (1f + zoom_coeff), zoom_info[2], zoom_info[3]
            };
        }

        public void ZoomOut(float[] cursor_coords)
        {
            zoom_coeff /= 0.99f;
            _vertices = new float[]
            {
                _vertices[0], _vertices[1], _vertices[2],
                Zoom_Normalize((_vertices[3] - cursor_coords[0] * zoom_coeff) / (1f - zoom_coeff)),
                Zoom_Normalize((_vertices[4] - cursor_coords[1] * zoom_coeff) / (1f - zoom_coeff)),
                _vertices[5], _vertices[6], _vertices[7],
                Zoom_Normalize((_vertices[8] - cursor_coords[0] * zoom_coeff) / (1f - zoom_coeff)),
                Zoom_Normalize((_vertices[9] - cursor_coords[1] * zoom_coeff) / (1f - zoom_coeff)),
                _vertices[10], _vertices[11], _vertices[12],
                Zoom_Normalize((_vertices[13] - cursor_coords[0] * zoom_coeff) / (1f - zoom_coeff)),
                Zoom_Normalize((_vertices[14] - cursor_coords[1] * zoom_coeff) / (1f - zoom_coeff)),
                _vertices[15], _vertices[16], _vertices[17],
                Zoom_Normalize((_vertices[18] - cursor_coords[0] * zoom_coeff) / (1f - zoom_coeff)),
                Zoom_Normalize((_vertices[19] - cursor_coords[1] * zoom_coeff) / (1f - zoom_coeff))
            };
            zoom_info = new float[]
            {
                (zoom_info[0] + cursor_coords[0] * zoom_coeff) / (1f + zoom_coeff),
                (zoom_info[1] + cursor_coords[0] * zoom_coeff) / (1f + zoom_coeff), zoom_info[2], zoom_info[3]
            };
        }

        public void ZoomReset()
        {
            zoom_coeff = 0.2f;
            _vertices = new float[]
            {
                _vertices[0], _vertices[1], _vertices[2], 1f, 1f,
                _vertices[5], _vertices[6], _vertices[7], 1f, 0f,
                _vertices[10], _vertices[11], _vertices[12], 0f, 0f,
                _vertices[15], _vertices[16], _vertices[17], 0f, 1f
            };
            zoom_info = new float[] { _vertices[0], _vertices[15], _vertices[1], _vertices[6] };
        }

        public void ZoomDrag(float[] cursor_coords1, float[] cursor_coords2)
        {
            float[] diff = new float[] { cursor_coords2[0] - cursor_coords1[0], cursor_coords2[1] - cursor_coords1[1] };
            if (IB10(_vertices[3] + diff[0]) && IB10(_vertices[8] + diff[0]) && IB10(_vertices[13] + diff[0]) &&
                IB10(_vertices[18] + diff[0]))
                _vertices = new float[]
                {
                    _vertices[0], _vertices[1], _vertices[2], _vertices[3] + diff[0], _vertices[4],
                    _vertices[5], _vertices[6], _vertices[7], _vertices[8] + diff[0], _vertices[9],
                    _vertices[10], _vertices[11], _vertices[12], _vertices[13] + diff[0], _vertices[14],
                    _vertices[15], _vertices[16], _vertices[17], _vertices[18] + diff[0], _vertices[19]
                };
            if (IB10(_vertices[4] + diff[0]) && IB10(_vertices[9] + diff[0]) && IB10(_vertices[14] + diff[0]) &&
                IB10(_vertices[19] + diff[0]))
                _vertices = new float[]
                {
                    _vertices[0], _vertices[1], _vertices[2], _vertices[3], _vertices[4] + diff[1],
                    _vertices[5], _vertices[6], _vertices[7], _vertices[8], _vertices[9] + diff[1],
                    _vertices[10], _vertices[11], _vertices[12], _vertices[13], _vertices[14] + diff[1],
                    _vertices[15], _vertices[16], _vertices[17], _vertices[18], _vertices[19] + diff[1]
                };
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


            _vertices[1] /= Misc.screen_ratio;
            _vertices[6] /= Misc.screen_ratio;
            _vertices[11] /= Misc.screen_ratio;
            _vertices[16] /= Misc.screen_ratio;

            rotation_angle = angle;
        }

        public void rotate_nc(float angle)
        {
            _vertices[0] = centre.X + rad * (float)Math.Cos(angle + rel_angles[0]);
            _vertices[1] = centre.Y + rad * (float)Math.Sin(angle + rel_angles[0]);
            _vertices[5] = centre.X + rad * (float)Math.Cos(angle + rel_angles[1]);
            _vertices[6] = centre.Y + rad * (float)Math.Sin(angle + rel_angles[1]);
            _vertices[10] = centre.X + rad * (float)Math.Cos(angle + rel_angles[3]);
            _vertices[11] = centre.Y + rad * (float)Math.Sin(angle + rel_angles[3]);
            _vertices[15] = centre.X + rad * (float)Math.Cos(angle + rel_angles[2]);
            _vertices[16] = centre.Y + rad * (float)Math.Sin(angle + rel_angles[2]);


            rotation_angle = angle;
        }
        private float Zoom_Normalize(float number)
        {
            return Math.Min(Math.Max(0f, number), 1f);
        }

        private bool IB10(float n)
        {
            return (n <= 1.0f && n >= 0.0f);
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

            FPos AB = new FPos(obj.width, obj.height);
            FPos AC = new FPos(obj.width, -obj.height);
            this.rel_angles[0] = (float)Math.Atan2(AB.X, AB.Y);
            this.rel_angles[1] = (float)Math.Atan2(AC.X, AC.Y);
            this.rel_angles[2] = (float)Math.Atan2(AB.X, AB.Y) + (float)Math.PI;
            this.rel_angles[3] = (float)Math.Atan2(AC.X, AC.Y) + (float)Math.PI;

            rotate(rotation_angle);
        }
    }
}
