using System.Collections;
using UnityEngine;
using HarmonyLib;

namespace ProdigalArchipelago
{
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

        public PlayerCharacter.Boots BootSlot = PlayerCharacter.Boots.Base;
        public PlayerCharacter.Rings RingSlot = PlayerCharacter.Rings.Base;

        public enum TrapType
        {
            Slowness,
            Rust,
            Confusion,
            Disarming,
            Light,
            Zombie,
            Shadow,
        }

        public static void Activate()
        {
            GameObject tc = new();
            TC = tc.AddComponent<TrapControl>();
            tc.transform.SetParent(GameMaster.GM.transform);
        }

        public static void Deactivate()
        {
            Object.Destroy(TC.gameObject);
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
                    break;
                case TrapType.Rust:
                    RustTimer = TRAP_TIME;
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
                case TrapType.Zombie:
                    SpawnZombies();
                    break;
                case TrapType.Shadow:
                    DreadHand();
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
            Object.Instantiate(beam, GameMaster.GM.PC.transform.position, GameMaster.GM.PC.transform.rotation);
        }

        private void SpawnZombies()
        {
            var spawnLocations = new (int, int)[] {(48, 0), (32, 32), (0, 48), (-32, 32), (-48, 0), (-32, -32), (0, -48), (32, -32)};
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
}