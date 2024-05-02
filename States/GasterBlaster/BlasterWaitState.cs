using System;
using Desktoptale.Distractions;

namespace Desktoptale.States.GasterBlaster
{
    public class BlasterWaitState : IState<GasterBlasterDistraction>
    {
        private IState<GasterBlasterDistraction> nextState;
        private TimeSpan delayTime;

        public BlasterWaitState(IState<GasterBlasterDistraction> nextState, TimeSpan delayTime)
        {
            this.nextState = nextState;
            this.delayTime = delayTime;
        }

        public void Enter(StateEnterContext<GasterBlasterDistraction> context)
        {

        }

        public void Update(StateUpdateContext<GasterBlasterDistraction> context)
        {
            if (context.StateTime > delayTime)
            {
                context.StateMachine.ChangeState(nextState);
            }
        }

        public void Exit(StateExitContext<GasterBlasterDistraction> context)
        {
            
        }
    }
}