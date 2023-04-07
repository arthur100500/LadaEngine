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
        // All the textures used in the project
        textures = new List<string>
        {
            "Textures/image1.png",
            "Textures/image2.png",
            "Textures/image3.png",
        };
        
        // Create texture atlas and sprite group
        _atlas = new TextureAtlas(textures);
        _group = new SpriteGroup(_atlas);

        // Generate tiles for the world
        GenTiles();
        
        // Create vertices for the objects
        _group.Update();
    }

    private void GenTiles()
    {
        Random random = new();
        
        // Create 100000 tiles for the level
        for (var i = 0; i < tileCount; i++)
        {
            // Tile creation with random position (-15 to 15) and one of three textures
            var t = new Sprite( 
                new Pos(random.NextSingle() * 30f - 15f, random.NextSingle() * 30f - 15f), 
                _atlas, 
                textures[random.Next() % textures.Count]
            );
            
            // Setting width, height and rotation to random values
            t.Width = t.Height = random.NextSingle() * 0.2f;
            t.Rotation = random.NextSingle() * MathF.PI;
            
            // Add sprite to group
            _group.AddSprite(t);
        }
    }

    public void Render(Camera camera)
    {
        /* 
         * Render the world group
         * !!! We don't update vertices as it is a very expensive operation for 100000 objects.
         * We call the _group.Update() method only once some changes are made.
         */ 
        _group.Render(camera);
    }
}