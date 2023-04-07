namespace LadaEngine.Engine.Base;

/// <summary>
///     Class for storing 2d integer position
/// </summary>
public class IntPos
{
    public int X;
    public int Y;

    public IntPos(int X, int Y)
    {
        this.X = X;
        this.Y = Y;
    }

    public static bool operator ==(IntPos obj1, IntPos obj2)
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

    public static IntPos operator *(IntPos obj1, int another)
    {
        if (obj1 is null)
            return null;
        return new IntPos(obj1.X * another, obj1.Y * another);
    }

    public override string ToString()
    {
        return Convert.ToString(X) + " " + Convert.ToString(Y);
    }

    public IntPos Copy()
    {
        return new IntPos(X, Y);
    }

    public void Add(IntPos another)
    {
        X += another.X;
        Y += another.Y;
    }

    public static explicit operator IntPos(Pos v)
    {
        return new IntPos((int)Math.Floor(v.X), (int)Math.Floor(v.Y));
    }

    public override bool Equals(object obj)
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

/// <summary>
///     Class for storing 2d float position
/// </summary>
public class Pos
{
    public float X;
    public float Y;

    public static Pos Zero 
    { 
        get
        {
            return new Pos(0, 0);
        }
    }

public Pos(float X, float Y)
    {
        this.X = X;
        this.Y = Y;
    }

    public Pos(IntPos pos)
    {
        X = pos.X;
        Y = pos.Y;
    }

    public static bool operator ==(Pos obj1, Pos obj2)
    {
        return obj1.X == obj2.X && obj1.Y == obj2.Y;
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
        return Convert.ToString(X) + " " + Convert.ToString(Y);
    }

    public Pos Copy()
    {
        return new Pos(X, Y);
    }

    public void Add(Pos another)
    {
        X += another.X;
        Y += another.Y;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj)) return true;

        if (ReferenceEquals(obj, null)) return false;

        if (obj.GetType() != typeof(Pos)) return false;

        return ((Pos)obj).X == X && ((Pos)obj).Y == Y;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}