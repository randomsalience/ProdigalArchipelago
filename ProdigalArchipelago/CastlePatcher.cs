using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;

namespace ProdigalArchipelago;

// Add sailor who takes you to Bjerg Castle
class CastleSailor : MonoBehaviour
{
    public static CastleSailor Instance;
    public static bool IsMarianaEvent;

    public static void Create()
    {
        GameObject obj = Instantiate(GameMaster.GM.transform.GetChild(2).GetChild(1).Find("MSailor").gameObject);
        obj.name = "CastleSailor";
        obj.transform.SetParent(GameMaster.GM.transform.GetChild(2).GetChild(1));
        Instance = obj.AddComponent<CastleSailor>();
        obj.GetComponent<Generic>().Facing = MotherBrain.Direction.Left;
        obj.SetActive(true);
        obj.GetComponent<BoxCollider2D>().enabled = true;
    }

    public void SpawnCheck()
    {
        if (Archipelago.Enabled && Archipelago.AP.Settings.ShuffleBjergCastle)
        {
            transform.position = new Vector3(415, -994, 0);
        }
        else
        {
            transform.position = new Vector3(-16, 16, 0);
        }
    }

    public void Chat()
    {
        List<GameMaster.Speech> chatter = [GameMaster.CreateSpeech(31, 0, "WANT TO GO TO BJERG CASTLE?", "SAILOR", 0)];
        GameMaster.GM.UI.InitiateChat(chatter, true);
    }

    public void Reply(bool Yes)
    {
        if (Yes)
        {
            StartCoroutine(GoToCastle());
        }
        else
        {
            List<GameMaster.Speech> chatter = [GameMaster.CreateSpeech(31, 0, "YEAH, I WOULDN'T WANT TO EITHER.", "SAILOR", 0)];
            GameMaster.GM.UI.InitiateChat(chatter, false);
        }
    }

    private IEnumerator GoToCastle()
    {
        IsMarianaEvent = false;
        Archipelago.AP.IsBjergCastle = true;
        GameMaster.GM.CUTSCENE(true);
        GameMaster.GM.CutsceneZoneLoad(8, new Vector2(880, -660));
        yield return new WaitForSeconds(1);
        MotherBrain.MB.DespawnAllNPCS();
        GameMaster.GM.CUTSCENE(false);
    }
}

[HarmonyPatch(typeof(MotherBrain))]
[HarmonyPatch(nameof(MotherBrain.LoadOverworldNPCs))]
class MotherBrain_LoadOverworldNPCs_Patch
{
    static void Postfix()
    {
        CastleSailor.Instance.SpawnCheck();
    }
}

[HarmonyPatch(typeof(MotherBrain))]
[HarmonyPatch(nameof(MotherBrain.DespawnAllNPCS))]
class MotherBrain_DespawnAllNPCS_Patch
{
    static void Postfix()
    {
        CastleSailor.Instance.gameObject.GetComponent<NPC>().Despawn();
    }
}

[HarmonyPatch(typeof(Generic))]
[HarmonyPatch(nameof(Generic.Chat))]
class Generic_Chat_Patch
{
    static bool Prefix(Generic __instance)
    {
        if (Archipelago.Enabled)
        {
            var castleSailor = __instance.gameObject.GetComponent<CastleSailor>();
            if (castleSailor is not null)
            {
                castleSailor.Chat();
                return false;
            }
        }
        return true;
    }
}

[HarmonyPatch(typeof(NPC))]
[HarmonyPatch(nameof(NPC.Response))]
class NPC_Response_Patch
{
    static void Postfix(NPC __instance, bool Yes)
    {
        if (Archipelago.Enabled)
        {
            var castleSailor = __instance.gameObject.GetComponent<CastleSailor>();
            castleSailor?.Reply(Yes);
        }
    }
}

// Add item on Captain Crossbones
[HarmonyPatch(typeof(Crossbones))]
[HarmonyPatch("DeathTimer")]
class Crossbones_DeathTimer_Patch
{
    static IEnumerator Postfix(IEnumerator __result)
    {
        while (__result.MoveNext())
        {
            yield return __result.Current;
        }
        
        if (Archipelago.Enabled && Archipelago.AP.Settings.ShuffleBjergCastle)
        {
            Archipelago.AP.CollectItem(248);
        }
    }
}

// Only marry Mariana through the normal relationship progression
[HarmonyPatch(typeof(Molly))]
[HarmonyPatch(nameof(Molly.FireMarriage))]
class Molly_FireMarriage_Patch
{
    static bool Prefix()
    {
        return !(Archipelago.Enabled && !CastleSailor.IsMarianaEvent);
    }
}

// Allow marriage to Mariana even if Crossbones is already dead
[HarmonyPatch(typeof(Molly))]
[HarmonyPatch("ProposalEvent")]
class Molly_ProposalEvent_Patch
{
    static IEnumerator Postfix(IEnumerator __result, Molly __instance)
    {
        while (__result.MoveNext())
        {
            yield return __result.Current;
        }

        if (Archipelago.Enabled)
        {
            Archipelago.AP.IsBjergCastle = true;
            CastleSailor.IsMarianaEvent = true;
            if (GameMaster.GM.Save.Data.Bosses.Contains(8))
            {
                __instance.StartCoroutine((IEnumerator)AccessTools.Method(typeof(Molly), "MarryMe").Invoke(__instance, []));
            }
        }
    }
}

// Prevent Crossbones item from interfering with marriage cutscene
[HarmonyPatch(typeof(Molly))]
[HarmonyPatch("MarryMe")]
class Molly_MarryMe_Patch
{
    static IEnumerator Postfix(IEnumerator __result)
    {
        if (Archipelago.Enabled)
        {
            while (GameMaster.GM.UI.SPEAKING())
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