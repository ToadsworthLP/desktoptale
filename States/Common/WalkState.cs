using System;

namespace Desktoptale.States.Common;

public class WalkState : IState<Character>
{
    public virtual void Enter(StateEnterContext<Character> context)
    {
        context.Target.UpdateSprite(context.Target.WalkSprite);
        context.Target.CurrentSprite.Play();
    }

    public virtual void Update(StateUpdateContext<Character> context)
    {
        if (context.Target.InputManager.DirectionalInput.LengthSquared() < float.Epsilon)
        {
            context.StateMachine.ChangeState(context.Target.IdleState);
        }
        
        context.Target.Velocity = 
            context.Target.InputManager.DirectionalInput *
            context.Target.MovementSpeed *
            (float)context.Time.ElapsedGameTime.TotalSeconds *
            MathF.Min(context.Target.Scale.X, context.Target.Scale.Y);
    }

    public virtual void Exit(StateExitContext<Character> context)
    {
        context.Target.CurrentSprite.Stop();
    }
}