using Microsoft.Xna.Framework;

namespace Desktoptale;

public interface IState<T>
{
    void Enter(StateEnterContext<T> context);
    void Update(StateUpdateContext<T> context);
    void Exit(StateExitContext<T> context);
}