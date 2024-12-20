﻿using System;
using Desktoptale.Characters;

namespace Desktoptale.States.Common
{
    public class SpawnState : IState<Character>
    {
        private TimeSpan duration;
        
        public void Enter(StateEnterContext<Character> context)
        {
            duration = TimeSpan.FromSeconds((1/(context.Target.SpawnSprite.Framerate > 0 ? context.Target.SpawnSprite.Framerate : 1)) * context.Target.SpawnSprite.FrameCount);
            
            context.Target.UpdateSprite(context.Target.SpawnSprite);
            context.Target.CurrentSprite.Play();
        }

        public void Update(StateUpdateContext<Character> context)
        {
            if (context.Target.IsActive && context.Target.InputManager.DirectionalInput.LengthSquared() > float.Epsilon)
            {
                context.StateMachine.ChangeState(context.Target.WalkState);
                return;
            }
            
            if (context.Target.IsBeingDragged)
            {
                context.StateMachine.ChangeState(context.Target.IdleState);
                return;
            }
            
            if (context.Time != null && context.StateTime > duration)
            {
                context.StateMachine.ChangeState(context.Target.IdleState);
                return;
            }
        }

        public void Exit(StateExitContext<Character> context)
        {
            context.Target.CurrentSprite.Stop();
        }
    }
}