using System;

namespace Desktoptale.Characters
{
    public class CharacterType
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public float WalkSpeed { get; set; } = 90f;
        public float RunSpeed { get; set; } = 180f;
        public Func<CharacterCreationContext, Character> FactoryFunction { get; set; }

        public CharacterType(string name, string category, Func<CharacterCreationContext, Character> factoryFunction)
        {
            Name = name;
            Category = category;
            FactoryFunction = factoryFunction;
        }

        public override string ToString()
        {
            return $"{Category}/{Name}";
        }
    }
}