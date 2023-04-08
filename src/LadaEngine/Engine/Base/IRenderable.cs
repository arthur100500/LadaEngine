using LadaEngine.Engine.Common;

namespace LadaEngine.Engine.Base;

/// <summary>
/// Interface that describes renerable stuff
/// </summary>
public interface IRenderable
{
    /// <summary>
    /// Renders the objects relatively to the camera
    /// </summary>
    /// <param name="camera"></param>
    void Render(Camera camera);
}