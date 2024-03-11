using System;

namespace Desktoptale.Characters
{
    public class CharacterType
    {
        public string Name { get; set; }
        public Func<CharacterCreationContext, Character> FactoryFunction { get; set; }

        public CharacterType(string name, Func<CharacterCreationContext, Character> factoryFunction)
        {
            Name = name;
            FactoryFunction = factoryFunction;
        }
    }
}