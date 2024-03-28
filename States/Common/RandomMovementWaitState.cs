using System;
using Desktoptale.Characters;
using Microsoft.Xna.Framework;

namespace Desktoptale.States.Common
{
    public class RandomMovementWaitState : IState<Character>
    {
        protected TimeSpan duration;
        protected Random rng;
    
        public RandomMovementWaitState()
        {
            rng = new Random();
        }
    
        public virtual void Enter(StateEnterContext<Character> context)
        {
            duration = TimeSpan.FromSeconds(rng.NextDouble() * 10);
        
            context.Target.UpdateSprite(context.Target.IdleSprite);
            context.Target.CurrentSprite.Play();
        
            context.Target.Velocity = Vector2.Zero;
        }

        public virtual void Update(StateUpdateContext<Character> context)
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
                context.StateMachine.ChangeState(context.Target.RandomMovementState);
                return;
            }
        }

        public virtual void Exit(StateExitContext<Character> context)
        {
        
        }
    }
}