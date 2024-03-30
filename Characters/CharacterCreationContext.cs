using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Desktoptale.Characters
{
    public class CharacterCreationContext
    {
        public CharacterCreationContext(CharacterProperties properties, SpriteBatch spriteBatch, InputManager inputManager, MonitorManager monitorManager, WindowTracker windowTracker)
        {
            Properties = properties;
            SpriteBatch = spriteBatch;
            InputManager = inputManager;
            MonitorManager = monitorManager;
            WindowTracker = windowTracker;
        }
        
        public CharacterProperties Properties { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }
        public InputManager InputManager { get; private set; }
        public MonitorManager MonitorManager { get; private set; }
        public WindowTracker WindowTracker { get; private set; }
    }
}