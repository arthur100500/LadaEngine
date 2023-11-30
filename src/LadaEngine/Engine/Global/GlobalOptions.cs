namespace LadaEngine.Engine.Global;

/// <summary>
///     Class for containing some global options.
///     Usually it is never used outside debug
/// </summary>
public static class GlobalOptions
{
    public const bool FullDebug = false;

    /// <summary>
    ///     If GLBased is used, entire screen is based on (-1, -1, 1, 1) grid. Light sources should be counted individually and
    ///     passed to a target accounting it's width, height and position, but not rotation
    ///     If NonGLBased is used, all objects will have some global coordinate, not bound to screen coordinate. Light will
    ///     automatically account for width, height, rotation and position of the object
    /// </summary>
    public static CoordinateMode CoordinateMode = CoordinateMode.NonGLBased;

    internal static int bfbo = 0;

    // These are used for optimisation in order to minimize the state changes
    internal static int LastShaderUsed = -1;

    internal static readonly int[] LastTextureUsed =
    {
        -1, -1, -1, -1,
        -1, -1, -1, -1,
        -1, -1, -1, -1,
        -1, -1, -1, -1
    };
}

public enum CoordinateMode
{
    GLBased = 0,
    NonGLBased = 1
}