using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections;
using UnityEngine;
using HarmonyLib;

namespace ProdigalArchipelago
{
    // Make specific keys work
    [HarmonyPatch(typeof(Interactable))]
    [HarmonyPatch(nameof(Interactable.Interact))]
    class Interactable_Interact_Patch
    {
        static bool Prefix(Interactable __instance, ref List<GameMaster.Speech> ___Chatter, ref bool ___Locked)
        {
            if (__instance.Type == Interactable.IType.LockBlock && Archipelago.Enabled && Archipelago.AP.Settings.SpecificKeys)
            {
                GameMaster.GM.UI.US = UI.UIState.NULL;
                ___Chatter.Clear();
                GameMaster.GM.UI.SLOT_INT(__instance);

                int currentScene = (int)AccessTools.Field(typeof(GameMaster), "CurrentScene").GetValue(GameMaster.GM);
                bool hasKey = false;
                for (int i = 0; i < Archipelago.KEY_SCENES.Length; i++)
                {
                    if (Archipelago.KEY_SCENES[i] == currentScene && GameMaster.GM.Save.Data.Inventory[Archipelago.KEY_ID_START + i].Count > 0 && !___Locked)
                    {
                        hasKey = true;
                        ___Locked = true;
                        GameMaster.GM.LoadSFX(76);
                        GameMaster.GM.Save.Data.Inventory[Archipelago.KEY_ID_START + i].Count--;
                        __instance.GetComponent<LockBlock>().Unlock();
                    }
                }

                if (!hasKey)
                {
                    ___Chatter.Clear();
                    ___Chatter.Add(GameMaster.CreateSpeech(46, 0, "I NEED A KEY FOR THIS.", "", 0));
                    GameMaster.GM.UI.InitiateChat(___Chatter, false);
                }

                return false;
            }
            return true;
        }
    }

    // Change requirements for the true ending
    [HarmonyPatch(typeof(FinalBoss))]
    [HarmonyPatch("TrueCheck")]
    class FinalBoss_TrueCheck_Patch
    {
        static bool Prefix(ref bool __result)
        {
            if (Archipelago.Enabled)
            {
                __result = true;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(SpecialDoor))]
    [HarmonyPatch(nameof(SpecialDoor.OpeningCheck))]
    class SpecialDoor_OpeningCheck_Patch
    {
        static bool Prefix(SpecialDoor __instance, ref List<GameMaster.Speech> ___Chatter)
        {
            if (Archipelago.Enabled && __instance.Owner == SpecialDoor.DoorOwner.Lighthouse)
            {
                if (GameMaster.GM.Save.Data.Recolored)
                {
                    GameMaster.GM.LoadSFX(1);
                    if (GameMaster.GM.Save.Data.OverworldState.Contains(22))
                    {
                        __instance.GetComponent<Animator>().SetTrigger("Open");
                        __instance.GetComponent<EntryExit>().Transition();
                    }
                    else
                    {
                        MotherBrain.MB.Population[11].GetComponent<Bolivar>().Lighthouse();
                    }
                }
                else
                {
                    ___Chatter.Clear();
                    ___Chatter.Add(GameMaster.CreateSpeech(46, 0, $"SOMETHING IS OFF. . .*I CAN'T TOUCH*THE DOOR.*I ONLY HAVE {Archipelago.AP.ColorCount()} OF {Archipelago.AP.Settings.ColorsRequired} COLORS.", "", 0));
                    GameMaster.GM.UI.InitiateChat(___Chatter, false);
                }
                return false;
            }
            return true;
        }
    }

    // Send finish on defeating Var
    [HarmonyPatch(typeof(TrueBoss))]
    [HarmonyPatch("DeathScene")]
    [HarmonyPatch(MethodType.Enumerator)]
    class TrueBoss_DeathScene_Patch
    {
        static void Prefix()
        {
            if (Archipelago.Enabled && Archipelago.AP.Settings.GoalVar())
            {
                Archipelago.AP.Finish();
            }
        }
    }

    // Send finish on Hero's Rest
    [HarmonyPatch(typeof(SpecialInteract))]
    [HarmonyPatch("HerosRestEnd")]
    [HarmonyPatch(MethodType.Enumerator)]
    class SpecialInteract_HerosRestEnd_Patch
    {
        static void Prefix()
        {
            if (Archipelago.Enabled && Archipelago.AP.Settings.GoalRest())
            {
                Archipelago.AP.Finish();
            }
        }
    }

    // Send finish on defeating Torran
    [HarmonyPatch(typeof(FIFTH))]
    [HarmonyPatch("ENDING")]
    [HarmonyPatch(MethodType.Enumerator)]
    class FIFTH_ENDING_Patch
    {
        static void Prefix()
        {
            if (Archipelago.Enabled && Archipelago.AP.Settings.GoalTorran())
            {
                Archipelago.AP.Finish();
            }
        }
    }

    // Remove major items from Zaegul's shop, also disable rocket boots
    [HarmonyPatch(typeof(ShopItem))]
    [HarmonyPatch("OnEnable")]
    class ShopItem_OnEnable_Patch
    {
        static void Prefix(ShopItem __instance)
        {
            if (Archipelago.Enabled && __instance.IT == ShopItem.ItemType.SecretShop)
            {
                switch (__instance.IDNum)
                {
                    case ShopItem.ID.One:
                        __instance.IDNum = ShopItem.ID.Six;
                        break;
                    case ShopItem.ID.Three:
                        __instance.IDNum = ShopItem.ID.Seven;
                        break;
                    case ShopItem.ID.Four:
                        __instance.IDNum = ShopItem.ID.Eight;
                        break;
                    case ShopItem.ID.Five:
                        __instance.IDNum = ShopItem.ID.Nine;
                        break;
                }
            }

            if (Archipelago.Enabled && __instance.IT == ShopItem.ItemType.Boots && __instance.IDNum == ShopItem.ID.Seven)
            {
                __instance.IT = ShopItem.ItemType.RUSTED;
            }
        }
    }

    // Prevent buying keys if specific keys setting is on
    [HarmonyPatch(typeof(ShopItem))]
    [HarmonyPatch(nameof(ShopItem.AskAbout))]
    class ShopItem_AskAbout_Patch
    {
        static bool Prefix(ShopItem __instance, List<GameMaster.Speech> ___FullChat)
        {
            if (Archipelago.Enabled && Archipelago.AP.Settings.SpecificKeys && __instance.IT == ShopItem.ItemType.SecretShop && __instance.IDNum == ShopItem.ID.Two)
            {
                ___FullChat.Clear();
                ___FullChat.Add(GameMaster.CreateSpeech(23, 0, "KEY ALWAYS USEFUL. . . EXCEPT THIS ONE. NOT WORK ON ANY DOOR.", "ZAEGUL", 10));
                ___FullChat.Add(GameMaster.CreateSpeech(23, 0, "AS ZAEGUL IS HONEST MERCHANT, CANNOT SELL YOU THIS.", "ZAEGUL", 10));
                GameMaster.GM.UI.InitiateChat(___FullChat, false);
                return false;
            }
            return true;
        }
    }

    // Don't change color on obtaining color correction check
    [HarmonyPatch(typeof(COLOR_CONTROL))]
    [HarmonyPatch(nameof(COLOR_CONTROL.COLOR_CORRECT))]
    class COLOR_CONTROL_COLOR_CORRECT_Patch
    {
        static bool Prefix()
        {
            return !Archipelago.Enabled;
        }
    }

    // Remove the Bolivar reveal scene when opening the Castle Vann gate
    [HarmonyPatch(typeof(Lock))]
    [HarmonyPatch(nameof(Lock.Unlock))]
    class Lock_Unlock_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            int count = 0;
            int skip = 0;
            bool patched = false;

            var label_ap_enabled = il.DefineLabel();

            foreach (var code in instructions)
            {
                if (count == 2 && !patched)
                {
                    patched = true;
                    yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Archipelago), nameof(Archipelago.Enabled)))
                    {
                        labels = new List<Label>(code.labels)
                    };
                    yield return new CodeInstruction(OpCodes.Brtrue, label_ap_enabled);
                    code.labels.Clear();
                    skip = 15;
                }
                if (code.opcode == OpCodes.Ret)
                {
                    count++;
                }
                if (skip > 0)
                {
                    skip--;
                    if (skip == 0)
                    {
                        code.labels.Add(label_ap_enabled);
                    }
                }
                yield return code;
            }
        }
    }

    // Get Amadeus out of the way at the hidden dock
    [HarmonyPatch(typeof(Amadeus))]
    [HarmonyPatch(nameof(Amadeus.SpawnCheck))]
    class Amadeus_SpawnCheck_Patch
    {
        static bool Prefix(Amadeus __instance)
        {
            if (Archipelago.Enabled)
            {
                __instance.transform.position = new Vector3(-320, 320, 0);
                return false;
            }
            return true;
        }
    }

    // Change the required number of crest fragments and coins
    [HarmonyPatch(typeof(QuestDoor))]
    [HarmonyPatch("QuestCheck")]
    class QuestDoor_QuestCheck_Patch
    {
        static bool Prefix(QuestDoor __instance, ref bool __result)
        {
            if (Archipelago.Enabled && __instance.ID == QuestDoor.DoorID.ID12)
            {
                __result = GameMaster.GM.Save.Data.Inventory[50].Count >= Archipelago.AP.Settings.CrestFragmentsRequired;
                return false;
            }

            if (Archipelago.Enabled && __instance.ID == QuestDoor.DoorID.ID24)
            {
                __result = GameMaster.GM.Save.Data.Inventory[20].Count >= Archipelago.AP.Settings.CoinsOfCrowlRequired;
                return false;
            }

            return true;
        }
    }

    // Change Kaelum's text for number of coins required
    [HarmonyPatch(typeof(QuestDoor))]
    [HarmonyPatch("Awake")]
    class QuestDoor_Awake_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var code in instructions)
            {
                if (code.opcode == OpCodes.Ldstr && (string)code.operand == " OF 13.")
                {
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(QuestDoor_Awake_Patch), nameof(RequirementString)));
                }
                else
                {
                    yield return code;
                }
            }
        }

        static string RequirementString()
        {
            return Archipelago.Enabled ? $" OF {Archipelago.AP.Settings.CoinsOfCrowlRequired}." : " OF 13.";
        }
    }

    // Change Alexan's text for number of crest fragments required
    [HarmonyPatch(typeof(QuestDoor))]
    [HarmonyPatch(nameof(QuestDoor.Chat))]
    class QuestDoor_Chat_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var code in instructions)
            {
                if (code.opcode == OpCodes.Ldstr && (string)code.operand == "*OF 5.")
                {
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(QuestDoor_Chat_Patch), nameof(RequirementString)));    
                }
                else
                {
                    yield return code;
                }
            }
        }

        static string RequirementString()
        {
            return Archipelago.Enabled ? $"*OF {Archipelago.AP.Settings.CrestFragmentsRequired}." : "*OF 5.";
        }
    }

    // Allow using warp statues without dread hand
    [HarmonyPatch(typeof(LevelStatue))]
    [HarmonyPatch(nameof(LevelStatue.TeleCheck))]
    class LevelStatue_TeleCheck_Patch
    {
        static bool Prefix(LevelStatue __instance, bool ___Enabled)
        {
            if (Archipelago.Enabled && Archipelago.AP.Settings.FakeDreadHand && ___Enabled)
            {
                MethodInfo teleport = AccessTools.Method(typeof(LevelStatue), "Teleport");
                teleport.Invoke(__instance, new object[] {});
                return false;
            }
            return true;
        }
    }

    // Faster fishing minigame
    [HarmonyPatch(typeof(UI))]
    [HarmonyPatch(nameof(UI.UpdateFP))]
    class UI_UpdateFP_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var code in instructions)
            {
                if (code.opcode == OpCodes.Ldc_I4 && (int)code.operand == 500)
                {
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(UI_UpdateFP_Patch), nameof(FPRequirement)));
                }
                else
                {
                    yield return code;
                }
            }
        }

        static int FPRequirement()
        {
            return (Archipelago.Enabled && Archipelago.AP.Settings.FastFishing) ? 100 : 500;
        }
    }

    // Talking to Armadel near old house the first time triggers boss fight
    [HarmonyPatch(typeof(Skipper))]
    [HarmonyPatch("Event1")]
    class Skipper_Event1_Patch
    {
        static bool Prefix(Skipper __instance)
        {
            if (Archipelago.Enabled)
            {
                __instance.StartCoroutine((IEnumerator)AccessTools.Method(typeof(Skipper), "Event5").Invoke(__instance, new object[] {}));
                return false;
            }
            return true;
        }
    }

    // Prevent taking crystal keys out of dungeons
    [HarmonyPatch(typeof(GameMaster))]
    [HarmonyPatch("LoadFade")]
    [HarmonyPatch(MethodType.Enumerator)]
    class GameMaster_LoadFade_Patch
    {
        static void Postfix()
        {
            int currentScene = (int)AccessTools.Field(typeof(GameMaster), "CurrentScene").GetValue(GameMaster.GM);
            if (GameMaster.GM.PC.CrystalKey && (currentScene == 2 || currentScene == 3))
            {
                GameMaster.GM.PC.CrystalKey.GetComponent<Pickup>().KeyBreak(false);
            }
        }
    }

    // Prevent Keaton from taking a recipe for free fish
    [HarmonyPatch(typeof(ShopItem))]
    [HarmonyPatch("TRADE_CHECK")]
    class ShopItem_TRADE_CHECK_Patch
    {
        static bool Prefix(ref bool __result, int ___Price)
        {
            if (Archipelago.Enabled && ___Price == 0)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }

    // Save on statue activation
    [HarmonyPatch(typeof(LevelStatue))]
    [HarmonyPatch("StatueEnable")]
    class LevelStatue_StatueEnable_Patch
    {
        static void Postfix()
        {
            if (Archipelago.Enabled)
                GameMaster.GM.Save.Save();
        }
    }

    // Save on boss death
    [HarmonyPatch(typeof(Boss))]
    [HarmonyPatch(nameof(Boss.BossDead))]
    class Boss_BossDead_Patch
    {
        static void Postfix()
        {
            if (Archipelago.Enabled)
                GameMaster.GM.Save.Save();
        }
    }

    // Save on puzzle completion
    [HarmonyPatch(typeof(Puzzle))]
    [HarmonyPatch(nameof(Puzzle.PuzzleCompletion))]
    class Puzzle_PuzzleCompletion_Patch
    {
        static void Postfix()
        {
            if (Archipelago.Enabled)
                GameMaster.GM.Save.Save();
        }
    }

    // For Debug Purposes Only
    [HarmonyPatch(typeof(LockBlock))]
    [HarmonyPatch(nameof(LockBlock.Unlock))]
    class LockBlock_Unlock_Patch
    {
        static void Prefix(LockBlock __instance)
        {
            Plugin.Logger.LogInfo($"Unlocking door {__instance.ID}");
        }
    }
}