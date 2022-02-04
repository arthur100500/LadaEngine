using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadaEngine
{
    public static class GlobalOptions
    {
        /// <summary>
        /// If GLBased is used, entire screen is based on (-1, -1, 1, 1) grid. Light sources should be counted individually and passed to a target accounting it's width, height and position, but not rotation
        /// If NonGLBased is used, all objects will have some global coordinate, not bound to screen coordinate. Light will automatically account for width, height, rotation and position of the object
        /// </summary>
        public static CoordinateMode coordinate_mode = CoordinateMode.NonGLBased;

        public static bool full_debug = false;
    }


    public enum CoordinateMode : int
    {
        GLBased = 0,
        NonGLBased = 1
    }
}
