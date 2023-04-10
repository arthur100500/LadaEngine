using LadaEngine.Engine.Base;
using LadaEngine.Engine.Common;
using LadaEngine.Engine.Common.SpriteGroup;
using LadaEngine.Engine.Renderables;
using OpenTK.Mathematics;

namespace LadaEngine.Engine.SpriteGroup;

public class SpriteGroup : IRenderable
{
    private readonly LadaEngine.Engine.Common.SpriteGroup.SpriteGroup _lvl;

    public SpriteGroup(ITextureAtlas textureAtlas)
    {
        _lvl = new LadaEngine.Engine.Common.SpriteGroup.SpriteGroup(textureAtlas);
    }

    public void Render(Camera camera)
    {
        _lvl.Renderer.Shader.Use();
        _lvl.Renderer.Shader.SetVector2("position",
            new Vector2(camera.Position.X / camera.Zoom, camera.Position.Y / camera.Zoom));
        _lvl.Render(camera);
    }

    public void AddSprite(Sprite t)
    {
        _lvl.Sprites.Add(t);
    }

    public void Update()
    {
        _lvl.Renderer.UpdateVerts();
        _lvl.Renderer.UpdateBuffers();
    }

    public List<Sprite> GetList()
    {
        return _lvl.Sprites;
    }
}