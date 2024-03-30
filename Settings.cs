using CommandLine;

namespace Desktoptale
{
    public class Settings
    {
        [Value(0)]
        public string Preset { get; set; }
        
        [Option("character", HelpText = "Registry key of the character to use", Default = null)]
        public string Character { get; set; }

        [Option("scale", HelpText = "Scale of the character (must be greater than 1)", Default = 2)]
        public int Scale { get; set; } = 2;

        [Option("idle-roaming", HelpText = "Whether to enable the Idle Roaming option", Default = true)]
        public bool IdleRoaming { get; set; } = true;
        
        [Option("unfocused-input", HelpText = "Whether to enable the Unfocused Input option", Default = false)]
        public bool UnfocusedInput { get; set; }
        
        [Option("window", HelpText = "The window the character should stay in", Default = null)]
        public string Window { get; set; }
        
        [Option("print-registry-keys", HelpText = "Lists the registry keys of all currently available characters", Default = false)]
        public bool PrintRegistryKeys { get; set; }

        public void Validate()
        {
            if (Scale < 1) Scale = 1;
        }
    }
}