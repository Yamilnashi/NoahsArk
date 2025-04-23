using System;
using Microsoft.Xna.Framework;
using NoahsArk.Entities.GameObjects;

namespace NoahsArk.Utilities
{
    public static class CollisionHelper
    {
        public static bool CircleIntersectsRectangle(Circle circle, Rectangle rectangle, out Vector2 displacement)
        {
            float closestX = MathHelper.Clamp(circle.Center.X, rectangle.X, rectangle.X + rectangle.Width);
            float closestY = MathHelper.Clamp(circle.Center.Y, rectangle.Y, rectangle.Y + rectangle.Height);
            float dx = circle.Center.X - closestX;
            float dy = circle.Center.Y - closestY;
            float distanceSquared = dx * dx + dy * dy;
            bool intersects = distanceSquared < (circle.Radius * circle.Radius);
            displacement = Vector2.Zero;
            if (intersects)
            {
                float distance = (float)Math.Sqrt(distanceSquared);
                if (distance == 0) // avoid division by zero if centers coincide
                {
                    // psuh to the right
                    displacement = new Vector2(circle.Radius, 0);
                }
                else
                {
                    float overlap = circle.Radius - distance;
                    Vector2 direction = new Vector2(dx, dy) / distance;
                    displacement = direction * overlap;
                }
            }
            return intersects;
        }
        public static bool CircleIntersectsRectangle(Circle circle, Rectangle rectangle)
        {
            float closestX = MathHelper.Clamp(circle.Center.X, rectangle.X, rectangle.X + rectangle.Width);
            float closestY = MathHelper.Clamp(circle.Center.Y, rectangle.Y, rectangle.Y + rectangle.Height);
            float dx = circle.Center.X - closestX;
            float dy = circle.Center.Y - closestY;
            float distanceSquared = dx * dx + dy * dy;
            return distanceSquared < (circle.Radius * circle.Radius);
        }
    }
}
