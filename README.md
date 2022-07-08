![image](https://media.discordapp.net/attachments/952637214574141540/994476470036611122/IMG20220501143811_01.jpg?width=507&height=676)

# LadaEngine
Library for easier low level game development
Contains a lot of stuff to do rendering

Usage:
Import dll to your project and use it via "using LadaEngine"

Create instance of a window class and add event handlers to Update, Load and Render
```
    window.Render += RenderEv;
    window.Load += LoadEv;
    window.Update += UpdateEv;
```

To draw something on screen renderable item should be created, loaded and rendered.
```
    Sprite sprite;
    …
    sprite = new Sprite(Texture.LoadFromFile("path")); // In LoadEv
    sprite.Load();
    …
    sprite.Render(); // In RenderEV
```
