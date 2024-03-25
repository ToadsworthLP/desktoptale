using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Desktoptale.Characters
{
    public class CharacterCreationContext
    {
        public CharacterCreationContext(GraphicsDeviceManager graphics, GameWindow window, SpriteBatch spriteBatch, InputManager inputManager, MonitorManager monitorManager)
        {
            Graphics = graphics;
            Window = window;
            SpriteBatch = spriteBatch;
            InputManager = inputManager;
            MonitorManager = monitorManager;
        }

        public GraphicsDeviceManager Graphics { get; private set; }
        public GameWindow Window { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }
        public InputManager InputManager { get; private set; }
        public MonitorManager MonitorManager { get; private set; }
    }
}