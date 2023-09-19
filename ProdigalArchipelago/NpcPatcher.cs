using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using UnityEngine;
using HarmonyLib;

namespace ProdigalArchipelago
{
    [HarmonyPatch(typeof(Tara))]
    [HarmonyPatch("SmallFavorEnd")]
    [HarmonyPatch(MethodType.Enumerator)]
    class Tara_SmallFavorEnd_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return GenericItemPatcher.Transpiler(instructions, il, new int[] {0, 200});
        }
    }

    [HarmonyPatch(typeof(Tess))]
    [HarmonyPatch("Event3")]
    [HarmonyPatch(MethodType.Enumerator)]
    class Tess_Event3_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return GenericItemPatcher.Transpiler(instructions, il, 201);
        }
    }

    [HarmonyPatch(typeof(Hackett))]
    [HarmonyPatch("Bought")]
    [HarmonyPatch(MethodType.Enumerator)]
    class Hackett_Bought_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return GenericItemPatcher.Transpiler(instructions, il, 202);
        }
    }

    [HarmonyPatch(typeof(Hackett))]
    [HarmonyPatch("ItemCheck")]
    class Hackett_ItemCheck_Patch
    {
        static bool Prefix(ref bool __result)
        {
            if (Archipelago.Enabled)
            {
                __result = !GameMaster.GM.Save.Data.Chests.Contains(202);
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Gravedigger))]
    [HarmonyPatch("BootGift")]
    [HarmonyPatch(MethodType.Enumerator)]
    class Gravedigger_BootGift_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return GenericItemPatcher.Transpiler(instructions, il, 203);
        }
    }

    [HarmonyPatch(typeof(Gravedigger))]
    [HarmonyPatch("PostDocks")]
    [HarmonyPatch(MethodType.Enumerator)]
    class Gravedigger_PostDocks_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return GenericItemPatcher.Transpiler(instructions, il, 203);
        }
    }

    [HarmonyPatch(typeof(Gravedigger))]
    [HarmonyPatch("QuestCheck")]
    class Gravedigger_QuestCheck_Patch
    {
        static bool Prefix(ref bool __result)
        {
            if (Archipelago.Enabled && GameMaster.GM.Save.Data.Quests[3] == SaveSystem.Quest.QUESTCOMPLETE)
            {
                __result = !GameMaster.GM.Save.Data.Chests.Contains(203);
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Keaton))]
    [HarmonyPatch("HearthRing")]
    [HarmonyPatch(MethodType.Enumerator)]
    class Keaton_HearthRing_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return GenericItemPatcher.Transpiler(instructions, il, 205);
        }
    }

    [HarmonyPatch(typeof(Keaton))]
    [HarmonyPatch(nameof(Keaton.Chat))]
    class Keaton_Chat_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            bool hearthCheck = false;
            int remove = 0;

            foreach (var code in instructions)
            {
                if (code.opcode == OpCodes.Ldc_I4 && (int)code.operand == 999)
                {
                    hearthCheck = true;
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Keaton_Chat_Patch), nameof(FPRequirement)));
                }
                else
                {
                    if (hearthCheck && code.opcode == OpCodes.Ldsfld && (FieldInfo)code.operand == AccessTools.Field(typeof(GameMaster), nameof(GameMaster.GM)))
                    {
                        hearthCheck = false;
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Keaton_Chat_Patch), nameof(HearthCheck)));
                        remove = 7;
                    }
                    if (remove > 0)
                    {
                        remove--;
                    }
                    else
                    {
                        yield return code;
                    }
                }
            }
        }

        static bool HearthCheck()
        {
            if (Archipelago.Enabled)
            {
                return GameMaster.GM.Save.Data.Chests.Contains(205);
            }
            else
            {
                return GameMaster.GM.Save.Data.Inventory[84].Acquired;
            }
        }

        static int FPRequirement()
        {
            return (Archipelago.Enabled && Archipelago.AP.Settings.FastFishing) ? 100 : 999;
        }
    }

    [HarmonyPatch(typeof(Tess))]
    [HarmonyPatch("BeginMerch")]
    [HarmonyPatch(MethodType.Enumerator)]
    class Tess_BeginMerch_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return GenericItemPatcher.Transpiler(instructions, il, 206);
        }
    }

    [HarmonyPatch(typeof(Quinlan))]
    [HarmonyPatch("BeginMerch")]
    [HarmonyPatch(MethodType.Enumerator)]
    class Quinlan_BeginMerch_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return GenericItemPatcher.Transpiler(instructions, il, 207);
        }
    }

    [HarmonyPatch(typeof(Xavier))]
    [HarmonyPatch("CursedPickReturn")]
    [HarmonyPatch(MethodType.Enumerator)]
    class Xavier_CursedPickReturn_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return GenericItemPatcher.Transpiler(instructions, il, 208);
        }
    }

    [HarmonyPatch(typeof(Crocodile))]
    [HarmonyPatch("DustyKey")]
    [HarmonyPatch(MethodType.Enumerator)]
    class Crocodile_DustyKey_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return GenericItemPatcher.Transpiler(instructions, il, 209);
        }
    }

    [HarmonyPatch(typeof(Crocodile))]
    [HarmonyPatch("QuestCheck")]
    class Crocodile_QuestCheck_Patch
    {
        static bool Prefix(ref bool __result)
        {
            if (Archipelago.Enabled)
            {
                __result = !GameMaster.GM.Save.Data.Chests.Contains(209);
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Tess))]
    [HarmonyPatch("UltimateBoots")]
    [HarmonyPatch(MethodType.Enumerator)]
    class Tess_UltimateBoots_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return GenericItemPatcher.Transpiler(instructions, il, 210);
        }
    }

    [HarmonyPatch(typeof(Tess))]
    [HarmonyPatch(nameof(Tess.Chat))]
    class Tess_Chat_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool bootsCheck = false;
            int remove = 0;
            List<Label> labels = new();

            foreach (var code in instructions)
            {
                if (code.opcode == OpCodes.Call && (MethodInfo)code.operand == AccessTools.Method(typeof(Tess), "Greet"))
                {
                    bootsCheck = true;
                }
                if (bootsCheck && code.opcode == OpCodes.Ldsfld && (FieldInfo)code.operand == AccessTools.Field(typeof(GameMaster), nameof(GameMaster.GM)))
                {
                    bootsCheck = false;
                    remove = 16;
                    labels = code.labels;
                }
                if (remove > 0)
                {
                    remove--;
                }
                else
                {
                    if (labels.Count != 0)
                    {
                        code.labels = new(labels);
                        labels = new();
                    }
                    yield return code;
                }
            }
        }
    }

    [HarmonyPatch(typeof(Tess))]
    [HarmonyPatch("AllBoots")]
    class Tess_AllBoots_Patch
    {
        static bool Prefix(ref bool __result)
        {
            if (Archipelago.Enabled)
            {
                if (GameMaster.GM.Save.Data.Chests.Contains(210) || !GameMaster.GM.Save.Data.Inventory[71].Acquired)
                {
                    __result = false;
                    return false;
                }
                
                int boots_count = (from id in Archipelago.BOOTS_IDS where GameMaster.GM.Save.Data.Inventory[id].Acquired select id).Count();
                __result = boots_count >= 4;
                return false;
            }
            else
            {
                if (GameMaster.GM.Save.Data.Inventory[15].Acquired || !GameMaster.GM.Save.Data.Inventory[71].Acquired)
                {
                    __result = false;
                    return false;
                }
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Lynn))]
    [HarmonyPatch("PaintingGift")]
    [HarmonyPatch(MethodType.Enumerator)]
    class Lynn_PaintingGift_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            Label[] switchTable = null;
            int currentCase = -1;
            bool branchPointFound = false;
            bool mergePointFound = false;
            Label label_not_ap = il.DefineLabel();
            Label label_merge = il.DefineLabel();

            foreach (var code in instructions)
            {
                if (switchTable is null && code.opcode == OpCodes.Switch)
                {
                    switchTable = (Label[])code.operand;
                }
                
                foreach (var label in code.labels)
                {
                    int index = Array.IndexOf(switchTable, label);
                    if (index != -1)
                    {
                        currentCase = index;
                    }
                }

                if (currentCase == 1 && !branchPointFound && code.opcode == OpCodes.Ldloc_1)
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Archipelago), nameof(Archipelago.Enabled)))
                    {
                        labels = new(code.labels)
                    };
                    yield return new CodeInstruction(OpCodes.Brfalse, label_not_ap);
                    yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Archipelago), nameof(Archipelago.AP)));
                    yield return new CodeInstruction(OpCodes.Ldc_I4, 211);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Archipelago), nameof(Archipelago.CollectItem)));
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Br, label_merge);

                    code.labels.Clear();
                    code.labels.Add(label_not_ap);
                    branchPointFound = true;
                }

                if (branchPointFound && !mergePointFound && code.opcode == OpCodes.Br)
                {
                    code.labels.Add(label_merge);
                    mergePointFound = true;
                }

                yield return code;
            }
        }
    }

    [HarmonyPatch(typeof(Bolivar))]
    [HarmonyPatch("AllTogether")]
    [HarmonyPatch(MethodType.Enumerator)]
    class Bolivar_AllTogether_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return GenericItemPatcher.Transpiler(instructions, il, 212);
        }
    }

    [HarmonyPatch(typeof(Bolivar))]
    [HarmonyPatch("QuestComplete")]
    [HarmonyPatch(MethodType.Enumerator)]
    class Bolivar_QuestComplete_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return GenericItemPatcher.Transpiler(instructions, il, 212);
        }
    }

    [HarmonyPatch(typeof(Bolivar))]
    [HarmonyPatch(nameof(Bolivar.SpawnCheck))]
    class Bolivar_SpawnCheck_Patch
    {
        static void Prefix()
        {
            if (Archipelago.Enabled)
                GameMaster.GM.Save.Data.CurrentAct = SaveSystem.Act.Act1;
        }

        static void Postfix()
        {
            if (Archipelago.Enabled)
                GameMaster.GM.Save.Data.CurrentAct = SaveSystem.Act.Act2;
        }
    }

    [HarmonyPatch(typeof(SkipperBoss))]
    [HarmonyPatch("DeathTimer")]
    [HarmonyPatch(MethodType.Enumerator)]
    class SkipperBoss_DeathTimer_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return GenericItemPatcher.Transpiler(instructions, il, 213);
        }
    }

    [HarmonyPatch(typeof(Oni))]
    [HarmonyPatch("BeginMerch")]
    [HarmonyPatch(MethodType.Enumerator)]
    class Oni_BeginMerch_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return GenericItemPatcher.Transpiler(instructions, il, 214);
        }
    }

    [HarmonyPatch(typeof(Dullhallen))]
    [HarmonyPatch("BeginMerch")]
    [HarmonyPatch(MethodType.Enumerator)]
    class Dullhallen_BeginMerch_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return GenericItemPatcher.Transpiler(instructions, il, 215);
        }
    }

    [HarmonyPatch(typeof(Mynir))]
    [HarmonyPatch("BeginMerch")]
    [HarmonyPatch(MethodType.Enumerator)]
    class Mynir_BeginMerch_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return GenericItemPatcher.Transpiler(instructions, il, 216);
        }
    }

    [HarmonyPatch(typeof(Slime))]
    [HarmonyPatch("BeginMerch")]
    [HarmonyPatch(MethodType.Enumerator)]
    class Slime_BeginMerch_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return GenericItemPatcher.Transpiler(instructions, il, 217);
        }
    }

    [HarmonyPatch(typeof(Thia))]
    [HarmonyPatch("BeginMerch")]
    [HarmonyPatch(MethodType.Enumerator)]
    class Thia_BeginMerch_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return GenericItemPatcher.Transpiler(instructions, il, 218);
        }
    }

    [HarmonyPatch(typeof(Leer))]
    [HarmonyPatch("BeginMerch")]
    [HarmonyPatch(MethodType.Enumerator)]
    class Leer_BeginMerch_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return GenericItemPatcher.Transpiler(instructions, il, 219);
        }
    }

    [HarmonyPatch(typeof(Giant))]
    [HarmonyPatch("BeginMerch")]
    [HarmonyPatch(MethodType.Enumerator)]
    class Giant_BeginMerch_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return GenericItemPatcher.Transpiler(instructions, il, 220);
        }
    }

    [HarmonyPatch(typeof(Crelon))]
    [HarmonyPatch("BeginMerch")]
    [HarmonyPatch(MethodType.Enumerator)]
    class Crelon_BeginMerch_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return GenericItemPatcher.Transpiler(instructions, il, 221);
        }
    }

    [HarmonyPatch(typeof(Tedra))]
    [HarmonyPatch("BeginMerch")]
    [HarmonyPatch(MethodType.Enumerator)]
    class Tedra_BeginMerch_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return GenericItemPatcher.Transpiler(instructions, il, 222);
        }
    }

    [HarmonyPatch(typeof(Ulni))]
    [HarmonyPatch("BeginMerch")]
    [HarmonyPatch(MethodType.Enumerator)]
    class Ulni_BeginMerch_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return GenericItemPatcher.Transpiler(instructions, il, 223);
        }
    }

    [HarmonyPatch(typeof(Ulni))]
    [HarmonyPatch(nameof(Ulni.EventCheck))]
    class Ulni_EventCheck_Patch
    {
        static bool Prefix(ref bool __result)
        {
            if (Archipelago.Enabled)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Ulni))]
    [HarmonyPatch(nameof(Ulni.SpawnCheck))]
    class Ulni_SpawnCheck_Patch
    {
        static bool Prefix(Ulni __instance)
        {
            if (Archipelago.Enabled)
            {
                __instance.transform.position = new Vector3(944, -512, 0);
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Colorgrave))]
    [HarmonyPatch("TurnIn")]
    [HarmonyPatch(MethodType.Enumerator)]
    class Colorgrave_TurnIn_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return GenericItemPatcher.Transpiler(instructions, il, 224);
        }

        // Open Time Out 2 at the same time as Time Out 1
        static void Postfix()
        {
            if (GameMaster.GM.Save.Data.Quests[79] == SaveSystem.Quest.STAGE2)
            {
                GameMaster.GM.Save.Data.UnlockedDoors.Add(77);
            }
        }
    }

    [HarmonyPatch(typeof(Revulan))]
    [HarmonyPatch("GrantBlessing")]
    [HarmonyPatch(MethodType.Enumerator)]
    class Revulan_GrantBlessing_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return GenericItemPatcher.Transpiler(instructions, il, 226);
        }
    }

    [HarmonyPatch(typeof(Revulan))]
    [HarmonyPatch("FireEvent")]
    class Revulan_FireEvent_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            Queue<CodeInstruction> queue = new();
            foreach (var code in instructions)
            {
                queue.Enqueue(code);
                if (code.opcode == OpCodes.Ldfld && (FieldInfo)code.operand == AccessTools.Field(typeof(SaveSystem.Item), nameof(SaveSystem.Item.Acquired)))
                {
                    var labels = queue.Peek().labels;
                    queue.Clear();
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Revulan_FireEvent_Patch), nameof(BlessingCheck)))
                    {
                        labels = new(labels)
                    };
                }
                else if (queue.Count == 7)
                {
                    yield return queue.Dequeue();
                }
            }
            while (queue.TryPeek(out _))
            {
                yield return queue.Dequeue();
            }
        }

        static bool BlessingCheck()
        {
            if (Archipelago.Enabled)
            {
                return GameMaster.GM.Save.Data.Chests.Contains(226);
            }
            else
            {
                return GameMaster.GM.Save.Data.Inventory[91].Acquired;
            }
        }
    }

    [HarmonyPatch(typeof(Revulan))]
    [HarmonyPatch(nameof(Revulan.EventCheck))]
    class Revulan_EventCheck_Patch
    {
        static bool Prefix(ref bool __result)
        {
            if (Archipelago.Enabled)
            {
                __result = !GameMaster.GM.Save.Data.Chests.Contains(226);
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Butler))]
    [HarmonyPatch("ButlerEnding")]
    [HarmonyPatch(MethodType.Enumerator)]
    class Butler_ButlerEnding_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return GenericItemPatcher.Transpiler(instructions, il, 238);
        }
    }

    // Prevent the casino meeting from resetting Caroline's relationship stage
    [HarmonyPatch(typeof(Caroline))]
    [HarmonyPatch(nameof(Caroline.Meet))]
    class Caroline_Meet_Patch
    {
        static void Prefix(out SaveSystem.NPCData.Stages __state)
        {
            __state = GameMaster.GM.Save.Data.Relationships[5].Stage;
        }

        static void Postfix(SaveSystem.NPCData.Stages __state)
        {
            if (Archipelago.Enabled)
            {
                GameMaster.GM.Save.Data.Relationships[5].Stage = __state;
            }
        }
    }

    // Keep Caroline's original dialogue in the Crocasino regardless of relationship stage
    [HarmonyPatch(typeof(Caroline))]
    [HarmonyPatch(nameof(Caroline.Chat))]
    class Caroline_Chat_Patch
    {
        static bool Prefix(List<GameMaster.Speech> ___Chatter)
        {
            int currentScene = (int)AccessTools.Field(typeof(GameMaster), "CurrentScene").GetValue(GameMaster.GM);

            if (Archipelago.Enabled && currentScene == 6)
            {
                ___Chatter.Clear();
                ___Chatter.Add(GameMaster.CreateSpeech(5, 0, "THIS DOOR'S REAL SWEET ON ME.*CONVINCED HIM TO LET US INTO THE VAULT.", "CAROLINE", 6));
                GameMaster.GM.UI.InitiateChat(___Chatter, false);
                return false;
            }

            return true;
        }
    }

    // Keep Caroline's original dialogue at Pirate's Pier
    [HarmonyPatch(typeof(PirateCaroline))]
    [HarmonyPatch(nameof(PirateCaroline.NormalLoop))]
    class PirateCaroline_NormalLoop_Patch
    {
        static void Prefix()
        {
            if (Archipelago.Enabled)
            {
                GameMaster.GM.Save.Data.Quests[45] = SaveSystem.Quest.STAGE3;
            }
        }

        static void Postfix()
        {
            if (Archipelago.Enabled)
            {
                GameMaster.GM.Save.Data.Quests[45] = SaveSystem.Quest.QUESTCOMPLETE;
            }
        }
    }

    // Prevent the Kir Hasa quest from resetting the Crocodile quest
    [HarmonyPatch(typeof(KirHasa))]
    [HarmonyPatch(nameof(KirHasa.Lynn))]
    class KirHasa_Lynn_Patch
    {
        static void Postfix()
        {
            if (Archipelago.Enabled)
            {
                GameMaster.GM.Save.Data.Quests[40] = SaveSystem.Quest.QUESTCOMPLETE;
            }
        }
    }
}