namespace ProdigalArchipelago;

public class Key(int id, int scene, int max, string name, string altName)
{
    public int ID = id;
    public int Scene = scene;
    public int Max = max;
    public string Name = name;
    public string AltName = altName;

    public int Count
    {
        get
        {
            return GameMaster.GM.Save.Data.Inventory[ID + 124].Count;
        }

        set
        {
            GameMaster.GM.Save.Data.Inventory[ID + 124].Count = value;
        }
    }

    public static readonly Key Boneyard = new(0, 5, 1, "Boneyard", "the Boneyard");
    public static readonly Key TidalMines = new(1, 12, 4, "Tidal Mines", "the Tidal Mines");
    public static readonly Key Crocasino = new(2, 6, 2, "Crocasino", "the Crocasino");
    public static readonly Key HowlingBjerg = new(3, 8, 1, "Howling Bjerg", "the Howling Bjerg");
    public static readonly Key CastleVann = new(4, 10, 4, "Castle Vann", "Castle Vann");
    public static readonly Key MagmaHeart = new(5, 9, 2, "Magma Heart", "the Magma Heart");
    public static readonly Key TimeOut = new(6, 13, 3, "Time Out", "Time Out");
    public static readonly Key Lighthouse = new(7, 11, 1, "Lighthouse", "the Lighthouse");
    public static readonly Key CrystalCaves = new(8, 24, 3, "Crystal Caves", "the Crystal Caves");
    public static readonly Key HauntedHall = new(9, 16, 2, "Haunted Hall", "the Haunted Hall");
    public static readonly Key SiskasWorkshop = new(10, 19, 3, "Siska's Wkshp", "Siska's Workshop");
    public static readonly Key Backrooms = new(11, 17, 2, "Backrooms", "the Backrooms");
    public static readonly Key PiratesPier = new(12, 27, 5, "Pirate's Pier", "Pirate's Pier");
    public static readonly Key BjergCastle = new(13, 8, 1, "Bjerg Castle", "Bjerg Castle");

    public static readonly Key[] Keys = [
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
    ];
}