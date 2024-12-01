using System;
using Desktoptale.Characters;

namespace Desktoptale.States.Common
{
    public class PartyTeleportAppearState : IState<Character>
    {
        private TimeSpan duration;
        
        public void Enter(StateEnterContext<Character> context)
        {
            duration = TimeSpan.FromSeconds((1/(context.Target.AppearSprite.Framerate > 0 ? context.Target.AppearSprite.Framerate : 1)) * context.Target.AppearSprite.FrameCount);
            
            context.Target.UpdateSprite(context.Target.AppearSprite);
            context.Target.CurrentSprite.Play();
        }

        public void Update(StateUpdateContext<Character> context)
        {
            if (context.Time != null && context.StateTime > duration)
            {
                context.StateMachine.ChangeState(context.Target.PartyIdleState);
                return;
            }
        }

        public void Exit(StateExitContext<Character> context)
        {
            context.Target.CurrentSprite.Stop();
        }
    }
}