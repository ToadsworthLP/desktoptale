using System;
using Desktoptale.Characters;

namespace Desktoptale.States.Clover
{
    public class CloverDanceState : IState<Character>
    {
        private Random rng;
        private TimeSpan duration;
        
        public CloverDanceState()
        {
            rng = new Random();
        }
        
        public void Enter(StateEnterContext<Character> context)
        {
            Characters.Clover clover = (Characters.Clover)context.Target;
            
            int loops = rng.Next(4, 17);
            duration = TimeSpan.FromSeconds((1/clover.DanceSprite.Framerate) * 6 * loops);
            
            context.Target.UpdateSprite(clover.DanceSprite);
            context.Target.CurrentSprite.Play();
        }

        public void Update(StateUpdateContext<Character> context)
        {
            if (context.Target.InputManager.DirectionalInput.LengthSquared() > float.Epsilon)
            {
                context.StateMachine.ChangeState(context.Target.WalkState);
                return;
            }
            
            if (context.Target.IsBeingDragged || !context.Target.EnableIdleMovement)
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