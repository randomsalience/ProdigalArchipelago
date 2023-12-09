using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using UnityEngine;
using HarmonyLib;

namespace ProdigalArchipelago;

[Serializable]
public class CompoundSave
{
    public SaveSystem.PlayerSave GameData;
    public Archipelago.SaveData ArchipelagoData;
}

[HarmonyPatch(typeof(SaveButton))]
[HarmonyPatch(nameof(SaveButton.Setup))]
class SaveButton_Setup_Patch
{
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
    {
        bool check_ap_file_patched = false;
        bool check_file_patched = false;
        bool load_file_patched = false;
        bool sprite_code_found = false;
        bool attach_label_save_loaded = false;

        int to_skip = 0;

        var local_filename = il.DeclareLocal(typeof(string));
        var local_is_archipelago = il.DeclareLocal(typeof(bool));
        var local_player_save = il.DeclareLocal(typeof(SaveSystem.PlayerSave));

        var label_no_ap_save = il.DefineLabel();
        var label_save_exists = il.DefineLabel();
        var label_no_ap_sprite = il.DefineLabel();
        var label_load_normal_save = il.DefineLabel();
        var label_save_loaded = il.DefineLabel();

        CodeInstruction prev = null;

        foreach (CodeInstruction code in instructions)
        {
            if (!check_ap_file_patched && code.opcode == OpCodes.Call && (MethodInfo)code.operand == AccessTools.Method(typeof(Application), "get_persistentDataPath"))
            {
                yield return new CodeInstruction(OpCodes.Ldc_I4_0)
                {
                    labels = new List<Label>(code.labels)
                };
                yield return new CodeInstruction(OpCodes.Stloc, local_is_archipelago);
                yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Application), "get_persistentDataPath"));
                yield return new CodeInstruction(OpCodes.Ldstr, "/APS");
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(SaveButton), "ID"));
                yield return new CodeInstruction(OpCodes.Ldstr, ".syr");
                yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(string), nameof(String.Concat), [typeof(string), typeof(string), typeof(string), typeof(string)]));
                yield return new CodeInstruction(OpCodes.Stloc, local_filename);
                yield return new CodeInstruction(OpCodes.Ldloc, local_filename);
                yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(File), nameof(File.Exists), [typeof(string)]));
                yield return new CodeInstruction(OpCodes.Brfalse, label_no_ap_save);
                yield return new CodeInstruction(OpCodes.Ldc_I4_1);
                yield return new CodeInstruction(OpCodes.Stloc, local_is_archipelago);
                yield return new CodeInstruction(OpCodes.Br, label_save_exists);

                check_ap_file_patched = true;

                code.labels.Clear();
                code.labels.Add(label_no_ap_save);
                yield return code;
            }
            else if (check_ap_file_patched && !check_file_patched && code.opcode == OpCodes.Call && (MethodInfo)code.operand == AccessTools.Method(typeof(File), nameof(File.Exists), [typeof(string)]))
            {
                yield return new CodeInstruction(OpCodes.Stloc, local_filename);
                yield return new CodeInstruction(OpCodes.Ldloc, local_filename);
                yield return code;

                check_file_patched = true;
            }
            else if (check_file_patched && !load_file_patched && code.opcode == OpCodes.Newobj)
            {
                code.labels.Add(label_save_exists);
                yield return code;

                yield return new CodeInstruction(OpCodes.Ldloc, local_is_archipelago);
                yield return new CodeInstruction(OpCodes.Brfalse, label_load_normal_save);
                yield return new CodeInstruction(OpCodes.Pop);
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(SaveButton), "ID"));
                yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ArchipelagoSave), nameof(ArchipelagoSave.Deserialize)));
                yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(ArchipelagoSave), nameof(ArchipelagoSave.GameData)));
                yield return new CodeInstruction(OpCodes.Stloc, local_player_save);
                yield return new CodeInstruction(OpCodes.Ldloc, local_filename);
                yield return new CodeInstruction(OpCodes.Ldc_I4_3);
                yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(File), nameof(File.Open), [typeof(string), typeof(FileMode)]));
                yield return new CodeInstruction(OpCodes.Stloc_1);
                yield return new CodeInstruction(OpCodes.Br, label_save_loaded);
                yield return new CodeInstruction(OpCodes.Ldloc_1)
                {
                    labels = [label_load_normal_save]
                };
                yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(BinaryFormatter), nameof(BinaryFormatter.Deserialize), [typeof(Stream)]));
                yield return new CodeInstruction(OpCodes.Castclass, typeof(SaveSystem).Assembly.GetType("PlayerData"));
                yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(SaveSystem).Assembly.GetType("PlayerData"), "PlayerStats"));
                yield return new CodeInstruction(OpCodes.Stloc, local_player_save);
                
                to_skip = 13;
                load_file_patched = true;
                attach_label_save_loaded = true;
            }
            else
            {
                if (code.opcode == OpCodes.Ldc_I4_S && (sbyte)code.operand == 55 && prev.opcode == OpCodes.Ldfld && (FieldInfo)prev.operand == AccessTools.Field(typeof(SaveSystem.PlayerSave), "Quests"))
                {
                    sprite_code_found = true;
                }
                if (sprite_code_found && code.opcode == OpCodes.Ldloc_2)
                {
                    sprite_code_found = false;

                    yield return new CodeInstruction(OpCodes.Ldloc, local_is_archipelago)
                    {
                        labels = new List<Label>(code.labels)
                    };
                    yield return new CodeInstruction(OpCodes.Brfalse, label_no_ap_sprite);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(SaveButton), nameof(SaveButton.Icons)));
                    yield return new CodeInstruction(OpCodes.Ldc_I4, 13);
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(List<SpriteRenderer>), "get_Item"));
                    yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(ResourceManager), nameof(ResourceManager.ArchipelagoSprite)));
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(SpriteRenderer), "set_sprite"));

                    yield return new CodeInstruction(OpCodes.Ldloc, local_player_save)
                    {
                        labels = [label_no_ap_sprite]
                    };
                    to_skip = 1;
                }
                else if (code.opcode == OpCodes.Ldloc_2)
                {
                    yield return new CodeInstruction(OpCodes.Ldloc, local_player_save)
                    {
                        labels = new(code.labels)
                    };
                    to_skip = 1;
                }
                else if (to_skip > 0)
                {
                    to_skip--;
                }
                else
                {
                    if (attach_label_save_loaded)
                    {
                        code.labels.Add(label_save_loaded);
                        attach_label_save_loaded = false;
                    }
                    yield return code;
                }
            }

            prev = code;
        }
    }
}

[HarmonyPatch(typeof(SaveSystem))]
[HarmonyPatch(nameof(SaveSystem.Load))]
class SaveSystem_Load_Patch
{
    static bool Prefix(SaveSystem __instance, SaveSystem.Slot SS, ref string ___ID)
    {
        switch (SS)
        {
            case SaveSystem.Slot.Slot1:
                ___ID = "1";
                break;
            case SaveSystem.Slot.Slot2:
                ___ID = "2";
                break;
            case SaveSystem.Slot.Slot3:
                ___ID = "3";
                break;
        }
        __instance.SaveSlot = SS;

        Archipelago.Enabled = false;
        
        if (File.Exists(Application.persistentDataPath + "/APS" + ___ID + ".syr"))
        {
            var save = ArchipelagoSave.Deserialize(___ID);
            __instance.Data = save.GameData;
            Archipelago.AP.Data = save.ArchipelagoData;
            Archipelago.AP.Stats = save.ArchipelagoStats;
            switch (__instance.Data.CurrentAct)
            {
                case SaveSystem.Act.Intro:
                case SaveSystem.Act.Pre1:
                    __instance.Data.Day = 1;
                    break;
                default:
                    __instance.Data.Day++;
                    break;
            }
            Archipelago.Enabled = true;
            Menu.ArchipelagoConnect(false);
            return false;
        }
        
        return true;
    }
}

[HarmonyPatch(typeof(SaveSystem))]
[HarmonyPatch(nameof(SaveSystem.CreateSave))]
class SaveSystem_CreateSave_Patch
{
    static bool Prefix(SaveSystem __instance, SaveSystem.Slot SS, ref string ___ID)
    {
        switch (SS)
        {
            case SaveSystem.Slot.Slot1:
                ___ID = "1";
                break;
            case SaveSystem.Slot.Slot2:
                ___ID = "2";
                break;
            case SaveSystem.Slot.Slot3:
                ___ID = "3";
                break;
        }
        __instance.SaveSlot = SS;
        __instance.Data = new();
        Menu.NewGame(SS);
        return false;
    }
}

[HarmonyPatch(typeof(SaveSystem))]
[HarmonyPatch(nameof(SaveSystem.Save))]
class SaveSystem_Save_Patch
{
    static bool Prefix(SaveSystem __instance, ref string ___ID)
    {
        if (Archipelago.Enabled)
        {
            switch (__instance.SaveSlot)
            {
                case SaveSystem.Slot.Slot1:
                    ___ID = "1";
                    break;
                case SaveSystem.Slot.Slot2:
                    ___ID = "2";
                    break;
                case SaveSystem.Slot.Slot3:
                    ___ID = "3";
                    break;
            }
            var save = new ArchipelagoSave {
                GameData = __instance.Data,
                ArchipelagoData = Archipelago.AP.Data,
                ArchipelagoStats = Archipelago.AP.Stats,
            };
            save.Serialize(___ID);
            return false;
        }

        return true;
    }
}

[HarmonyPatch(typeof(SaveSystem))]
[HarmonyPatch(nameof(SaveSystem.InGameSave))]
class SaveSystem_InGameSave_Patch
{
    static bool Prefix(SaveSystem __instance)
    {
        __instance.Save();
        return false;
    }
}

[HarmonyPatch(typeof(SaveSystem))]
[HarmonyPatch(nameof(SaveSystem.QuitSave))]
class SaveSystem_QuitSave_Patch
{
    static bool Prefix(SaveSystem __instance)
    {
        __instance.Save();
        GameMaster.GM.ReturnToMain();
        return false;
    }
}

[HarmonyPatch(typeof(SaveSystem))]
[HarmonyPatch(nameof(SaveSystem.NewSave))]
class SaveSystem_NewSave_Patch
{
    static bool Prefix(SaveSystem __instance, string name)
    {
        if (Archipelago.Enabled)
        {
            __instance.Data = new SaveSystem.PlayerSave
            {
                Name = name,
                Chests = __instance.Data.Chests,
            };
            AccessTools.Method(typeof(SaveSystem), "PopulateData").Invoke(__instance, []);
            Archipelago.AP.InitialPatches();
            __instance.Save();
            GameMaster.GM.LoadIntoGame();
            return false;
        }

        return true;
    }
}

[HarmonyPatch(typeof(SaveSystem))]
[HarmonyPatch(nameof(SaveSystem.DeleteSave))]
class SaveSystem_DeleteSave_Patch
{
    static void Prefix(int SID)
    {
        File.Delete(Application.persistentDataPath + "/APS" + SID + ".syr");
    }
}

[HarmonyPatch(typeof(SaveSystem))]
[HarmonyPatch(nameof(SaveSystem.ForceDelete))]
class SaveSystem_ForceDelete_Patch
{
    static bool Prefix(SaveSystem __instance)
    {
        int SID = 0;
        switch (__instance.SaveSlot)
        {
            case SaveSystem.Slot.Slot1:
                SID = 1;
                break;
            case SaveSystem.Slot.Slot2:
                SID = 2;
                break;
            case SaveSystem.Slot.Slot3:
                SID = 3;
                break;
        }

        __instance.DeleteSave(SID);
        return false;
    }
}

[HarmonyPatch(typeof(SaveSystem))]
[HarmonyPatch(nameof(SaveSystem.FileCheck))]
class SaveSystem_FileCheck_Patch
{
    static void Postfix(int ID, ref bool __result)
    {
        if (File.Exists(Application.persistentDataPath + "/APS" + ID + ".syr"))
        {
            __result = true;
        }
    }
}

public class ArchipelagoSave
{
    public SaveSystem.PlayerSave GameData;
    public Archipelago.SaveData ArchipelagoData;
    public ArchipelagoStats ArchipelagoStats;

    public static ArchipelagoSave Deserialize(string id)
    {
        string fileName = Application.persistentDataPath + "/APS" + id + ".syr";
        try
        {
            string fileText = File.ReadAllText(fileName);
            return JsonConvert.DeserializeObject<ArchipelagoSave>(fileText);
        }
        catch (JsonException)
        {
            // Use old save file format
            BinaryFormatter binaryFormatter = new();
            FileStream fileStream = File.Open(fileName, FileMode.Open);
            var oldSave = (CompoundSave)binaryFormatter.Deserialize(fileStream);
            fileStream.Close();
            var save = new ArchipelagoSave
            {
                GameData = oldSave.GameData,
                ArchipelagoData = oldSave.ArchipelagoData,
                ArchipelagoStats = new()
                {
                    Enabled = false,
                },
            };
            // Resave in new format
            save.Serialize(id);
            return save;
        }
    }

    public void Serialize(string id)
    {
        string fileName = Application.persistentDataPath + "/APS" + id + ".syr";
        string fileText = JsonConvert.SerializeObject(this);
        File.WriteAllText(fileName, fileText);
    }
}
