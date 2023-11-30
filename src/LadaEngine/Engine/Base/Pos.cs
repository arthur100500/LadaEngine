using System.Globalization;

namespace LadaEngine.Engine.Base;

/// <summary>
///     Class for storing 2d float position
/// </summary>
public class Pos
{
    /// <summary>
    ///     X coordinate of the Pos
    /// </summary>
    public float X;

    /// <summary>
    ///     Y coordinate of the Pos
    /// </summary>
    public float Y;

    /// <summary>
    ///     Creates Pos from 2 floats
    /// </summary>
    public Pos(float x, float y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    ///     Creates Pos from IntPos
    /// </summary>
    public Pos(IntPos pos)
    {
        X = pos.X;
        Y = pos.Y;
    }

    /// <summary>
    ///     Creates Pos from 2 doubles
    /// </summary>
    public Pos(double x, double y)
    {
        X = (float)x;
        Y = (float)y;
    }

    /// <summary>
    ///     Get new Pos instance with representing (0, 0) vector
    /// </summary>
    public static Pos Zero => new(0, 0);

    public static bool operator ==(Pos obj1, Pos obj2)
    {
        return Math.Abs(obj1.X - obj2.X) < float.Epsilon && Math.Abs(obj1.Y - obj2.Y) < float.Epsilon;
    }

    public static bool operator !=(Pos obj1, Pos obj2)
    {
        return !(obj1 == obj2);
    }

    public static Pos operator +(Pos obj1, Pos obj2)
    {
        return new Pos(obj1.X + obj2.X, obj1.Y + obj2.Y);
    }

    public static Pos operator +(Pos obj1, IntPos obj2)
    {
        return new Pos(obj1.X + obj2.X, obj1.Y + obj2.Y);
    }

    public static Pos operator +(IntPos obj1, Pos obj2)
    {
        return new Pos(obj1.X + obj2.X, obj1.Y + obj2.Y);
    }

    public static Pos operator -(Pos obj1, Pos obj2)
    {
        return new Pos(obj1.X - obj2.X, obj1.Y - obj2.Y);
    }

    public static Pos operator *(Pos obj1, float another)
    {
        return new Pos(obj1.X * another, obj1.Y * another);
    }

    public static Pos operator *(float another, Pos obj1)
    {
        return new Pos(obj1.X * another, obj1.Y * another);
    }

    public override string ToString()
    {
        return Convert.ToString(X, CultureInfo.InvariantCulture) + " " +
               Convert.ToString(Y, CultureInfo.InvariantCulture);
    }

    /// <summary>
    ///     Creates a copy of the Pos
    /// </summary>
    /// <returns>Pos which is identical to this</returns>
    public Pos Copy()
    {
        return new Pos(X, Y);
    }

    /// <summary>
    ///     Adds another to self, X to X and Y to Y
    /// </summary>
    /// <param name="another"></param>
    public void Add(Pos another)
    {
        X += another.X;
        Y += another.Y;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;

        if (ReferenceEquals(obj, null)) return false;

        if (obj.GetType() != typeof(Pos)) return false;


        return Math.Abs(((Pos)obj).X - X) < float.Epsilon && Math.Abs(((Pos)obj).Y - Y) < float.Epsilon;
    }

    public override int GetHashCode()
    {
        // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
        return base.GetHashCode();
    }
}