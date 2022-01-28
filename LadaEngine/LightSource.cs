namespace LadaEngine
{
    // Represents light source
    public class LightSource
    {
        public float X;
        public float Y;
        public float Z;
        public float Density;

        public float[] Color;

        public LightSource(float x, float y, float z, float density, float[] color)
        {
            // copy is sent but in this class no operations are made with this variable
            Color = color;

            X = x;
            Y = y;
            Z = z;
            Density = density;
        }
    }
}