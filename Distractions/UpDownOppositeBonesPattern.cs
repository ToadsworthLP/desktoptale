using Microsoft.Xna.Framework;

namespace Desktoptale.Distractions
{
    public class UpDownOppositeBonesPattern : IDistractionPattern
    {
        private int count;
        private float velocity;
        private float spacing;
        private float xOffset;

        public UpDownOppositeBonesPattern(int count, float velocity, float spacing, float xOffset)
        {
            this.count = count;
            this.velocity = velocity;
            this.spacing = spacing;
            this.xOffset = xOffset;
        }

        public float Spawn(IDistractionManager manager, Rectangle screenRectangle)
        {
            Vector2 screenCenter = new Vector2(screenRectangle.Width / 2, screenRectangle.Height / 2);
            Vector2 upperOrigin = new Vector2(screenCenter.X + xOffset, 0);
            Vector2 lowerOrigin = new Vector2(screenCenter.X - xOffset, screenRectangle.Height);
            
            for (int i = 0; i < count; i++)
            {
                MovingBoneDistraction upBone = new MovingBoneDistraction(BoneDistraction.BoneLength.Long, new Vector2(0, -velocity));
                upBone.Position = lowerOrigin + new Vector2(0, spacing * (i + 1));
                upBone.Rotation = MathHelper.ToRadians(90);
                manager.AddDistraction(upBone);
                
                MovingBoneDistraction downBone = new MovingBoneDistraction(BoneDistraction.BoneLength.Long, new Vector2(0, velocity));
                downBone.Position = upperOrigin + new Vector2(0, -spacing * (i + 1));
                downBone.Rotation = MathHelper.ToRadians(90);
                manager.AddDistraction(downBone);
            }

            return 1f;
        }
    }
}