using System;
using Desktoptale.Distractions;
using Microsoft.Xna.Framework;

namespace Desktoptale.States.GasterBlaster
{
    public class BlasterFireState : IState<GasterBlasterDistraction>
    {
        private TimeSpan fireTime;
        private Vector2 velocity;

        private float acceleration = 2f;
        private Vector2 movementVector;

        public BlasterFireState(TimeSpan fireTime)
        {
            this.fireTime = fireTime;
        }

        public void Enter(StateEnterContext<GasterBlasterDistraction> context)
        {
            movementVector = Vector2.Transform(new Vector2(0, -1), Matrix.CreateRotationZ(context.Target.Rotation));
            
            context.Target.BlasterSprite.Play();
        }

        public void Update(StateUpdateContext<GasterBlasterDistraction> context)
        {
            velocity += movementVector * acceleration;
            context.Target.Position += velocity * (float)context.Time.ElapsedGameTime.TotalSeconds;

            acceleration += acceleration * (acceleration * 0.55f) * (float)context.Time.ElapsedGameTime.TotalSeconds;
            
            float progress = MathUtilities.Clamp(context.StateTime.Ticks / (float)fireTime.Ticks, 0, 1);
            float beamLevel = GetBeamLevelAtTime(progress);

            context.Target.BeamOpacity = beamLevel;
            context.Target.BeamWidth = beamLevel + (float)Math.Sin(context.StateTime.TotalSeconds * 50f) * 0.1f;

            context.Target.BlasterOpacity = progress > 0.85f ? MathHelper.Lerp(1, 0, 5 * (progress - 0.85f)) : 1f;
            
            if (context.StateTime > fireTime)
            {
                context.Target.Disposed = true;
            }
        }

        public void Exit(StateExitContext<GasterBlasterDistraction> context)
        {
            
        }

        private float GetBeamLevelAtTime(float t)
        {
            float val = 2 * (t - 0.5f);
            float final = 1 - (val * val * val * val);
            return final;
        }
    }
}