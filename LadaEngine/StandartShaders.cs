using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadaEngine
{
	public class StandartShaders
	{

		internal static string light_gen = @"#version 430
											layout(local_size_x = 1, local_size_y = 1) in;
											layout(rgba32f, location = 0, binding = 0) uniform image2D light_map;
											uniform sampler2D normal_map;
											uniform vec4 light_colors;
											uniform vec4 light_position;

											uniform int resolution_x;
											uniform int resolution_y;
											
											uniform vec2 texture_size;
											uniform float texture_rotation;

											vec4 count_light(vec4 color_in, vec3 normal_in, vec2 texCoord){
												vec4 result_light = vec4(vec3(0.0), 1.0);
												float dx;
												float dy;
												float dist;

												vec3 LightDir = vec3(light_position.xy - texCoord.xy, light_position.z);
												// For rot mult by sin and cos
												LightDir.x = LightDir.x / texture_size.x;
												LightDir.y = LightDir.y / texture_size.y;
												float D = length(LightDir);
												vec3 N = normalize(normal_in.rgb / (1.0 + light_position.z));
												vec3 L = normalize(LightDir);
												vec3 Diffuse = (light_colors.rgb * light_colors.a) * max(dot(N, L), 0.0);
												vec3 Falloff = vec3(0.4, 3, 20);
												float Attenuation = 1.0 / ( Falloff.x + (Falloff.y*D) + (Falloff.z*D*D) );
												vec3 Intensity = Diffuse * Attenuation;
												vec3 FinalColor = color_in.rgb * Intensity;
												result_light += vec4(FinalColor, 0.0);
												
												result_light.a = color_in.a;
												return result_light;
											}


											void main(){
											  ivec2 pixel_coords = ivec2(gl_GlobalInvocationID.xy);
											  vec2 texCoord = vec2(pixel_coords.x / float(resolution_x), pixel_coords.y / float(resolution_y));
											  vec3 nm_color = texture(normal_map, texCoord).rgb * 2.0 - 1.0;

											  imageStore(light_map, pixel_coords, count_light(vec4(1.0), nm_color, texCoord) + imageLoad(light_map, pixel_coords));
											}";
		internal static string tm_light_gen = @"#version 430
											layout(local_size_x = 1, local_size_y = 1) in;
											layout(rgba32f, location = 0, binding = 0) uniform image2D light_map;
											uniform sampler2D normal_map;
											uniform vec4 light_colors;
											uniform vec4 light_position;

											uniform int resolution_x;
											uniform int resolution_y;

											uniform int[10000] map_array;
											uniform int height;
											uniform int width;
											uniform int texture_length;
											uniform int texture_width;

											uniform vec2 texture_size;
											uniform float texture_rotation;

											vec4 count_light(vec4 color_in, vec3 normal_in, vec2 texCoord){
												vec4 result_light = vec4(vec3(0.0), 1.0);
												float dx;
												float dy;
												float dist;

												vec3 LightDir = vec3(light_position.xy - texCoord.xy, light_position.z);
												// For rot mult by sin and cos
												LightDir.x = LightDir.x / texture_size.x;
												LightDir.y = LightDir.y / texture_size.y;
												float D = length(LightDir);
												vec3 N = normalize(normal_in.rgb / (1.0 + light_position.z));
												vec3 L = normalize(LightDir);
												vec3 Diffuse = (light_colors.rgb * light_colors.a) * max(dot(N, L), 0.0);
												vec3 Falloff = vec3(0.4, 3, 20);
												float Attenuation = 1.0 / ( Falloff.x + (Falloff.y*D) + (Falloff.z*D*D) );
												vec3 Intensity = Diffuse * Attenuation;
												vec3 FinalColor = color_in.rgb * Intensity;
												result_light += vec4(FinalColor, 0.0);
												
												result_light.a = color_in.a;
												return result_light;
											}


											void main(){
											  ivec2 pixel_coords = ivec2(gl_GlobalInvocationID.xy);
											  vec2 texCoord = vec2(pixel_coords.x / float(resolution_x), pixel_coords.y / float(resolution_y));
											  
											  int type = map_array[width * int((1.0 - texCoord.y) * height) + int(texCoord.x * width)];
											  vec2 newTexCoord = vec2(texCoord.x * width, texCoord.y * height);
												newTexCoord.x = fract((type % texture_length + fract(newTexCoord.x)) / texture_length);
												newTexCoord.y = fract((texture_length - type / texture_length + fract(newTexCoord.y) - 1) / texture_width);

											  vec3 nm_color = texture(normal_map, newTexCoord).rgb * 2.0 - 1.0;

											  imageStore(light_map, pixel_coords, count_light(vec4(1.0), nm_color, texCoord) + imageLoad(light_map, pixel_coords));
											}";
		private static string standart_vert = @"#version 330 core
                                        layout(location = 0) in vec3 aPosition;
                                        layout(location = 1) in vec2 aTexCoord;
                                        out vec2 texCoord;

                                        void main(void)
                                        {
                                            texCoord = aTexCoord;

                                            gl_Position = vec4(aPosition, 1.0);
                                        }";
		private static string standart_frag = @"#version 330
                                        out vec4 outputColor;
                                        in vec2 texCoord;
                                        uniform sampler2D texture0;
                                        void main()
                                        {
	                                        outputColor = texture(texture0, texCoord);
                                        }";
		private static string standart_nm = @"#version 330

											out vec4 outputColor;

											in vec2 texCoord;

											uniform sampler2D texture0;
											uniform sampler2D texture1;

											uniform vec4[200] light_sources;
											uniform vec4[200] light_sources_colors; 

											uniform vec2 texture_size;
											uniform float texture_rotation;

											uniform vec4 ambient;

											vec4 count_light(vec4 color_in, vec3 normal_in){
												vec4 result_light = color_in * ambient * ambient.a;
												float dx;
												float dy;
												float dist;
												for (int i = 0; i < 200; i++){
													// w - density
													if (light_sources[i].w < 0.001) break;
		
													vec3 LightDir = vec3(light_sources[i].xy - texCoord.xy, light_sources[i].z);
													LightDir.x = LightDir.x / texture_size.x;
													LightDir.y = LightDir.y / texture_size.y;
													float D = length(LightDir);
													vec3 N = normalize(normal_in.rgb / (1.0 + light_sources[i].z));
													vec3 L = normalize(LightDir);
		
													vec3 Diffuse = (light_sources_colors[i].rgb * light_sources_colors[i].a) * max(dot(N, L), 0.0);
		
													vec3 Falloff = vec3(0.4, 3, 20);
		
													float Attenuation = 1.0 / ( Falloff.x + (Falloff.y*D) + (Falloff.z*D*D) );
	
													vec3 Intensity = Diffuse * Attenuation;
		
													vec3 FinalColor = color_in.rgb * Intensity;
		
													result_light += vec4(FinalColor, 0.0);
												}
												result_light.a = color_in.a;
												return result_light;
											}


											void main()
											{

												vec3 normal = texture(texture1, texCoord).rgb  * 2.0 - 1.0;

												vec4 diffuse = texture(texture0, texCoord);
	
												outputColor = count_light(diffuse, normal);
											}";
		private static string standart_nm_sl = @"#version 330

											out vec4 outputColor;

											in vec2 texCoord;

											uniform sampler2D texture0;
											uniform sampler2D texture1;
											uniform sampler2D static_light;

											uniform vec4[200] light_sources;
											uniform vec4[200] light_sources_colors; 

											uniform vec2 texture_size;
											uniform float texture_rotation;

											uniform vec4 ambient;

											vec4 count_light(vec4 color_in, vec3 normal_in){
												vec4 result_light = color_in * ambient * ambient.a;
												float dx;
												float dy;
												float dist;
												for (int i = 0; i < 200; i++){

													// w - density
													if (light_sources[i].w < 0.001) break;	
													vec3 LightDir = vec3(light_sources[i].xy - texCoord.xy, light_sources[i].z);
													LightDir.x = LightDir.x / texture_size.x;
													LightDir.y = LightDir.y / texture_size.y;
													float D = length(LightDir);
													vec3 N = normalize(normal_in.rgb / (1.0 + light_sources[i].z));
													vec3 L = normalize(LightDir);
													vec3 Diffuse = (light_sources_colors[i].rgb * light_sources_colors[i].a) * max(dot(N, L), 0.0);	
													vec3 Falloff = vec3(0.4, 3, 20);	
													float Attenuation = 1.0 / ( Falloff.x + (Falloff.y*D) + (Falloff.z*D*D) );
													vec3 Intensity = Diffuse * Attenuation;
													vec3 FinalColor = color_in.rgb * Intensity;
													result_light += vec4(FinalColor, 0.0);
												}
												result_light.a = color_in.a;



												result_light.rgb += color_in.rgb * texture(static_light, texCoord).rgb;
												return result_light;
											}


											void main()
											{

												vec3 normal = texture(texture1, texCoord).rgb  * 2.0 - 1.0;

												vec4 diffuse = texture(texture0, texCoord);
	
												outputColor = count_light(diffuse, normal);
											}";
		// ADD WIDTH HERE
		private static string tm_normal_frag = @"#version 330

out vec4 outputColor;

in vec2 texCoord;

uniform sampler2D texture0;
uniform sampler2D texture1;

uniform int[10000] map_array;
uniform vec4[200] light_sources;
uniform vec4[200] light_sources_colors; 

uniform int height;
uniform int width;
uniform int texture_length;

uniform vec4 ambient;

vec4 count_light(vec4 color_in, vec3 normal_in){
	vec4 result_light = color_in * ambient * ambient.a;
	float dx;
	float dy;
	float dist;
	for (int i = 0; i < 200; i++){
		// w - density
		if (light_sources[i].w < 0.001) break;
		
		vec3 LightDir = vec3(light_sources[i].xy - texCoord.xy, light_sources[i].z);
		
		float D = length(LightDir);
		vec3 N = normalize(normal_in.rgb / (1.0 + light_sources[i].z));
		vec3 L = normalize(LightDir);
		
		vec3 Diffuse = (light_sources_colors[i].rgb * light_sources_colors[i].a) * max(dot(N, L), 0.0);
		
		vec3 Falloff = vec3(0.4, 3, 20);
		
		float Attenuation = 1.0 / ( Falloff.x + (Falloff.y*D) + (Falloff.z*D*D) );
	
		vec3 Intensity = Diffuse * Attenuation;
		
		vec3 FinalColor = color_in.rgb * Intensity;
		
		result_light += vec4(FinalColor, 0.0);
	}
	result_light.a = color_in.a;
	return result_light;
}


void main()
{
	int type = map_array[width * int((1.0 -texCoord.y)  * height) + int(texCoord.x * width)];
	vec2 newTexCoord = vec2(texCoord.x * width, texCoord.y * height);
	newTexCoord.x = fract((type + fract(newTexCoord.x)) / texture_length);
	newTexCoord.y = fract(newTexCoord.y);
	vec3 normal = texture(texture1, newTexCoord).rgb  * 2.0 - 1.0;
	//normal.r = - normal.r;
	vec4 diffuse = texture(texture0, newTexCoord);
	
	outputColor = count_light(diffuse, normal);
}";
		private static string tm_default = @"#version 330

									out vec4 outputColor;

									in vec2 texCoord;

									uniform sampler2D texture0;

									uniform int[10000] map_array;
									uniform int height;
									uniform int width;
									uniform int texture_length;
									uniform int texture_width;

									void main()
									{

										int type = map_array[width * int((1.0 - texCoord.y) * height) + int(texCoord.x * width)];
										vec2 newTexCoord = vec2(texCoord.x * width, texCoord.y * height);
												newTexCoord.x = fract((type % texture_length + fract(newTexCoord.x)) / texture_length);
												newTexCoord.y = fract((texture_length - type / texture_length + fract(newTexCoord.y) - 1) / texture_width);
	
										outputColor = texture(texture0, newTexCoord);
									}";
		private static string tm_nm_sl = @"#version 330
											out vec4 outputColor;

											in vec2 texCoord;

											uniform sampler2D texture0;
											uniform sampler2D texture1;
											uniform sampler2D static_light;

											uniform vec4[200] light_sources;
											uniform vec4[200] light_sources_colors; 

											uniform int[10000] map_array;

											uniform int height;
											uniform int width;
											uniform int texture_length;
											uniform int texture_width;

											uniform vec2 texture_size;
											uniform float texture_rotation;

											uniform vec4 ambient;

											vec4 count_light(vec4 color_in, vec3 normal_in){
												vec4 result_light = color_in * ambient * ambient.a;
												float dx;
												float dy;
												float dist;
												for (int i = 0; i < 200; i++){
													// w - density
													if (light_sources[i].w < 0.001) break;	
													vec3 LightDir = vec3(light_sources[i].xy - texCoord.xy, light_sources[i].z);
													LightDir.x = LightDir.x / texture_size.x;
													LightDir.y = LightDir.y / texture_size.y;
													float D = length(LightDir);
													vec3 N = normalize(normal_in.rgb / (1.0 + light_sources[i].z));
													vec3 L = normalize(LightDir);
													vec3 Diffuse = (light_sources_colors[i].rgb * light_sources_colors[i].a) * max(dot(N, L), 0.0);	
													vec3 Falloff = vec3(0.4, 3, 20);	
													float Attenuation = 1.0 / ( Falloff.x + (Falloff.y*D) + (Falloff.z*D*D) );
													vec3 Intensity = Diffuse * Attenuation;
													vec3 FinalColor = color_in.rgb * Intensity;
													result_light += vec4(FinalColor, 0.0);
												}
												result_light.a = color_in.a;

												result_light.rgb += color_in.rgb * texture(static_light, texCoord).rgb;
												return result_light;
											}


											void main()
											{
												int type = map_array[width * int((1.0 - texCoord.y) * height) + int(texCoord.x * width)];
												vec2 newTexCoord = vec2(texCoord.x * width, texCoord.y * height);
												newTexCoord.x = fract((type % texture_length + fract(newTexCoord.x)) / texture_length);
												newTexCoord.y = fract((texture_length - type / texture_length + fract(newTexCoord.y) - 1) / texture_width);

												vec3 normal = texture(texture1, newTexCoord).rgb  * 2.0 - 1.0;

												vec4 diffuse = texture(texture0, newTexCoord);
	
												outputColor = count_light(diffuse, normal);
											}";

		public static Shader STANDART_SHADER = new Shader(standart_vert, standart_frag, 0);
		public static Shader STANDART_SHADER_NM = new Shader(standart_vert, standart_nm, 0);
		public static Shader TILEMAP_SHADER = new Shader(standart_vert, tm_default, 0);
		public static Shader TILEMAP_SHADER_NM = new Shader(standart_vert, tm_normal_frag, 0);

		public static Shader GenStandartShader() { return new Shader(standart_vert, standart_frag, 0); }
		public static Shader GenStandartShaderNM() { return new Shader(standart_vert, standart_nm, 0); }
		public static Shader GenStandartShaderNMSL() { return new Shader(standart_vert, standart_nm_sl, 0); }
		public static Shader GenTilemapShader() { return new Shader(standart_vert, tm_default, 0); }
		public static Shader GenTilemapShaderNM() { return new Shader(standart_vert, tm_normal_frag, 0); }
		public static Shader GenTilemapShaderNMSL() { return new Shader(standart_vert, tm_nm_sl, 0); }

	}
}
