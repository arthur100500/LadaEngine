using LadaEngine.Engine.Base;

namespace LadaEngine.Engine.Scene
{
    public interface IScene
    {
        public string GetName();
        public void Load();
        public void Render();
        public void Update();
        public void FixedUpdate();
        public void Resize();
    }
}