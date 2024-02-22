using System;
using System.Reflection;
using UnityEngine;
using BepInEx;
using HarmonyLib;

namespace ProdigalArchipelago;

public enum Button
{
    A,
    B,
    X,
    Y,
    Start,
    Trigger,
}

[BepInPlugin("com.randomsalience.prodigal.archipelago", "Archipelago", "0.2.1")]
[BepInProcess("Prodigal.exe")]
public class Plugin : BaseUnityPlugin
{
    private static Plugin Instance;
    public static new BepInEx.Logging.ManualLogSource Logger;
    public static Action<Button, bool> ButtonInput;

    private void Awake()
    {
        Instance = this;
        Logger = base.Logger;
        Archipelago.Enabled = false;
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        Application.wantsToQuit += CloseButtonPressed;
    }

    public static Version GetVersion()
    {
        return Instance.Info.Metadata.Version;
    }

    private static bool CloseButtonPressed()
    {
        if (Archipelago.Enabled && Archipelago.AP.Connected)
        {
            Archipelago.AP.Disconnect();
        }
        return true;
    }
}

[HarmonyPatch(typeof(GameMaster))]
[HarmonyPatch("Awake")]
class GameMaster_Awake_Patch
{
    static void Postfix()
    {
        ResourceManager.Load();
        Menu.Setup();
        UIPatch.Setup();
        Archipelago.Setup();
        NewGameMenu.Create();
        ConnectionMenu.Create();
        KeyPage.Create();
        StatsScreen.Create();
        MapTracker.Create();
        CastleSailor.Create();
        ArchipelagoConsole.Create();
    }
}

[HarmonyPatch(typeof(GameMaster))]
[HarmonyPatch(nameof(GameMaster.AButton))]
class GameMaster_AButton_Patch
{
    static bool Prefix(bool Up)
    {
        if (Plugin.ButtonInput is not null)
        {
            Plugin.ButtonInput(Button.A, Up);
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(GameMaster))]
[HarmonyPatch(nameof(GameMaster.BButton))]
class GameMaster_BButton_Patch
{
    static bool Prefix(bool Up)
    {
        if (Plugin.ButtonInput is not null)
        {
            Plugin.ButtonInput(Button.B, Up);
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(GameMaster))]
[HarmonyPatch(nameof(GameMaster.XButton))]
class GameMaster_XButton_Patch
{
    static bool Prefix(bool Up)
    {
        if (Plugin.ButtonInput is not null)
        {
            Plugin.ButtonInput(Button.X, Up);
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(GameMaster))]
[HarmonyPatch(nameof(GameMaster.YButton))]
class GameMaster_YButton_Patch
{
    static bool Prefix(bool Up)
    {
        if (Plugin.ButtonInput is not null)
        {
            Plugin.ButtonInput(Button.Y, Up);
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(GameMaster))]
[HarmonyPatch(nameof(GameMaster.StartButton))]
class GameMaster_StartButton_Patch
{
    static bool Prefix()
    {
        if (Plugin.ButtonInput is not null)
        {
            Plugin.ButtonInput(Button.Start, false);
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(GameMaster))]
[HarmonyPatch(nameof(GameMaster.TRIGGER_Button))]
class GameMaster_TRIGGER_Button_Patch
{
    static bool Prefix(bool Up)
    {
        if (Plugin.ButtonInput is not null)
        {
            Plugin.ButtonInput(Button.Trigger, Up);
            return false;
        }
        return true;
    }
}
