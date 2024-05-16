using System;
using Microsoft.Xna.Framework;

namespace Desktoptale
{
    public class MathUtilities
    {
        public static float Min(float a, float b)
        {
            return a < b ? a : b;
        }
        
        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
                value = min;
            else if (value > max)
                value = max;
            return value;
        }
        
        public static float InterpolateQuadraticGetSlowerTowardsEnd(float a, float b, float t)
        {
            float oneMinusT = 1 - t;
            float progress = 1 - oneMinusT * oneMinusT * oneMinusT * oneMinusT;

            return MathHelper.Lerp(a, b, progress);
        }

        public static float SignedAngleBetween(Vector2 v, Vector2 w)
        {
            return (float)Math.Atan2((w.Y * v.X) - (w.X * v.Y), (w.X * v.X) + (w.Y * v.Y));
        }

        public static Orientation GetClosestOrientation(Vector2 direction)
        {
            float xDot = Vector2.Dot(direction, Vector2.UnitX);
            float yDot = Vector2.Dot(direction, Vector2.UnitY);

            if (Math.Abs(xDot) > Math.Abs(yDot))
            {
                if (direction.X < 0) return Orientation.Left;
                return Orientation.Right;
            }
            else
            {
                if (direction.Y < 0) return Orientation.Up;
                return Orientation.Down;
            }
        }
    }
}