using LadaEngine.Engine.Base;
using LadaEngine.Engine.Common;
using LadaEngine.Engine.Global;
using OpenTK.Graphics.OpenGL4;

namespace LadaEngine.Engine.Renderables.GroupRendering;

/// <summary>
/// SpriteGroupRenderer class
/// Does all the job rendering a SpriteGroup object.
/// Supports camera position and zoom via shader by default
/// </summary>
public sealed class SpriteGroupRenderer
{
    private static readonly string StandartVert = @"#version 330 core
                                        layout(location = 0) in vec3 aPosition;
                                        layout(location = 1) in vec2 aTexCoord;

										uniform vec2 position;
                                        out vec2 texCoord;

                                        void main(void)
                                        {
                                            texCoord = aTexCoord;

                                            gl_Position = vec4(aPosition.xy - position.xy, aPosition.z + 0.1, 1.0);
                                        }";

    private static readonly string StandartFrag = StandartShaders.StandartFrag;
    private readonly ITextureAtlas _atlas;
    private readonly int _ebo;

    private readonly int _vao;
    private readonly int _vbo;
    private readonly List<float> _verts;

    private int[] _indices;
    private Common.SpriteGroup.SpriteGroup SpriteGroup;

    /// <summary>
    /// Shader for the all group
    /// Recommended to use StandartVert shader for vertex one as it supports camera position and zoom
    /// </summary>
    public Shader Shader;

    /// <summary>
    /// Create a SpriteGroupRenderer instance
    /// </summary>
    /// <param name="atlas">Texture atlas to be used</param>
    /// <param name="self">SpriteGroup to be rendered</param>
    public SpriteGroupRenderer(ITextureAtlas atlas, Common.SpriteGroup.SpriteGroup self)
    {
        SpriteGroup = self;
        _atlas = atlas;
        _verts = new List<float>();
        _indices = Array.Empty<int>();
        Shader = new Shader(StandartVert, StandartFrag, 0);


        _vao = GL.GenVertexArray();
        GL.BindVertexArray(_vao);

        _vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _verts.Count * sizeof(float), _verts.ToArray(),
            BufferUsageHint.DynamicDraw);

        _ebo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices,
            BufferUsageHint.StaticDraw);

        Shader.Use();
        var vertexLocation = Shader.GetAttribLocation("aPosition");
        GL.EnableVertexAttribArray(vertexLocation);
        GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

        var texCoordLocation = Shader.GetAttribLocation("aTexCoord");
        GL.EnableVertexAttribArray(texCoordLocation);
        GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float),
            3 * sizeof(float));
    }

    /// <summary>
    /// Refreshes information in GPU Buffers
    /// </summary>
    public void UpdateBuffers()
    {
        GL.BindVertexArray(_vao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, _verts.Count * sizeof(float), _verts.ToArray(),
            BufferUsageHint.DynamicDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices,
            BufferUsageHint.StaticDraw);
    }

    /// <summary>
    /// Render all sprites in the SpriteGroup
    /// </summary>
    /// <param name="camera">Unused param</param>
    /// <param name="updateVerts">If set to true UpdateVerts will ve called</param>
    public void Render(Camera camera, bool updateVerts = false)
    {
        if (updateVerts)
            UpdateVerts();

        GL.BindVertexArray(_vao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);

        _atlas.Use(TextureUnit.Texture0);
        Shader.Use();

        GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
    }

    /// <summary>
    /// Refreshes the vertices of the sprites it has
    /// !!! Can be costly for many sprites, use carefully!
    /// </summary>
    public void UpdateVerts()
    {
        _verts.Clear();
        foreach (var tile in SpriteGroup.Sprites) tile.AddToVerts(_verts);

        var objectCount = _verts.Count / 20;
        _indices = new int[6 * objectCount];
        for (var i = 0; i < objectCount; i++)
        {
            _indices[6 * i + 0] = 4 * i;
            _indices[6 * i + 1] = 4 * i + 1;
            _indices[6 * i + 2] = 4 * i + 3;
            _indices[6 * i + 3] = 4 * i + 1;
            _indices[6 * i + 4] = 4 * i + 2;
            _indices[6 * i + 5] = 4 * i + 3;
        }
    }
}