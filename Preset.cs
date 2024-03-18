namespace Desktoptale
{
    public class Preset
    {
        public const int FILE_FORMAT_VERSION = 0;

        public int Version { get; set; }
        public string Character { get; set; }
        public int Scale { get; set; }
        public bool IdleRoaming { get; set; }
        public bool UnfocusedInput { get; set; }
        public bool AlwaysOnTop { get; set; }
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
            AlwaysOnTop = settings.AlwaysOnTop;
            Window = settings.Window;
        }
        
        public void Apply(Settings settings)
        {
            settings.Character = Character;
            settings.Scale = Scale;
            settings.IdleRoaming = IdleRoaming;
            settings.UnfocusedInput = UnfocusedInput;
            settings.AlwaysOnTop = AlwaysOnTop;
            settings.Window = Window;
            
            settings.Validate();
        }
    }
}