using Desktoptale.Characters;

namespace Desktoptale.States.Common
{
    public class WalkState : IState<Character>
    {
        protected float Speed;
        protected bool UseRawInput;
        
        public WalkState(float speed, bool useRawInput)
        {
            this.Speed = speed;
            this.UseRawInput = useRawInput;
        }

        public virtual void Enter(StateEnterContext<Character> context)
        {
            context.Target.UpdateSprite(context.Target.WalkSprite);
            context.Target.CurrentSprite.Play();
        }

        public virtual void Update(StateUpdateContext<Character> context)
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
        
            if (context.Target.InputManager.RunButtonPressed)
            {
                context.StateMachine.ChangeState(context.Target.RunState);
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

        public virtual void Exit(StateExitContext<Character> context)
        {
            context.Target.CurrentSprite.Stop();
        }
    }
}