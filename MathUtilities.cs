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

        public static float InterpolateCubicGetSlowerTowardsEnd(float a, float b, float t)
        {
            float oneMinusT = 1 - t;
            float progress = 1 - oneMinusT * oneMinusT * oneMinusT;

            return MathHelper.Lerp(a, b, progress);
        }
        
        public static float InterpolateQuadraticGetSlowerTowardsEnd(float a, float b, float t)
        {
            float oneMinusT = 1 - t;
            float progress = 1 - oneMinusT * oneMinusT * oneMinusT * oneMinusT;

            return MathHelper.Lerp(a, b, progress);
        }

        // Adapted from https://stackoverflow.com/questions/52004232/how-to-calculate-the-distance-from-a-point-to-the-nearest-point-of-a-rectange
        public static double DistanceToNearestPointOnRectangle(Rectangle rectangle, Point point)
        {
            int dTop = Math.Abs(rectangle.Top - point.Y);
            int dBottom = Math.Abs(rectangle.Bottom - point.Y);
            int dLeft = Math.Abs(rectangle.Left - point.X);
            int dRight = Math.Abs(rectangle.Right - point.X);

            if ((rectangle.Left <= point.X && point.X <= rectangle.Right) || (rectangle.Bottom <= point.Y && point.Y <= rectangle.Top))
            {
                return Math.Min(dTop, Math.Min(dBottom, Math.Min(dLeft, dRight)));
            }
            else
            {
                int cornerY = dTop < dBottom ? rectangle.Top : rectangle.Bottom;
                int cornerX = dLeft < dRight ? rectangle.Left : rectangle.Right;

                int dCx = cornerX - point.X;
                int dCy = cornerY - point.Y;
                double dCorner = Math.Sqrt(dCx * dCx + dCy * dCy);
                return dCorner;
            }
        }

        public static float SignedAngleBetween(Vector2 v, Vector2 w)
        {
            return (float)Math.Atan2((w.Y * v.X) - (w.X * v.Y), (w.X * v.X) + (w.Y * v.Y));
        }
    }
}