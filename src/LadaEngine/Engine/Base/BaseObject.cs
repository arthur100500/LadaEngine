using LadaEngine.Engine.Common;

namespace LadaEngine.Engine.Base;

public abstract class BaseObject : IRenderable
{
    /// <summary>
    ///     Center of the object
    /// </summary>
    public Pos center;

    /// <summary>
    ///     Height of the object (in initial position, not rotated)
    /// </summary>
    public float height;

    /// <summary>
    ///     Rotation of the object
    /// </summary>
    public float rotation;

    /// <summary>
    ///     Width of the object (in initial position, not rotated)
    /// </summary>
    public float width;

    public abstract void Render(Camera cam);

    // To be implemented in any other child
    public abstract void ReshapeVertexArray();
}