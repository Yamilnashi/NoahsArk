using Microsoft.Xna.Framework;

namespace NoahsArk.Extensions
{
    public static class RectangleExtensions
    {
        public static Vector2 GetCenter(this Rectangle rect)
        {
            return new Vector2(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f);
        }
        public static Vector2 GetCenteredPosition(this Rectangle rect, Vector2 objectSize)
        {
            Vector2 center = rect.GetCenter();
            return new Vector2(center.X - objectSize.X / 2f, center.Y - objectSize.Y / 2f);
        }
        public static Vector2 GetBottomCenteredPosition(this Rectangle rect, Vector2 objectSize, float margin = 20)
        {
            Vector2 center = rect.GetCenter();
            return new Vector2(center.X - objectSize.X / 2f, rect.Bottom - objectSize.Y - margin);
        }
    }
}
