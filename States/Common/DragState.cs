using Desktoptale.Characters;

namespace Desktoptale.States.Common
{
    public class DragState : IState<Character>
    {
        public void Enter(StateEnterContext<Character> context)
        {
            if (context.Target.DragSprite != null)
            {
                context.Target.UpdateSprite(context.Target.DragSprite);
                context.Target.CurrentSprite.Play();
            }
        }

        public void Update(StateUpdateContext<Character> context)
        {
            
        }

        public void Exit(StateExitContext<Character> context)
        {
            context.Target.CurrentSprite.Stop();
        }
    }
}