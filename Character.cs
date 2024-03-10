using System;
using Desktoptale.Messages;
using Desktoptale.Messaging;
using Desktoptale.States.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Desktoptale
{
    public abstract class Character : IGameObject
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Scale;
    
        public InputManager InputManager { get; }
        public StateMachine<Character> StateMachine { get; protected set; }
        public IState<Character> IdleState { get; protected set; }
        public IState<Character> WalkState { get; protected set; }
        public IState<Character> RunState { get; protected set; }
        public IState<Character> RandomMovementState { get; protected set; }
        public IState<Character> RandomMovementWaitState { get; protected set; }
    
        public IAnimatedSprite IdleSprite { get; protected set; }
        public IAnimatedSprite WalkSprite { get; protected set; }
        public IAnimatedSprite RunSprite { get; protected set; }
        public IAnimatedSprite CurrentSprite { get; set; }
        public bool IsBeingDragged => dragging;
        public bool EnableIdleMovement { get; set; }

        protected virtual IState<Character> InitialState => IdleState;
    
        private GameWindow window;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private bool dragging;
        private Point previousSpriteFrameSize;
        private Orientation orientation = Orientation.Down;
        private Orientation previousOrientation = Orientation.Down;
    
        private Subscription scaleChangeRequestedSubscription;
        private Subscription idleMovementChangeRequestedSubscription;

        public Character(GraphicsDeviceManager graphics, GameWindow window, SpriteBatch spriteBatch, InputManager inputManager)
        {
            this.spriteBatch = spriteBatch;
            this.InputManager = inputManager;
            this.graphics = graphics;
            this.window = window;
        }

        public void UpdateSprite(IAnimatedSprite sprite)
        {
            CurrentSprite?.Stop();
            CurrentSprite = sprite;
        
            if(previousSpriteFrameSize != sprite.FrameSize)
                UpdateWindowSize();

            previousSpriteFrameSize = sprite.FrameSize;
        }

        public virtual void Initialize()
        {
            scaleChangeRequestedSubscription = MessageBus.Subscribe<ScaleChangeRequestedMessage>(OnScaleChangeRequestedMessage);
            idleMovementChangeRequestedSubscription = MessageBus.Subscribe<IdleMovementChangeRequestedMessage>(OnIdleMovementChangeRequestedMessage);

            IdleState = new IdleState();
            WalkState = new WalkState(90f);
            RunState = new RunState(180f);
            RandomMovementState = new RandomMovementState(90);
            RandomMovementWaitState = new RandomMovementWaitState();
        
            StateMachine = new StateMachine<Character>(this, InitialState);
            StateMachine.StateChanged += (state, newState) => UpdateOrientation();
        
            InputManager.GrabFocus();
        }

        public virtual void LoadContent(ContentManager contentManager) { }
    
        public virtual void Update(GameTime gameTime)
        {
            StateMachine.Update(gameTime);
        
            UpdateOrientation();
        
            Position.X += (int)Math.Round(Velocity.X);
            Position.Y += (int)Math.Round(Velocity.Y);
        
            DragCharacter();
        
            PreventLeavingScreenArea();

            window.Position = new Point((int)Position.X, (int)Position.Y);
        
            CurrentSprite.Update(gameTime);
        }

        public virtual void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            Vector2 center = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2f, graphics.GraphicsDevice.Viewport.Height / 2f);
            Vector2 origin = new Vector2(CurrentSprite.FrameSize.X / 2f, CurrentSprite.FrameSize.Y / 2f);
            CurrentSprite.Draw(spriteBatch, center, Color.White, 0, origin, Scale, SpriteEffects.None, 0);
            spriteBatch.End();
        }

        public virtual void Dispose()
        {
            MessageBus.Unsubscribe(scaleChangeRequestedSubscription);
            MessageBus.Unsubscribe(idleMovementChangeRequestedSubscription);
        }

        private void OnScaleChangeRequestedMessage(ScaleChangeRequestedMessage message)
        {
            Scale = new Vector2(message.ScaleFactor, message.ScaleFactor);
            UpdateWindowSize();
            InputManager.GrabFocus();
        }
        
        private void OnIdleMovementChangeRequestedMessage(IdleMovementChangeRequestedMessage message)
        {
            EnableIdleMovement = message.Enabled;
        }

        private void UpdateOrientation()
        {
            Orientation? updatedOrientation = GetOrientationFromVelocity(Velocity);
            if (updatedOrientation != null)
            {
                previousOrientation = orientation;
                orientation = updatedOrientation.Value;
            }

            if (CurrentSprite is OrientedAnimatedSprite sprite)
            {
                sprite.Orientation = orientation;

                if (previousOrientation != orientation)
                {
                    UpdateWindowSize();
                }
            }
        }

        private Orientation? GetOrientationFromVelocity(Vector2 input)
        {
            if (input.Y < -float.Epsilon) return Orientation.Up;
            if (input.Y > float.Epsilon) return Orientation.Down;
            if (input.X < -float.Epsilon) return Orientation.Left;
            if (input.X > float.Epsilon) return Orientation.Right;

            return null;
        }

        private void DragCharacter()
        {
            if (InputManager.LeftClickJustPressed &&
                graphics.GraphicsDevice.Viewport.Bounds.Contains(InputManager.PointerPosition))
            {
                dragging = true;
            } else if (!InputManager.LeftClickPressed)
            {
                dragging = false;
            }

            if (dragging)
            {
                Point windowCenter = window.Position -
                                     new Point(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);
                Position = (windowCenter + InputManager.PointerPosition).ToVector2();
            }
        }

        private void PreventLeavingScreenArea()
        {
            Point screenSize = GetScreenSize();
            Point windowSize = GetWindowSize();

            float xPivot = CurrentSprite.FrameSize.X * Scale.X / 2;
            float yPivot = CurrentSprite.FrameSize.Y * Scale.X / 2;

            if (Position.X < -windowSize.X + xPivot) Position.X = -windowSize.X + xPivot;
            if (Position.Y < -windowSize.Y + yPivot) Position.Y = -windowSize.Y + yPivot;
            if (Position.X > screenSize.X - xPivot) Position.X = screenSize.X - xPivot;
            if (Position.Y > screenSize.Y - yPivot) Position.Y = screenSize.Y - yPivot;
        }

        private Point GetScreenSize()
        {
            return new Point(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
        }

        private Point GetWindowSize()
        {
            return new Point((int)(CurrentSprite.FrameSize.X * Scale.X), (int)(CurrentSprite.FrameSize.Y * Scale.Y));
        }

        private void UpdateWindowSize()
        {
            int oldWidth = graphics.PreferredBackBufferWidth;
            int oldHeight = graphics.PreferredBackBufferHeight;
        
            Point windowSize = GetWindowSize();
            graphics.PreferredBackBufferWidth = windowSize.X;
            graphics.PreferredBackBufferHeight = windowSize.Y;
            graphics.ApplyChanges();

            if (windowSize.X != 0 && windowSize.Y != 0)
            {
                int newWidth = windowSize.X;
                int newHeight = windowSize.Y;

                int widthDiff = oldWidth - newWidth;
                int heightDiff = oldHeight - newHeight;

                int xShift = widthDiff / 2;
                int yShift = heightDiff;

                Position.X += xShift;
                Position.Y += yShift;
            }
        }
    }
}