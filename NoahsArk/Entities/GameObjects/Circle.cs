using Microsoft.Xna.Framework;

namespace NoahsArk.Entities.GameObjects
{
    public struct Circle
    {
        #region Fields
        private Vector2 _center;
        private float _radius;
        #endregion

        #region Properties
        public Vector2 Center { get { return _center; } }
        public float Radius { get { return _radius; } }
        #endregion

        #region Constructor
        public Circle(Vector2 center, float radius)
        {
            _center = center;
            _radius = radius;
        }
        #endregion

        #region Methods
        public void SetCenter(Vector2 center)
        {
            _center = center;
        }
        #endregion
    }
}
