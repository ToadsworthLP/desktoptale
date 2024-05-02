using System;
using Microsoft.Xna.Framework;
using SharpDX;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Desktoptale.Distractions
{
    public class SideGasterBlasterPattern : IDistractionPattern
    {
        private int count;
        private float interval;
        private Random random;

        public SideGasterBlasterPattern(int count, float interval)
        {
            this.count = count;
            this.interval = interval;

            random = new Random();
        }

        public float Spawn(IDistractionManager manager, Rectangle screenRectangle, Vector2 scale)
        {
            float leftEdge, rightEdge, yMin, yMax;
            leftEdge = screenRectangle.Width * 0.1f;
            rightEdge = screenRectangle.Width * 0.9f;
            yMin = screenRectangle.Height * 0.1f;
            yMax = screenRectangle.Height * 0.9f;
            
            for (int i = 0; i < count; i++)
            {
                bool left = random.Next(0, 2) == 0;
                
                Vector2 position = new Vector2(
                    left ? leftEdge : rightEdge,
                    MathHelper.Lerp(yMin, yMax, random.NextFloat(0, 1))
                );
                
                GasterBlasterDistraction gasterBlaster = new GasterBlasterDistraction(TimeSpan.FromSeconds(interval * i));
                gasterBlaster.Position = position;
                gasterBlaster.Rotation = left ? -MathHelper.Pi / 2 : MathHelper.Pi / 2;
                gasterBlaster.Scale = new Vector2(0.5f, 1);
                
                manager.AddDistraction(gasterBlaster);
            }
            
            return 1f;
        }
    }
}