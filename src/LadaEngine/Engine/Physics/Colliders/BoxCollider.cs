using LadaEngine.Engine.Base;
using LadaEngine.Engine.Common;

namespace LadaEngine.Engine.Physics.Colliders;

/// <summary>
/// Represents box collider with set width and height and constant rotation
/// </summary>
public class BoxCollider : GameObject
{
    /// <summary>
    /// Action to be called when collision happens
    /// </summary>
    public Action OnCollisionAction = () => { };

    /// <summary>
    /// Creates new box collider
    ///
    /// !!! Copies the position
    /// </summary>
    /// <param name="position">Position of collider (will be copied)</param>
    /// <param name="width">Width of collider</param>
    /// <param name="height">Height of collider</param>
    public BoxCollider(Pos position, float width, float height) : base(position, width, height)
    {
    }

    /// <summary>
    /// Is the object collidable?
    /// </summary>
    public bool IsNotCollidable { get; set; }

    /// <summary>
    /// Checks for collision with other game object. 
    /// </summary>
    /// <param name="other">Other game object to check collision with</param>
    /// <returns>true if collision happened and false if not</returns>
    public bool CheckCollision(GameObject other)
    {
        if (Position.X + Width / 2 >= other.Position.X - other.Width / 2 &&
            Position.X - Width / 2 <= other.Position.X + other.Width / 2 &&
            Position.Y + Height / 2 >= other.Position.Y - other.Height / 2 &&
            Position.Y - Height / 2 <= other.Position.Y + other.Height / 2)
            return true;
        return false;
    }

    /// <summary>
    /// On collision method
    /// </summary>
    /// <param name="other"></param>
    public void OnCollision(BoxCollider other)
    {
        OnCollisionAction();
    }
}