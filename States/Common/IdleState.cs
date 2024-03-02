namespace Desktoptale.States.Common;

public class IdleState : IState<Character>
{
    public virtual void Enter(StateEnterContext<Character> context)
    {
        context.Target.UpdateSprite(context.Target.IdleSprite);
        context.Target.CurrentSprite.Play();
    }

    public virtual void Update(StateUpdateContext<Character> context)
    {
        if (context.Target.InputManager.DirectionalInput.LengthSquared() > float.Epsilon)
        {
            context.StateMachine.ChangeState(context.Target.WalkState);
        }
    }

    public virtual void Exit(StateExitContext<Character> context)
    {
        context.Target.CurrentSprite.Stop();
    }
}