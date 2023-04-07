using LadaEngine.Engine.Base;
using LadaEngine.Engine.Common;

namespace LadaEngine.Engine.Physics.Colliders
{
    public class BoxCollider : GameObject
    {
        public Action onCollision = () => { };

        public BoxCollider(Pos position, float width, float height) : base(position, width, height)
        {
        }

        public bool IsNotCollidable { get; set; }

        public bool CheckCollision(GameObject other)
        {
            if (Position.X + Width / 2 >= other.Position.X - other.Width / 2 &&
                Position.X - Width / 2 <= other.Position.X + other.Width / 2 &&
                Position.Y + Height / 2 >= other.Position.Y - other.Height / 2 &&
                Position.Y - Height / 2 <= other.Position.Y + other.Height / 2)
                return true;
            return false;
        }

        public void OnCollision(BoxCollider other)
        {
            onCollision();
        }
    }
}
