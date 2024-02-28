using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Desktoptale;

public class InputManager
{
    private Game game;

    public InputManager(Game game)
    {
        this.game = game;
    }

    public Vector2 DirectionalInput => GetKeyboardInputVector();

    private Vector2 GetKeyboardInputVector()
    {
        if (!game.IsActive) return Vector2.Zero;
        
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
        return input;
    }
}