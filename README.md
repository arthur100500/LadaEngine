# LadaEngine
Library for easier low level game development
Contains a lot of stuff to do rendering

Usage:
Import dll to your project and use it via "using LadaEngine"

Create instance of a window class and add event handlers to Update, Load and Render
<code>
    window.Render += RenderEv;
    window.Load += LoadEv;
    window.Update += UpdateEv;
</code>

To draw something on screen renderable item should be created, loaded and rendered.
<code>
    Sprite sprite;
    …
    sprite = new Sprite(Texture.LoadFromFile("path")); // In LoadEv
    sprite.Load();
    …
    sprite.Render(); // In RenderEV
</code>
