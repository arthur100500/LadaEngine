using LadaEngine.Engine.Base;

namespace LadaEngine.Engine.Common;

public class GameObject
{
    public int Group;
    public float Height;
    public int Level;
    public Pos Position;
    public float Rotation;
    public float Width;

    public GameObject(Pos position, float width, float height)
    {
        Group = -1;
        Level = 1;
        Width = width;
        Height = height;
        Position = position.Copy();
        Rotation = 0f;
    }
}