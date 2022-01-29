﻿using System;
using System.Collections.Generic;
using OpenTK.Input;

namespace LadaEngine
{
    public static class Misc
    {
        /// <summary>
        /// Global window object
        /// </summary>
        public static Window window;

        /// <summary>
        /// Screen ratio
        /// </summary>
        public static float screen_ratio = 0.6f / 0.8f;
        /// <summary>
        /// Prints float array
        /// </summary>
        /// <param name="arr"></param>
        public static void PrintArr(float[] arr)
        {
            foreach (float x in arr)
            {
                Console.Write(x);
                Console.Write(" ");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Counts the distance between two points
        /// </summary>
        public static float Len(FPos p1, FPos p2)
        {
            return (float)Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }

        /// <summary>
        /// Vertecies for fullscreen Quad
        /// </summary>
        static public float[] fullscreenverticies =
        {
             1f,  1f, 0.0f, 1.0f, 1.0f,
             1f, -1f, 0.0f, 1.0f, 0.0f,
            -1f, -1f, 0.0f, 0.0f, 0.0f,
            -1f,  1f, 0.0f, 0.0f, 1.0f
        };
        public static string str(object arg)
        {
            return Convert.ToString(arg);
        }

        public static float normalize(float x)
        {
            if (x < 0.0f) return 0f;
            if (x > 1.0f) return 1.0f;
            return x;
        }
        /// <summary>
        /// Currently prints message in the console
        /// </summary>
        /// <param name="message"></param>
        public static void Log(object message)
        {
            Console.WriteLine(message);
        }
        private static bool isdigit(char x)
        {
            if (x == '1' || x == '2' || x == '3' || x == '4' || x == '5' || x == '6' || x == '7' || x == '8' || x == '9' || x == '0')
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Prints shader error
        /// </summary>
        /// <param name="shader">Shader code</param>
        /// <param name="error">Error information</param>
        public static void PrintShaderError(string shader, string error)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Shader compilation error!\n\n");
            Console.ForegroundColor = ConsoleColor.White;
            string[] splitted = error.Split('\n');
            string buffer = "";
            List<int> lines = new List<int>();
            foreach (string s in splitted)
            {
                buffer = "";
                if (s.Length > 10)
                {
                    if (s[0] == 'E' && s[1] == 'R' && s[2] == 'R' && s[3] == 'O' && s[4] == 'R' && s[5] == ':')
                    {
                        for (int i = 9; i < 100; i++)
                        {
                            if (isdigit(s[i]))
                                buffer += s[i];
                            else
                            {
                                break;
                            }
                        }
                        if (buffer != "")
                            lines.Add(Convert.ToInt32(buffer) - 1);
                    }
                }
            }
            splitted = shader.Split('\n');
            for (int i = 0; i < splitted.Length; i++)
            {
                if (lines.Contains(i))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                Console.WriteLine(str(i + 1) + " " + splitted[i]);
            }
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(error);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}