using System;
using Desktoptale.Messages;
using Desktoptale.Messaging;
using Desktoptale.States.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Desktoptale.Characters
{
    public abstract class Character : ICharacter
    {
        public CharacterProperties Properties => properties;

        public Vector2 Position
        {
            get => properties.Position;
            set => properties.Position = value;
        }
        
        public Vector2 Velocity { get; set; }
        public Vector2 Scale
        {
            get => properties.Scale;
            set => properties.Scale = value;
        }

        public bool IsActive => focused || UnfocusedMovementEnabled;
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
        public bool IdleRoamingEnabled
        {
            get => properties.IdleRoamingEnabled;
            set => properties.IdleRoamingEnabled = value;
        }
        
        public bool UnfocusedMovementEnabled
        {
            get => properties.UnfocusedInputEnabled;
            set => properties.UnfocusedInputEnabled = value;
        }

        public bool EnabledAutoOrientation { get; set; } = true;

        protected virtual IState<Character> InitialState => IdleState;
        
        protected MonitorManager MonitorManager;

        private CharacterProperties properties;

        private Rectangle? boundary;

        private bool focused;
        private bool enableDragging = true;
        private bool dragging;
        private Vector2 previousPosition;
        private bool firstFrame = true;
        private Point maxFrameSize;
        private float depthOffset;
        
        private Subscription scaleChangeRequestedSubscription;
        private Subscription idleMovementChangeRequestedSubscription;
        private Subscription unfocusedMovementChangedSubscription;
        private Subscription boundaryChangeSubscription;
        private Subscription contextMenuStateChangeSubscription;
        private Subscription focusCharacterSubscription;
        
        public Character(CharacterCreationContext characterCreationContext)
        {
            this.properties = characterCreationContext.Properties;
            this.MonitorManager = characterCreationContext.MonitorManager;
            this.InputManager = characterCreationContext.InputManager;
        }

        public void UpdateSprite(IAnimatedSprite sprite)
        {
            CurrentSprite?.Stop();
            CurrentSprite = sprite;
        }

        public virtual void Initialize()
        {
            Random random = new Random(GetHashCode());
            depthOffset = (float)((random.NextDouble() % 0.0001d) - 0.00005d);
            
            scaleChangeRequestedSubscription = MessageBus.Subscribe<ScaleChangeRequestedMessage>(OnScaleChangeRequestedMessage);
            idleMovementChangeRequestedSubscription = MessageBus.Subscribe<IdleRoamingChangedMessage>(OnIdleRoamingChangedMessage);
            unfocusedMovementChangedSubscription = MessageBus.Subscribe<UnfocusedMovementChangedMessage>(OnUnfocusedMovementChangedMessage);
            boundaryChangeSubscription = MessageBus.Subscribe<UpdateBoundaryMessage>(OnBoundaryUpdateMessage);
            contextMenuStateChangeSubscription = MessageBus.Subscribe<ContextMenuStateChangedMessage>(OnContextMenuStateChangedMessage);
            focusCharacterSubscription = MessageBus.Subscribe<FocusCharacterMessage>(OnFocusCharacterMessage);

            IdleState = new IdleState();
            WalkState = new WalkState(90f, true);
            RunState = new RunState(180f, true);
            RandomMovementState = new RandomMovementState(90);
            RandomMovementWaitState = new RandomMovementWaitState();
        
            StateMachine = new StateMachine<Character>(this, InitialState);
            StateMachine.StateChanged += (state, newState) => UpdateOrientation();
        }

        public virtual void LoadContent(ContentManager contentManager) { }
    
        public virtual void Update(GameTime gameTime)
        {
            if (firstFrame)
            {
                maxFrameSize = GetMaximumFrameSize();
                UpdatePhysicsProperties();
                PreventLeavingScreenArea();
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

            Position += new Vector2((int)Math.Round(Velocity.X), (int)Math.Round(Velocity.Y));
            
            if (enableDragging)
            {
                DragCharacter();
            }
            
            if (previousPosition != Position)
            {
                UpdatePhysicsProperties();
            }

            Vector2 beforeCorrection = Position;
            PreventLeavingScreenArea();
            
            if (beforeCorrection != Position)
            {
                UpdatePhysicsProperties();
            }
            
            CurrentSprite.Update(gameTime); ;
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2(CurrentSprite.FrameSize.X / 2f, CurrentSprite.FrameSize.Y / 2f);
            CurrentSprite.Draw(spriteBatch, Position, Color.White, 0, origin, Scale, SpriteEffects.None, Depth);
        }

        public virtual void Dispose()
        {
            MessageBus.Unsubscribe(scaleChangeRequestedSubscription);
            MessageBus.Unsubscribe(idleMovementChangeRequestedSubscription);
            MessageBus.Unsubscribe(unfocusedMovementChangedSubscription);
            MessageBus.Unsubscribe(boundaryChangeSubscription);
            MessageBus.Unsubscribe(contextMenuStateChangeSubscription);
            MessageBus.Unsubscribe(focusCharacterSubscription);
        }

        private void UpdatePhysicsProperties()
        {
            HitBox = new Rectangle((int)(Position.X - maxFrameSize.X / 2f), (int)(Position.Y - maxFrameSize.Y / 2f), maxFrameSize.X, maxFrameSize.Y);
            
            Depth = MathF.Clamp(1 - ((Position.Y + maxFrameSize.Y / 2f) / MonitorManager.BoundingRectangle.Height), 0 ,1);
            Depth += depthOffset;
            if (Depth < 0)
            {
                Depth += Math.Abs(depthOffset) * 2;
            } 
            else if (Depth > 1)
            {
                Depth -= Math.Abs(depthOffset) * 2;
            }
        }

        private void OnScaleChangeRequestedMessage(ScaleChangeRequestedMessage message)
        {
            if(message.Target != this) return;
            
            Scale = new Vector2(message.ScaleFactor, message.ScaleFactor);
            maxFrameSize = GetMaximumFrameSize();
            UpdatePhysicsProperties();
            previousPosition = Vector2.Zero;
            PreventLeavingScreenArea();
            UpdatePhysicsProperties();
        }
        
        private void OnIdleRoamingChangedMessage(IdleRoamingChangedMessage message)
        {
            if(message.Target != this) return;
            
            IdleRoamingEnabled = message.Enabled;
        }

        private void OnUnfocusedMovementChangedMessage(UnfocusedMovementChangedMessage message)
        {
            if(message.Target != this) return;
            
            UnfocusedMovementEnabled = message.Enabled;
        }

        private void OnFocusCharacterMessage(FocusCharacterMessage message)
        {
            focused = message.Character == this;
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
        
        public void OnLeftClicked()
        {
            MessageBus.Send(new FocusCharacterMessage() { Character = this });
            dragging = true;
        }

        public void OnRightClicked()
        {
            MessageBus.Send(new OpenContextMenuRequestedMessage() { Target = this});
        }

        private void DragCharacter()
        {
            if (!InputManager.LeftClickPressed)
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
                Vector2 position = Position;
                if (position.X < boundary.Value.X) position.X = boundary.Value.X;
                if (position.X + scaledWidth > boundary.Value.X + boundary.Value.Width) position.X = boundary.Value.X + boundary.Value.Width - scaledWidth;
                if (position.Y < boundary.Value.Y) position.Y = boundary.Value.Y;
                if (position.Y + scaledHeight > boundary.Value.Y + boundary.Value.Height) position.Y = boundary.Value.Y + boundary.Value.Height - scaledHeight;
                Position = position;
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
            
            Vector2 corrected = MonitorManager.GetClosestVisiblePoint(newPosition);
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
            if(message.Target != this) return;
            
            boundary = message.Boundary;
        }

        private void OnContextMenuStateChangedMessage(ContextMenuStateChangedMessage message)
        {
            if(message.Target != this) return;
            
            enableDragging = !message.Open;
        }
    }
}