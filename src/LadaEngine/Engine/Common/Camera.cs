using LadaEngine.Engine.Base;

namespace LadaEngine.Engine.Common;

/// <summary>
///     Objects that has position and zoom properties
///     Zoom and position are applied on SpriteGroup render
///     (passed its via shader uniform)
/// </summary>
public class Camera
{
    /// <summary>
    ///     Position of the camera
    /// </summary>
    public Pos Position;

    /// <summary>
    ///     Zoom of the camera
    /// </summary>
    public float Zoom;

    /// <summary>
    ///     Creates new camera object
    /// </summary>
    /// <param name="position">Position of the camera</param>
    /// <param name="zoom">Zoom of the camera</param>
    public Camera(Pos position, float zoom)
    {
        Position = position;
        Zoom = zoom;
    }

    /// <summary>
    ///     Creates new camera object in (0, 0) and with zoom of 1
    /// </summary>
    public Camera()
    {
        Position = new Pos(0, 0);
        Zoom = 1f;
    }
}