using Desktoptale.Characters;
using Microsoft.Xna.Framework;

namespace Desktoptale.States.Common
{
    public class ActionState : IState<Character>
    {
        public void Enter(StateEnterContext<Character> context)
        {
            if (context.Target.ActionSprite != null)
            {
                context.Target.UpdateSprite(context.Target.ActionSprite);
                context.Target.CurrentSprite.Play();
            }
            
            context.Target.Velocity = Vector2.Zero;
        }

        public void Update(StateUpdateContext<Character> context)
        {
            if (!context.Target.IsActive || !context.Target.InputManager.ActionButtonPressed)
            {
                context.StateMachine.ChangeState(context.Target.IdleState);
            }
        }

        public void Exit(StateExitContext<Character> context)
        {
            context.Target.CurrentSprite.Stop();
        }
    }
}