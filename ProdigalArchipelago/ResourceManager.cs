using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ProdigalArchipelago;

public static class ResourceManager
{
    private static Font Font8;
    private static Font Font16;
    private static Font Font20;
    private static Font Font30;
    private static Font Font40;

    public static Sprite ArchipelagoSprite;
    public static Sprite ArrowSprite;
    public static Sprite ConnectionSetupBGSprite;
    public static Sprite ConsoleBGSprite;
    public static Sprite ErrorSprite;
    public static Sprite GameChoiceBGSprite;
    public static Sprite KeyScreenBGSprite;
    public static Sprite StatsScreenBGSprite;
    public static Sprite TrackerDotSprite;
    public static Sprite TrackerDotLargeSprite;
    public static Sprite WarpSelectedSprite;
    public static Sprite WarpNormalSprite;
    public static Sprite WarpHitSprite;

    public static void Load()
    {
        AssetBundle bundle = AssetBundle.LoadFromFile($"{GetPath()}/res/font");
        Font8 = bundle.LoadAsset<Font>("Atkinson-Hyperlegible-Regular-8.ttf");
        Font16 = bundle.LoadAsset<Font>("Atkinson-Hyperlegible-Regular-16.ttf");
        Font20 = bundle.LoadAsset<Font>("Atkinson-Hyperlegible-Regular-20.ttf");
        Font30 = bundle.LoadAsset<Font>("Atkinson-Hyperlegible-Regular-30.ttf");
        Font40 = bundle.LoadAsset<Font>("Atkinson-Hyperlegible-Regular-40.ttf");
        bundle.Unload(false);
        
        ArchipelagoSprite = LoadSprite("Archipelago.png");
        ArrowSprite = LoadSprite("Arrow.png");
        ConnectionSetupBGSprite = LoadSprite("ConnectionSetupBG.png");
        ConsoleBGSprite = LoadSprite("ConsoleBG.png");
        ErrorSprite = LoadSprite("Error.png");
        GameChoiceBGSprite = LoadSprite("GameChoiceBG.png");
        KeyScreenBGSprite = LoadSprite("KeyScreenBG.png");
        StatsScreenBGSprite = LoadSprite("StatsScreenBG.png");
        TrackerDotSprite = LoadSprite("TrackerDot.png");
        TrackerDotLargeSprite = LoadSprite("TrackerDotLarge.png");
        WarpSelectedSprite = LoadSprite("WarpSelected.png");
        WarpNormalSprite = LoadSprite("WarpNormal.png");
        WarpHitSprite = LoadSprite("WarpHit.png");

        GameMaster.GM.UI.THIN_FONT[8] = LoadSprite("ParenLThin.png");
        GameMaster.GM.UI.THIN_FONT[9] = LoadSprite("ParenRThin.png");
        GameMaster.GM.UI.THIN_FONT[15] = LoadSprite("SlashThin.png");
        GameMaster.GM.UI.THICK_FONT[8] = LoadSprite("ParenLThick.png");
        GameMaster.GM.UI.THICK_FONT[9] = LoadSprite("ParenRThick.png");
        GameMaster.GM.UI.THICK_FONT[15] = LoadSprite("SlashThick.png");
    }

    static Sprite LoadSprite(string filename)
    {
        var tex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        tex.LoadImage(File.ReadAllBytes($"{GetPath()}/res/{filename}"));
        tex.filterMode = FilterMode.Point;
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 1);
    }

    public static Font GetFont()
    {
        return GameMaster.GM.Save.PlayerOptions.Resolution switch
        {
            1 => Font16,
            2 => Font20,
            3 => Font30,
            4 => Font40,
            _ => Font8,
        };
    }

    public static int GetFontSize()
    {
        return GameMaster.GM.Save.PlayerOptions.Resolution switch
        {
            1 => 16,
            2 => 20,
            3 => 30,
            4 => 40,
            _ => 8,
        };
    }

    static string GetPath() {
        Uri uri = new(Assembly.GetExecutingAssembly().CodeBase);
        return Path.GetDirectoryName(uri.LocalPath);
    }
}