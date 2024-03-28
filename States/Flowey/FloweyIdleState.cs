using Desktoptale.Characters;
using Microsoft.Xna.Framework;

namespace Desktoptale.States.Flowey
{
    public class FloweyIdleState : IState<Character>
    {
        public void Enter(StateEnterContext<Character> context)
        {
            context.Target.UpdateSprite(context.Target.IdleSprite);
            context.Target.Velocity = Vector2.Zero;
        }

        public void Update(StateUpdateContext<Character> context)
        {
            if (!context.Target.IsActive) return;
            
            Orientation? orientation = GetOrientationFromVelocity(context.Target.InputManager.DirectionalInput);
            if (orientation != null)
            {
                context.Target.Orientation = orientation.Value;
            }
        }

        public void Exit(StateExitContext<Character> context) {}
        
        private Orientation? GetOrientationFromVelocity(Vector2 input)
        {
            if (input.Y < -float.Epsilon) return Orientation.Up;
            if (input.Y > float.Epsilon) return Orientation.Down;
            if (input.X < -float.Epsilon) return Orientation.Left;
            if (input.X > float.Epsilon) return Orientation.Right;

            return null;
        }
    }
}