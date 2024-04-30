using Microsoft.Xna.Framework;

namespace Desktoptale.Distractions
{
    public class LeftRightOppositeBonesPattern : IDistractionPattern
    {
        private int count;
        private float velocity;
        private float spacing;
        private float yOffset;

        public LeftRightOppositeBonesPattern(int count, float velocity, float spacing, float yOffset)
        {
            this.count = count;
            this.velocity = velocity;
            this.spacing = spacing;
            this.yOffset = yOffset;
        }

        public void Spawn(IDistractionManager manager, Rectangle screenRectangle)
        {
            Vector2 screenCenter = new Vector2(screenRectangle.Width / 2, screenRectangle.Height / 2);
            Vector2 leftOrigin = new Vector2(0, screenCenter.Y + yOffset);
            Vector2 rightOrigin = new Vector2(screenRectangle.Width, screenCenter.Y - yOffset);
            
            for (int i = 0; i < count; i++)
            {
                MovingBoneDistraction rightBone = new MovingBoneDistraction(BoneDistraction.BoneLength.Long, new Vector2(-velocity, 0));
                rightBone.Position = rightOrigin + new Vector2(spacing * (i + 1), 0);
                manager.AddDistraction(rightBone);
                
                MovingBoneDistraction leftBone = new MovingBoneDistraction(BoneDistraction.BoneLength.Long, new Vector2(velocity, 0));
                leftBone.Position = leftOrigin + new Vector2(-spacing * (i + 1), 0);
                manager.AddDistraction(leftBone);
            }
        }
    }
}