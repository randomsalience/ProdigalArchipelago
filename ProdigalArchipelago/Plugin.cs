using System;
using System.Reflection;
using UnityEngine;
using BepInEx;
using HarmonyLib;

namespace ProdigalArchipelago;

[BepInPlugin("com.randomsalience.prodigal.archipelago", "Archipelago", "0.1.4")]
[BepInProcess("Prodigal.exe")]
public class Plugin : BaseUnityPlugin
{
    private static Plugin Instance;
    public static new BepInEx.Logging.ManualLogSource Logger;

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
    private static void Postfix()
    {
        SpriteManager.LoadSprites();
        Menu.Setup();
        UIPatch.Setup();
        Archipelago.Setup();
        NewGameMenu.Create();
        ConnectionMenu.Create();
        KeyPage.Create();
        MapTracker.Create();
        CastleSailor.Create();
    }
}