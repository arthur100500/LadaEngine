using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadaEngine
{
    public abstract class BaseObject : IRenderable
    {
        /// <summary>
        /// Centre of the object
        /// </summary>
        public FPos centre;
        /// <summary>
        /// Rotation of the object
        /// </summary>
        public float rotation;
        /// <summary>
        /// Width of the object (in initial position, not rotated)
        /// </summary>
        public float width;
        /// <summary>
        /// Height of the object (in initial position, not rotated)
        /// </summary>
        public float height;

        // To be implemented in any other child
        public abstract void ReshapeVertexArray(FPos camera_position);
        public abstract void Render(FPos cam);
    }
}
