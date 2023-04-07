using System.Globalization;
using LadaEngine.Engine.Base;
using LadaEngine.Engine.Common.Renderables.GroupRendering;
using LadaEngine.Engine.Renderables;
using LadaEngine.Engine.Renderables.GroupRendering;

namespace LadaEngine.Engine.Common.SpriteGroup;

public class SpriteGroup
{
    public SpriteGroupRenderer Renderer;
    public ITextureAtlas textureAtlas;
    public List<Sprite> tiles;

    public SpriteGroup(ITextureAtlas textureAtlas)
    {
        this.textureAtlas = textureAtlas;
        tiles = new List<Sprite>();
        Renderer = new SpriteGroupRenderer(textureAtlas, this);
    }

    public void Update(Camera cam)
    {
        Renderer.UpdateVerts(cam);
        Renderer.UpdateBuffers();
    }

    public void Render(Camera cam)
    {
        Renderer.Render(cam);
    }

    public void AddSprite(Sprite sprite)
    {
        tiles.Add(sprite);
    }

    public static SpriteGroup FromFile(string fileName, ITextureAtlas atlas)
    {
        var levelData = File.ReadAllText(fileName);
        var result = new SpriteGroup(atlas);
        var culture = CultureInfo.InvariantCulture;

        foreach (var line in levelData.Split("\n"))
            // Tile
            if (line.StartsWith("|"))
            {
                var messages = line.Replace("|", "").Split(":");
                var t = new Sprite(new Pos(float.Parse(messages[1], culture), float.Parse(messages[2], culture)),
                    result.textureAtlas, messages[0]);
                t.Rotation = float.Parse(messages[3], culture);
                t.Group = int.Parse(messages[4], culture);
                t.Width = float.Parse(messages[5], culture);
                t.Height = float.Parse(messages[5], culture);
                result.tiles.Add(t);
            }

        return result;
    }

    public void SaveToFile(string fileName)
    {
        var levelData = "# Level Format 1.0\n";
        var culture = CultureInfo.InvariantCulture;

        foreach (var tile in tiles)
            levelData += "|" +
                         tile.textureName + ":" +
                         tile.Position.X.ToString(culture) + ":" +
                         tile.Position.Y.ToString(culture) + ":" +
                         tile.Rotation.ToString(culture) + ":" +
                         tile.Group.ToString(culture) + ":" +
                         tile.Width.ToString(culture) + ":" +
                         tile.Height.ToString(culture) + "\n";

        File.WriteAllText(fileName, levelData);
    }
}