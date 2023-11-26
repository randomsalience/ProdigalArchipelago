using System;
using HarmonyLib;

namespace ProdigalArchipelago;

[Serializable]
public class ArchipelagoStats
{
    public bool Enabled = true; // for backward compatibility
    public int FinishTime;
    public int ItemsCollected;
    public int ItemsTotal;
    public int PickTime;
    public int HandTime;
    public int EyeTime;
    public int LariatTime;
    public int KnuckleTime;
    public int FlareTime;
    public int DeathCount;
    public int WarpCount;
    public int KillCount;
    public int FallCount;
    public int DamageTaken;
    public int KeysBroken;

    public void Collect(Item item)
    {
        switch (item)
        {
            case Item.IronPick:
                PickTime = (int)GameMaster.GM.Save.Data.PlayTime;
                break;
            case Item.DreadHand:
                HandTime = (int)GameMaster.GM.Save.Data.PlayTime;
                break;
            case Item.EmpoweredHand:
                EyeTime = (int)GameMaster.GM.Save.Data.PlayTime;
                break;
            case Item.Lariat:
                LariatTime = (int)GameMaster.GM.Save.Data.PlayTime;
                break;
            case Item.RustKnuckle:
                KnuckleTime = (int)GameMaster.GM.Save.Data.PlayTime;
                break;
            case Item.FlareKnuckle:
                FlareTime = (int)GameMaster.GM.Save.Data.PlayTime;
                break;
        }
    }
}

[HarmonyPatch(typeof(ENEMY))]
[HarmonyPatch(nameof(ENEMY.DEATH))]
class ENEMY_DEATH_Patch {
    static void Prefix() {
        if (Archipelago.Enabled) {
            Archipelago.AP.Stats.KillCount++;
        }
    }
}

[HarmonyPatch(typeof(PlayerCharacter))]
[HarmonyPatch("APPLY_DAMAGE")]
class PlayerCharacter_APPLY_DAMAGE_Patch
{
    static void Prefix(out int __state)
    {
        __state = GameMaster.GM.PC.HP + GameMaster.GM.PC.GrayHP;
    }

    static void Postfix(int __state)
    {
        if (Archipelago.Enabled)
        {
            int damage = __state - (GameMaster.GM.PC.HP + GameMaster.GM.PC.GrayHP);
            Archipelago.AP.Stats.DamageTaken += damage;
        }
    }
}

[HarmonyPatch(typeof(PlayerCharacter))]
[HarmonyPatch("FALL_DAMAGE")]
class PlayerCharacter_FALL_DAMAGE_Patch
{
    static void Prefix()
    {
        if (Archipelago.Enabled)
        {
            Archipelago.AP.Stats.FallCount++;
            if (!GameMaster.GM.PC.BUFF_CHECK(PlayerCharacter.BUFFS.FEATHER) && GameMaster.GM.Save.Data.RingSlot != PlayerCharacter.Rings.WEDDING_LYNN)
            {
                Archipelago.AP.Stats.DamageTaken++;
            }
        }
    }
}

[HarmonyPatch(typeof(Pickup))]
[HarmonyPatch(nameof(Pickup.KeyBreak))]
class Pickup_KeyBreak_Patch
{
    static void Prefix(bool USED)
    {
        if (Archipelago.Enabled && !USED)
        {
            Archipelago.AP.Stats.KeysBroken++;
        }
    }
}