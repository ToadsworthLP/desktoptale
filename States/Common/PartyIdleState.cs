using Desktoptale.Characters;
using Microsoft.Xna.Framework;

namespace Desktoptale.States.Common
{
    public class PartyIdleState : IState<Character>
    {
        private float walkTriggerDistance;

        public PartyIdleState(float walkTriggerDistance)
        {
            this.walkTriggerDistance = walkTriggerDistance;
        }

        public virtual void Enter(StateEnterContext<Character> context)
        {
            context.Target.UpdateSprite(context.Target.IdleSprite);
            context.Target.CurrentSprite.Play();
        
            context.Target.Velocity = Vector2.Zero;

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

            float scaleFactor = MathUtilities.Min(context.Target.Scale.X, context.Target.Scale.Y);
            if ((context.Target.Position - inFront.Position).Length() > walkTriggerDistance * scaleFactor)
            {
                context.StateMachine.ChangeState(context.Target.PartyWalkState);
            }
        }

        public virtual void Exit(StateExitContext<Character> context)
        {
            context.Target.CurrentSprite.Stop();
            
            context.Target.EnabledAutoOrientation = true;
        }
    }
}