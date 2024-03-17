using Desktoptale.Characters;

namespace Desktoptale.Registry
{
    public class CharacterRegistry : Registry<CharacterType>
    {
        public static readonly CharacterType FRISK = new CharacterType("Frisk", "Undertale", ctx => new Frisk(ctx));
        public static readonly CharacterType FLOWEY = new CharacterType("Flowey", "Undertale", ctx => new Flowey(ctx));
        public static readonly CharacterType TORIEL = new CharacterType("Toriel", "Undertale", ctx => new StandardCharacter(ctx, "Toriel", false));
        public static readonly CharacterType PAPYRUS = new CharacterType("Papyrus", "Undertale", ctx => new StandardCharacter(ctx, "Papyrus", true));
        public static readonly CharacterType SANS = new CharacterType("Sans", "Undertale", ctx => new StandardCharacter(ctx, "Sans", false));
        public static readonly CharacterType UNDYNE = new CharacterType("Undyne", "Undertale", ctx => new Undyne(ctx));
        public static readonly CharacterType UNDYNE_ARMORED = new CharacterType("Undyne (Armored)", "Undertale", ctx => new StandardCharacter(ctx, "UndyneArmored", true));
        public static readonly CharacterType ALPHYS = new CharacterType("Alphys", "Undertale", ctx => new Alphys(ctx));
        public static readonly CharacterType ASGORE = new CharacterType("Asgore", "Undertale", ctx => new Asgore(ctx));
        
        public static readonly CharacterType CLOVER = new CharacterType("Clover", "Undertale Yellow", ctx => new Clover(ctx));
        public static readonly CharacterType DALV = new CharacterType("Dalv", "Undertale Yellow", ctx => new Dalv(ctx));
        public static readonly CharacterType MARTLET = new CharacterType("Martlet", "Undertale Yellow", ctx => new Martlet(ctx));
        public static readonly CharacterType ED = new CharacterType("Ed", "Undertale Yellow", ctx => new StandardCharacter(ctx, "Ed", false));
        public static readonly CharacterType MORAY = new CharacterType("Moray", "Undertale Yellow", ctx => new Moray(ctx));
        public static readonly CharacterType ACE = new CharacterType("Ace", "Undertale Yellow", ctx => new StandardCharacter(ctx, "Ace", false));
        public static readonly CharacterType MOOCH = new CharacterType("Mooch", "Undertale Yellow", ctx => new StandardCharacter(ctx, "Mooch", true));
        public static readonly CharacterType STARLO = new CharacterType("Starlo", "Undertale Yellow", ctx => new Starlo(ctx));
        public static readonly CharacterType CEROBA = new CharacterType("Ceroba", "Undertale Yellow", ctx => new Ceroba(ctx));
        public static readonly CharacterType KANAKO = new CharacterType("Kanako", "Undertale Yellow", ctx => new Kanako(ctx));
        public static readonly CharacterType CHUJIN = new CharacterType("Chujin", "Undertale Yellow", ctx => new Chujin(ctx));
        public static readonly CharacterType AXIS = new CharacterType("Axis", "Undertale Yellow", ctx => new Axis(ctx));
        
        public static readonly CharacterType KRIS = new CharacterType("Kris", "Deltarune", ctx => new StandardCharacter(ctx, "Kris", false));
        public static readonly CharacterType KRIS_DW = new CharacterType("Kris (Dark World)", "Deltarune", ctx => new StandardCharacter(ctx, "KrisDW", true));
        public static readonly CharacterType SUSIE = new CharacterType("Susie", "Deltarune", ctx => new StandardCharacter(ctx, "Susie", false));
        public static readonly CharacterType SUSIE_DW = new CharacterType("Susie (Dark World)", "Deltarune", ctx => new StandardCharacter(ctx, "SusieDW", false));
        public static readonly CharacterType RALSEI = new CharacterType("Ralsei", "Deltarune", ctx => new StandardCharacter(ctx, "Ralsei", false));
        public static readonly CharacterType RALSEI_HATTED = new CharacterType("Ralsei (Hatted)", "Deltarune", ctx => new StandardCharacter(ctx, "RalseiHat", false));
        public static readonly CharacterType NOELLE = new CharacterType("Noelle", "Deltarune", ctx => new StandardCharacter(ctx, "Noelle", false));
        public static readonly CharacterType NOELLE_DW = new CharacterType("Noelle (Dark World)", "Deltarune", ctx => new StandardCharacter(ctx, "NoelleDW", false));
        
        public CharacterRegistry()
        {
            Add(FRISK);
            Add(FLOWEY);
            Add(TORIEL);
            Add(PAPYRUS);
            Add(SANS);
            Add(UNDYNE);
            Add(UNDYNE_ARMORED);
            Add(ALPHYS);
            Add(ASGORE);
            
            Add(CLOVER);
            Add(DALV);
            Add(MARTLET);
            Add(ED);
            Add(MORAY);
            Add(ACE);
            Add(MOOCH);
            Add(STARLO);
            Add(CEROBA);
            Add(CHUJIN);
            Add(KANAKO);
            Add(AXIS);

            Add(KRIS);
            Add(KRIS_DW);
            Add(SUSIE);
            Add(SUSIE_DW);
            Add(RALSEI_HATTED);
            Add(RALSEI);
            Add(NOELLE);
            Add(NOELLE_DW);
        }
    }
}