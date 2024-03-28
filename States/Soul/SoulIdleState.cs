using System;
using Desktoptale.Characters;
using Microsoft.Xna.Framework;

namespace Desktoptale.States.Soul
{
    public class SoulIdleState : IState<Character>
    {
        public virtual void Enter(StateEnterContext<Character> context)
        {
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
        }

        public virtual void Exit(StateExitContext<Character> context)
        {
            context.Target.CurrentSprite.Stop();
        }
    }
}