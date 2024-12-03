using System;

namespace Desktoptale.Characters
{
    public class CharacterType
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public int Order { get; set; } = 0;
        public bool Hidden { get; set; } = false;
        public float WalkSpeed { get; set; } = 90f;
        public float RunSpeed { get; set; } = 180f;
        public bool Teleport { get; set; } = false;
        public bool BuiltIn { get; set; } = true;

        public Func<CharacterCreationContext, Character> FactoryFunction { get; set; }

        public CharacterType(string name, string category, Func<CharacterCreationContext, Character> factoryFunction)
        {
            Name = name;
            Category = category;
            FactoryFunction = factoryFunction;
        }
        
        public CharacterType(string name, string category, Func<CharacterCreationContext, Character> factoryFunction, float walkSpeed = 90f, float runSpeed = 180f, bool teleport = false)
        {
            Name = name;
            Category = category;
            FactoryFunction = factoryFunction;
            WalkSpeed = walkSpeed;
            RunSpeed = runSpeed;
            Teleport = teleport;
        }

        public override string ToString()
        {
            return $"{Category}/{Name}";
        }
    }
}