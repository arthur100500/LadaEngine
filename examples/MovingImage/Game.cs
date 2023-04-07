using LadaEngine.Engine.Global;
using LadaEngine.Engine.Scene;


namespace MovingImage
{
    public class Game
    {
        private IScene _currentScene;
        private readonly Window _window;
        public Game()
        {
            _window = Window.Create(800, 800, "");

            _currentScene = new GameScene(_window, this);

            _window.Title = _currentScene.GetName();
            _window.Render += RenderEvent;
            _window.Update += UpdateEvent;
            _window.FixedUpdate += FixedUpdateEvent;
            _window.Load += LoadEvent;
            _window.Resize += ResizeEvent;

            _window.VSync = OpenTK.Windowing.Common.VSyncMode.On;
        }
        private void ResizeEvent()
        {
            _currentScene.Resize();
        }

        private void RenderEvent()
        {
            _currentScene.Render();
        }

        private void UpdateEvent()
        {
            _currentScene.Update();
        }

        private void FixedUpdateEvent()
        {
            _currentScene.FixedUpdate();
        }

        private void LoadEvent()
        {
            _currentScene.Load();
        }


        public void Run()
        {
            _window.Run();
        }

        public void LoadScene(IScene other)
        {
            _currentScene = other;
            _window.Title = _currentScene.GetName();
            _currentScene.Load();
        }
    }
}