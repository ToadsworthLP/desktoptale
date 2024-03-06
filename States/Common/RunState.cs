using System;

namespace Desktoptale.States.Common;

public class RunState : WalkState
{
    public RunState(float speed) : base(speed)
    {
    }

    public override void Enter(StateEnterContext<Character> context)
    {
        context.Target.UpdateSprite(context.Target.RunSprite);
        context.Target.CurrentSprite.Play();
    }

    public override void Update(StateUpdateContext<Character> context)
    {
        if (context.Target.InputManager.DirectionalInput.LengthSquared() < float.Epsilon)
        {
            context.StateMachine.ChangeState(context.Target.IdleState);
            return;
        }
        
        if (!context.Target.InputManager.RunButtonPressed)
        {
            context.StateMachine.ChangeState(context.Target.WalkState);
            return;
        }
        
        context.Target.Velocity = 
            context.Target.InputManager.DirectionalInput *
            Speed *
            (float)context.Time.ElapsedGameTime.TotalSeconds *
            MathF.Min(context.Target.Scale.X, context.Target.Scale.Y);
    }
}