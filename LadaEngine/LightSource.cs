namespace LadaEngine
{
    /// <summary>
    /// Class for storing light source, can be alternative to arrays with positions and colors in a baked light
    /// </summary>
    public class LightSource
    {
        public float X;
        public float Y;
        public float Z;
        public float Density;

        public float[] Color;

        public LightSource(float x, float y, float z, float density, float[] color)
        {
            Color = color;

            X = x;
            Y = y;
            Z = z;
            Density = density;
        }
    }
}