![image](https://user-images.githubusercontent.com/57834711/152866833-465b3c15-59c4-4ba1-877d-c7ecc8e2bfc4.png)

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
