using System.Globalization;
using LadaEngine.Engine.Base;
using LadaEngine.Engine.Common.Renderables.GroupRendering;
using LadaEngine.Engine.Renderables;
using LadaEngine.Engine.Renderables.GroupRendering;

namespace LadaEngine.Engine.Common.SpriteGroup;

/// <summary>
/// The object containing group of Sprites
/// For effective use try to group sprite that don't ever move to there groups,
/// and avoid calling Update with a lot of Sprites in.
/// Also you can create a shader to apply for entire group (idk if you really can)
/// Example of correct usage of this class to maximise performance: https://github.com/arthur100500/LadaEngine/tree/main/examples/MovingCamera
/// </summary>
public class SpriteGroup
{
    /// <summary>
    /// Renderer of Sprite group
    /// </summary>
    public readonly SpriteGroupRenderer Renderer;
    
    /// <summary>
    /// Texture atlas of the Sprite group
    /// </summary>
    public readonly ITextureAtlas TextureAtlas;
    
    /// <summary>
    /// Texture atlas of the Sprite group
    /// </summary>
    public readonly List<Sprite> Sprites;

    /// <summary>
    /// Create new SpriteGroup instance
    /// </summary>
    /// <param name="textureAtlas">Texture atlas</param>
    public SpriteGroup(ITextureAtlas textureAtlas)
    {
        TextureAtlas = textureAtlas;
        Sprites = new List<Sprite>();
        Renderer = new SpriteGroupRenderer(textureAtlas, this);
    }

    /// <summary>
    /// Recount all vertices
    /// It's a costly operation, try to avoid it for big groups!
    /// </summary>
    public void Update()
    {
        Renderer.UpdateVerts();
        Renderer.UpdateBuffers();
    }

    /// <summary>
    /// Render all sprites to camera,
    /// applying it's zoom and position
    /// </summary>
    /// <param name="cam">Camera</param>
    public void Render(Camera cam)
    {
        Renderer.Render(cam);
    }

    /// <summary>
    /// Adds sprite to Sprites list
    /// </summary>
    /// <param name="sprite">Sprite to add</param>
    public void AddSprite(Sprite sprite)
    {
        Sprites.Add(sprite);
    }
    
    /// <summary>
    /// Loads sprite group from file
    /// </summary>
    /// <param name="fileName">Path to load from</param>
    /// <param name="atlas">Texture atlas of new sprite group</param>
    /// <returns>New SpriteGroup</returns>
    public static SpriteGroup FromFile(string fileName, ITextureAtlas atlas)
    {
        var levelData = File.ReadAllText(fileName);
        var result = new SpriteGroup(atlas);
        var culture = CultureInfo.InvariantCulture;

        foreach (var line in levelData.Split("\n"))
            if (line.StartsWith("|"))
            {
                var messages = line.Replace("|", "").Split(":");
                var t = new Sprite(new Pos(float.Parse(messages[1], culture), float.Parse(messages[2], culture)),
                    result.TextureAtlas, messages[0]);
                t.Rotation = float.Parse(messages[3], culture);
                t.Group = int.Parse(messages[4], culture);
                t.Width = float.Parse(messages[5], culture);
                t.Height = float.Parse(messages[5], culture);
                result.Sprites.Add(t);
            }

        return result;
    }
    
    /// <summary>
    /// Save SpriteGroup to file
    /// </summary>
    /// <param name="fileName">Path to file to save to</param>
    public void SaveToFile(string fileName)
    {
        var culture = CultureInfo.InvariantCulture;

        var levelData = Sprites.Aggregate("# Level Format 1.0\n", 
            (current, tile) => 
                current + ("|" + tile.TextureName + ":" + 
                           tile.Position.X.ToString(culture) + ":" + 
                           tile.Position.Y.ToString(culture) + ":" + 
                           tile.Rotation.ToString(culture) + ":" + 
                           tile.Group.ToString(culture) + ":" + 
                           tile.Width.ToString(culture) + ":" + 
                           tile.Height.ToString(culture) + "\n"));

        File.WriteAllText(fileName, levelData);
    }
}