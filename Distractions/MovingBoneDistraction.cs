using System;
using Microsoft.Xna.Framework;

namespace Desktoptale.Distractions
{
    public class MovingBoneDistraction : BoneDistraction
    {
        public Vector2 Velocity;
        
        private bool onScreenBefore = false;
        
        public MovingBoneDistraction(BoneLength length, Vector2 velocity) : base(length)
        {
            Velocity = velocity;
        }

        public override void Update(GameTime gameTime, Rectangle screenRectangle)
        {
            if (!onScreenBefore)
            { 
                onScreenBefore = IsOnScreen(screenRectangle);
            }
            
            Position += Velocity * Scale * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (onScreenBefore && !IsOnScreen(screenRectangle))
            {
                Disposed = true;
            }
        }

        protected bool IsOnScreen(Rectangle screenRectangle)
        {
            Rectangle rect = new Rectangle(0, 0, screenRectangle.Width, screenRectangle.Height);

            if (rect.Contains(Position))
            {
                return true;
            }
            else
            {
                double distance = MathUtilities.DistanceToNearestPointOnRectangle(rect, Position.ToPoint());
                return distance < Math.Max(boneTexture.Width * Scale.X, boneTexture.Height * Scale.Y) / 2f;
            }
        }
    }
}