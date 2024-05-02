using System;
using SharpDX;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Desktoptale.Distractions
{
    public class ScreenEdgeBonesPattern : IDistractionPattern
    {
        private int count;
        private float velocity;
        private float spacing;
        private Random random;
        private bool upper;

        public ScreenEdgeBonesPattern(int count, float velocity, float spacing, bool upper)
        {
            this.count = count;
            this.velocity = velocity;
            this.spacing = spacing;
            this.upper = upper;

            random = new Random();
        }

        public float Spawn(IDistractionManager manager, Rectangle screenRectangle, Vector2 scale)
        {
            float yPosition = upper ? 0 : 1;
            bool left = random.Next(0, 2) == 0;
            float velocityMultiplier = (left ? 1 : -1) * random.NextFloat(0.75f, 1.5f);
            Vector2 origin = new Vector2(
                left ? 0 : screenRectangle.Width, 
                screenRectangle.Height * yPosition + random.NextFloat(0f, 0.5f) * (yPosition > 0.5f ?  200f * scale.Y : -200f * scale.Y) + 50f * scale.Y * (yPosition > 0.5f ? -1 : 1));
            
            for (int i = 0; i < count; i++)
            {
                MovingBoneDistraction bone = new MovingBoneDistraction(BoneDistraction.BoneLength.Long, new Vector2(velocity * velocityMultiplier, 0));
                bone.Position = origin + new Vector2(spacing * scale.X * (i + 1), 0) * (left ? -1 : 1);
                manager.AddDistraction(bone);
            }
            
            return 1f;
        }
    }
}