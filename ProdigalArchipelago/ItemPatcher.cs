using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using HarmonyLib;

namespace ProdigalArchipelago
{
    [HarmonyPatch(typeof(ItemDatabase))]
    [HarmonyPatch(nameof(ItemDatabase.BeginDatabase))]
    class ItemDatabase_BeginDatabase_Patch
    {
        static void Postfix()
        {
            GameMaster.GM.ItemData.Database.Add(new ItemDatabase.ItemData
            {
                Name = "ARCHIPELAGO ITEM",
                ItemSprite = SpriteManager.ArchipelagoSprite,
            });
            GameMaster.GM.ItemData.Database.Add(new ItemDatabase.ItemData
            {
                Name = "PROGRESSIVE KNUCKLE",
                ItemSprite = GameMaster.GM.ItemData.ItemSprites[5],
            });
            GameMaster.GM.ItemData.Database.Add(new ItemDatabase.ItemData
            {
                Name = "PROGRESSIVE HAND",
                ItemSprite = GameMaster.GM.ItemData.ItemSprites[6],
            });
            GameMaster.GM.ItemData.Database.Add(new ItemDatabase.ItemData
            {
                Name = "PROGRESSIVE PICK",
                ItemSprite = GameMaster.GM.ItemData.ItemSprites[9],
            });
            GameMaster.GM.ItemData.Database.Add(NewKey("BONEYARD", "THE BONEYARD"));
            GameMaster.GM.ItemData.Database.Add(NewKey("TIDAL MINES", "THE TIDAL MINES"));
            GameMaster.GM.ItemData.Database.Add(NewKey("CROCASINO", "THE CROCASINO"));
            GameMaster.GM.ItemData.Database.Add(NewKey("HOWLING BJERG", "THE HOWLING BJERG"));
            GameMaster.GM.ItemData.Database.Add(NewKey("CASTLE VANN", "CASTLE VANN"));
            GameMaster.GM.ItemData.Database.Add(NewKey("MAGMA HEART", "THE MAGMA HEART"));
            GameMaster.GM.ItemData.Database.Add(NewKey("TIME OUT", "TIME OUT"));
            GameMaster.GM.ItemData.Database.Add(NewKey("LIGHTHOUSE", "THE LIGHTHOUSE"));
            GameMaster.GM.ItemData.Database.Add(NewKey("CRYSTAL CAVES", "THE CRYSTAL CAVES"));
            GameMaster.GM.ItemData.Database.Add(NewKey("HAUNTED HALL", "THE HAUNTED HALL"));
            GameMaster.GM.ItemData.Database.Add(NewKey("SISKA'S WORKSHOP", "SISKA'S WORKSHOP"));
            GameMaster.GM.ItemData.Database.Add(NewKey("BACKROOMS", "THE BACKROOMS"));
            GameMaster.GM.ItemData.Database.Add(NewKey("PIRATE'S PIER", "PIRATE'S PIER"));

            ChangeItemAboutText(10, "BLESSED PICKAXE!*I BET THIS WILL DO SOME SERIOUS DAMAGE!");
            ChangeItemAboutText(13, "CLEATED BOOTS!*I SHOULD TALK TO TESS ABOUT THESE.");
            ChangeItemAboutText(14, "ANCHOR GREAVES!*I SHOULD SEE TESS ABOUT THESE.");
            ChangeItemAboutText(15, "BOOTS OF GRAILE!*DAD...");
            ChangeItemAboutText(16, "WINGED BOOTS!*I SHOULD ASK TESS ABOUT THESE.");
            ChangeItemAboutText(23, "THAT IS A PIPIN' HOT CUP OF COFFEE.*IT SMELLS GREAT!");
            ChangeItemAboutText(24, "A TATTERED CAPE?*...*POSSIBLY.");
            ChangeItemAboutText(25, "IS THIS A BALL OF PLAIN YARN?*...*YEP.");
            ChangeItemAboutText(26, "SLIME SOAP?*I PROBABLY SHOULDN'T THINK ABOUT IT...");
            ChangeItemAboutText(27, "A SERPENT BRACELET!*THIS ACTUALLY LOOKS VALUABLE!");
            ChangeItemAboutText(28, "the CARROT CAKE!*I AM TOLD IT DOESN'T GET BETTER THAN THIS.");
            ChangeItemAboutText(29, "HUNTING BOW!*THE LOVE AND CARE IT RECIEVED IS APPARENT.");
            ChangeItemAboutText(30, "DOWN PILLOW!*EXTRA LARGE SIZE.*WAIT WHERE ARE THE FEATHERS FROM?");
            ChangeItemAboutText(31, "GIANT 'MONOCLE'!*HALF A PAIR OF SPECTACLES...*HOW USEFUL.");
            ChangeItemAboutText(32, "FORBIDDEN COOKBOOK!*I AM BOTH CONCERNED AND CURIOUS.");
            ChangeItemAboutText(33, "KELP ROLLS!*SLIMEY YET SATISFYING.");
            ChangeItemAboutText(35, "THE KEY TO THE GATES OF @Ccastle vann@!*I CAN FINALLY SEE WHAT IS INSIDE!");
            ChangeItemAboutText(36, "A RUSTY KEY!*I CAN NOW USE THE CRANE ON THE BEACH TO GET INTO THE @CTIDAL MINE@!");
            ChangeItemAboutText(44, "SHATTERED SOUL!*WHAT A STRANGE COLOR...*I FEEL LIKE IT'S LOOKING AT ME.");
            ChangeItemAboutText(52, "BASEMENT KEY!*WAIT...*IS THIS A BONE KEY?!");
            ChangeItemAboutText(53, "SUNSET PAINTING!*WOW...*THE COLORS IN THIS ARE BEAUTIFUL. SHE PUT A LOT OF WORK INTO THIS.");
            ChangeItemAboutText(63, "THE KEY TO THE CELLS!*WAIT...*HOW DID CAROLINE GET THIS?*WAIT WHERE DID SHE GO!?");
            ChangeItemAboutText(64, "THERE IS A MAP IN HIS COAT POCKET.*THIS IS IT!*WITH THIS WE CAN FIND REVULAN'S BASE AND RESCUE HUGH.");
            ChangeItemAboutText(65, "HARMONICA!*...*I WISH I PAID MORE ATTENTION WHEN DAD TAUGHT ME HOW TO PLAY IT.");
            ChangeItemAboutText(68, "SCARF!*HEY...*WHERE'S THE COLOR?");
            ChangeItemAboutText(71, "A HAIRPIN?");
            ChangeItemAboutText(74, "FROZEN HEART!*...*WHAT EVEN IS THIS?");
            ChangeItemAboutText(75, "SPLINTER!*...*IT ALMOST DOESN'T SEEM REAL.");
            ChangeItemAboutText(76, "HEY IT ABSORBED*THE EYE!*ALL THAT FOR*NOTHING. . .");
            ChangeItemAboutText(77, "FLARE KNUCKLE!*...*I CAN NOW CHARGE THE GAUNTLET WITH HEAT AND BURST FORWARD!");
            ChangeItemAboutText(78, "WEAPON CHAIN!*MY CHARGE SWINGS NOW HAVE A GREATER RANGE!");
            ChangeItemAboutText(79, "CLIMBING GEAR!*I CAN NOW SCALE CERTAIN WALLS.");
            ChangeItemAboutText(84, "HEARTH RING!*IT MAKES ME FEEL SO WARM AND COZY.");
            ChangeItemAboutText(85, "DAEMON'S RING!*THIS FEELS LIKE A TRICK.");
            ChangeItemAboutText(87, "A DUSTY KEY!*THIS LOOKS ANCIENT.");
            ChangeItemAboutText(89, "LIFE BLESSING!");
            ChangeItemAboutText(90, "BLESSING OF LIGHT!*...*I GUESS THAT WASN'T A DREAM.");
            ChangeItemAboutText(91, "STORM BLESSING!*I WONDER IF SHE WANTS THAT RING BACK...");
            ChangeItemAboutText(92, "A BLESSING OF EARTH!*THIS FEELS PRIMORDIAL...*I WONDER WHAT IT IS FOR.");
            ChangeItemAboutText(93, "A BLESSING OF WATER!*THIS FEELS PRIMORDIAL...*I WONDER WHAT IT IS FOR.");
        }

        private static ItemDatabase.ItemData NewKey(string name, string alt_name)
        {
            return new ItemDatabase.ItemData
            {
                Name = $"{name} KEY",
                TooltipText = $"A KEY FOR {alt_name}.",
                AboutText = new List<GameMaster.Speech> { GameMaster.CreateSpeech(46, 0, $"A KEY FOR {alt_name}!", "", 0) },
                ItemSprite = GameMaster.GM.ItemData.ItemSprites[39]
            };
        }

        private static void ChangeItemAboutText(int id, string text)
        {
            GameMaster.GM.ItemData.Database[id].AboutText.Clear();
            GameMaster.GM.ItemData.Database[id].AboutText.Add(GameMaster.CreateSpeech(46, 0, text, "", 0));
        }
    }

    [HarmonyPatch(typeof(Chest))]
    [HarmonyPatch(nameof(Chest.PLUNDER))]
    class Chest_PLUNDER_Patch
    {
        private static bool Prefix(Chest __instance)
        {
            if (Archipelago.Enabled)
            {
                __instance.Puzz?.PuzzleCompletion();
                if (__instance.WARP_CHEST)
                {
                    GameMaster.GM.CUTSCENE(true);
                }
                if (__instance.TYPE == Chest.CHEST_TYPE.GRAB)
                {
                    __instance.GetComponent<SpriteRenderer>().enabled = false;
                }
                __instance.ChestState = Chest.Status.COLLECT_LOCK;
                return !Archipelago.AP.CollectItem(__instance.ID);
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Chest))]
    [HarmonyPatch("TrappedChest")]
    [HarmonyPatch(MethodType.Enumerator)]
    class Chest_TrappedChest_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            Queue<CodeInstruction> queue = new();

            foreach (var code in GenericItemPatcher.Transpiler(instructions, il, 109))
            {
                queue.Enqueue(code);

                if (code.opcode == OpCodes.Stfld && (FieldInfo)code.operand == AccessTools.Field(typeof(SaveSystem.PlayerSave), nameof(SaveSystem.PlayerSave.BSlot)))
                {
                    queue.Clear();
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Chest_TrappedChest_Patch), nameof(SetBSlot)));
                }

                if (queue.Count >= 5)
                {
                    yield return queue.Dequeue();
                }
            }

            foreach (var code in queue)
            {
                yield return code;
            }
        }

        static void SetBSlot()
        {
            if (!Archipelago.Enabled)
            {
                GameMaster.GM.Save.Data.BSlot = PlayerCharacter.Item.RustKnuckle2;
            }
        }
    }

    [HarmonyPatch(typeof(Chest))]
    [HarmonyPatch(nameof(Chest.Open))]
    class Chest_Open_Patch
    {
        static bool Prefix(Chest __instance, List<GameMaster.Speech> ___Chatter)
        {
            if (Archipelago.Enabled && __instance.TYPE == Chest.CHEST_TYPE.TRAPPED)
            {
                ___Chatter.Clear();
                if (GameMaster.GM.Save.Data.Chests.Contains(109))
                {
                    ___Chatter.Add(GameMaster.CreateSpeech(46, 0, "THOSE SPIKES*REALLY HURT.*MANAGED TO PUNCH A*HOLE IN MY*GAUNTLET.*NOW IF I HOLD A*FIST TOO LONG IT*CUTS MY PALM.", "", 0));
                    GameMaster.GM.UI.InitiateChat(___Chatter, false);
                }
                else
                {
                    GameMaster.GM.UI.SLOT_INT(__instance.GetComponent<Interactable>());
                    ___Chatter.Add(GameMaster.CreateSpeech(46, 0, "THIS CHEST LOOKS*TRAPPED. . .*OPEN IT ANYWAY?", "", 0));
                    GameMaster.GM.UI.InitiateChat(___Chatter, QuestionEnd: true);
                }
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(Chest))]
    [HarmonyPatch("OnEnable")]
    class Chest_OnEnable_Patch
    {
        static void Prefix(Chest __instance)
        {
            if (Archipelago.Enabled)
            {
                __instance.WARP_CHEST = false;
                if (__instance.TYPE == Chest.CHEST_TYPE.GRAB || __instance.TYPE == Chest.CHEST_TYPE.SPRITE)
                {
                    int id = Archipelago.AP.GetLocationItem(__instance.ID)?.SpriteID() ?? 0;
                    if (id == 0) return;
                    SpriteRenderer renderer = __instance.gameObject.GetComponent<SpriteRenderer>();
                    renderer.sprite = GameMaster.GM.ItemData.Database[id].ItemSprite;
                    __instance.gameObject.GetComponent<Animator>().enabled = false;
                }
            }
        }
    }

    [HarmonyPatch(typeof(SaveSystem))]
    [HarmonyPatch(nameof(SaveSystem.AddToInventory))]
    class SaveSystem_AddToInventory_Patch
    {
        static bool Prefix(int ItemID)
        {
            if (Archipelago.Enabled && ItemID == Archipelago.AP_ITEM_ID)
                return false;
            return true;
        }

        static void Postfix()
        {
            if (Archipelago.Enabled)
                GameMaster.GM.Save.Save();
        }
    }

    [HarmonyPatch(typeof(LootTable))]
    [HarmonyPatch(nameof(LootTable.GrelinDrop))]
    class LootTable_GrelinDrop_Patch
    {
        static bool Prefix(LootTable __instance)
        {
            if (Archipelago.Enabled && Archipelago.AP.Settings.ShuffleGrelinDrops)
            {
                if (!GameMaster.GM.Save.Data.Chests.Contains(240))
                {
                    UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Interacts/Pickup"), __instance.transform.position, __instance.transform.rotation).GetComponent<Pickup>().Initiate(96);
                    UnityEngine.Object.Destroy(__instance.gameObject);
                }
                else if (!GameMaster.GM.Save.Data.Chests.Contains(241))
                {
                    UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Interacts/Pickup"), __instance.transform.position, __instance.transform.rotation).GetComponent<Pickup>().Initiate(97);
                    UnityEngine.Object.Destroy(__instance.gameObject);
                }
                else if (!GameMaster.GM.Save.Data.Chests.Contains(242))
                {
                    UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Interacts/Pickup"), __instance.transform.position, __instance.transform.rotation).GetComponent<Pickup>().Initiate(98);
                    UnityEngine.Object.Destroy(__instance.gameObject);
                }
                else if (!GameMaster.GM.Save.Data.Chests.Contains(243))
                {
                    UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Interacts/Pickup"), __instance.transform.position, __instance.transform.rotation).GetComponent<Pickup>().Initiate(99);
                    UnityEngine.Object.Destroy(__instance.gameObject);
                }
                else if (__instance.GetComponent<LootTable>() is not null)
                {
                    __instance.GetComponent<LootTable>().Drop();
                }
                else
                {
                    UnityEngine.Object.Destroy(__instance.gameObject);
                }

                return false;
            }

            return true;
        }
    }

    // Make grelin drop appearance match item
    [HarmonyPatch(typeof(Pickup))]
    [HarmonyPatch(nameof(Pickup.Initiate))]
    class Pickup_Initiate_Patch
    {
        static void Postfix(int I, SpriteRenderer ___SpriteRen)
        {
            if (Archipelago.Enabled && Archipelago.AP.Settings.ShuffleGrelinDrops && I >= 96 && I <= 99)
            {
                int id = Archipelago.AP.GetLocationItem(I - 96 + 240)?.SpriteID() ?? 0;
                if (id == 0) return;
                ___SpriteRen.sprite = GameMaster.GM.ItemData.Database[id].ItemSprite;
            }
        }
    }

    // Shuffle grelin drops and gator key
    [HarmonyPatch(typeof(Pickup))]
    [HarmonyPatch(nameof(Pickup.Collect))]
    class Pickup_Collect_Patch
    {
        static bool Prefix(Pickup __instance, ref bool ___Collected)
        {
            if (Archipelago.Enabled && Archipelago.AP.Settings.ShuffleGrelinDrops && __instance.ID >= 96 && __instance.ID <= 99)
            {
                if (___Collected)
                {
                    return false;
                }
                ___Collected = true;
                Archipelago.AP.CollectItem(__instance.ID - 96 + 240);
                UnityEngine.Object.Destroy(__instance.gameObject);
                return false;
            }

            if (Archipelago.Enabled && __instance.ID == 37)
            {
                if (___Collected)
                {
                    return false;
                }
                ___Collected = true;
                Archipelago.AP.CollectItem(232);
                UnityEngine.Object.Destroy(__instance.gameObject);
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Pickup))]
    [HarmonyPatch("Awake")]
    class Pickup_Awake_Patch
    {
        static bool Prefix(Pickup __instance, ref SpriteRenderer ___SpriteRen)
        {
            if (Archipelago.Enabled && __instance.ID == 37)
            {
                if (GameMaster.GM.Save.Data.Chests.Contains(232))
                {
                    UnityEngine.Object.Destroy(__instance.gameObject);
                    return false;
                }
                ___SpriteRen = __instance.GetComponent<SpriteRenderer>();
			    ___SpriteRen.sprite = GameMaster.GM.ItemData.Database[__instance.ID].ItemSprite;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Pickup))]
    [HarmonyPatch("OnEnable")]
    class Pickup_OnEnable_Patch
    {
        static bool Prefix(Pickup __instance, ref SpriteRenderer ___SpriteRen)
        {
            if (Archipelago.Enabled && __instance.ID == 37)
            {
                if (GameMaster.GM.Save.Data.Chests.Contains(232))
                {
                    UnityEngine.Object.Destroy(__instance.gameObject);
                    return false;
                }
                ___SpriteRen = __instance.GetComponent<SpriteRenderer>();
			    ___SpriteRen.sprite = GameMaster.GM.ItemData.Database[__instance.ID].ItemSprite;
                return false;
            }

            return true;
        }
    }

    // Generic transpiler used to patch item get code in a number of methods
    public class GenericItemPatcher
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il, int[] ids, bool noChat = false)
        {
            bool itemStringFound = false;
            bool itemGetFound = false;
            int itemGetCount = 0;
            bool beginElse = false;
            bool endElse = false;

            var label_not_ap = new List<Label>();
            var label_merge = new List<Label>();
            for (int i = 0; i < ids.Length; i++)
            {
                label_not_ap.Add(il.DefineLabel());
                label_merge.Add(il.DefineLabel());
            }

            foreach (CodeInstruction code in instructions)
            {
                if (itemStringFound && code.opcode == OpCodes.Ldc_I4_1)
                {
                    itemGetFound = ids[itemGetCount] != -1;
                    if (ids[itemGetCount] == -1)
                        itemGetCount++;
                }

                if (itemGetFound && code.opcode == OpCodes.Ldsfld && (FieldInfo)code.operand == AccessTools.Field(typeof(GameMaster), nameof(GameMaster.GM)))
                {
                    itemGetFound = false;
                    yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Archipelago), nameof(Archipelago.Enabled)));
                    yield return new CodeInstruction(OpCodes.Brfalse_S, label_not_ap[itemGetCount]);
                    if (ids[itemGetCount] != 0)
                    {
                        yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Archipelago), nameof(Archipelago.AP)));
                        yield return new CodeInstruction(OpCodes.Ldc_I4, ids[itemGetCount]);
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Archipelago), nameof(Archipelago.CollectItem)));
                        yield return new CodeInstruction(OpCodes.Brtrue, label_merge[itemGetCount]);
                    }
                    else
                    {
                        yield return new CodeInstruction(OpCodes.Br, label_merge[itemGetCount]);
                    }
                    code.labels.Add(label_not_ap[itemGetCount]);
                    beginElse = true;
                }

                itemStringFound = code.opcode == OpCodes.Ldstr && ((string)code.operand).ToUpper() == "ITEM";

                if (endElse)
                {
                    endElse = false;
                    code.labels.Add(label_merge[itemGetCount]);
                    itemGetCount++;
                }

                yield return code;

                if (beginElse && code.opcode == OpCodes.Callvirt && (MethodInfo)code.operand == AccessTools.Method(typeof(UI), "InitiateChat"))
                {
                    beginElse = false;
                    endElse = true;
                }
                
                if (beginElse && noChat && code.opcode == OpCodes.Stfld && (FieldInfo)code.operand == AccessTools.Field(typeof(SaveSystem.Item), nameof(SaveSystem.Item.Count)))
                {
                    beginElse = false;
                    endElse = true;
                }
            }
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il, int id)
        {
            return Transpiler(instructions, il, new int[] {id}, false);
        }
    }

    [HarmonyPatch(typeof(SpecialInteract))]
    [HarmonyPatch("GetHarmonica")]
    [HarmonyPatch(MethodType.Enumerator)]
    class SpecialInteract_GetHarmonica_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return GenericItemPatcher.Transpiler(instructions, il, 227);
        }
    }

    [HarmonyPatch(typeof(SpecialInteract))]
    [HarmonyPatch("Splinter")]
    [HarmonyPatch(MethodType.Enumerator)]
    class SpecialInteract_Splinter_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return GenericItemPatcher.Transpiler(instructions, il, 179);
        }
    }

    [HarmonyPatch(typeof(SpecialInteract))]
    [HarmonyPatch("PlayHarmonica")]
    [HarmonyPatch(MethodType.Enumerator)]
    class SpecialInteract_PlayHarmonica_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return GenericItemPatcher.Transpiler(instructions, il, 228);
        }

        static void Postfix()
        {
            GameMaster.GM.PC.Anim.SetBool("WEPCHAIN", GameMaster.GM.Save.Data.Inventory[78].Acquired);
        }
    }

    [HarmonyPatch(typeof(SpecialInteract))]
    [HarmonyPatch("TrappedChest")]
    [HarmonyPatch(MethodType.Enumerator)]
    class SpecialInteract_TrappedChest_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            Queue<CodeInstruction> queue = new();

            foreach (var code in GenericItemPatcher.Transpiler(instructions, il, 109))
            {
                queue.Enqueue(code);

                if (code.opcode == OpCodes.Stfld && (FieldInfo)code.operand == AccessTools.Field(typeof(SaveSystem.PlayerSave), nameof(SaveSystem.PlayerSave.BSlot)))
                {
                    queue.Clear();
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Chest_TrappedChest_Patch), nameof(SetBSlot)));
                }

                if (queue.Count >= 5)
                {
                    yield return queue.Dequeue();
                }
            }

            foreach (var code in queue)
            {
                yield return code;
            }
        }

        static void SetBSlot()
        {
            if (!Archipelago.Enabled)
            {
                GameMaster.GM.Save.Data.BSlot = PlayerCharacter.Item.RustKnuckle2;
            }
        }
    }

    [HarmonyPatch(typeof(SpecialInteract))]
    [HarmonyPatch("CrystalHeartGet")]
    [HarmonyPatch(MethodType.Enumerator)]
    class SpecialInteract_CrystalHeartGet_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return GenericItemPatcher.Transpiler(instructions, il, 177);
        }
    }

    [HarmonyPatch(typeof(SpecialInteract))]
    [HarmonyPatch("CarolineRescue")]
    [HarmonyPatch(MethodType.Enumerator)]
    class SpecialInteract_CarolineRescue_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return GenericItemPatcher.Transpiler(instructions, il, 225);
        }
    }

    [HarmonyPatch(typeof(SpecialInteract))]
    [HarmonyPatch(nameof(SpecialInteract.InteractWith))]
    class SpecialInteract_InteractWith_Patch
    {
        static bool Prefix(SpecialInteract __instance, List<GameMaster.Speech> ___Chatter)
        {
            if (Archipelago.Enabled && __instance.INTERACTABLE == SpecialInteract.INT.SKIP_BOOK)
            {
                ___Chatter.Clear();
                ___Chatter.Add(GameMaster.CreateSpeech(46, 0, "NICE TRY.", "", 0));
                GameMaster.GM.UI.InitiateChat(___Chatter, false);
                return false;
            }

            if (Archipelago.Enabled && __instance.INTERACTABLE == SpecialInteract.INT.TORRANCRYPT)
            {
                ___Chatter.Clear();
                if (Archipelago.AP.BlessingCount() >= Archipelago.AP.Settings.BlessingsRequired)
                {
                    if (GameMaster.GM.Save.Data.Inventory[94].Acquired)
                    {
                        ___Chatter.Add(GameMaster.CreateSpeech(46, 0, "THE HERO'S SOUL SLEPT HERE...*I WAS TOLD TO RETURN IT TO WHAT WAS HIS?*WHATEVER THAT MEANS.", "", 0));
					    GameMaster.GM.UI.InitiateChat(___Chatter, false);
                    }
                    else
                    {
                        __instance.StartCoroutine((IEnumerator)AccessTools.Method(typeof(SpecialInteract), "GetSoul").Invoke(__instance, new object[] {}));
                    }
                }
                else
                {
                    string godString = Archipelago.AP.Settings.BlessingsRequired == 1 ? "GOD" : "GODS";
                    ___Chatter.Add(GameMaster.CreateSpeech(46, 0, $"A STRANGE LIGHT SLUMBERS IN THE CRYSTAL.*IT FEELS KIND SOMEHOW.*WITH THE BLESSINGS OF {Archipelago.AP.Settings.BlessingsRequired} {godString},*I COULD AWAKEN IT.*I ONLY HAVE {Archipelago.AP.BlessingCount()}.", "", 0));
				    GameMaster.GM.UI.InitiateChat(___Chatter, false);
                }
                return false;
            }

            if (Archipelago.Enabled && __instance.INTERACTABLE == SpecialInteract.INT.PIERPRISON)
            {
                ___Chatter.Clear();
                if (!GameMaster.GM.Save.Data.Chests.Contains(225))
                {
                    __instance.StartCoroutine((IEnumerator)AccessTools.Method(typeof(SpecialInteract), "CarolineRescue").Invoke(__instance, new object[] {}));
                }
                else if (GameMaster.GM.Save.Data.Inventory[63].Acquired)
                {
                    GameMaster.GM.LoadSFX(76);
                    GameMaster.GM.PC.Unlock();
                    __instance.gameObject.SetActive(false);
                    MotherBrain.MB.Population[72].Despawn();
                }
                else if (GameMaster.GM.PC.transform.position.y < __instance.transform.position.y)
                {
                    ___Chatter.Add(GameMaster.CreateSpeech(46, 0, "IT'S EMPTY.*MAYBE HUGH BROKE OUT.*...*OR HE'S LOCKED UP SOMEWHERE ELSE.", "", 0));
                    GameMaster.GM.UI.InitiateChat(___Chatter, false);
                }
                else
                {
                    ___Chatter.Add(GameMaster.CreateSpeech(46, 0, "IT'S LOCKED.*WHY DIDN'T CAROLINE GIVE ME THE KEY?", "", 0));
                    GameMaster.GM.UI.InitiateChat(___Chatter, false);
                }
                return false;
            }

            return true;
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            Label[] switchTable = null;
            int currentCase = -1;
            int currentItem = -1;
            Queue<CodeInstruction> queue = new();

            foreach (var code in GenericItemPatcher.Transpiler(instructions, il, new int[] {230, 235}, true))
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

                queue.Enqueue(code);
                
                if (code.opcode == OpCodes.Ldc_I4_S)
                {
                    currentItem = (sbyte)code.operand;
                }

                if (code.opcode == OpCodes.Ldfld && (FieldInfo)code.operand == AccessTools.Field(typeof(SaveSystem.Item), nameof(SaveSystem.Item.Acquired)))
                {
                    yield return new CodeInstruction(OpCodes.Ldc_I4, currentCase)
                    {
                        labels = new(queue.Dequeue().labels)
                    };
                    yield return new CodeInstruction(OpCodes.Ldc_I4, currentItem);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SpecialInteract_InteractWith_Patch), nameof(AcquiredCheck)));
                    queue.Clear();
                }

                if (code.opcode == OpCodes.Bne_Un && currentCase == (int)SpecialInteract.INT.TRAPCHEST)
                {
                    yield return queue.Dequeue();
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SpecialInteract_InteractWith_Patch), nameof(KnuckleCheck)))
                    {
                        labels = new(queue.Dequeue().labels)
                    };
                    yield return new CodeInstruction(OpCodes.Brfalse, code.operand);
                    queue.Clear();
                }

                if (queue.Count == 7)
                {
                    yield return queue.Dequeue();
                }
            }

            while (queue.TryPeek(out _))
            {
                yield return queue.Dequeue();
            }
        }

        static bool AcquiredCheck(int interact, int item)
        {
            if (Archipelago.Enabled)
            {
                switch ((SpecialInteract.INT)interact)
                {
                    case SpecialInteract.INT.CRYSTALHEART:
                        return GameMaster.GM.Save.Data.Chests.Contains(177);
                    case SpecialInteract.INT.MUSICBOX:
                        if (item == 78)
                            return GameMaster.GM.Save.Data.Chests.Contains(228);
                        break;
                    case SpecialInteract.INT.HARMONICA:
                        return GameMaster.GM.Save.Data.Chests.Contains(227);
                    case SpecialInteract.INT.WATERFALLDOOR:
                        return GameMaster.GM.Save.Data.Chests.Contains(238);
                    case SpecialInteract.INT.WATERPILLARDG:
                        return GameMaster.GM.Save.Data.Chests.Contains(230);
                    case SpecialInteract.INT.EARTHPILLARDG:
                        return GameMaster.GM.Save.Data.Chests.Contains(235);
                }
            }
            return GameMaster.GM.Save.Data.Inventory[item].Acquired;
        }

        static bool KnuckleCheck()
        {
            if (Archipelago.Enabled)
            {
                return GameMaster.GM.Save.Data.Chests.Contains(109);
            }
            return GameMaster.GM.Save.Data.BSlot == PlayerCharacter.Item.RustKnuckle2;
        }
    }

    [HarmonyPatch(typeof(SpecialInteract))]
    [HarmonyPatch("AcquTime")]
    class SpecialInteract_AcquTime_Patch
    {
        static bool Prefix(ref IEnumerator __result)
        {
            if (Archipelago.Enabled)
            {
                __result = AcquTime();
                return false;
            }
            return true;
        }

        static IEnumerator AcquTime()
        {
            Plugin.Logger.LogInfo("AcquTime");
            yield return new WaitForSeconds(0.25f);
            GameMaster.GM.CUTSCENE(true);
            while (GameMaster.GM.UI.SPEAKING())
            {
                yield return null;
            }
            GameMaster.GM.PC.Anim.SetBool("ITEM", false);
            yield return new WaitForSeconds(0.5f);
            GameMaster.GM.DGTPOUT();
        }
    }

    // Allow competing in arena without a pick
    [HarmonyPatch(typeof(Molly))]
    [HarmonyPatch(nameof(Molly.ArenaChat))]
    class Molly_ArenaChat_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool weaponPatched = false;
            bool moneyPatched = false;
            foreach (var code in instructions)
            {
                if (!weaponPatched && code.opcode == OpCodes.Brtrue)
                {
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Molly_ArenaChat_Patch), nameof(WeaponCheck)));
                    weaponPatched = true;
                }
                if (!moneyPatched && code.opcode == OpCodes.Blt)
                {
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Molly_ArenaChat_Patch), nameof(MoneyCheck)));
                    yield return new CodeInstruction(OpCodes.Brfalse, code.operand);
                    moneyPatched = true;
                }
                else
                {
                    yield return code;
                }
            }
        }

        static bool WeaponCheck()
        {
            return Archipelago.Enabled || GameMaster.GM.Save.Data.PickSlot != PlayerCharacter.Picks.Null;
        }

        static bool MoneyCheck()
        {
            return (Archipelago.Enabled && GameMaster.GM.Save.Data.Quests[19] == SaveSystem.Quest.STAGE0) || GameMaster.GM.Save.Data.Currency >= 50;
        }
    }

    // Change arena item
    [HarmonyPatch(typeof(CombatChallenge))]
    [HarmonyPatch("ArenaWin")]
    [HarmonyPatch(MethodType.Enumerator)]
    class CombatChallenge_ArenaWin_Patch
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

                if (currentCase == 2 && !branchPointFound && code.opcode == OpCodes.Ldloc_1)
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Archipelago), nameof(Archipelago.Enabled)))
                    {
                        labels = new(code.labels)
                    };
                    yield return new CodeInstruction(OpCodes.Brfalse, label_not_ap);
                    yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Archipelago), nameof(Archipelago.AP)));
                    yield return new CodeInstruction(OpCodes.Ldc_I4, 204);
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

    // Remove cutscene and add item after defeating Yhote
    [HarmonyPatch(typeof(Amadeus))]
    [HarmonyPatch(nameof(Amadeus.FireBjerg))]
    class Amadeus_FireBjerg_Patch
    {
        static bool Prefix(Amadeus __instance)
        {
            if (Archipelago.Enabled)
            {
                __instance.StartCoroutine(PostBjergScene());
                return false;
            }
            return true;
        }

        static IEnumerator PostBjergScene()
        {
            GameMaster.GM.PC.CUTSCENE(true);
            GameMaster.GM.CutsceneFade(false, 1);
            yield return new WaitForSeconds(1);
            Archipelago.AP.CollectItem(233);
            while (GameMaster.GM.UI.SPEAKING())
            {
                yield return null;
            }
            GameMaster.GM.PC.Anim.SetBool("ITEM", false);
            yield return new WaitForSeconds(0.5f);
            GameMaster.GM.DGTPOUT();
        }
    }

    // Add item after defeating Inkwell
    [HarmonyPatch(typeof(GameMaster))]
    [HarmonyPatch("PiratePierEnding")]
    [HarmonyPatch(MethodType.Enumerator)]
    class GameMaster_PiratePierEnding_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return GenericItemPatcher.Transpiler(instructions, il, new int[] {239, -1});
        }
    }

    // Add item after defeating Siska
    [HarmonyPatch(typeof(SiskaP2))]
    [HarmonyPatch("PostScene")]
    class SiskaP2_PostScene_Patch
    {
        static IEnumerator Postfix(IEnumerator __result)
        {
            if (Archipelago.Enabled)
            {
                Archipelago.AP.CollectItem(231);
                while (GameMaster.GM.UI.SPEAKING())
                {
                    yield return null;
                }
                GameMaster.GM.PC.Anim.SetBool("ITEM", false);
            }

            while (__result.MoveNext())
            {
                yield return __result.Current;
            }
        }
    }

    // Change item from defeating Killer
    [HarmonyPatch(typeof(KillerBoss))]
    [HarmonyPatch("DeathTimer")]
    [HarmonyPatch(MethodType.Enumerator)]
    class KillerBoss_DeathTimer_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return GenericItemPatcher.Transpiler(instructions, il, 229);
        }
    }

    // Change item from defeating Vann
    [HarmonyPatch(typeof(HeadVann))]
    [HarmonyPatch("DeathTimer")]
    [HarmonyPatch(MethodType.Enumerator)]
    class HeadVann_DeathTimer_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            bool skip = false;
            Label label_archipelago = il.DefineLabel();

            foreach (var code in instructions)
            {
                if (code.opcode == OpCodes.Call && (MethodInfo)code.operand == AccessTools.Method(typeof(UnityEngine.Object), nameof(UnityEngine.Object.Destroy), new Type[] {typeof(UnityEngine.Object)}))
                {
                    skip = true;
                    yield return code;
                    yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Archipelago), nameof(Archipelago.Enabled)));
                    yield return new CodeInstruction(OpCodes.Brtrue, label_archipelago);
                }
                else
                {
                    if (skip && code.opcode == OpCodes.Ldloc_1)
                    {
                        code.labels.Add(label_archipelago);
                    }
                    yield return code;
                }
            }
        }
    }

    [HarmonyPatch(typeof(GameMaster))]
    [HarmonyPatch("CastleVannOver")]
    [HarmonyPatch(MethodType.Enumerator)]
    class GameMaster_CastleVannOver_Enumerator_Patch
    {
        #pragma warning disable CS0649
        public static GameObject Item;
        #pragma warning restore CS0649

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            bool skip = false;
            bool twistedSoulFound = false;
            FieldInfo twistedSoulField = null;
            Label label_not_archipelago = il.DefineLabel();

            foreach (var code in instructions)
            {
                if (skip)
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Archipelago), nameof(Archipelago.Enabled)));
                    yield return new CodeInstruction(OpCodes.Brfalse, label_not_archipelago);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, twistedSoulField);
                    yield return new CodeInstruction(OpCodes.Stsfld, AccessTools.Field(typeof(GameMaster_CastleVannOver_Enumerator_Patch), nameof(Item)));
                    yield return new CodeInstruction(OpCodes.Ldc_I4_0);
                    yield return new CodeInstruction(OpCodes.Ret);
                    code.labels.Add(label_not_archipelago);
                    skip = false;
                }
                
                yield return code;

                if (code.opcode == OpCodes.Ldstr && (string)code.operand == "Prefabs/Cutscene/TwistedSoul")
                {
                    twistedSoulFound = true;
                }

                if (twistedSoulFound && code.opcode == OpCodes.Stfld)
                {
                    twistedSoulFound = false;
                    skip = true;
                    twistedSoulField = (FieldInfo)code.operand;
                }
            }
        }
    }

    [HarmonyPatch(typeof(GameMaster))]
    [HarmonyPatch("CastleVannOver")]
    class GameMaster_CastleVannOver_Patch
    {
        static IEnumerator Postfix(IEnumerator __result)
        {
            while (__result.MoveNext())
            {
                yield return __result.Current;
            }

            if (Archipelago.Enabled)
            {
                GameObject itemObj = GameMaster_CastleVannOver_Enumerator_Patch.Item;
                int spriteID = Archipelago.AP.GetLocationItem(234).SpriteID();
                itemObj.GetComponent<SpriteRenderer>().sprite = GameMaster.GM.ItemData.Database[spriteID].ItemSprite;
                yield return new WaitForSeconds(2);
                GameMaster.GM.PC.WalkTo(new Vector3(872, -848, 0), 1, 0);
                while (!GameMaster.GM.PC.MovementDone)
                {
                    yield return null;
                }
                UnityEngine.Object.Destroy(itemObj);
                Archipelago.AP.CollectItem(234);
                while (GameMaster.GM.UI.SPEAKING())
                {
                    yield return null;
                }
                GameMaster.GM.PC.Anim.SetBool("ITEM", false);
                yield return new WaitForSeconds(0.5f);
                GameMaster.GM.DGTPOUT();
            }
        }
    }

    // Change item from Enetha
    [HarmonyPatch(typeof(GameMaster))]
    [HarmonyPatch("LightBlessing")]
    [HarmonyPatch(MethodType.Enumerator)]
    class GameMaster_LightBlessing_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return GenericItemPatcher.Transpiler(instructions, il, 237);
        }
    }

    [HarmonyPatch(typeof(Interactable))]
    [HarmonyPatch(nameof(Interactable.Reply))]
    class Interactable_Reply_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            Queue<CodeInstruction> queue = new();

            foreach (var code in instructions)
            {
                queue.Enqueue(code);

                if (code.opcode == OpCodes.Ldfld && (FieldInfo)code.operand == AccessTools.Field(typeof(SaveSystem.Item), nameof(SaveSystem.Item.Acquired)))
                {
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Interactable_Reply_Patch), nameof(LightBlessingCheck)));
                    queue.Clear();
                }

                if (queue.Count == 7)
                {
                    yield return queue.Dequeue();
                }
            }

            while (queue.TryPeek(out _))
            {
                yield return queue.Dequeue();
            }
        }

        static bool LightBlessingCheck()
        {
            if (Archipelago.Enabled)
            {
                return GameMaster.GM.Save.Data.Chests.Contains(237);
            }
            return GameMaster.GM.Save.Data.Inventory[90].Acquired;
        }
    }

    // Change Color Correction
    [HarmonyPatch(typeof(GameMaster))]
    [HarmonyPatch("TrialComplete")]
    [HarmonyPatch(MethodType.Enumerator)]
    class GameMaster_TrialComplete_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            Label label_not_archipelago = il.DefineLabel();
            bool attach_label = false;

            foreach (var code in instructions)
            {
                if (attach_label)
                {
                    code.labels.Add(label_not_archipelago);
                    attach_label = false;
                }

                yield return code;

                if (code.opcode == OpCodes.Callvirt && (MethodInfo)code.operand == AccessTools.Method(typeof(Animator), nameof(Animator.ResetTrigger), new Type[] {typeof(string)}))
                {
                    yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Archipelago), nameof(Archipelago.Enabled)));
                    yield return new CodeInstruction(OpCodes.Brfalse, label_not_archipelago);
                    yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Archipelago), nameof(Archipelago.AP)));
                    yield return new CodeInstruction(OpCodes.Ldc_I4, 236);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Archipelago), nameof(Archipelago.CollectItem)));
                    yield return new CodeInstruction(OpCodes.Pop);
                    attach_label = true;
                }
            }
        }

        static void Postfix()
        {
            if (Archipelago.Enabled)
            {
                GameMaster.GM.Save.Data.Recolored = false;
                Archipelago.AP.ColorCheck();
            }
        }
    }

    // Change item from Shadow Oran, and send victory upon defeat
    [HarmonyPatch(typeof(ShadowOranHM))]
    [HarmonyPatch("DeathScene")]
    [HarmonyPatch(MethodType.Enumerator)]
    class ShadowOranHM_DeathScene_Patch
    {
        static void Prefix()
        {
            if (Archipelago.Enabled && Archipelago.AP.Settings.GoalShadow())
            {
                Archipelago.AP.Finish();
            }
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            return GenericItemPatcher.Transpiler(instructions, il, 244);
        }
    }
}