using Desktoptale.Characters;
using Microsoft.Xna.Framework;

namespace Desktoptale.States.Common
{
    public class PartyRunState : IState<Character>
    {
        private float walkTriggerDistance;

        private float speed;
        
        public PartyRunState(float speed, float walkTriggerDistance)
        {
            this.speed = speed;
            this.walkTriggerDistance = walkTriggerDistance;
        }

        public virtual void Enter(StateEnterContext<Character> context)
        {
            context.Target.UpdateSprite(context.Target.RunSprite);
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
            float scaleFactor = MathUtilities.Min(context.Target.Scale.X, context.Target.Scale.Y);
            if (toInFront.Length() < walkTriggerDistance * scaleFactor)
            {
                context.StateMachine.ChangeState(context.Target.PartyWalkState);
                return;
            }
            
            Vector2 direction = toInFront;
            direction.Normalize();
            
            context.Target.Velocity = 
                direction *
                speed *
                (context.Target.InputManager.RawDirectionalInput.X != 0 && context.Target.InputManager.RawDirectionalInput.Y != 0 ? 1.4142135624f : 1f) *
                (float)(context.Time != null ? context.Time.ElapsedGameTime.TotalSeconds : 0)  *
                MathUtilities.Min(context.Target.Scale.X, context.Target.Scale.Y);

            context.Target.Orientation = MathUtilities.GetClosestOrientation(direction);
        }

        public virtual void Exit(StateExitContext<Character> context)
        {
            context.Target.CurrentSprite.Stop();
            
            context.Target.EnabledAutoOrientation = true;
        }
    }
}