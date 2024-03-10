using Desktoptale.Characters;

namespace Desktoptale.Registry
{
    public class CharacterRegistry : IntKeyRegistry<CharacterType>
    {
        public static readonly CharacterType CLOVER = new CharacterType("Clover", ctx => new Clover(ctx));
        public static readonly CharacterType MARTLET = new CharacterType("Martlet", ctx => new Martlet(ctx));
        public static readonly CharacterType STARLO = new CharacterType("Starlo", ctx => new Starlo(ctx));
        public static readonly CharacterType CEROBA = new CharacterType("Ceroba", ctx => new Ceroba(ctx));
        public static readonly CharacterType AXIS = new CharacterType("Axis", ctx => new Axis(ctx));

        public CharacterRegistry()
        {
            Add(CLOVER);
            Add(MARTLET);
            Add(STARLO);
            Add(CEROBA);
            Add(AXIS);
        }
    }
}