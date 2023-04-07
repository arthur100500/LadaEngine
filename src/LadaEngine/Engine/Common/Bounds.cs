namespace LadaEngine.Engine.Common;

public class Bounds
{
    private readonly float[] coordinates;

    public Bounds(float x1, float y1, float x2, float y2)
    {
        coordinates = new[] { x1, y1, x2, y2 };
    }

    public Bounds(double x1, double y1, double x2, double y2)
    {
        coordinates = new[] { (float)x1, (float)y1, (float)x2, (float)y2 };
    }

    public float this[int index]
    {
        get => coordinates[index];
        set => coordinates[index] = value;
    }
}