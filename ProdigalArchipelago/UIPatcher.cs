using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;
using System.Text.RegularExpressions;

namespace ProdigalArchipelago;

class UIPatch
{
    public static int Page = 1;
    public static GameObject WarpButton;
    public static GameObject ErrorIndicator;
    public static Color TextColorOverride = Color.white;

    public static void Setup()
    {
        CreateWarpButton();
        CreateErrorIndicator();
        CreateVersionText();
    }

    private static void CreateWarpButton()
    {
        WarpButton = Object.Instantiate(GameMaster.GM.UI.transform.GetChild(2).GetChild(0).GetChild(2).gameObject);
        WarpButton.name = "WarpButton";
        WarpButton.transform.parent = GameMaster.GM.UI.transform.GetChild(2).GetChild(0);
        WarpButton.SetActive(false);
        var button = WarpButton.GetComponent<UIButton>();
        button.SelectedSprite = ResourceManager.WarpSelectedSprite;
        button.HitSprite = ResourceManager.WarpHitSprite;
        AccessTools.Field(typeof(UIButton), "NormalSprite").SetValue(button, ResourceManager.WarpNormalSprite);
    }

    private static void CreateErrorIndicator()
    {
        ErrorIndicator = new("ErrorIndicator");
        ErrorIndicator.transform.parent = GameMaster.GM.UI.transform.GetChild(3);
        ErrorIndicator.transform.localPosition = new Vector3(98, -20, 0);
        ErrorIndicator.SetActive(false);
        var sprite = ErrorIndicator.AddComponent<SpriteRenderer>();
        sprite.sprite = ResourceManager.ErrorSprite;
        sprite.sortingLayerName = "UI";
        sprite.sortingOrder = 10;
    }

    private static void CreateVersionText()
    {
        string versionString = $"AP {Plugin.GetVersion()}";
        List<GameObject> versionText = Menu.CreateTextObjects("VersionText", versionString.Length, GameMaster.GM.UI.transform.GetChild(1).GetChild(0), -120, -56, new Color32(235, 223, 193, 255));
        Menu.RenderText(versionText, versionString);
    }

    public static void StartArchipelago()
    {
        WarpButton.SetActive(true);
        GameMaster.GM.UI.gameObject.transform.GetChild(2).GetChild(0).GetChild(2).gameObject.SetActive(false);
    }

    public static void StopArchipelago()
    {
        WarpButton.SetActive(false);
        GameMaster.GM.UI.gameObject.transform.GetChild(2).GetChild(0).GetChild(2).gameObject.SetActive(true);
    }

    public static string Sanitize(string name)
    {
        name = name.ToUpper()
            .Replace('[', '(')
            .Replace(']', ')');
        return Regex.Replace(name, @"[^A-Z0-9 ,!?.:;\-%'()/]", "?");
    }
}

// Replace save button with warp button
[HarmonyPatch(typeof(UIButton))]
[HarmonyPatch("ENGAGE")]
class UIButton_ENGAGE_Patch
{
    static bool Prefix(UIButton __instance, ref SpriteRenderer ___SpriteRen, ref bool ___Locked)
    {
        if (Archipelago.Enabled && __instance.BT == UIButton.ButtonType.SaveButton)
        {
            ___SpriteRen.sprite = __instance.SelectedSprite;
            ___Locked = false;
            Archipelago.AP.StartWarp();
            return false;
        }
        return true;
    }
}

class KeyPage : MonoBehaviour
{
    public static GameObject Page1;
    public static GameObject Page2;

    private readonly List<List<GameObject>> KeyText = [];
    private int StartIndex;

    public static void Create()
    {
        Page1 = new GameObject("KeyPage1");
        Page1.SetActive(false);
        var keyPage1 = Page1.AddComponent<KeyPage>();
        Page1.transform.parent = GameMaster.GM.UI.transform.GetChild(2).GetChild(0).GetChild(1);
        Page1.transform.localPosition = new Vector3(0, 0, 0);
        keyPage1.Setup(0, 7);

        Page2 = new GameObject("KeyPage2");
        Page2.SetActive(false);
        var keyPage2 = Page2.AddComponent<KeyPage>();
        Page2.transform.parent = GameMaster.GM.UI.transform.GetChild(2).GetChild(0).GetChild(1);
        Page2.transform.localPosition = new Vector3(0, 0, 0);
        keyPage2.Setup(7, 7);
    }

    private void Setup(int start, int num)
    {
        StartIndex = start;
        for (int i = 0; i < num; i++)
        {
            KeyText.Add(Menu.CreateTextObjects($"KeyText{start + i}", Key.Keys[start + i].Name.Length + 6, transform, -74, 44 - 12 * i, new Color32(130, 109, 95, 255)));
        }

        GameObject bg = new("KeyPageBG");
        var renderer = bg.AddComponent<SpriteRenderer>();
        renderer.sprite = ResourceManager.KeyScreenBGSprite;
        renderer.sortingLayerName = "UI";
        renderer.sortingOrder = 8;
        bg.transform.parent = transform;
        bg.transform.localPosition = new Vector3(-20, 4, 0);
    }

    private void Update()
    {
        for (int i = 0; i < KeyText.Count; i++)
        {
            int keyCount = 0;
            int keyTotal = 0;
            if (105 + StartIndex + i < GameMaster.GM.Save.Data.Inventory.Count) // for backwards compatibility
            {
                keyCount = Key.Keys[StartIndex + i].Count;
                keyTotal = Archipelago.AP.Data.KeyTotals[StartIndex + i];
            }
            int keyMax = Key.Keys[StartIndex + i].Max;
            string dungeonName = Key.Keys[StartIndex + i].Name;
            Menu.RenderText(KeyText[i], $"{keyCount} {keyTotal} {keyMax} {dungeonName}");
        }
    }
}

[HarmonyPatch(typeof(UI))]
[HarmonyPatch(nameof(UI.PAGE))]
class UI_PAGE_Patch
{
    static bool Prefix(UI __instance, UI.SCREEN ___CURRENT_SCREEN)
    {
        if (Archipelago.Enabled && Archipelago.AP.Settings.SpecificKeys && ___CURRENT_SCREEN == UI.SCREEN.PAUSE1)
        {
            UIPatch.Page++;
            if (UIPatch.Page == 5)
            {
                UIPatch.Page = 1;
            }
            __instance.OPEN_SCREEN(UI.SCREEN.PAUSE1);
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(UI))]
[HarmonyPatch(nameof(UI.OPEN_SCREEN))]
class UI_OPEN_SCREEN_Patch
{
    static void Postfix(UI __instance, UI.SCREEN TARGET)
    {
        if (Archipelago.Enabled && Archipelago.AP.Settings.SpecificKeys && TARGET == UI.SCREEN.PAUSE1)
        {
            __instance.transform.GetChild(2).GetChild(0).GetChild(1).GetChild(0).gameObject.SetActive(false);
            __instance.transform.GetChild(2).GetChild(0).GetChild(1).GetChild(1).gameObject.SetActive(false);
            KeyPage.Page1.SetActive(false);
            KeyPage.Page2.SetActive(false);
            switch (UIPatch.Page)
            {
                case 1:
                    __instance.transform.GetChild(2).GetChild(0).GetChild(1).GetChild(0).gameObject.SetActive(true);
                    break;
                case 2:
                    __instance.transform.GetChild(2).GetChild(0).GetChild(1).GetChild(1).gameObject.SetActive(true);
                    break;
                case 3:
                    KeyPage.Page1.SetActive(true);
                    break;
                case 4:
                    KeyPage.Page2.SetActive(true);
                    break;
            }
        }
    }
}

// Display key count for current dungeon on pause menu
[HarmonyPatch(typeof(UI))]
[HarmonyPatch(nameof(UI.PauseMenu))]
class UI_PauseMenu_Patch
{
    static void Postfix(UI __instance, bool Pause)
    {
        if (Pause && Archipelago.Enabled && Archipelago.AP.Settings.SpecificKeys)
        {
            Key key = Archipelago.AP.CurrentDungeonKey();
            int keyCount = key is null ? 0 : key.Count;
            __instance.KEYS.UPDATE_COUNT(keyCount);
        }
    }
}

// Add text color options
[HarmonyPatch(typeof(CHAT_BOX))]
[HarmonyPatch("APPLY_LETTER")]
class CHAT_BOX_APPLY_LETTER_Patch
{
    static void Prefix(List<char> ___KeysToPress, ref int ___Key_ID)
    {
        if (Archipelago.Enabled && ___KeysToPress[___Key_ID] == '@')
        {
            switch (___KeysToPress[___Key_ID + 1])
            {
                case 'P':
                    ___Key_ID += 2;
                    UIPatch.TextColorOverride = new Color32(221, 160, 221, 255);
                    break;
                case 'U':
                    ___Key_ID += 2;
                    UIPatch.TextColorOverride = new Color32(106, 90, 205, 255);
                    break;
                case 'F':
                    ___Key_ID += 2;
                    UIPatch.TextColorOverride = new Color32(0, 255, 255, 255);
                    break;
                case 'T':
                    ___Key_ID += 2;
                    UIPatch.TextColorOverride = new Color32(250, 128, 114, 255);
                    break;
                default:
                    UIPatch.TextColorOverride = Color.white;
                    break;
            }
        }
    }
}

[HarmonyPatch(typeof(UI))]
[HarmonyPatch(nameof(UI.LETTER_COLOR))]
class UI_LETTER_COLOR_Patch
{
    static bool Prefix(ref Color __result)
    {
        if (Archipelago.Enabled && UIPatch.TextColorOverride != Color.white)
        {
            __result = UIPatch.TextColorOverride;
            return false;
        }
        return true;
    }
}

// Break lines at underscores and hyphens
[HarmonyPatch(typeof(CHAT_BOX))]
[HarmonyPatch(nameof(CHAT_BOX.WORD_COUNT))]
class CHAT_BOX_WORD_COUNT_Patch
{
    static bool Prefix(int KID, List<char> KEYS, ref int __result)
    {
        if (Archipelago.Enabled)
        {
            for (int i = 0; KID + i < KEYS.Count; i++)
            {
                switch (KEYS[KID + i])
                {
                    case ' ':
                    case '_':
                    case '-':
                    case '*':
                        __result = i;
                        return false;
                }
            }
            __result = KEYS.Count - KID;
            return false;
        }

        return true;
    }
}

// Add stats screen to ending
[HarmonyPatch(typeof(GameMaster))]
[HarmonyPatch("ClosingSplashes")]
class GameMaster_ClosingSplashes_Patch
{
    static IEnumerator Postfix(IEnumerator __result)
    {
        if (Archipelago.Enabled && Archipelago.AP.Stats.Enabled && Archipelago.AP.Stats.FinishTime != 0)
        {
            StatsScreen.Instance.Activate();

            while (StatsScreen.Instance.Active)
            {
                yield return null;
            }
        }

        while (__result.MoveNext())
        {
            yield return __result.Current;
        }
    }
}

[HarmonyPatch(typeof(GameMaster))]
[HarmonyPatch("FinaleEnding")]
class GameMaster_FinaleEnding_Patch
{
    static IEnumerator Postfix(IEnumerator __result)
    {
        if (Archipelago.Enabled && Archipelago.AP.Stats.Enabled && Archipelago.AP.Stats.FinishTime != 0)
        {
            StatsScreen.Instance.Activate();

            while (StatsScreen.Instance.Active)
            {
                yield return null;
            }
        }

        while (__result.MoveNext())
        {
            yield return __result.Current;
        }
    }
}

[HarmonyPatch(typeof(GameMaster))]
[HarmonyPatch("CANNON_ENDING")]
class GameMaster_CANNON_ENDING_Patch
{
    static IEnumerator Postfix(IEnumerator __result)
    {
        if (Archipelago.Enabled && Archipelago.AP.Stats.Enabled && Archipelago.AP.Stats.FinishTime != 0)
        {
            StatsScreen.Instance.Activate();

            while (StatsScreen.Instance.Active)
            {
                yield return null;
            }
        }

        while (__result.MoveNext())
        {
            yield return __result.Current;
        }
    }
}

// Font size change
[HarmonyPatch(typeof(OPTIONS_BUTTON))]
[HarmonyPatch(nameof(OPTIONS_BUTTON.Adjust))]
class OPTIONS_BUTTON_Adjust_Patch
{
    static void Postfix(OPTIONS_BUTTON __instance)
    {
        if (__instance.SETTING == OPTIONS_BUTTON.OPTION.RESO)
        {
            ConnectionMenu.Instance.GetComponent<ConnectionMenu>().UpdateFontSize();
            ArchipelagoConsole.Instance.GetComponent<ArchipelagoConsole>().UpdateFontSize();
        }
    }
}
