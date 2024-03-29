using System;
using Desktoptale.Characters;
using Microsoft.Xna.Framework;

namespace Desktoptale
{
    public class Preset
    {
        public const int FILE_FORMAT_VERSION = 1;

        public int Version { get; set; }
        public string Character { get; set; }
        public float XPosition { get; set; } = -1;
        public float YPosition { get; set; } = -1;
        public int Scale { get; set; }
        public bool IdleRoaming { get; set; }
        public bool UnfocusedInput { get; set; }
        public string Window { get; set; }

        public Preset()
        {
            Version  = FILE_FORMAT_VERSION;
        }
        
        public Preset(Settings settings)
        {
            Version  = FILE_FORMAT_VERSION;
            Character = settings.Character;
            Scale = settings.Scale;
            IdleRoaming = settings.IdleRoaming;
            UnfocusedInput = settings.UnfocusedInput;
            Window = settings.Window;
        }

        public Preset(CharacterProperties properties, Func<CharacterType, string> idResolver)
        {
            Character = idResolver.Invoke(properties.Type);
            XPosition = properties.Position.X;
            YPosition = properties.Position.Y;
            Scale = (int)properties.Scale.X;
            IdleRoaming = properties.IdleRoamingEnabled;
            UnfocusedInput = properties.UnfocusedInputEnabled;
            Window = properties.StayInsideWindow == null ? "" : properties.StayInsideWindow.ProcessName;
        }
        
        public void Apply(Settings settings)
        {
            settings.Character = Character;
            settings.Scale = Scale;
            settings.IdleRoaming = IdleRoaming;
            settings.UnfocusedInput = UnfocusedInput;
            settings.Window = Window;
            
            settings.Validate();
        }

        public CharacterProperties ToCharacterProperties(Func<string, CharacterType> typeResolver, Func<string, WindowInfo> windowResolver)
        {
            CharacterProperties result = new CharacterProperties(
                typeResolver.Invoke(Character),
                new Vector2(XPosition, YPosition),
                new Vector2(Scale),
                IdleRoaming,
                UnfocusedInput,
                windowResolver.Invoke(Window)
            );
            
            return result;
        }
    }
}