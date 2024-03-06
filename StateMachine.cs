using System;
using Microsoft.Xna.Framework;

namespace Desktoptale
{
    public class StateMachine<T>
    {
        private T target;
        private IState<T> currentState;
        private TimeSpan currentTime;
        private TimeSpan lastStateChangeTime;
        private StateUpdateContext<T> lastUpdateContext;

        public delegate void StateChangedEventHandler(IState<T> oldState, IState<T> newState);
        public event StateChangedEventHandler StateChanged; 

        public StateMachine(T target, IState<T> initialState)
    {
        this.target = target;
        
        currentState = initialState;
        currentTime = TimeSpan.Zero;
        
        currentState.Enter(new StateEnterContext<T>()
        {
            PreviousState = null,
            StateMachine = this,
            Target = target
        });
    }
    
        public void Update(GameTime gameTime)
    {
        currentTime = gameTime.TotalGameTime;
        
        lastUpdateContext = new StateUpdateContext<T>
        {
            Time = gameTime,
            LastStateChangeTime = lastStateChangeTime,
            StateMachine = this,
            Target = target
        };
        
        currentState.Update(lastUpdateContext);
    }

        public void ChangeState(IState<T> newState)
    {
        IState<T> previousState = currentState;
        
        currentState.Exit(new StateExitContext<T>()
        {
            NextState = newState,
            StateMachine = this,
            Target = target
        });
        
        currentState = newState;
        lastStateChangeTime = currentTime;
        
        currentState.Enter(new StateEnterContext<T>()
        {
            PreviousState = previousState,
            StateMachine = this,
            Target = target
        });

        StateChanged?.Invoke(previousState, newState);
        
        currentState.Update(new StateUpdateContext<T>
        {
            Time = lastUpdateContext.Time,
            LastStateChangeTime = lastStateChangeTime,
            StateMachine = lastUpdateContext.StateMachine,
            Target = lastUpdateContext.Target
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