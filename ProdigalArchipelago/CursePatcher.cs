using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using HarmonyLib;

namespace ProdigalArchipelago;

// Prevent permadeath
[HarmonyPatch(typeof(GameMaster))]
[HarmonyPatch(nameof(GameMaster.PlayerDied))]
class GameMaster_PlayerDied_Patch
{
    static bool Prefix(GameMaster __instance)
    {
        if (Archipelago.Enabled)
        {
            if ((bool)AccessTools.Field(typeof(GameMaster), "Dying").GetValue(__instance))
            {
                return false;
            }

            Archipelago.AP.Stats.DeathCount++;

            __instance.ChatOver();
            __instance.GS = GameMaster.GameState.LOCKED;
            AccessTools.Field(typeof(GameMaster), "Dying").SetValue(__instance, true);
            AccessTools.Field(typeof(GameMaster), "TimeReady").SetValue(__instance, false);

            __instance.SkipToNextDay();
            __instance.StartCoroutine((IEnumerator)AccessTools.Method(typeof(GameMaster), "DeathSequence").Invoke(__instance, []));

            return false;
        }

        return true;
    }
}

[HarmonyPatch(typeof(PlayerCharacter))]
[HarmonyPatch(nameof(PlayerCharacter.DEATH))]
class PlayerCharacter_DEATH_Patch
{
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (var code in instructions)
        {
            yield return code;

            if (code.opcode == OpCodes.Bne_Un)
            {
                yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Archipelago), nameof(Archipelago.Enabled)));
                yield return new CodeInstruction(OpCodes.Brtrue, code.operand);
            }
        }
    }
}

// Allow obtaining gold with Curse of Horns active
[HarmonyPatch(typeof(SaveSystem))]
[HarmonyPatch(nameof(SaveSystem.AddCurrency))]
class SaveSystem_AddCurrency_Patch
{
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        bool patched = false;

        foreach (var code in instructions)
        {
            yield return code;

            if (!patched && code.opcode == OpCodes.Bne_Un)
            {
                patched = true;
                yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Archipelago), nameof(Archipelago.Enabled)));
                yield return new CodeInstruction(OpCodes.Brtrue, code.operand);
            }
        }
    }
}

// Allow loot drops, but with reduced frequency, with Zolei altar active
[HarmonyPatch(typeof(LootTable))]
[HarmonyPatch(nameof(LootTable.Drop))]
class LootTable_Drop_Patch
{
    static void Prefix(LootTable __instance, ref int ___Dice, int ___Normal, int ___Rare, ref GameObject ___G)
    {
        if (Archipelago.Enabled && GameMaster.GM.Save.Data.Quests[37] == SaveSystem.Quest.QUESTCOMPLETE)
        {
            ___Dice = Random.Range(0, 1000) + GameMaster.GM.PC.CALCULATE_LUCK();
            if (___Dice >= 990 && ___Rare != 0)
            {
                ___G = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Interacts/Pickup"), __instance.transform.position, __instance.transform.rotation);
                ___G.transform.parent = __instance.transform.parent;
                ___G.GetComponent<Pickup>().Initiate(___Rare);
            }
            else if (___Dice >= 900 && ___Normal != 0)
            {
                ___G = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Interacts/Pickup"), __instance.transform.position, __instance.transform.rotation);
                ___G.transform.parent = __instance.transform.parent;
                ___G.GetComponent<Pickup>().Initiate(___Normal);
            }
        }
    }
}