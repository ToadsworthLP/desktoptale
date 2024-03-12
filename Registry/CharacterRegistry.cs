using Desktoptale.Characters;

namespace Desktoptale.Registry
{
    public class CharacterRegistry : IntKeyRegistry<CharacterType>
    {
        public static readonly CharacterType FRISK = new CharacterType("Frisk", "Undertale", ctx => new Frisk(ctx));
        public static readonly CharacterType TORIEL = new CharacterType("Toriel", "Undertale", ctx => new Toriel(ctx));
        public static readonly CharacterType PAPYRUS = new CharacterType("Papyrus", "Undertale", ctx => new Papyrus(ctx));
        public static readonly CharacterType SANS = new CharacterType("Sans", "Undertale", ctx => new Sans(ctx));
        public static readonly CharacterType UNDYNE = new CharacterType("Undyne", "Undertale", ctx => new Undyne(ctx));
        
        public static readonly CharacterType CLOVER = new CharacterType("Clover", "Undertale Yellow", ctx => new Clover(ctx));
        public static readonly CharacterType DALV = new CharacterType("Dalv", "Undertale Yellow", ctx => new Dalv(ctx));
        public static readonly CharacterType MARTLET = new CharacterType("Martlet", "Undertale Yellow", ctx => new Martlet(ctx));
        public static readonly CharacterType STARLO = new CharacterType("Starlo", "Undertale Yellow", ctx => new Starlo(ctx));
        public static readonly CharacterType CEROBA = new CharacterType("Ceroba", "Undertale Yellow", ctx => new Ceroba(ctx));
        public static readonly CharacterType AXIS = new CharacterType("Axis", "Undertale Yellow", ctx => new Axis(ctx));
        public static readonly CharacterType KANAKO = new CharacterType("Kanako", "Undertale Yellow", ctx => new Kanako(ctx));
        
        public CharacterRegistry()
        {
            Add(FRISK);
            Add(TORIEL);
            Add(PAPYRUS);
            Add(SANS);
            Add(UNDYNE);
            
            Add(CLOVER);
            Add(DALV);
            Add(MARTLET);
            Add(STARLO);
            Add(CEROBA);
            Add(AXIS);
            Add(KANAKO);
        }
    }
}