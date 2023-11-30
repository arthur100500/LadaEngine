namespace LadaEngine.Engine.Scene;

/// <summary>
///     Scene
/// </summary>
public interface IScene
{
    /// <summary>
    ///     Name of the scene, often to be used for a window name
    /// </summary>
    /// <returns></returns>
    public string GetName();

    /// <summary>
    ///     Load Event for the scene for load logic
    /// </summary>
    public void Load();

    /// <summary>
    ///     Render Event for the scene for render logic
    /// </summary>
    public void Render();

    /// <summary>
    ///     Update Event for the scene for update logic
    /// </summary>
    public void Update();

    /// <summary>
    ///     Fixed Update Event for the scene for fixed update logic
    /// </summary>
    public void FixedUpdate();

    /// <summary>
    ///     Resize Event for the scene for resize logic
    /// </summary>
    public void Resize();
}