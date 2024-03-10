using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Desktoptale
{
    public class CharacterCreationContext
    {
        public CharacterCreationContext(GraphicsDeviceManager graphics, GameWindow window, SpriteBatch spriteBatch, InputManager inputManager)
        {
            Graphics = graphics;
            Window = window;
            SpriteBatch = spriteBatch;
            InputManager = inputManager;
        }

        public GraphicsDeviceManager Graphics { get; private set; }
        public GameWindow Window { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }
        public InputManager InputManager { get; private set; }
    }
}