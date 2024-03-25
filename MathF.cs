namespace Desktoptale
{
    public class MathF
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
    }
}