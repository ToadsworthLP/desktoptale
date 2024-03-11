using Desktoptale.Characters;
using Desktoptale.States.Common;

namespace Desktoptale.States.Clover
{
    public class CloverRandomMovementWaitState : RandomMovementWaitState
    {
        public override void Update(StateUpdateContext<Character> context)
        {
            if (context.StateTime > duration)
            {
                int random = rng.Next(100);
                if (random == 99)
                {
                    Characters.Clover clover = (Characters.Clover)context.Target;
                    context.StateMachine.ChangeState(clover.DanceState);
                }
                else
                {
                    context.StateMachine.ChangeState(context.Target.RandomMovementState);
                }
                return;
            }
            
            base.Update(context);
        }
    }
}