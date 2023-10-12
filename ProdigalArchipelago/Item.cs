using System.Collections.Generic;

namespace ProdigalArchipelago
{
    public enum Item
    {
        Gold1 = 1,
        Gold10 = 2,
        Gold20 = 3,
        Gold100 = 4,
        RustKnuckle = 5,
        DreadHand = 6,
        Lariat = 7,
        IronPick = 9,
        BlessedPick = 10,
        BoltAnchor = 11,
        LuckyBoots = 12,
        CleatedBoots = 13,
        AnchorGreaves = 14,
        BootsOfGraile = 15,
        WingedBoots = 16,
        OldRecipe = 17,
        RareSeeds = 18,
        HerbPounch = 19,
        CoinOfCrowl = 20,
        TidalGem = 21,
        LumenBeetle = 22,
        Coffee = 23,
        TatteredCape = 24,
        BallOfYarn = 25,
        SlimeSoap = 26,
        SerpentBracelet = 27,
        TheCarrotCake = 28,
        HuntingBow = 29,
        DownPillow = 30,
        GiantsMonocle = 31,
        ForbiddenBook = 32,
        KelpRolls = 33,
        BossKey = 34,
        HallowedKey = 35,
        RustyKey = 36,
        GatorKey = 37,
        BunnyKey = 38,
        NormalKey = 39,
        Toadstool = 40,
        FrozenThorn = 41,
        LuminousCrystal = 42,
        FuryHeart = 43,
        ShatteredSoul = 44,
        DrownedOre = 45,
        Shaedrite = 46,
        MiasmicExtract = 47,
        LostShipment = 48,
        BrokenSword = 49,
        CrestFragment = 50,
        CursedBones = 51,
        BoneKey = 52,
        SunsetPainting = 53,
        CursedPick = 55,
        DaemonsKey = 56,
        DivineKey = 62,
        PrisonKey = 63,
        StindlesMap = 64,
        Harmonica = 65,
        IceKey = 67,
        SilveredScarf = 68,
        HeartOre = 70,
        OldHairpin = 71,
        OraMoa = 72,
        EerieMask = 73,
        FrozenHeart = 74,
        RedCrystal = 75,
        EmpoweredHand = 76,
        FlareKnuckle = 77,
        WeaponChain = 78,
        ClimbingGear = 79,
        IllusionRing = 80,
        GolemRing = 81,
        MagnetRing = 82,
        SpikedRing = 83,
        HearthRing = 84,
        DaemonsRing = 85,
        KingsRing = 86,
        DustyKey = 87,
        LifeBlessing = 89,
        LightBlessing = 90,
        StormBlessing = 91,
        EarthBlessing = 92,
        WaterBlessing = 93,
        HerosSoul = 94,
        Bandana = 95,
        HolyRelic = 96,
        WeddingRing = 97,
        SilverMirror = 98,
        Painting = 99,
        ArchipelagoItem = 101,
        ProgressiveKnuckle = 102,
        ProgressiveHand = 103,
        ProgressivePick = 104,
        BoneyardKey = 105,
        TidalMinesKey = 106,
        CrocasinoKey = 107,
        HowlingBjergKey = 108,
        CastleVannKey = 109,
        MagmaHeartKey = 110,
        TimeOutKey = 111,
        LighthouseKey = 112,
        CrystalCavesKey = 113,
        HauntedHallKey = 114,
        SiskasWorkshopKey = 115,
        BackroomsKey = 116,
        PiratesPierKey = 117,
        BjergCastleKey = 118,
        SlownessTrap = 119,
        RustTrap = 120,
        ConfusionTrap = 121,
        DisarmingTrap = 122,
        LightTrap = 123,
        ZombieTrap = 124,
        ShadowTrap = 125,
    }

    public static class ItemExtension
    {
        public static bool Acquired(this Item item)
        {
            return GameMaster.GM.Save.Data.Inventory[(int)item].Acquired;
        }

        public static int Count(this Item item)
        {
            return GameMaster.GM.Save.Data.Inventory[(int)item].Count;
        }

        public static ItemDatabase.ItemData Data(this Item item)
        {
            return GameMaster.GM.ItemData.Database[(int)item];
        }

        public static bool IsKey(this Item item)
        {
            return (int)item >= 105 && (int)item <= 118;
        }

        public static int ToKeyID(this Item item)
        {
            return (int)item - 105;
        }

        public static Item NonProgressive(this Item item)
        {
            switch (item)
            {
                case Item.ProgressiveKnuckle:
                    if (Item.RustKnuckle.Acquired())
                        return Item.FlareKnuckle;
                    return Item.RustKnuckle;
                case Item.ProgressiveHand:
                    if (Item.DreadHand.Acquired())
                        return Item.EmpoweredHand;
                    return Item.DreadHand;
                case Item.ProgressivePick:
                    if (Item.BlessedPick.Acquired())
                        return Item.BoltAnchor;
                    if (Item.IronPick.Acquired())
                        return Item.BlessedPick;
                    return Item.IronPick;
                default:
                    return item;
            }
        }

        public static List<Item> AllBoots()
        {
            return new List<Item>
            {
                Item.LuckyBoots,
                Item.CleatedBoots,
                Item.AnchorGreaves,
                Item.BootsOfGraile,
                Item.WingedBoots,
            };
        }

        public static List<Item> AllColors()
        {
            return new List<Item>
            {
                Item.FuryHeart,
                Item.ShatteredSoul,
                Item.SunsetPainting,
                Item.FrozenHeart,
                Item.RedCrystal,
            };
        }

        public static List<Item> AllBlessings()
        {
            return new List<Item>
            {
                Item.LifeBlessing,
                Item.LightBlessing,
                Item.StormBlessing,
                Item.EarthBlessing,
                Item.WaterBlessing,
            };
        }

        public static List<Item> MajorItems()
        {
            return new List<Item>
            {
                Item.Lariat,
                Item.LuckyBoots,
                Item.CleatedBoots,
                Item.AnchorGreaves,
                Item.BootsOfGraile,
                Item.WingedBoots,
                Item.HallowedKey,
                Item.RustyKey,
                Item.FuryHeart,
                Item.ShatteredSoul,
                Item.Shaedrite,
                Item.DrownedOre,
                Item.MiasmicExtract,
                Item.BrokenSword,
                Item.CursedBones,
                Item.BoneKey,
                Item.SunsetPainting,
                Item.DaemonsKey,
                Item.StindlesMap,
                Item.Harmonica,
                Item.SilveredScarf,
                Item.FrozenHeart,
                Item.RedCrystal,
                Item.WeaponChain,
                Item.ClimbingGear,
                Item.IllusionRing,
                Item.GolemRing,
                Item.MagnetRing,
                Item.SpikedRing,
                Item.HearthRing,
                Item.DaemonsRing,
                Item.KingsRing,
                Item.DustyKey,
                Item.LifeBlessing,
                Item.LightBlessing,
                Item.StormBlessing,
                Item.EarthBlessing,
                Item.WaterBlessing,
                Item.Bandana,
                Item.ProgressiveKnuckle,
                Item.ProgressiveHand,
                Item.ProgressivePick,
            };
        }
    }
}