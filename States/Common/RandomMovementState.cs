using System;
using Desktoptale.Characters;
using SharpDX;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Desktoptale.States.Common
{
    public class RandomMovementState : IState<Character>
    {
        private static Vector2[] directions = {
            new Vector2(1, 0),
            new Vector2(-1, 0),
            new Vector2(0, 1),
            new Vector2(0, -1)
        };

        private float speed;
    
        private Random rng;
    
        private TimeSpan duration;
        private Vector2 direction;

        public RandomMovementState(float speed)
        {
            rng = new Random(GetHashCode());
            this.speed = speed;
        }
    
        public void Enter(StateEnterContext<Character> context)
        {
            duration = TimeSpan.FromSeconds(0.25 + rng.NextDouble() / 2);

            int directionIndex = rng.Next(0, 4);
            direction = directions[directionIndex];
        
            context.Target.UpdateSprite(context.Target.WalkSprite);
            context.Target.CurrentSprite.Play();
        }

        public void Update(StateUpdateContext<Character> context)
        {
            if (context.Target.IsActive && context.Target.InputManager.DirectionalInput.LengthSquared() > float.Epsilon)
            {
                context.StateMachine.ChangeState(context.Target.WalkState);
                return;
            }
            
            if (context.Target.IsBeingDragged || !context.Target.IdleRoamingEnabled)
            {
                context.StateMachine.ChangeState(context.Target.IdleState);
                return;
            }
        
            if (context.StateTime > duration)
            {
                context.StateMachine.ChangeState(context.Target.RandomMovementWaitState);
                return;
            }
        
            context.Target.Velocity = 
                direction *
                speed *
                (float)context.Time.ElapsedGameTime.TotalSeconds *
                MathF.Min(context.Target.Scale.X, context.Target.Scale.Y);
        }

        public void Exit(StateExitContext<Character> context)
        {
            context.Target.CurrentSprite.Stop();
        }
    }
}