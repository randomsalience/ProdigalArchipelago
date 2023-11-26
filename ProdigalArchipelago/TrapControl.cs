using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using HarmonyLib;

namespace ProdigalArchipelago;

public class TrapControl : MonoBehaviour
{
    public static TrapControl TC;

    private const int TRAP_TIME = 20 * 60;
    private const int LIGHT_FREQUENCY = 210;

    public int SlownessTimer = 0;
    public int RustTimer = 0;
    public int ConfusionTimer = 0;
    public int DisarmingTimer = 0;
    public int LightTimer = 0;
    public int GlitchTimer = 0;

    public int GlitchCount = 0;
    public bool RunToggle = false;
    public PlayerCharacter.Boots BootSlot = PlayerCharacter.Boots.Base;
    public PlayerCharacter.Rings RingSlot = PlayerCharacter.Rings.Base;

    public GameObject Glitch;

    private readonly List<GameMaster.Speech> Chatter = [];

    public enum TrapType
    {
        Slowness,
        Rust,
        Confusion,
        Disarming,
        Light,
        Glitch,
        Zombie,
        Shadow,
        Love,
    }

    public static void Activate()
    {
        GameObject tc = new();
        TC = tc.AddComponent<TrapControl>();
        tc.transform.SetParent(GameMaster.GM.transform);
    }

    public static void Deactivate()
    {
        if (TC is not null)
        {
            Destroy(TC.gameObject);
        }
    }

    public void NewTrap(TrapType type)
    {
        StartCoroutine(CreateTrap(type));
    }

    private IEnumerator CreateTrap(TrapType type)
    {
        while (!Archipelago.NormalGameState())
        {
            yield return null;
        }

        switch (type)
        {
            case TrapType.Slowness:
                SlownessTimer = TRAP_TIME;
                RunToggle = GameMaster.GM.Save.PlayerOptions.RUN_TOGGLE == SaveSystem.TOGGLE.ON &&
                            GameMaster.GM.PC.MSTATUS == PlayerCharacter.RUN_STATE.RUNNING;
                break;
            case TrapType.Rust:
                RustTimer = TRAP_TIME;
                RunToggle = GameMaster.GM.Save.PlayerOptions.RUN_TOGGLE == SaveSystem.TOGGLE.ON &&
                            GameMaster.GM.PC.MSTATUS == PlayerCharacter.RUN_STATE.RUNNING;
                break;
            case TrapType.Disarming:
                DisarmingTimer = TRAP_TIME;
                break;
            case TrapType.Confusion:
                ConfusionTimer = TRAP_TIME;
                break;
            case TrapType.Light:
                LightTimer = TRAP_TIME;
                break;
            case TrapType.Glitch:
                GlitchTimer = TRAP_TIME;
                break;
            case TrapType.Zombie:
                SpawnZombies();
                break;
            case TrapType.Shadow:
                DreadHand();
                break;
            case TrapType.Love:
                if (GameMaster.GM.Save.Data.Wife != NPC.Name.Purple) {
                    StartCoroutine(LoveTrap());
                }
                break;
        }
    }

    private void Update()
    {
        if (!Archipelago.NormalGameState())
        {
            return;
        }

        if (SlownessTimer > 0) 
        {
            SlownessTimer--;
        }

        if (RustTimer > 0)
        {
            RustTimer--;
            Rust();
        }
        else
        {
            Unrust();
        }

        if (ConfusionTimer > 0)
        {
            ConfusionTimer--;
        }

        if (DisarmingTimer > 0)
        {
            DisarmingTimer--;
        }

        if (LightTimer > 0)
        {
            LightTimer--;
            if (LightTimer % LIGHT_FREQUENCY == 0)
            {
                LightAttack();
            }
        }

        if (GlitchTimer > 0)
        {
            if (Random.Range(0, GlitchCount) == 0)
            {
                GlitchCount++;
                GameObject obj = Instantiate(Glitch);
                obj.GetComponent<SpriteRenderer>().sortingLayerName = "UI";
                obj.transform.SetParent(GameMaster.GM.UI.transform);
                obj.transform.localPosition = new Vector3(Random.Range(-4, 5) * 16, Random.Range(-4, 5) * 16, 0);
                obj.AddComponent<GlitchTimer>();
                obj.SetActive(true);
            }
            GlitchTimer--;
        }
    }

    private void Rust()
    {
        if (GameMaster.GM.Save.Data.BootSlot != PlayerCharacter.Boots.Base)
        {
            BootSlot = GameMaster.GM.Save.Data.BootSlot;
            GameMaster.GM.Save.Data.BootSlot = PlayerCharacter.Boots.Base;
        }
        if (GameMaster.GM.Save.Data.RingSlot != PlayerCharacter.Rings.Base)
        {
            RingSlot = GameMaster.GM.Save.Data.RingSlot;
            GameMaster.GM.Save.Data.RingSlot = PlayerCharacter.Rings.Base;
        }
    }

    private void Unrust()
    {
        if (BootSlot != PlayerCharacter.Boots.Base)
        {
            GameMaster.GM.Save.Data.BootSlot = BootSlot;
            BootSlot = PlayerCharacter.Boots.Base;
        }
        if (RingSlot != PlayerCharacter.Rings.Base)
        {
            GameMaster.GM.Save.Data.RingSlot = RingSlot;
            RingSlot = PlayerCharacter.Rings.Base;
        }
    }

    private void LightAttack()
    {
        var beam = (GameObject)AccessTools.Field(typeof(PlayerCharacter), "RaemBeam").GetValue(GameMaster.GM.PC);
        Instantiate(beam, GameMaster.GM.PC.transform.position, GameMaster.GM.PC.transform.rotation);
    }

    private void SpawnZombies()
    {
        var spawnLocations = new (int, int)[] {(24, 0), (16, 16), (0, 24), (-16, 16), (-24, 0), (-16, -16), (0, -24), (16, -16)};
        foreach ((var deltaX, var deltaY) in spawnLocations)
        {
            Vector3 position = GameMaster.GM.PC.transform.position;
            position.x += deltaX;
            position.y += deltaY;
            if (Physics2D.BoxCast(position, new Vector2(14, 14), 0, Vector2.zero, 0, MotherBrain.MB.AI_GROUNDED))
            {
                continue;
            }
            var zombie = Instantiate(Resources.Load<GameObject>("Prefabs/Enemies/Zombie"), position, GameMaster.GM.PC.transform.rotation);
            zombie.transform.SetParent(transform);
        }
    }

    private void DreadHand()
    {
        GameMaster.GM.PC.Anim.ResetTrigger("DREAD_IN");
        GameMaster.GM.PC.DreadHand(true);
    }

    private IEnumerator LoveTrap()
    {
        GameMaster.GM.CUTSCENE(true);
        GameMaster.GM.PC.Hide(true);
        GameMaster.GM.CutsceneZoneLoad(2, new Vector3(1944, -1984, 0));
        yield return new WaitForSeconds(3);
        GameMaster.GM.CG.LoadCG(9);
        while ((bool)AccessTools.Field(typeof(GameMaster), "Loading").GetValue(GameMaster.GM))
        {
            yield return null;
        }
        yield return new WaitForSeconds(3);
        Chatter.Clear();
        Chatter.Add(GameMaster.CreateSpeech(2, 1, "I HAVE NEVER FELT LOVE, IN ALL MY LIFE.", "OAKLEY", 5));
        Chatter.Add(GameMaster.CreateSpeech(2, 0, "IT IS A POWERFUL, PASSIONATE FORCE.", "OAKLEY", 5));
        Chatter.Add(GameMaster.CreateSpeech(2, 3, "MY LOVE FOR YOU IS UNMATCHED,", "OAKLEY", 5));
        Chatter.Add(GameMaster.CreateSpeech(2, 0, "AND I WILL NEVER ALLOW ANYONE TO COME BETWEEN US AGAIN.", "OAKLEY", 5));
        Chatter.Add(GameMaster.CreateSpeech(2, 3, "FINALLY WE WILL BE TOGETHER FOREVER...", "OAKLEY", 5));
        GameMaster.GM.UI.InitiateChat(Chatter, false);
        while (GameMaster.GM.UI.SPEAKING())
        {
            yield return null;
        }
        GameMaster.GM.Save.Data.Wife = NPC.Name.Purple;
        GameMaster.GM.Save.Data.Married = true;
        GameMaster.GM.Save.Data.Relationships[2].Stage = SaveSystem.NPCData.Stages.MARRIED;
        GameMaster.GM.Save.Data.Quests[18] = SaveSystem.Quest.QUESTCOMPLETE;
        foreach (int id in new int[] {0, 1, 3, 5, 6, 8, 17, 48, 55})
        {
            if (GameMaster.GM.Save.Data.Relationships[id].Stage == SaveSystem.NPCData.Stages.MARRIED)
            {
                GameMaster.GM.Save.Data.Relationships[id].Stage = SaveSystem.NPCData.Stages.QUESTCOMPLETE;
            }
        }
        GameMaster.GM.CG.UnloadCG();
        GameMaster.GM.NewDay();
        GameMaster.GM.CutsceneZoneLoad(2, new Vector3(848, -830, 0));
        yield return new WaitForSeconds(0.5f);
        GameMaster.GM.PC.Hide(false);
        while (GameMaster.GM.Fader.Status != 0)
        {
            yield return null;
        }
        GameMaster.GM.CUTSCENE(false);
    }
}

public class GlitchTimer : MonoBehaviour
{
    public int Timer;

    private void Awake()
    {
        Timer = Random.Range(120, 360);
    }

    private void Update()
    {
        if (!Archipelago.NormalGameState())
        {
            return;
        }

        Timer--;
        if (Timer <= 0 || TrapControl.TC.GlitchTimer <= 0)
        {
            TrapControl.TC.GlitchCount--;
            Destroy(gameObject);
        }
    }
}

[HarmonyPatch(typeof(GameMaster))]
[HarmonyPatch("LoadInSequence")]
class GameMaster_LoadInSequence_Patch
{
    public static bool SkipFade = false;

    static IEnumerator Postfix(IEnumerator __result, GameMaster __instance)
    {
        if (Archipelago.Enabled)
        {
            __instance.Fader.LoadScreen(true);
            while (__instance.Fader.Status != 0)
            {
                yield return null;
            }
            var aload = SceneManager.LoadSceneAsync(23);
            while (!aload.isDone)
            {
                yield return null;
            }
            TrapControl.TC.Glitch = Object.Instantiate(GameObject.Find("Ending").GetComponent<Room>().RoomItems.transform.Find("Error1").gameObject);
            TrapControl.TC.Glitch.transform.SetParent(TrapControl.TC.transform);
            TrapControl.TC.Glitch.SetActive(false);
            SkipFade = true;
        }

        while (__result.MoveNext())
        {
            yield return __result.Current;
        }

        SkipFade = false;
    }
}

[HarmonyPatch(typeof(FadeController))]
[HarmonyPatch(nameof(FadeController.LoadScreen))]
class FadeController_LoadScreen_Patch
{
    static bool Prefix(bool On)
    {
        return !(Archipelago.Enabled && On && GameMaster_LoadInSequence_Patch.SkipFade);
    }
}

[HarmonyPatch(typeof(PlayerCharacter))]
[HarmonyPatch("CAN_RUN")]
class PlayerCharacter_CAN_RUN_Patch
{
    static bool Postfix(bool __result)
    {
        if (Archipelago.Enabled && TrapControl.TC.SlownessTimer > 0)
        {
            return false;
        }
        return __result;
    }
}

[HarmonyPatch(typeof(PlayerCharacter))]
[HarmonyPatch("GET_RUN")]
class PlayerCharacter_GET_RUN_Patch
{
    static bool Prefix(ref bool __result)
    {
        if (Archipelago.Enabled && GameMaster.GM.Save.PlayerOptions.RUN_TOGGLE == SaveSystem.TOGGLE.ON && TrapControl.TC.RunToggle &&
                    (bool)AccessTools.Method(typeof(PlayerCharacter), "CAN_RUN").Invoke(GameMaster.GM.PC, [])) {
            TrapControl.TC.RunToggle = false;
            __result = true;
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(PlayerCharacter))]
[HarmonyPatch("GET_MOVE")]
class PlayerCharacter_GET_MOVE_Patch
{
    static void Postfix(PlayerCharacter __instance)
    {
        if (Archipelago.Enabled && TrapControl.TC.ConfusionTimer > 0)
        {
            Vector2 movementVector = new(0, 0);
            switch (__instance.Facing)
            {
                case MotherBrain.Direction.Up:
                    movementVector = new(-1, 0);
                    __instance.Facing = MotherBrain.Direction.Down;
                    break;
                case MotherBrain.Direction.Down:
                    movementVector = new(1, 0);
                    __instance.Facing = MotherBrain.Direction.Up;
                    break;
                case MotherBrain.Direction.Right:
                    movementVector = new(0, -1);
                    __instance.Facing = MotherBrain.Direction.Left;
                    break;
                case MotherBrain.Direction.Left:
                    movementVector = new(0, 1);
                    __instance.Facing = MotherBrain.Direction.Right;
                    break;
            }
            AccessTools.Field(typeof(PlayerCharacter), "MovementVector").SetValue(__instance, movementVector);
            __instance.AnimDirection(__instance.Facing);
        }
    }
}

[HarmonyPatch(typeof(PlayerCharacter))]
[HarmonyPatch("SwingPick")]
class PlayerCharacter_SwingPick_Patch
{
    static bool Prefix()
    {
        if (Archipelago.Enabled && TrapControl.TC.DisarmingTimer > 0)
        {
            return false;
        }
        return true;
    }
}