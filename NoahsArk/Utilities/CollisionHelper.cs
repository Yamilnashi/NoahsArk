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
        public static bool CircleIntersectsCircle(Circle circle1, Circle circle2, out Vector2 displacement)
        {
            displacement = Vector2.Zero;
            // Vector from circle1 to circle2
            Vector2 vectorBetweenCenters = circle2.Center - circle1.Center;
            float distanceSquared = vectorBetweenCenters.LengthSquared();
            float sumRadii = circle1.Radius + circle2.Radius;

            if (distanceSquared < sumRadii * sumRadii)
            {
                float distance = (float)Math.Sqrt(distanceSquared);
                if (distance == 0)
                {
                    // Circles are coincident; displace along X-axis arbitrarily
                    displacement = new Vector2(sumRadii, 0);
                }
                else
                {
                    // Direction from circle1 to circle2
                    Vector2 direction = vectorBetweenCenters / distance;
                    // Amount of overlap
                    float overlap = sumRadii - distance;
                    // Displacement to move circle1 away from circle2
                    displacement = -direction * overlap;
                }
                return true;
            }
            return false;
        }
        public static bool PlayerIntersectsCircle(Circle circle1, Circle circle2, out Vector2 contactPoint)
        {
            contactPoint = Vector2.Zero;
            Vector2 direction = circle2.Center - circle1.Center;
            float distanceSquared = direction.LengthSquared();
            float sumRadii = circle1.Radius + circle2.Radius;

            if (distanceSquared >= sumRadii * sumRadii)
            {
                return false; // no intersection
            }

            float distance = (float)Math.Sqrt(distanceSquared);
            if (distance == 0f)
            {
                contactPoint = circle1.Center;
                return true;
            }

            Vector2 unitDirection = direction / distance;
            contactPoint = circle1.Center + unitDirection * circle1.Radius;
            return true;
            //Vector2 vectorBetweenCenters = circle2.Center - circle1.Center;
            //float distanceSquared = vectorBetweenCenters.LengthSquared();
            //float sumRadii = circle1.Radius + circle2.Radius;
            //return distanceSquared < sumRadii * sumRadii;
        }
    }
}
