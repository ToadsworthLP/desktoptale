using Desktoptale.Messages;
using Desktoptale.Messaging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Desktoptale
{
    public class InputManager
    {
        public Vector2 DirectionalInput { get; private set; }
        public Vector2 RawDirectionalInput { get; private set; }
        public Point PointerPosition { get; private set; }
        public Point VirtualScreenPointerPosition { get; private set; }
        public bool LeftClickPressed { get; private set; }
        public bool LeftClickJustPressed { get; private set; }
        public bool RightClickPressed { get; private set; }
        public bool RightClickJustPressed { get; private set; }
        public bool RunButtonPressed { get; private set; }
        public bool ShiftPressed { get; private set; }
        public bool CtrlPressed { get; private set; }
        public bool ActionButtonPressed { get; private set; }
        public bool InteractButtonPressed { get; private set; }
        public bool InteractButtonJustPressed { get; private set; }

        private bool previousFrameLeftClick, previousFrameRightClick, previousFrameInteractButtonPressed;
        private MonitorManager monitorManager;

        public InputManager(MonitorManager monitorManager)
        {
            this.monitorManager = monitorManager;
        }

        public void Update()
        {
            UpdateKeyboardInput();
            UpdateMouseInput();
        }
    
        private void UpdateKeyboardInput()
        {
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

            CtrlPressed = keyboardState.IsKeyDown(Keys.LeftControl) || keyboardState.IsKeyDown(Keys.RightControl);
            ShiftPressed = keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift);
            ActionButtonPressed = keyboardState.IsKeyDown(Keys.C);

            previousFrameInteractButtonPressed = InteractButtonPressed;
            
            InteractButtonPressed = keyboardState.IsKeyDown(Keys.Z) || keyboardState.IsKeyDown(Keys.Space);
            InteractButtonJustPressed = InteractButtonPressed && !previousFrameInteractButtonPressed;
        }

        private void UpdateMouseInput()
        {
            previousFrameLeftClick = LeftClickPressed;
            previousFrameRightClick = RightClickPressed;
            
            MouseState mouseState = Mouse.GetState();
            
            LeftClickPressed = mouseState.LeftButton == ButtonState.Pressed;
            LeftClickJustPressed = LeftClickPressed && !previousFrameLeftClick;
            
            RightClickPressed = mouseState.RightButton == ButtonState.Pressed;
            RightClickJustPressed = RightClickPressed && !previousFrameRightClick;

            PointerPosition = mouseState.Position;
            VirtualScreenPointerPosition = monitorManager.ToVirtualScreenCoordinates(PointerPosition.ToVector2()).ToPoint();
        }
    }
}