using System;
using OpenTK.Graphics.OpenGL4;

namespace LadaEngine
{
    internal static class LightShaders
    {
		internal static string textureGen = @"
			#version 430
			layout(local_size_x = 1, local_size_y = 1) in;
			layout(rgba32f, location = 0) uniform image2D light_map;

			uniform int resolution_x;
			uniform int resolution_y;
			uniform vec4 color;

			void main(){
				ivec2 pixel_coords = ivec2(gl_GlobalInvocationID.xy);
				vec2 uv = pixel_coords / vec2(resolution_x, resolution_y);

				float d = 0.5 - sqrt((0.5 - uv.x) * (0.5 - uv.x) + (0.5 - uv.y) * (0.5 - uv.y));
				d = d * 2;
				imageStore(light_map, pixel_coords, vec4(color.rgb * d * d, d));
			}";
		internal static string normalTextureGen = @"
			#version 430
			layout(local_size_x = 1, local_size_y = 1) in;
			layout(rgba32f, location = 0) uniform image2D light_map;

			uniform int resolution_x;
			uniform int resolution_y;
			uniform vec4 color;

			void main(){
				ivec2 pixel_coords = ivec2(gl_GlobalInvocationID.xy);

				vec2 uv = pixel_coords / vec2(resolution_x, resolution_y);
				float b = 0.5;
				float r = uv.x;
				float g =  uv.y;

				imageStore(light_map, pixel_coords, vec4(r, g, b, 1.0));
			}";
		internal static string lightRendererCode = @"
		#version 330
		out vec4 outputColor;
		in vec2 texCoord;

		uniform sampler2D diffuse_frame;
		uniform sampler2D normal_frame;
		uniform sampler2D texture0;

		uniform vec4 position;
		uniform vec4 color;

		uniform vec2 texture_size;
		
		vec4 count_light(vec4 color_in, vec3 normal_in, vec4 light, vec2 transformedTexCoord)
		{

			//vec4 result_light = color_in;

			//vec3 LightDir = vec3(position.xy - transformedTexCoord.xy, position.z);

			//float D = length(LightDir);
			//vec3 N = normalize(normal_in.rgb / (1.0 + position.z));
			//vec3 L = normalize(LightDir);

			//vec3 Diffuse = (color.rgb * color.a) * max(dot(N, L), 0.0);

			//vec3 Falloff = vec3(0.4, 3, 20);

			//float Attenuation = 1.0 / (Falloff.x + (Falloff.y * D) + (Falloff.z * D * D));

			//vec3 Intensity = Diffuse * Attenuation;

			//vec3 FinalColor = color_in.rgb * Intensity;

			//result_light = vec4(FinalColor, 0.0);
			
			//result_light.a = color_in.a;

			return color_in * light;
		}
		void main()
		{
			vec2 transformedTexCoord = vec2(texCoord.x * texture_size.x, texCoord.y * texture_size.y);
			
			vec3 normal = texture(normal_frame, transformedTexCoord).rgb * 2.0 - 1.0;
			vec4 diffuse = texture(diffuse_frame, transformedTexCoord);

			transformedTexCoord.x += position.x * texture_size.x;
			transformedTexCoord.y += position.y * texture_size.y;

			vec4 light = texture(texture0, transformedTexCoord);
			outputColor = count_light(diffuse, normal, light, transformedTexCoord);
		}";

		internal static Shader lightRenderer = new Shader(StandartShaders.standart_vert, lightRendererCode, 0);
		internal static Shader spotLightFiller = CreateShader(textureGen);
		internal static Shader spotLightNormalFiller = CreateShader(normalTextureGen);
		private static Shader CreateShader(string shader_origin)
		{
			int light_generator_id;
			int light_generator_shader = GL.CreateShader(ShaderType.ComputeShader);
			GL.ShaderSource(light_generator_shader, shader_origin);

			GL.CompileShader(light_generator_shader);
			GL.GetShader(light_generator_shader, ShaderParameter.CompileStatus, out var code);
			if (code != (int)All.True)
			{
				var infoLog = GL.GetShaderInfoLog(light_generator_shader);
				Misc.PrintShaderError(shader_origin, infoLog);
				throw new Exception($"Error occurred whilst compiling Shader({light_generator_shader}).\n\n{infoLog}");
			}

			light_generator_id = GL.CreateProgram();
			GL.AttachShader(light_generator_id, light_generator_shader);
			GL.LinkProgram(light_generator_id);
			GL.GetProgram(light_generator_id, GetProgramParameterName.LinkStatus, out var c2ode);
			if (c2ode != (int)All.True) throw new Exception($"Error occurred whilst linking Program({light_generator_id})");

			GL.DetachShader(light_generator_id, light_generator_shader);
			GL.DeleteShader(light_generator_shader);

			Shader to_return = new Shader(light_generator_id);

			return to_return;
		}
	}
}
