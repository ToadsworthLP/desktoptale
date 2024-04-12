using System;
using Microsoft.Xna.Framework;

namespace Desktoptale
{
    public class StateMachine<T>
    {
        public IState<T> CurrentState { get; private set; }
        
        private T target;
        private TimeSpan currentTime;
        private TimeSpan lastStateChangeTime;
        private StateUpdateContext<T> lastUpdateContext;
        private bool firstUpdate = true;

        public delegate void StateChangedEventHandler(IState<T> oldState, IState<T> newState);
        public event StateChangedEventHandler StateChanged; 

        public StateMachine(T target, IState<T> initialState)
        {
            this.target = target;
            
            CurrentState = initialState;
            currentTime = TimeSpan.Zero;
            
            CurrentState.Enter(new StateEnterContext<T>()
            {
                PreviousState = null,
                StateMachine = this,
                Target = target
            });
        }
        
        public void Update(GameTime gameTime)
        {
            if (firstUpdate)
            {
                firstUpdate = false;
                lastStateChangeTime = gameTime.TotalGameTime;
            }
            
            currentTime = gameTime.TotalGameTime;
            
            lastUpdateContext = new StateUpdateContext<T>
            {
                Time = gameTime,
                LastStateChangeTime = lastStateChangeTime,
                StateMachine = this,
                Target = target
            };
            
            CurrentState.Update(lastUpdateContext);
        }

        public void ChangeState(IState<T> newState)
        {
            IState<T> previousState = CurrentState;
            
            CurrentState.Exit(new StateExitContext<T>()
            {
                NextState = newState,
                StateMachine = this,
                Target = target
            });
            
            CurrentState = newState;
            lastStateChangeTime = currentTime;
            
            CurrentState.Enter(new StateEnterContext<T>()
            {
                PreviousState = previousState,
                StateMachine = this,
                Target = target
            });

            StateChanged?.Invoke(previousState, newState);
            
            CurrentState.Update(new StateUpdateContext<T>
            {
                Time = lastUpdateContext.Time,
                LastStateChangeTime = lastStateChangeTime,
                StateMachine = this,
                Target = target
            });
        }
    }

    public struct StateUpdateContext<T>
    {
        public GameTime Time { get; set; }
        public TimeSpan LastStateChangeTime { get; set; }
        public TimeSpan StateTime => Time.TotalGameTime - LastStateChangeTime;
        public StateMachine<T> StateMachine { get; set; }
        public T Target { get; set; }
    }

    public struct StateEnterContext<T>
    {
        public IState<T> PreviousState { get; set; }
        public StateMachine<T> StateMachine { get; set; }
        public T Target { get; set; }
    }

    public struct StateExitContext<T>
    {
        public IState<T> NextState { get; set; }
        public StateMachine<T> StateMachine { get; set; }
        public T Target { get; set; }
    }
}