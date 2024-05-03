using System;
using Microsoft.Xna.Framework;

namespace Desktoptale.Distractions
{
    public class MovingBoneDistraction : BoneDistraction
    {
        public Vector2 Velocity;
        
        protected bool onScreenBefore = false;

        private const float FadeDistance = 30f;
        private static readonly TimeSpan DespawnDelay = TimeSpan.FromSeconds(0.5f);
        
        private double movementAxisDistance = float.MaxValue;
        private TimeSpan? despawnAtTime;
        
        public MovingBoneDistraction(BoneLength length, Vector2 velocity) : base(length)
        {
            Velocity = velocity;
        }

        public override void Update(GameTime gameTime, Rectangle screenRectangle)
        {
            if (Math.Abs(Velocity.X) > Math.Abs(Velocity.Y))
            {
                movementAxisDistance = (Position.X > screenRectangle.Left && Position.X < screenRectangle.Right)
                    ? 0
                    : Math.Min(Math.Abs(screenRectangle.Left - Position.X), Math.Abs(screenRectangle.Right - Position.X));
            }
            else
            {
                movementAxisDistance = (Position.Y > screenRectangle.Top && Position.Y < screenRectangle.Bottom)
                    ? 0
                    : Math.Min(Math.Abs(screenRectangle.Top - Position.Y), Math.Abs(screenRectangle.Bottom - Position.Y));
            }
            
            float scaledFadeDistance = FadeDistance * Math.Max(Scale.X, Scale.Y);
            if (movementAxisDistance < scaledFadeDistance && movementAxisDistance > 0)
            {
                opacity = 1 - ((float)movementAxisDistance / scaledFadeDistance);
            }
            else if(movementAxisDistance <= 0)
            {
                opacity = 1;
            }
            else
            {
                opacity = 0;
            }
            
            if (!onScreenBefore)
            { 
                onScreenBefore = IsOnScreen();
            }
            
            if (onScreenBefore && !IsOnScreen() && !despawnAtTime.HasValue)
            {
                despawnAtTime = gameTime.TotalGameTime + DespawnDelay;
            }

            if (despawnAtTime < gameTime.TotalGameTime)
            {
                Disposed = true;
            }
            
            Position += Velocity * Scale * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        protected bool IsOnScreen()
        {
            return Math.Abs(movementAxisDistance) <= 0f;
        }
    }
}