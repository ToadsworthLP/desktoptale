using Desktoptale.Characters;

namespace Desktoptale.Registry
{
    public class CharacterRegistry : IntKeyRegistry<CharacterType>
    {
        public static readonly CharacterType CLOVER = new CharacterType("Clover", "Undertale Yellow", ctx => new Clover(ctx));
        public static readonly CharacterType MARTLET = new CharacterType("Martlet", "Undertale Yellow", ctx => new Martlet(ctx));
        public static readonly CharacterType STARLO = new CharacterType("Starlo", "Undertale Yellow", ctx => new Starlo(ctx));
        public static readonly CharacterType CEROBA = new CharacterType("Ceroba", "Undertale Yellow", ctx => new Ceroba(ctx));
        public static readonly CharacterType AXIS = new CharacterType("Axis", "Undertale Yellow", ctx => new Axis(ctx));

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