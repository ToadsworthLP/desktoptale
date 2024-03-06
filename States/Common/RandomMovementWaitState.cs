using System;
using Microsoft.Xna.Framework;

namespace Desktoptale.States.Common
{
    public class RandomMovementWaitState : IState<Character>
    {
        private TimeSpan duration;
    
        private Random rng;
    
        public RandomMovementWaitState()
        {
            rng = new Random();
        }
    
        public void Enter(StateEnterContext<Character> context)
        {
            duration = TimeSpan.FromSeconds(rng.NextDouble() * 10);
        
            context.Target.UpdateSprite(context.Target.IdleSprite);
            context.Target.CurrentSprite.Play();
        
            context.Target.Velocity = Vector2.Zero;
        }

        public void Update(StateUpdateContext<Character> context)
        {
            if (context.Target.InputManager.DirectionalInput.LengthSquared() > float.Epsilon)
            {
                context.StateMachine.ChangeState(context.Target.WalkState);
                return;
            }
        
            if (context.Target.IsBeingDragged)
            {
                context.StateMachine.ChangeState(context.Target.IdleState);
                return;
            }
            
            if (context.StateTime > duration)
            {
                context.StateMachine.ChangeState(context.Target.RandomMovementState);
                return;
            }
        }

        public void Exit(StateExitContext<Character> context)
        {
        
        }
    }
}