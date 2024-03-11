namespace Desktoptale.Characters
{
    public class ExternalCharacterDefinition
    {
        public string Name { get; set; }
        public ExternalCharacterStateDefinition Idle { get; set; }
        public ExternalCharacterStateDefinition Walk { get; set; }
        public ExternalCharacterStateDefinition Run { get; set; }
        
        public class ExternalCharacterStateDefinition
        {
            public ExternalCharacterSpriteDefinition Sprites { get; set; }
            public double FrameRate { get; set; }
            public int FrameCount { get; set; }
            public bool Loop { get; set; }
            public int StartFrame { get; set; }
        }
        
        public class ExternalCharacterSpriteDefinition
        {
            public string Up { get; set; }
            public string Down { get; set; }
            public string Left { get; set; }
            public string Right { get; set; }
        }
    }
}