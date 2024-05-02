using System;
using Microsoft.Xna.Framework;

namespace Desktoptale.Distractions
{
    public class GasterBlasterPattern : IDistractionPattern
    {
        public float Spawn(IDistractionManager manager, Rectangle screenRectangle)
        {
            Vector2 screenCenter = new Vector2(screenRectangle.Width / 2, screenRectangle.Height / 2);

            GasterBlasterDistraction gasterBlaster = new GasterBlasterDistraction(TimeSpan.FromSeconds(1f));
            gasterBlaster.Position = screenCenter;
            gasterBlaster.Rotation = MathHelper.ToRadians(45f);
            manager.AddDistraction(gasterBlaster);
            
            return 1f;
        }
    }
}