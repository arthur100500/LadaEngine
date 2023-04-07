namespace LadaEngine.Engine.Base;

public class Line
{
    public Pos First;
    public Pos Second;

    public Line(Pos fst, Pos snd)
    {
        First = fst;
        Second = snd;
    }

    public static Line LineCP(Pos fst, Pos snd)
    {
        return new Line(fst.Copy(), snd.Copy());
    }

    public override string ToString()
    {
        return "Segment: A(" + First + "), B(" + Second + ")";
    }

    public Pos GetIntersection(Line other)
    {
        double deltaACy = First.Y - other.First.Y;
        double deltaDCx = other.Second.X - other.First.X;
        double deltaACx = First.X - other.First.X;
        double deltaDCy = other.Second.Y - other.First.Y;
        double deltaBAx = Second.X - First.X;
        double deltaBAy = Second.Y - First.Y;

        var denominator = deltaBAx * deltaDCy - deltaBAy * deltaDCx;
        var numerator = deltaACy * deltaDCx - deltaACx * deltaDCy;

        if (denominator == 0)
        {
            if (numerator == 0)
            {
                // collinear. Potentially infinite intersection points.
                // Check and return one of them.
                if (First.X >= other.First.X && First.X <= other.Second.X)
                    return First;
                if (other.First.X >= First.X && other.First.X <= Second.X)
                    return other.First;
                return null;
            } // parallel

            return null;
        }

        var r = numerator / denominator;
        if (r < 0 || r > 1) return null;

        var s = (deltaACy * deltaBAx - deltaACx * deltaBAy) / denominator;
        if (s < 0 || s > 1) return null;

        return new Pos((float)(First.X + r * deltaBAx), (float)(First.Y + r * deltaBAy));
    }
}