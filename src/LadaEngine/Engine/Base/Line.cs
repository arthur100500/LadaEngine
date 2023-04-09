namespace LadaEngine.Engine.Base;

/// <summary>
/// Class Representing Line segment
/// </summary>
public class Line
{
    /// <summary>
    /// First point of the segment
    /// </summary>
    public Pos First;
    
    /// <summary>
    /// Second point of the segment
    /// </summary>
    public Pos Second;

    /// <summary>
    /// Creates line from point fst to snd
    /// </summary>
    /// <param name="fst">First point</param>
    /// <param name="snd">Second point</param>
    public Line(Pos fst, Pos snd)
    {
        First = fst;
        Second = snd;
    }

    /// <summary>
    /// Make Line from two points and copy them in the process
    /// </summary>
    /// <param name="fst"></param>
    /// <param name="snd"></param>
    /// <returns></returns>
    public static Line LineCp(Pos fst, Pos snd)
    {
        return new Line(fst.Copy(), snd.Copy());
    }
    
    /// <summary>
    /// Returns string representation of the object in the format:
    /// Segment: A(First), B(Second)
    /// </summary>
    /// <returns>string representation</returns>
    public override string ToString()
    {
        return "Segment: A(" + First + "), B(" + Second + ")";
    }
    
    /// <summary>
    /// Gets Point of intersection between two segments or null if there is none
    /// </summary>
    /// <param name="other">Line to check intersection with</param>
    /// <returns>Pos intersection or null</returns>
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