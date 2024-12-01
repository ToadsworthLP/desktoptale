using System;
using Desktoptale.Characters;
using Microsoft.Xna.Framework;

namespace Desktoptale.States.Common
{
    public class PartyTeleportDisappearState : IState<Character>
    {
        private TimeSpan duration;
        
        public void Enter(StateEnterContext<Character> context)
        {
            duration = TimeSpan.FromSeconds((1/(context.Target.DisappearSprite.Framerate > 0 ? context.Target.DisappearSprite.Framerate : 1)) * context.Target.DisappearSprite.FrameCount);
            
            context.Target.UpdateSprite(context.Target.DisappearSprite);
            context.Target.CurrentSprite.Play();
            
            context.Target.Velocity = Vector2.Zero;
        }

        public void Update(StateUpdateContext<Character> context)
        {
            if (context.Target.Properties.Party == null)
            {
                context.StateMachine.ChangeState(context.Target.IdleState);
                return;
            }

            ICharacter inFront = context.Target.Properties.Party.GetCharacterInFront(context.Target);
            if (inFront == null)
            {
                if (context.Time != null && context.StateTime > duration)
                {
                    context.StateMachine.ChangeState(context.Target.IdleState);
                    return;
                }
                
                return;
            }
            
            if (context.Time != null && context.StateTime > duration)
            {
                context.StateMachine.ChangeState(context.Target.PartyTeleportFollowState);
                return;
            }
        }

        public void Exit(StateExitContext<Character> context)
        {
            context.Target.CurrentSprite.Stop();
        }
    }
}