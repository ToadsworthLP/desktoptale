﻿using Desktoptale.Characters;
using Microsoft.Xna.Framework;

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
        public static readonly CharacterType ASRIEL = new CharacterType("Asriel", "Undertale", ctx => new StandardCharacter(ctx, "Asriel", false, 4, 4, 2));
        public static readonly CharacterType CHARA = new CharacterType("Chara", "Undertale", ctx => new StandardCharacter(ctx, "Chara", false, 4, 4, 2));
        
        public static readonly CharacterType CLOVER = new CharacterType("Clover", "Undertale Yellow", ctx => new Clover(ctx));
        public static readonly CharacterType CLOVER_VENGEANCE = new CharacterType("Clover (Vengeance)", "Undertale Yellow", ctx => new CloverVengeance(ctx));
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
        public static readonly CharacterType BERDLEY = new CharacterType("Berdley", "Deltarune", ctx => new StandardCharacter(ctx, "Berdley", true));
        public static readonly CharacterType BERDLEY_DW = new CharacterType("Berdley (Dark World)", "Deltarune", ctx => new StandardCharacter(ctx, "BerdleyDW", true));

        public static readonly CharacterType RED_SOUL = new CharacterType("Red Soul", "Souls", ctx => new Soul(ctx, new Color(1f, 0f, 0f)));
        public static readonly CharacterType CYAN_SOUL = new CharacterType("Cyan Soul", "Souls", ctx => new Soul(ctx, new Color(0.2588235294f, 0.9882352941f, 1f)));
        public static readonly CharacterType ORANGE_SOUL = new CharacterType("Orange Soul", "Souls", ctx => new Soul(ctx, new Color(0.9882352941f, 0.6509803922f, 0f)));
        public static readonly CharacterType BLUE_SOUL = new CharacterType("Blue Soul", "Souls", ctx => new Soul(ctx, new Color(0f, 0.2352941176f, 1f)));
        public static readonly CharacterType PURPLE_SOUL = new CharacterType("Purple Soul", "Souls", ctx => new Soul(ctx, new Color(0.8352941176f, 0.2078431373f, 0.8509803922f)));
        public static readonly CharacterType GREEN_SOUL = new CharacterType("Green Soul", "Souls", ctx => new Soul(ctx, new Color(0f, 0.7529411765f, 0f)));
        public static readonly CharacterType YELLOW_SOUL = new CharacterType("Yellow Soul", "Souls", ctx => new Soul(ctx, new Color(1f, 1f, 0f)));
        public static readonly CharacterType MONSTER_SOUL = new CharacterType("Monster Soul", "Souls", ctx => new Soul(ctx, new Color(1f, 1f, 1f), true));
        
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
            Add(ASRIEL);
            Add(CHARA);
            
            Add(CLOVER);
            Add(CLOVER_VENGEANCE);
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
            Add(BERDLEY);
            Add(BERDLEY_DW);

            Add(RED_SOUL);
            Add(CYAN_SOUL);
            Add(ORANGE_SOUL);
            Add(BLUE_SOUL);
            Add(PURPLE_SOUL);
            Add(GREEN_SOUL);
            Add(YELLOW_SOUL);
            Add(MONSTER_SOUL);
        }
    }
}