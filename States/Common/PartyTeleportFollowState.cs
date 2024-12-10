using System;
using Desktoptale.Characters;
using Microsoft.Xna.Framework;

namespace Desktoptale.States.Common
{
    public class PartyTeleportFollowState : IState<Character>
    {
        private float idleTriggerDistance;
        private TimeSpan minStateTime;

        public PartyTeleportFollowState(float idleTriggerDistance, TimeSpan minStateTime)
        {
            this.idleTriggerDistance = idleTriggerDistance;
            this.minStateTime = minStateTime;
        }

        public virtual void Enter(StateEnterContext<Character> context)
        {
            context.Target.IsVisible = false;
            context.Target.EnabledAutoOrientation = false;
        }

        public virtual void Update(StateUpdateContext<Character> context)
        {
            if (context.Target.Properties.Party == null)
            {
                context.StateMachine.ChangeState(context.Target.PartyTeleportAppearState);
                return;
            }

            ICharacter inFront = context.Target.Properties.Party.GetCharacterInFront(context.Target);
            if (inFront == null)
            {
                context.StateMachine.ChangeState(context.Target.PartyTeleportAppearState);
                return;
            }

            Vector2 toInFront = inFront.Position - context.Target.Position;
            float distanceToInFront = toInFront.Length();

            float scaleFactor = MathUtilities.Min(context.Target.Scale.X, context.Target.Scale.Y);
            if (distanceToInFront < idleTriggerDistance * scaleFactor && context.Time != null && context.StateTime >= minStateTime)
            {
                context.StateMachine.ChangeState(context.Target.PartyTeleportAppearState);
                return;
            }
            
            Vector2 direction = toInFront;
            direction.Normalize();
            
            context.Target.Velocity = 
                direction *
                (distanceToInFront > 200f * scaleFactor ? 500f : (context.Target.InputManager.RunButtonPressed ? Math.Max(inFront.Properties.Type.RunSpeed, 180f) : Math.Max(inFront.Properties.Type.WalkSpeed, 90f))) *
                (context.Target.InputManager.RawDirectionalInput.X != 0 && context.Target.InputManager.RawDirectionalInput.Y != 0 ? 1.4142135624f : 1f) *
                (float)(context.Time != null ? context.Time.ElapsedGameTime.TotalSeconds : 0)  *
                MathUtilities.Min(context.Target.Scale.X, context.Target.Scale.Y);

            context.Target.Orientation = MathUtilities.GetClosestOrientation(direction);
        }

        public virtual void Exit(StateExitContext<Character> context)
        {
            context.Target.IsVisible = true;
            context.Target.EnabledAutoOrientation = true;
            
            context.Target.Velocity = Vector2.Zero;
        }
    }
}