using System.IO;
using UnityEngine;

namespace ProdigalArchipelago;

public static class SpriteManager
{
    public static Sprite ArchipelagoSprite;
    public static Sprite ArrowSprite;
    public static Sprite ConnectionSetupBGSprite;
    public static Sprite ErrorSprite;
    public static Sprite GameChoiceBGSprite;
    public static Sprite KeyScreenBGSprite;
    public static Sprite TrackerDotSprite;
    public static Sprite TrackerDotLargeSprite;
    public static Sprite WarpSelectedSprite;
    public static Sprite WarpNormalSprite;
    public static Sprite WarpHitSprite;

    public static void LoadSprites()
    {
        ArchipelagoSprite = LoadSprite("Archipelago.png");
        ArrowSprite = LoadSprite("Arrow.png");
        ConnectionSetupBGSprite = LoadSprite("ConnectionSetupBG.png");
        ErrorSprite = LoadSprite("Error.png");
        GameChoiceBGSprite = LoadSprite("GameChoiceBG.png");
        KeyScreenBGSprite = LoadSprite("KeyScreenBG.png");
        TrackerDotSprite = LoadSprite("TrackerDot.png");
        TrackerDotLargeSprite = LoadSprite("TrackerDotLarge.png");
        WarpSelectedSprite = LoadSprite("WarpSelected.png");
        WarpNormalSprite = LoadSprite("WarpNormal.png");
        WarpHitSprite = LoadSprite("WarpHit.png");

        GameMaster.GM.UI.THIN_FONT[8] = LoadSprite("ParenLThin.png");
        GameMaster.GM.UI.THIN_FONT[9] = LoadSprite("ParenRThin.png");
        GameMaster.GM.UI.THICK_FONT[8] = LoadSprite("ParenLThick.png");
        GameMaster.GM.UI.THICK_FONT[9] = LoadSprite("ParenRThick.png");
    }

    static Sprite LoadSprite(string filename)
    {
        var tex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        tex.LoadImage(File.ReadAllBytes($"{Application.dataPath}/../BepInEx/plugins/Archipelago/res/{filename}"));
        tex.filterMode = FilterMode.Point;
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 1);
    }
}