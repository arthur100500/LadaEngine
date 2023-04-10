using LadaEngine.Engine.Base;

namespace LadaEngine.Engine.Global;

/// <summary>
/// Standart vertex and fragment shader holder.
/// !!! Vertex shader does not support camera
/// </summary>
public class StandartShaders
{
    public static readonly string StandartVert = @"#version 330 core
                                        layout(location = 0) in vec3 aPosition;
                                        layout(location = 1) in vec2 aTexCoord;
                                        out vec2 texCoord;

                                        void main(void)
                                        {
                                            texCoord = aTexCoord;

                                            gl_Position = vec4(aPosition, 1.0);
                                        }";

    public static readonly string StandartFrag = @"#version 330
                                        out vec4 outputColor;
                                        in vec2 texCoord;
                                        uniform sampler2D texture0;
                                        void main()
                                        {
	                                        outputColor = texture(texture0, texCoord);
                                        }";
    
}