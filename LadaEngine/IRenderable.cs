namespace LadaEngine
{
    public interface IRenderable
    {
        void Render();
        void SetLightSources(float[] positions, float[] colors);
    }
}