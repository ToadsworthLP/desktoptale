using System;
using Desktoptale.Messages;
using Desktoptale.Messaging;
using Desktoptale.States.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Desktoptale.Characters
{
    public abstract class Character : IGameObject, ICollider
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Scale;

        public Rectangle HitBox { get; private set; }
        public float Depth { get; private set; }

        public Orientation Orientation = Orientation.Down;
    
        public InputManager InputManager { get; }
        public StateMachine<Character> StateMachine { get; protected set; }
        public IState<Character> IdleState { get; protected set; }
        public IState<Character> WalkState { get; protected set; }
        public IState<Character> RunState { get; protected set; }
        public IState<Character> RandomMovementState { get; protected set; }
        public IState<Character> RandomMovementWaitState { get; protected set; }
    
        public IAnimatedSprite IdleSprite { get; set; }
        public IAnimatedSprite WalkSprite { get; set; }
        public IAnimatedSprite RunSprite { get; set; }
        public IAnimatedSprite CurrentSprite { get; set; }
        public bool IsBeingDragged => dragging;
        public bool EnableIdleMovement { get; set; }
        public bool EnabledAutoOrientation { get; set; } = true;

        protected virtual IState<Character> InitialState => IdleState;

        protected GameWindow window;
        protected GraphicsDeviceManager graphics;
        protected MonitorManager monitorManager;

        private Rectangle? boundary;

        private bool enableDragging = true;
        private bool dragging;
        private Vector2 previousPosition;
        private bool firstFrame = true;
        private Point maxFrameSize;
        
        private Subscription scaleChangeRequestedSubscription;
        private Subscription idleMovementChangeRequestedSubscription;
        private Subscription boundaryChangeSubscription;
        private Subscription contextMenuStateChangeSubscription;
        
        public Character(CharacterCreationContext characterCreationContext)
        {
            this.monitorManager = characterCreationContext.MonitorManager;
            this.InputManager = characterCreationContext.InputManager;
            this.graphics = characterCreationContext.Graphics;
            this.window = characterCreationContext.Window;
        }

        public void UpdateSprite(IAnimatedSprite sprite)
        {
            CurrentSprite?.Stop();
            CurrentSprite = sprite;
        }

        public virtual void Initialize()
        {
            scaleChangeRequestedSubscription = MessageBus.Subscribe<ScaleChangeRequestedMessage>(OnScaleChangeRequestedMessage);
            idleMovementChangeRequestedSubscription = MessageBus.Subscribe<IdleMovementChangeRequestedMessage>(OnIdleMovementChangeRequestedMessage);
            boundaryChangeSubscription = MessageBus.Subscribe<UpdateBoundaryMessage>(OnBoundaryUpdateMessage);
            contextMenuStateChangeSubscription = MessageBus.Subscribe<ContextMenuStateChangedMessage>(OnContextMenuStateChangedMessage);

            IdleState = new IdleState();
            WalkState = new WalkState(90f, true);
            RunState = new RunState(180f, true);
            RandomMovementState = new RandomMovementState(90);
            RandomMovementWaitState = new RandomMovementWaitState();
        
            StateMachine = new StateMachine<Character>(this, InitialState);
            StateMachine.StateChanged += (state, newState) => UpdateOrientation();
            
            InputManager.GrabFocus();
        }

        public virtual void LoadContent(ContentManager contentManager) { }
    
        public virtual void Update(GameTime gameTime)
        {
            if (firstFrame)
            {
                PreventLeavingScreenArea();
                maxFrameSize = GetMaximumFrameSize();
                UpdateHitbox();
                firstFrame = false;
            }
            
            previousPosition = Position;
            
            StateMachine.Update(gameTime);

            if (EnabledAutoOrientation)
            {
                UpdateOrientation();
            }
            
            if (CurrentSprite is OrientedAnimatedSprite sprite)
            {
                sprite.Orientation = Orientation;
            }
            
            Position.X += (int)Math.Round(Velocity.X);
            Position.Y += (int)Math.Round(Velocity.Y);

            
            if (enableDragging)
            {
                DragCharacter();
            }
            
            if (previousPosition != Position)
            {
                UpdateHitbox();
            }

            Vector2 beforeCorrection = Position;
            PreventLeavingScreenArea();
            
            if (beforeCorrection != Position)
            {
                UpdateHitbox();
            }
            
            CurrentSprite.Update(gameTime);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2(CurrentSprite.FrameSize.X / 2f, CurrentSprite.FrameSize.Y / 2f);
            CurrentSprite.Draw(spriteBatch, Position, Color.White, 0, origin, Scale, SpriteEffects.None, MathF.Clamp(Depth, 0, 1));
        }

        public virtual void Dispose()
        {
            MessageBus.Unsubscribe(scaleChangeRequestedSubscription);
            MessageBus.Unsubscribe(idleMovementChangeRequestedSubscription);
            MessageBus.Unsubscribe(boundaryChangeSubscription);
            MessageBus.Unsubscribe(contextMenuStateChangeSubscription);
        }

        private void UpdateHitbox()
        {
            HitBox = new Rectangle((int)(Position.X - maxFrameSize.X / 2f), (int)(Position.Y - maxFrameSize.Y / 2f), maxFrameSize.X, maxFrameSize.Y);
            Depth = (Position.Y + maxFrameSize.Y / 2f) / monitorManager.BoundingRectangle.Height;
        }

        private void OnScaleChangeRequestedMessage(ScaleChangeRequestedMessage message)
        {
            Scale = new Vector2(message.ScaleFactor, message.ScaleFactor);
            maxFrameSize = GetMaximumFrameSize();
            UpdateHitbox();
            previousPosition = Vector2.Zero;
            PreventLeavingScreenArea();
            UpdateHitbox();
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
                Orientation = updatedOrientation.Value;
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
                HitBox.Contains(InputManager.PointerPosition))
            {
                dragging = true;
            } else if (!InputManager.LeftClickPressed)
            {
                dragging = false;
            }

            if (dragging)
            {
                Position = InputManager.PointerPosition.ToVector2();
            }
        }

        private void PreventLeavingScreenArea()
        {
            if(previousPosition == Position) return;
            
            float scaledWidth = CurrentSprite.FrameSize.X * Scale.X;
            float scaledHeight = CurrentSprite.FrameSize.Y * Scale.Y;
            
            if (boundary.HasValue)
            {
                if (Position.X < boundary.Value.X) Position.X = boundary.Value.X;
                if (Position.X + scaledWidth > boundary.Value.X + boundary.Value.Width) Position.X = boundary.Value.X + boundary.Value.Width - scaledWidth;
                if (Position.Y < boundary.Value.Y) Position.Y = boundary.Value.Y;
                if (Position.Y + scaledHeight > boundary.Value.Y + boundary.Value.Height) Position.Y = boundary.Value.Y + boundary.Value.Height - scaledHeight;
            }
            else
            {
                Vector2 motion = Position - previousPosition;
                Vector2 previousBoundingRectMin = new Vector2(HitBox.Top, HitBox.Left);
                Vector2 previousBoundingRectMax = new Vector2(HitBox.Bottom, HitBox.Right);
                
                Vector2 topLeft = new Vector2(HitBox.Left, HitBox.Top);
                Vector2 topRight = new Vector2(HitBox.Right, HitBox.Top);
                Vector2 bottomLeft = new Vector2(HitBox.Left, HitBox.Bottom);
                Vector2 bottomRight = new Vector2(HitBox.Right, HitBox.Bottom);

                Vector2 correctionMotion = Vector2.Zero;
                AddCorrectionMotion(topLeft, previousBoundingRectMin, previousBoundingRectMax, ref correctionMotion);
                AddCorrectionMotion(topRight, previousBoundingRectMin, previousBoundingRectMax, ref correctionMotion);
                AddCorrectionMotion(bottomLeft, previousBoundingRectMin, previousBoundingRectMax, ref correctionMotion);
                AddCorrectionMotion(bottomRight, previousBoundingRectMin, previousBoundingRectMax, ref correctionMotion);
                
                Position = previousPosition + motion + correctionMotion;
            }
        }

        private void AddCorrectionMotion(Vector2 newPosition, Vector2 boundingRectMin, Vector2 boundingRectMax, ref Vector2 correctionMotion)
        {
            if ((Math.Abs(correctionMotion.X) > float.Epsilon && Math.Abs(correctionMotion.Y) > float.Epsilon) || ContainsInclusive(boundingRectMin, boundingRectMax, newPosition))
            {
                return;
            }
            
            Vector2 corrected = monitorManager.GetClosestVisiblePoint(newPosition);
            Vector2 motion = corrected - newPosition;
            if (Math.Abs(motion.X) > float.Epsilon) correctionMotion.X = motion.X;
            if (Math.Abs(motion.Y) > float.Epsilon) correctionMotion.Y = motion.Y;
        }

        private bool ContainsInclusive(Vector2 min, Vector2 max, Vector2 value)
        {
            return min.X <= value.X && value.X <= max.X && min.Y <= value.Y && value.Y <= max.Y;
        }
        
        private Point GetMaximumFrameSize()
        {
            Point size = new Point(0, 0);

            Point idleSize = IdleSprite is OrientedAnimatedSprite orientedIdle
                ? orientedIdle.GetMaximumFrameSize()
                : IdleSprite.FrameSize;
            
            size.X = Math.Max(size.X, idleSize.X);
            size.Y = Math.Max(size.Y, idleSize.Y);

            if (WalkSprite != null)
            {
                Point walkSize = WalkSprite is OrientedAnimatedSprite orientedWalk
                    ? orientedWalk.GetMaximumFrameSize()
                    : WalkSprite.FrameSize;
                
                size.X = Math.Max(size.X, walkSize.X);
                size.Y = Math.Max(size.Y, walkSize.Y);
            }

            if (RunSprite != null)
            {
                Point runSize = RunSprite is OrientedAnimatedSprite orientedRun
                    ? orientedRun.GetMaximumFrameSize()
                    : RunSprite.FrameSize;
                
                size.X = Math.Max(size.X, runSize.X);
                size.Y = Math.Max(size.Y, runSize.Y);
            }
            
            return new Point((int)(size.X * Scale.X), (int)(size.Y * Scale.Y));
        }

        private void OnBoundaryUpdateMessage(UpdateBoundaryMessage message)
        {
            boundary = message.Boundary;
        }

        private void OnContextMenuStateChangedMessage(ContextMenuStateChangedMessage message)
        {
            enableDragging = !message.Open;
        }
    }
}