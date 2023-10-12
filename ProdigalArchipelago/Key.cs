namespace ProdigalArchipelago
{
    public class Key
    {
        public int ID;
        public int Scene;
        public int Max;
        public string Name;
        public string AltName;

        public Key(int id, int scene, int max, string name, string altName)
        {
            ID = id;
            Scene = scene;
            Max = max;
            Name = name;
            AltName = altName;
        }

        public int Count
        {
            get
            {
                return GameMaster.GM.Save.Data.Inventory[ID + 105].Count;
            }

            set
            {
                GameMaster.GM.Save.Data.Inventory[ID + 105].Count = value;
            }
        }

        public static readonly Key Boneyard = new(0, 5, 1, "BONEYARD", "THE BONEYARD");
        public static readonly Key TidalMines = new(1, 12, 4, "TIDAL MINES", "THE TIDAL MINES");
        public static readonly Key Crocasino = new(2, 6, 2, "CROCASINO", "THE CROCASINO");
        public static readonly Key HowlingBjerg = new(3, 8, 1, "HOWLING BJERG", "THE HOWLING BJERG");
        public static readonly Key CastleVann = new(4, 10, 4, "CASTLE VANN", "CASTLE VANN");
        public static readonly Key MagmaHeart = new(5, 9, 2, "MAGMA HEART", "THE MAGMA HEART");
        public static readonly Key TimeOut = new(6, 13, 3, "TIME OUT", "TIME OUT");
        public static readonly Key Lighthouse = new(7, 11, 1, "LIGHTHOUSE", "THE LIGHTHOUSE");
        public static readonly Key CrystalCaves = new(8, 24, 3, "CRYSTAL CAVES", "THE CRYSTAL CAVES");
        public static readonly Key HauntedHall = new(9, 16, 2, "HAUNTED HALL", "THE HAUNTED HALL");
        public static readonly Key SiskasWorkshop = new(10, 19, 3, "SISKA'S WKSHP", "SISKA'S WORKSHOP");
        public static readonly Key Backrooms = new(11, 17, 2, "BACKROOMS", "THE BACKROOMS");
        public static readonly Key PiratesPier = new(12, 27, 5, "PIRATE'S PIER", "PIRATE'S PIER");
        public static readonly Key BjergCastle = new(13, 8, 1, "BJERG CASTLE", "BJERG CASTLE");

        public static readonly Key[] Keys = {
            Boneyard,
            TidalMines,
            Crocasino,
            HowlingBjerg,
            CastleVann,
            MagmaHeart,
            TimeOut,
            Lighthouse,
            CrystalCaves,
            HauntedHall,
            SiskasWorkshop,
            Backrooms,
            PiratesPier,
            BjergCastle,
        };
    }
}