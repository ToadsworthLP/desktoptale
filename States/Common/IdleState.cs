﻿using System;
using Desktoptale.Characters;
using Microsoft.Xna.Framework;

namespace Desktoptale.States.Common
{
    public class IdleState : IState<Character>
    {
        private TimeSpan idleAnimationTime;
    
        private Random rng;
    
        public IdleState()
        {
            rng = new Random(GetHashCode());
        }
    
        public virtual void Enter(StateEnterContext<Character> context)
        {
            idleAnimationTime = TimeSpan.FromSeconds(10);
        
            context.Target.UpdateSprite(context.Target.IdleSprite);
            context.Target.CurrentSprite.Play();
        
            context.Target.Velocity = Vector2.Zero;
        }

        public virtual void Update(StateUpdateContext<Character> context)
        {
            if (context.Target.IsActive && context.Target.InputManager.ActionButtonPressed && context.Target.ActionSprite != null)
            {
                context.StateMachine.ChangeState(context.Target.ActionState);
                return;
            }
            
            if (context.Target.IsActive && context.Target.InputManager.DirectionalInput.LengthSquared() > float.Epsilon)
            {
                context.StateMachine.ChangeState(context.Target.WalkState);
                return;
            }
        
            if (context.Target.IdleRoamingEnabled && context.Time != null && context.StateTime > idleAnimationTime && !context.Target.IsBeingDragged)
            {
                context.StateMachine.ChangeState(context.Target.RandomMovementWaitState);
                return;
            }
        }

        public virtual void Exit(StateExitContext<Character> context)
        {
            context.Target.CurrentSprite.Stop();
        }
    }
}