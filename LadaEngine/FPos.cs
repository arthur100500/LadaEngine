using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadaEngine
{

    /// <summary>
    /// Class for storing 2d integer position
    /// </summary>
    public class Pos
    {
        public int X;
        public int Y;

        public Pos(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public static bool operator ==(Pos obj1, Pos obj2)
        {
            if (obj1 is null || obj2 is null)
                return false;
            return (obj1.X == obj2.X) && (obj1.Y == obj2.Y);
        }

        public static bool operator !=(Pos obj1, Pos obj2)
        {
            return !(obj1 == obj2);
        }

        public static Pos operator +(Pos obj1, Pos obj2)
        {
            return new Pos(obj1.X + obj2.X, obj1.Y + obj2.Y);
        }

        public static Pos operator *(Pos obj1, int another)
        {
            return new Pos(obj1.X * another, obj1.Y * another);
        }

        public string ToString()
        {
            return Convert.ToString(this.X) + " " + Convert.ToString(this.Y);
        }

        public Pos Copy()
        {
            return new Pos(this.X, this.Y);
        }

        public void add(Pos another)
        {
            this.X += another.X;
            this.Y += another.Y;
        }

        public static explicit operator Pos(FPos v)
        {
            return new Pos((int)Math.Floor(v.X), (int)Math.Floor(v.Y));
        }
    }

    /// <summary>
    /// Class for storing 2d float position
    /// </summary>
    public class FPos
    {
        public float X;
        public float Y;

        public FPos(float X, float Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public static bool operator ==(FPos obj1, FPos obj2)
        {
            return (obj1.X == obj2.X) && (obj1.Y == obj2.Y);
        }

        public static bool operator !=(FPos obj1, FPos obj2)
        {
            return !(obj1 == obj2);
        }

        public static FPos operator +(FPos obj1, FPos obj2)
        {
            return new FPos(obj1.X + obj2.X, obj1.Y + obj2.Y);
        }

        public static FPos operator -(FPos obj1, FPos obj2)
        {
            return new FPos(obj1.X - obj2.X, obj1.Y - obj2.Y);
        }

        public static FPos operator *(FPos obj1, float another)
        {
            return new FPos(obj1.X * another, obj1.Y * another);
        }

        public static FPos operator *(float another, FPos obj1)
        {
            return new FPos(obj1.X * another, obj1.Y * another);
        }

        public string ToString()
        {
            return Convert.ToString(this.X) + " " + Convert.ToString(this.Y);
        }

        public FPos Copy()
        {
            return new FPos(this.X, this.Y);
        }

        public void add(FPos another)
        {
            this.X += another.X;
            this.Y += another.Y;
        }
    }
}
