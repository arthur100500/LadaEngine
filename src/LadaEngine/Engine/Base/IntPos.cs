using System.Globalization;

namespace LadaEngine.Engine.Base;

/// <summary>
///     Class for storing 2d integer position
/// </summary>
public class IntPos
{
    /// <summary>
    /// X coordinate of the IntPos
    /// </summary>
    public int X;
    
    /// <summary>
    /// Y coordinate of the IntPos
    /// </summary>
    public int Y;
    
    /// <summary>
    /// Constructor of IntPos
    /// </summary>
    public IntPos(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static bool operator ==(IntPos? obj1, IntPos? obj2)
    {
        if (obj1 is null || obj2 is null)
            return false;
        return obj1.X == obj2.X && obj1.Y == obj2.Y;
    }

    public static bool operator !=(IntPos obj1, IntPos obj2)
    {
        return !(obj1 == obj2);
    }

    public static IntPos operator +(IntPos obj1, IntPos obj2)
    {
        return new IntPos(obj1.X + obj2.X, obj1.Y + obj2.Y);
    }

    public static IntPos operator -(IntPos obj1, IntPos obj2)
    {
        return new IntPos(obj1.X - obj2.X, obj1.Y - obj2.Y);
    }

    public static IntPos? operator *(IntPos? obj1, int another)
    {
        if (obj1 is null)
            return null;
        return new IntPos(obj1.X * another, obj1.Y * another);
    }

    public override string ToString()
    {
        return Convert.ToString(X) + " " + Convert.ToString(Y);
    }
    /// <summary>
    /// Makes a copy of IntPos
    /// </summary>
    /// <returns>New IntPos instance</returns>
    public IntPos Copy()
    {
        return new IntPos(X, Y);
    }
    
    /// <summary>
    /// Adds another to self, X to X and Y to Y
    /// </summary>
    /// <param name="another"></param>
    public void Add(IntPos another)
    {
        X += another.X;
        Y += another.Y;
    }

    public static explicit operator IntPos(Pos v)
    {
        return new IntPos((int)Math.Floor(v.X), (int)Math.Floor(v.Y));
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;

        if (ReferenceEquals(obj, null)) return false;

        if (obj.GetType() != typeof(IntPos)) return false;

        return ((IntPos)obj).X == X && ((IntPos)obj).Y == Y;
    }

    public override int GetHashCode()
    {
        return X + short.MaxValue * Y;
    }
}