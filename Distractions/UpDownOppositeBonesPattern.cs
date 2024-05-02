using System;
using Microsoft.Xna.Framework;
using SharpDX;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Desktoptale.Distractions
{
    public class UpDownOppositeBonesPattern : IDistractionPattern
    {
        private int count;
        private float velocity;
        private float spacing;
        private float xOffset;
        private Random random;

        public UpDownOppositeBonesPattern(int count, float velocity, float spacing, float xOffset)
        {
            this.count = count;
            this.velocity = velocity;
            this.spacing = spacing;
            this.xOffset = xOffset;

            random = new Random();
        }

        public float Spawn(IDistractionManager manager, Rectangle screenRectangle, Vector2 scale)
        {
            Vector2 screenCenter = new Vector2(screenRectangle.Width / 2, screenRectangle.Height / 2);
            Vector2 upperOrigin = new Vector2(screenCenter.X + xOffset * scale.X, 0);
            Vector2 lowerOrigin = new Vector2(screenCenter.X - xOffset * scale.X, screenRectangle.Height);
            
            float velocityMultiplier = random.NextFloat(0.75f, 1.5f);
            
            for (int i = 0; i < count; i++)
            {
                MovingBoneDistraction upBone = new MovingBoneDistraction(BoneDistraction.BoneLength.Long, new Vector2(0, -velocity * velocityMultiplier));
                upBone.Position = lowerOrigin + new Vector2(0, spacing * scale.Y * (i + 1));
                upBone.Rotation = MathHelper.ToRadians(90);
                manager.AddDistraction(upBone);
                
                MovingBoneDistraction downBone = new MovingBoneDistraction(BoneDistraction.BoneLength.Long, new Vector2(0, velocity * velocityMultiplier));
                downBone.Position = upperOrigin + new Vector2(0, -spacing * scale.Y * (i + 1));
                downBone.Rotation = MathHelper.ToRadians(90);
                manager.AddDistraction(downBone);
            }

            return 1f;
        }
    }
}