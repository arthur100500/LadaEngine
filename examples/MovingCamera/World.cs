using LadaEngine.Engine.Base;
using LadaEngine.Engine.Common;
using LadaEngine.Engine.Common.SpriteGroup;
using LadaEngine.Engine.Renderables;
using LadaEngine.Engine.Renderables.GroupRendering;
using SpriteGroup = LadaEngine.Engine.SpriteGroup.SpriteGroup;

namespace MovingCamera;

public class World : IRenderable
{
    private SpriteGroup _group;
    private List<string> textures;
    private ITextureAtlas _atlas;
    private int tileCount = 100000;

    public World()
    {
        textures = new List<string>
        {
            "Textures/image1.png",
            "Textures/image2.png",
            "Textures/image3.png",
        };
        _atlas = new TextureAtlas(textures);
        _group = new SpriteGroup(_atlas);

        GenTiles();
        
        _group.Update();
    }

    private void GenTiles()
    {
        Random random = new();
        for (var i = 0; i < tileCount; i++)
        {
            var t = new Sprite( 
                new Pos(random.NextSingle() * 30f - 15f, random.NextSingle() * 30f - 15f), 
                _atlas, 
                textures[random.Next() % textures.Count]
            );
            
            t.Width = t.Height = random.NextSingle() * 0.2f;
            t.Rotation = random.NextSingle() * MathF.PI;
            
            _group.AddSprite(t);
        }
    }

    public void Render(Camera camera)
    {
        _group.Render(camera);
    }
}