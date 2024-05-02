using System;
using Microsoft.Xna.Framework;
using SharpDX;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Desktoptale.Distractions
{
    public class RandomGasterBlasterPattern : IDistractionPattern
    {
        private int count;
        private float interval;
        private Random random;

        public RandomGasterBlasterPattern(int count, float interval)
        {
            this.count = count;
            this.interval = interval;

            random = new Random();
        }

        public float Spawn(IDistractionManager manager, Rectangle screenRectangle, Vector2 scale)
        {
            float xMin, xMax, yMin, yMax;
            xMin = screenRectangle.Width * 0.1f;
            xMax = screenRectangle.Width * 0.9f;
            yMin = screenRectangle.Height * 0.1f;
            yMax = screenRectangle.Height * 0.9f;
            
            Vector2 screenCenter = new Vector2(screenRectangle.Width / 2, screenRectangle.Height / 2);
            
            for (int i = 0; i < count; i++)
            {
                Vector2 position = new Vector2(
                    MathHelper.Lerp(xMin, xMax, random.NextFloat(0, 1)),
                    MathHelper.Lerp(yMin, yMax, random.NextFloat(0, 1))
                );
                
                GasterBlasterDistraction gasterBlaster = new GasterBlasterDistraction(TimeSpan.FromSeconds(interval * i));
                gasterBlaster.Position = position;

                Vector2 toCenter = screenCenter - position;
                Vector2 initialDirection = new Vector2(0, 1);
                float angle = MathUtilities.SignedAngleBetween(initialDirection, toCenter);
                
                gasterBlaster.Rotation = random.NextFloat(-MathHelper.Pi / 4f, MathHelper.Pi / 4f) + angle;
                
                manager.AddDistraction(gasterBlaster);
            }

            return 1f;
        }
    }
}