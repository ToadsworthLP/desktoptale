using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Desktoptale;

public class InputManager
{
    public InputManager(Game game)
    {
        this.game = game;
    }

    public Vector2 DirectionalInput { get; private set; }
    public Point PointerPosition { get; private set; }
    public bool LeftClickPressed { get; private set; }
    public bool LeftClickJustPressed { get; private set; }
    public bool RightClickPressed { get; private set; }
    public bool RightClickJustPressed { get; private set; }

    private bool previousFrameLeftClick, previousFrameRightClick;
    private Game game;

    public void Update()
    {
        UpdateKeyboardInput();
        UpdateMouseInput();
    }
    
    private void UpdateKeyboardInput()
    {
        if (!game.IsActive) DirectionalInput = Vector2.Zero;
        
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

        if(input.LengthSquared() > float.Epsilon) input.Normalize();
        DirectionalInput = input;
    }

    private void UpdateMouseInput()
    {
        previousFrameLeftClick = LeftClickPressed;
        previousFrameRightClick = RightClickPressed;
        
        MouseState mouseState = Mouse.GetState();
        
        LeftClickPressed = mouseState.LeftButton == ButtonState.Pressed;
        if (LeftClickPressed) LeftClickJustPressed = !previousFrameLeftClick;
        
        RightClickPressed = mouseState.RightButton == ButtonState.Pressed;
        if (RightClickPressed) RightClickJustPressed = !previousFrameRightClick;

        PointerPosition = mouseState.Position;
    }
}