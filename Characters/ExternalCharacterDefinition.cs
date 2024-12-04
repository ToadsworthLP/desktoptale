namespace Desktoptale.Characters
{
    public class ExternalCharacterDefinition
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public bool? Hidden { get; set; }
        public bool? Override { get; set; }
        public float? WalkSpeed { get; set; }
        public float? RunSpeed { get; set; }
        public bool? Teleport { get; set; }
        public ExternalCharacterStateDefinition Idle { get; set; }
        public ExternalCharacterStateDefinition Walk { get; set; }
        public ExternalCharacterStateDefinition Run { get; set; }
        public ExternalCharacterStateDefinition Drag { get; set; }
        public ExternalCharacterStateDefinition Action { get; set; }
        public ExternalCharacterStateDefinition Spawn { get; set; }

        public ExternalCharacterStateDefinition Disappear { get; set; }
        public ExternalCharacterStateDefinition Appear { get; set; }
        
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
            public int FrameCount { get; set; } = 1;
            public int StartFrame { get; set; } = 0;
            public bool Loop { get; set; } = true;
            public int LoopPoint { get; set; } = 0;
        }
    }
}