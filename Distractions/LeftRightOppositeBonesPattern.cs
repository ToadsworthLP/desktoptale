using System;
using SharpDX;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Desktoptale.Distractions
{
    public class LeftRightOppositeBonesPattern : IDistractionPattern
    {
        private int count;
        private float velocity;
        private float spacing;
        private float yOffset;
        private Random random;

        public LeftRightOppositeBonesPattern(int count, float velocity, float spacing, float yOffset)
        {
            this.count = count;
            this.velocity = velocity;
            this.spacing = spacing;
            this.yOffset = yOffset;

            random = new Random();
        }

        public float Spawn(IDistractionManager manager, Rectangle screenRectangle, Vector2 scale)
        {
            Vector2 screenCenter = new Vector2(screenRectangle.Width / 2, screenRectangle.Height / 2);
            Vector2 leftOrigin = new Vector2(0, screenCenter.Y + yOffset * scale.Y);
            Vector2 rightOrigin = new Vector2(screenRectangle.Width, screenCenter.Y - yOffset * scale.Y);
            
            float velocityMultiplier = random.NextFloat(0.75f, 1.5f);
            
            for (int i = 0; i < count; i++)
            {
                MovingBoneDistraction rightBone = new MovingBoneDistraction(BoneDistraction.BoneLength.Long, new Vector2(-velocity * velocityMultiplier, 0));
                rightBone.Position = rightOrigin + new Vector2(spacing * scale.X * (i + 1), 0);
                manager.AddDistraction(rightBone);
                
                MovingBoneDistraction leftBone = new MovingBoneDistraction(BoneDistraction.BoneLength.Long, new Vector2(velocity * velocityMultiplier, 0));
                leftBone.Position = leftOrigin + new Vector2(-spacing * scale.X * (i + 1), 0);
                manager.AddDistraction(leftBone);
            }

            return 1f;
        }
    }
}