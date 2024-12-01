using System;
using Desktoptale.Characters;
using Microsoft.Xna.Framework;

namespace Desktoptale.States.Common
{
    public class PartyWalkState : IState<Character>
    {
        private float idleTriggerDistance;
        private float runTriggerDistance;

        private float speed;
        private TimeSpan teleportDelay;
        
        public PartyWalkState(float speed, float idleTriggerDistance, float runTriggerDistance, TimeSpan teleportDelay)
        {
            this.speed = speed;
            this.idleTriggerDistance = idleTriggerDistance;
            this.runTriggerDistance = runTriggerDistance;
            this.teleportDelay = teleportDelay;
        }

        public virtual void Enter(StateEnterContext<Character> context)
        {
            context.Target.UpdateSprite(context.Target.WalkSprite);
            context.Target.CurrentSprite.Play();
            
            context.Target.EnabledAutoOrientation = false;
        }

        public virtual void Update(StateUpdateContext<Character> context)
        {
            if (context.Target.Properties.Party == null)
            {
                context.StateMachine.ChangeState(context.Target.IdleState);
                return;
            }

            ICharacter inFront = context.Target.Properties.Party.GetCharacterInFront(context.Target);
            if (inFront == null)
            {
                context.StateMachine.ChangeState(context.Target.IdleState);
                return;
            }

            Vector2 toInFront = inFront.Position - context.Target.Position;
            float distanceToInFront = toInFront.Length();

            float scaleFactor = MathUtilities.Min(context.Target.Scale.X, context.Target.Scale.Y);
            if (distanceToInFront < idleTriggerDistance * scaleFactor)
            {
                context.StateMachine.ChangeState(context.Target.PartyIdleState);
                return;
            }

            Vector2 direction = toInFront;
            direction.Normalize();
            
            if (context.Target.Properties.Type.Teleport && context.Target.DisappearSprite != null && context.Target.AppearSprite != null)
            {
                if (context.Time != null && context.StateTime >= teleportDelay)
                {
                    context.StateMachine.ChangeState(context.Target.PartyTeleportDisappearState);
                }
            }
            else
            {
                if (distanceToInFront > runTriggerDistance  * scaleFactor)
                {
                    context.StateMachine.ChangeState(context.Target.PartyRunState);
                    return;
                }
                
                context.Target.Velocity = 
                    direction *
                    speed *
                    (context.Target.InputManager.RawDirectionalInput.X != 0 && context.Target.InputManager.RawDirectionalInput.Y != 0 ? 1.4142135624f : 1f) *
                    (float)(context.Time != null ? context.Time.ElapsedGameTime.TotalSeconds : 0)  *
                    MathUtilities.Min(context.Target.Scale.X, context.Target.Scale.Y);

            }
            
            context.Target.Orientation = MathUtilities.GetClosestOrientation(direction);
        }

        public virtual void Exit(StateExitContext<Character> context)
        {
            context.Target.CurrentSprite.Stop();
            
            context.Target.EnabledAutoOrientation = true;
        }
    }
}