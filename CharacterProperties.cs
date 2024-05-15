using Desktoptale.Characters;
using Desktoptale.Registry;
using Microsoft.Xna.Framework;

namespace Desktoptale
{
    public class CharacterProperties
    {
        public CharacterType Type { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Scale { get; set; }
        public bool IdleRoamingEnabled { get; set; }
        public bool UnfocusedInputEnabled { get; set; }
        public WindowInfo StayInsideWindow { get; set; }
        public Party Party { get; set; }

        public CharacterProperties()
        {
            Type = CharacterRegistry.FRISK;
            Position = Vector2.Zero;
            Scale = new Vector2(2, 2);
            IdleRoamingEnabled = true;
            UnfocusedInputEnabled = false;
            StayInsideWindow = null;
            Party = null;
        }
        
        public CharacterProperties(CharacterType type, Vector2 position, Vector2 scale, bool idleRoamingEnabled, bool unfocusedInputEnabled, WindowInfo stayInsideWindow = null, Party party = null)
        {
            Type = type;
            Position = position;
            Scale = scale;
            IdleRoamingEnabled = idleRoamingEnabled;
            UnfocusedInputEnabled = unfocusedInputEnabled;
            StayInsideWindow = stayInsideWindow;
            Party = party;
        }
        
        public CharacterProperties(CharacterProperties source)
        {
            Type = source.Type;
            Position = source.Position;
            Scale = source.Scale;
            IdleRoamingEnabled = source.IdleRoamingEnabled;
            UnfocusedInputEnabled = source.UnfocusedInputEnabled;
            StayInsideWindow = source.StayInsideWindow;
            Party = source.Party;
        }
    }
}