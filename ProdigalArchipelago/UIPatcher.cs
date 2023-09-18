using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using HarmonyLib;

namespace ProdigalArchipelago
{
    class UIPatch
    {
        public static int Page = 1;
        public static GameObject WarpButton;
        public static GameObject ErrorIndicator;
        public static Color TextColorOverride = Color.white;

        public static void Setup()
        {
            WarpButton = Object.Instantiate(GameMaster.GM.UI.transform.GetChild(2).GetChild(0).GetChild(2).gameObject);
            WarpButton.name = "WarpButton";
            WarpButton.transform.parent = GameMaster.GM.UI.transform.GetChild(2).GetChild(0);
            WarpButton.SetActive(false);
            var button = WarpButton.GetComponent<UIButton>();
            button.SelectedSprite = SpriteManager.WarpSelectedSprite;
            button.HitSprite = SpriteManager.WarpHitSprite;
            typeof(UIButton).GetField("NormalSprite", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(button, SpriteManager.WarpNormalSprite);

            ErrorIndicator = new("ErrorIndicator");
            ErrorIndicator.transform.parent = GameMaster.GM.UI.transform.GetChild(3);
            ErrorIndicator.transform.localPosition = new Vector3(98, -20, 0);
            ErrorIndicator.SetActive(false);
            var sprite = ErrorIndicator.AddComponent<SpriteRenderer>();
            sprite.sprite = SpriteManager.ErrorSprite;
            sprite.sortingLayerName = "UI";
            sprite.sortingOrder = 10;
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

        private readonly List<List<GameObject>> KeyText = new();
        private int StartIndex;

        public static void Create()
        {
            Page1 = new GameObject("KeyPage");
            Page1.SetActive(false);
            var keyPage1 = Page1.AddComponent<KeyPage>();
            Page1.transform.parent = GameMaster.GM.UI.transform.GetChild(2).GetChild(0).GetChild(1);
            Page1.transform.localPosition = new Vector3(0, 0, 0);
            keyPage1.Setup(0, 7);

            Page2 = new GameObject("KeyPage");
            Page2.SetActive(false);
            var keyPage2 = Page2.AddComponent<KeyPage>();
            Page2.transform.parent = GameMaster.GM.UI.transform.GetChild(2).GetChild(0).GetChild(1);
            Page2.transform.localPosition = new Vector3(0, 0, 0);
            keyPage2.Setup(7, 6);
        }

        private void Setup(int start, int num)
        {
            StartIndex = start;
            for (int i = 0; i < num; i++)
            {
                KeyText.Add(Menu.CreateTextObjects($"KeyText{start + i}", Archipelago.KEY_DUNGEONS[start + i].Length + 2, transform, -70, 45 - 10 * i, Color.black));
            }
        }

        private void Update()
        {
            for (int i = 0; i < KeyText.Count; i++)
            {
                int keyCount = GameMaster.GM.Save.Data.Inventory[Archipelago.KEY_ID_START + StartIndex + i].Count;
                string dungeonName = Archipelago.KEY_DUNGEONS[StartIndex + i];
                Menu.RenderText(KeyText[i], $"{keyCount} {dungeonName}");
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
                int keyCount = 0;
                int currentScene = (int)typeof(GameMaster).GetField("CurrentScene", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(GameMaster.GM);
                for (int i = 0; i < Archipelago.KEY_SCENES.Length; i++)
                {
                    if (Archipelago.KEY_SCENES[i] == currentScene)
                    {
                        keyCount = GameMaster.GM.Save.Data.Inventory[Archipelago.KEY_ID_START + i].Count;
                    }
                }
                __instance.KEYS.UPDATE_COUNT(keyCount);
            }
        }
    }

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
}