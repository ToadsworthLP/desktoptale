using System;
using Desktoptale.Characters;

namespace Desktoptale.States.Common
{
    public class RandomActionState : IState<Character>
    {
        private Random rng;
        private TimeSpan duration;
        private int minLoops;
        private int maxLoops;
        
        public RandomActionState(int minLoops = 1, int maxLoops = 1)
        {
            this.minLoops = minLoops;
            this.maxLoops = maxLoops;
            
            rng = new Random(GetHashCode());
        }
        
        public void Enter(StateEnterContext<Character> context)
        {
            int loops = rng.Next(minLoops, maxLoops + 1);
            duration = TimeSpan.FromSeconds((1/context.Target.ActionSprite.Framerate) * context.Target.ActionSprite.FrameCount * (context.Target.ActionSprite.Loop ? loops : 1));
            
            context.Target.UpdateSprite(context.Target.ActionSprite);
            context.Target.CurrentSprite.Play();
        }

        public void Update(StateUpdateContext<Character> context)
        {
            if (context.Target.IsActive && context.Target.InputManager.DirectionalInput.LengthSquared() > float.Epsilon)
            {
                context.StateMachine.ChangeState(context.Target.WalkState);
                return;
            }
            
            if (context.Target.IsBeingDragged || !context.Target.IdleRoamingEnabled)
            {
                context.StateMachine.ChangeState(context.Target.IdleState);
                return;
            }
            
            if (context.StateTime > duration)
            {
                context.StateMachine.ChangeState(context.Target.RandomMovementWaitState);
                return;
            }
        }

        public void Exit(StateExitContext<Character> context)
        {
            context.Target.CurrentSprite.Stop();
        }
    }
}