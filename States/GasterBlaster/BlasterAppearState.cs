using System;
using Desktoptale.Distractions;
using Microsoft.Xna.Framework;

namespace Desktoptale.States.GasterBlaster
{
    public class BlasterAppearState : IState<GasterBlasterDistraction>
    {
        private IState<GasterBlasterDistraction> nextState;
        private float targetRotation;
        private Vector2 initialPosition;
        private Vector2 targetPosition;
        private TimeSpan animationDuration;
        private float flyInDistance;

        public BlasterAppearState(IState<GasterBlasterDistraction> nextState, float targetRotation, TimeSpan animationDuration, float flyInDistance)
        {
            this.nextState = nextState;
            this.targetRotation = targetRotation;
            this.animationDuration = animationDuration;
            this.flyInDistance = flyInDistance;
        }

        public void Enter(StateEnterContext<GasterBlasterDistraction> context)
        {
            Vector2 movementVector = Vector2.Transform(new Vector2(0, 1), Matrix.CreateRotationZ(targetRotation));
            initialPosition = context.Target.Position - movementVector * flyInDistance;
            targetPosition = context.Target.Position;
            
            context.Target.BlasterOpacity = 0f;
            context.Target.Rotation = targetRotation - (float)Math.PI;
        }

        public void Update(StateUpdateContext<GasterBlasterDistraction> context)
        {
            float progress = MathUtilities.Clamp(context.StateTime.Ticks / (float)animationDuration.Ticks, 0, 1);
            context.Target.BlasterOpacity = progress;

            float smoothProgress = MathUtilities.InterpolateQuadraticGetSlowerTowardsEnd(0, 1, progress);
            context.Target.Rotation = MathHelper.Lerp(targetRotation - (float)Math.PI, targetRotation, smoothProgress);
            context.Target.Position = Vector2.Lerp(initialPosition, targetPosition, smoothProgress);
            
            if (context.StateTime > animationDuration)
            {
                context.StateMachine.ChangeState(nextState);
            }
        }

        public void Exit(StateExitContext<GasterBlasterDistraction> context)
        {
            
        }
    }
}