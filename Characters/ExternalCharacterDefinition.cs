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
            public ExternalCharacterSpriteDefinition Up { get; set; }
            public ExternalCharacterSpriteDefinition Down { get; set; }
            public ExternalCharacterSpriteDefinition Left { get; set; }
            public ExternalCharacterSpriteDefinition Right { get; set; }
        }
        
        public class ExternalCharacterSpriteDefinition
        {
            public string Sprite { get; set; }
            public double FrameRate { get; set; }
            public int FrameCount { get; set; }
            public int StartFrame { get; set; }
            public bool Loop { get; set; }
        }
    }
}