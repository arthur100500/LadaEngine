using LadaEngine.Engine.Base;

namespace LadaEngine.Engine.Common;

/// <summary>
///     Base object with
///     Group, Height, Level, Position, Rotation, Width and Height
/// </summary>
public class GameObject
{
    /// <summary>
    ///     Idk what it is
    /// </summary>
    public int Group;

    /// <summary>
    ///     Height of the game object
    /// </summary>
    public float Height;

    /// <summary>
    ///     Z Level of the game object
    /// </summary>
    public int Level;

    /// <summary>
    ///     Position of the game object
    /// </summary>
    public Pos Position;

    /// <summary>
    ///     Rotation of the game object
    /// </summary>
    public float Rotation;

    /// <summary>
    ///     Width of the game object
    /// </summary>
    public float Width;

    /// <summary>
    ///     Creates new game object with set position, width and height.
    ///     Also sets parameters to:
    ///     Group: -1
    ///     Level: 1
    ///     Rotation: 0
    ///     Copies position!
    /// </summary>
    /// <param name="position">Position of new object</param>
    /// <param name="width">Width of new object</param>
    /// <param name="height">height of new object</param>
    public GameObject(Pos position, float width, float height)
    {
        Group = -1;
        Level = 1;
        Width = width;
        Height = height;
        Position = position.Copy();
        Rotation = 0f;
    }

    public GameObject()
    {
        Group = -1;
        Level = 1;
        Width = 1;
        Height = 1;
        Position = Pos.Zero;
        Rotation = 0f;
    }
}
