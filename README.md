![image](https://media.discordapp.net/attachments/952637214574141540/994476470036611122/IMG20220501143811_01.jpg?width=507&height=676)

# LadaEngine
Lada Engine is a small library that allows basic game creation.
It works with OpenGL via OpenTK 4 library.

# Usefulness
The project is hardly useful, it is easier and better to learn other libs. But I did this library with love, so there will always be one user (me) to enjoy it.

# Examples
There are `Pong` and `MovingImage` examples, I am too bored to do more

### [Moving Image Example](https://github.com/arthur100500/LadaEngine/tree/main/examples/MovingImage)
There is an image, it is loaded and rendered, also it is moved and bounces off the edges

### [Pong Example](https://github.com/arthur100500/LadaEngine/tree/main/examples/Pong)
You know what pong is. To move left platform use `W` `S`, to move right one use `I` `K`, to respawn a ball use `R` key.

### [World Example](https://github.com/arthur100500/LadaEngine/tree/main/examples/MovingCamera)
You can control a camera with `W` `A` `S` `D` keys to move around. There is a world with 100000 objects.
This example is done to demonstrate that you have to be careful about `Update` method on sprite group as it can be very expensive operation.

# How to use LadaEngine
To use LadaEngine create a class that has event handlers for `Render`, `FixedUpdate` and `Load`.  Also you can add `Resize` and `Update` handlers.

The basic game class looks something like this
```csharp
public class Game
{
    private Window _window;
    
    public Game()
    {
        _window = Window.Create(800, 800, "");
        _window.Title = "cool title";
        _window.Render += RenderEvent;
        _window.Load += LoadEvent;
        _window.FixedUpdate += FixedUpdateEvent;

        _window.VSync = OpenTK.Windowing.Common.VSyncMode.On;
    }

    private void LoadEvent() { }
    private void RenderEvent() { }
    private void FixedUpdateEvent() { }
    public void Run() => _window.Run();
}
```

In your `Main` method, create an instance of this class and run it.
Now once the app loads, `LoadEvent` method will be called, once there is a new frame `RenderEvent` will be called and `FixedUpdateEvent` will be called 240 times a second.

# Rendering basic images
To render an image, you will have to
- Create the `TextureAtlas` with the image within
- Create `SpriteGroup` to contain the image
- Create `Sprite` object, that will be the actual object
- Put object in the `SpriteGroup`
- Call `Update` on `SpriteGroup` every time you change the object, to reassemble the vertices
- Call `Render` on `SpriteGroup`

See the example [Moving Image Example](https://github.com/arthur100500/LadaEngine/tree/main/examples/MovingImage)

# SpriteGroup
### What is it
SpriteGroup is a class made to contain a large quantity of sprites that will rarely change, for example the world.
There is an example for using it for large quantity of objects, see [World Example](https://github.com/arthur100500/LadaEngine/tree/main/examples/MovingCamera)
There are several methods, but main ones are `AddSprite`, `Render` and `Update`.
- `Render` is used to render all the sprites on screen. It requires a camera and uses it's zoom and position to move and zoom. (It is done via shader uniforms, so it is only done once for all objects and is quite effective)
- `Update` is used to create vertices. It should usually be called very rarely, for example if something moved or changed size. 
- `AddSprite` is used to add sprite to its inner list by reference.

### Intended usage
You use `SpriteGroup` for a group of sprite that either never moves, like world, or things that move quite often, like entities. The more sprites are there in the `SpriteGroup`, the more costly is the Update operation.

# Controls
### Overview
The `Controls` class contains the basic input things for PC like `mouse` and `keyboard`. These are standard [OpenTK](https://opentk.net/) objects, so you can find how to use them there.
Brief list of methods I used (with examples):
- `Controls.keyboard.IsKeyPressed(..)` - to see if key was pressed during this exact frame
- `Controls.keyboard.IsKeyDown(..)` - check if key is down
- `Controls.mouse.IsButtonDown(..)` - check if mouse button is down
- `Controls.mouse.IsButtonPressed(..)` - check if mouse button was pressed during this exact frame

Also, `control_direction_f`, `control_direction` exists, these fields are of type Pos (Vector2 in LadaEngine) and contain direction from `W` `A` `S` `D` keys.

### Usage
If you want to get input only for frame key was on (Like restart game on R in [Pong Example](https://github.com/arthur100500/LadaEngine/tree/main/examples/Pong))
```csharp
if (Controls.keyboard.IsKeyPressed(Keys.R))
    _ball.SetPosition(Pos.Zero);
```
If you want to get input only for frame button was on (Like spawn something one Left mouse button)
```csharp
if (Controls.mouse.IsButtonPressed(MouseButton.Left))
    SpawnStuff();
```
If you want to get input for all frames button is down (Like moving the player (It is better to use `control_direction_f` for this))
```csharp
if (Controls.mouse.IsKeyDown(Keys.S))
    _player.Position.Y -= speed;
```
Using `control_direction_f` for the player
```csharp
_player.Position += Controls.control_direction_f * speed;
```

# Good luck
IDK I think that's enough info for you to write the game of your dream
