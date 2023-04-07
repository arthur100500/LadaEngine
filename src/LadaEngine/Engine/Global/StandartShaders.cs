using LadaEngine.Engine.Base;

namespace LadaEngine.Engine.Global;

public class StandartShaders
{
    public static readonly string standart_vert = @"#version 330 core
                                        layout(location = 0) in vec3 aPosition;
                                        layout(location = 1) in vec2 aTexCoord;
                                        out vec2 texCoord;

                                        void main(void)
                                        {
                                            texCoord = aTexCoord;

                                            gl_Position = vec4(aPosition, 1.0);
                                        }";

    public static readonly string standart_frag = @"#version 330
                                        out vec4 outputColor;
                                        in vec2 texCoord;
                                        uniform sampler2D texture0;
                                        void main()
                                        {
	                                        outputColor = texture(texture0, texCoord);
                                        }";

    public static Shader GenStandartShader()
    {
        return new Shader(standart_vert, standart_frag, 0);
    }
}