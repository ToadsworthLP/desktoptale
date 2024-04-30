using Desktoptale.Characters;
using Microsoft.Xna.Framework;

namespace Desktoptale.States.Common
{
    public class RunState : WalkState
    {
        public RunState(float speed, bool useRawInput) : base(speed, useRawInput)
        {
        }

        public override void Enter(StateEnterContext<Character> context)
        {
            context.Target.UpdateSprite(context.Target.RunSprite);
            context.Target.CurrentSprite.Play();
        }

        public override void Update(StateUpdateContext<Character> context)
        {
            if (context.Target.IsActive && context.Target.InputManager.ActionButtonPressed && context.Target.ActionSprite != null)
            {
                context.StateMachine.ChangeState(context.Target.ActionState);
                return;
            }
            
            if (!context.Target.IsActive || context.Target.InputManager.DirectionalInput.LengthSquared() < float.Epsilon)
            {
                context.StateMachine.ChangeState(context.Target.IdleState);
                return;
            }
        
            if (!context.Target.InputManager.RunButtonPressed)
            {
                context.StateMachine.ChangeState(context.Target.WalkState);
                return;
            }
            
            if (Speed < float.Epsilon)
            {
                Orientation? orientation = context.Target.GetOrientationFromVelocity(context.Target.InputManager.DirectionalInput);
                if (orientation != null)
                {
                    context.Target.Orientation = orientation.Value;
                }
            }
        
            context.Target.Velocity = 
                (UseRawInput ? context.Target.InputManager.RawDirectionalInput : context.Target.InputManager.DirectionalInput) *
                Speed *
                (float)context.Time.ElapsedGameTime.TotalSeconds *
                MathUtilities.Min(context.Target.Scale.X, context.Target.Scale.Y);
        }
    }
}