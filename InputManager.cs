using Desktoptale.Messages;
using Desktoptale.Messaging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Desktoptale
{
    public class InputManager
    {
        public Vector2 DirectionalInput { get; private set; }
        public Vector2 RawDirectionalInput { get; private set; }
        public Point PointerPosition { get; private set; }
        public bool LeftClickPressed { get; private set; }
        public bool LeftClickJustPressed { get; private set; }
        public bool RightClickPressed { get; private set; }
        public bool RightClickJustPressed { get; private set; }
        public bool RunButtonPressed { get; private set; }

        private bool previousFrameLeftClick, previousFrameRightClick;
        private Game game;
        private GraphicsDevice graphics;

        private bool focused = true;
        private bool unfocusedMovementEnabled = false;
    
        public InputManager(Game game, GraphicsDevice graphics)
        {
            this.game = game;
            this.graphics = graphics;
            
            MessageBus.Subscribe<UnfocusedMovementChangeRequestedMessage>(OnUnfocusedMovementChangeRequestedMessage);
        }

        public void Update()
        {
            UpdateKeyboardInput();
            UpdateMouseInput();
        }

        public void GrabFocus()
        {
            focused = true;
        }
    
        private void UpdateKeyboardInput()
        {
            if (!unfocusedMovementEnabled && (!game.IsActive || !focused))
            {
                DirectionalInput = Vector2.Zero;
                RunButtonPressed = false;
                return;
            }
            
            KeyboardState keyboardState = Keyboard.GetState();

            Vector2 input = Vector2.Zero;
            
            if (keyboardState.IsKeyDown(Keys.W)) input.Y -= 1f;
            if (keyboardState.IsKeyDown(Keys.S)) input.Y += 1f;
            if (keyboardState.IsKeyDown(Keys.A)) input.X -= 1f;
            if (keyboardState.IsKeyDown(Keys.D)) input.X += 1f;
            
            if (keyboardState.IsKeyDown(Keys.Up)) input.Y -= 1f;
            if (keyboardState.IsKeyDown(Keys.Down)) input.Y += 1f;
            if (keyboardState.IsKeyDown(Keys.Left)) input.X -= 1f;
            if (keyboardState.IsKeyDown(Keys.Right)) input.X += 1f;

            RawDirectionalInput = new Vector2(input.X, input.Y);

            if(input.LengthSquared() > float.Epsilon) input.Normalize();
            DirectionalInput = input;

            RunButtonPressed = keyboardState.IsKeyDown(Keys.X) || keyboardState.IsKeyDown(Keys.LeftShift);
        }

        private void UpdateMouseInput()
        {
            // if (!game.IsActive)
            // {
            //     LeftClickPressed = false;
            //     LeftClickJustPressed = false;
            //     previousFrameLeftClick = false;
            //     RightClickPressed = false;
            //     RightClickJustPressed = false;
            //     previousFrameRightClick = false;
            //     return;
            // }
            
            previousFrameLeftClick = LeftClickPressed;
            previousFrameRightClick = RightClickPressed;
            
            MouseState mouseState = Mouse.GetState();
            
            LeftClickPressed = mouseState.LeftButton == ButtonState.Pressed;
            if (LeftClickPressed) LeftClickJustPressed = !previousFrameLeftClick;
            
            RightClickPressed = mouseState.RightButton == ButtonState.Pressed;
            if (RightClickPressed) RightClickJustPressed = !previousFrameRightClick;

            PointerPosition = mouseState.Position;

            if (LeftClickJustPressed || RightClickJustPressed)
            {
                focused = graphics.Viewport.Bounds.Contains(PointerPosition);
            }
            
            if (!graphics.Viewport.Bounds.Contains(PointerPosition))
            {
                RightClickPressed = false;
                RightClickJustPressed = false;
                previousFrameRightClick = false;
            }
        }
        
        private void OnUnfocusedMovementChangeRequestedMessage(UnfocusedMovementChangeRequestedMessage message)
        {
            unfocusedMovementEnabled = message.Enabled;
        }
    }
}