using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using HarmonyLib;

namespace ProdigalArchipelago;

public class Menu
{
    public enum NewUIState
    {
        Old,
        NewGame,
        ArchipelagoConnect,
    }

    public static NewUIState State;
    public static SaveSystem.Slot SaveSlot;

    public static void Setup()
    {
        State = NewUIState.Old;
    }

    public static void NewGame(SaveSystem.Slot SS)
    {
        State = NewUIState.NewGame;
        GameMaster.GM.UI.transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
        GameMaster.GM.UI.transform.GetChild(1).Find("NewGameMenu").gameObject.SetActive(true);
        SaveSlot = SS;
    }

    public static void StartNormalGame()
    {
        State = NewUIState.Old;
        GameMaster.GM.UI.transform.GetChild(1).Find("NewGameMenu").gameObject.SetActive(false);
        GameMaster.GM.UI.StartKeyboard(1);
    }

    public static void ArchipelagoConnect(bool newGame)
    {
        State = NewUIState.ArchipelagoConnect;
        ConnectionMenu.Instance.GetComponent<ConnectionMenu>().NewGame = newGame;
        GameMaster.GM.UI.transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
        GameMaster.GM.UI.transform.GetChild(1).Find("NewGameMenu").gameObject.SetActive(false);
        GameMaster.GM.UI.transform.GetChild(1).Find("ConnectionMenu").gameObject.SetActive(true);
    }

    public static void StartArchipelagoGame()
    {
        State = NewUIState.Old;
        GameMaster.GM.UI.transform.GetChild(1).Find("ConnectionMenu").gameObject.SetActive(false);
        UIPatch.StartArchipelago();
        GameMaster.GM.UI.StartKeyboard(1);
    }

    public static void LoadArchipelagoGame()
    {
        State = NewUIState.Old;
        GameMaster.GM.UI.transform.GetChild(1).Find("ConnectionMenu").gameObject.SetActive(false);
        UIPatch.StartArchipelago();
        GameMaster.GM.LoadIntoGame();
    }

    public static List<GameObject> CreateTextObjects(string name, int length, Transform parent, int x, int y, Color color)
    {
        var ret = new List<GameObject>();
        for (int i = 0; i < length; i++)
        {
            var letter = new GameObject($"{name}Letter{i}");
            var letterSprite = letter.AddComponent<SpriteRenderer>();
            letterSprite.sortingOrder = 9;
            letterSprite.sortingLayerName = "UI";
            letterSprite.color = color;
            letter.transform.parent = parent;
            letter.transform.localPosition = new Vector3(x + 6 * i, y, 0);
            ret.Add(letter);
        }
        return ret;
    }

    public static void RenderText(List<GameObject> textObjects, string text)
    {
        for (int i = 0; i < textObjects.Count; i++)
        {
            var letterSprite = textObjects[i].GetComponent<SpriteRenderer>();
            if (i < text.Length)
            {
                letterSprite.sprite = GameMaster.GM.UI.PULL_SPRITE(text[i], CHAT_BOX.TEXT_LANGUAGE.NORMAL);
            }
        }
    }
}

// Prevent WASD keys from moving the cursor on the connection menu
[HarmonyPatch(typeof(InputManager))]
[HarmonyPatch(nameof(InputManager.OnMove))]
class InputManager_OnMove_Patch
{
    static bool Prefix(ref InputAction.CallbackContext Cont)
    {
        if (GameMaster.GM.GS == GameMaster.GameState.UI && Menu.State == Menu.NewUIState.ArchipelagoConnect && ConnectionMenu.Instance.GetComponent<ConnectionMenu>().Typing())
        {
            var control = Cont.control;
            if (control.device is Keyboard && control.name != "upArrow" && control.name != "downArrow")
            {
                return false;
            }
        }
        return true;
    }
}

[HarmonyPatch(typeof(UI))]
[HarmonyPatch(nameof(UI.ReturnToMainMenu))]
class UI_ReturnToMainMenu_Patch
{
    static void Prefix()
    {
        if (Archipelago.Enabled && Archipelago.AP.Connected)
        {
            Archipelago.AP.Disconnect();
        }
        Archipelago.Enabled = false;
    }
}

[HarmonyPatch(typeof(UI))]
[HarmonyPatch(nameof(UI.StartButton))]
class UI_StartButton_Patch
{
    static bool Prefix()
    {
        switch (Menu.State)
        {
            case Menu.NewUIState.Old:
                return true;
            case Menu.NewUIState.NewGame:
                GameMaster.GM.UI.transform.GetChild(1).Find("NewGameMenu").gameObject.GetComponent<NewGameMenu>().Select();
                return false;
            case Menu.NewUIState.ArchipelagoConnect:
                GameMaster.GM.UI.transform.GetChild(1).Find("ConnectionMenu").gameObject.GetComponent<ConnectionMenu>().Select();
                return false;
        }

        return true;
    }
}

[HarmonyPatch(typeof(UI))]
[HarmonyPatch(nameof(UI.AButton))]
class UI_AButton_Patch
{
    static bool Prefix()
    {
        switch (Menu.State)
        {
            case Menu.NewUIState.Old:
                return true;
            case Menu.NewUIState.NewGame:
                GameMaster.GM.UI.transform.GetChild(1).Find("NewGameMenu").gameObject.GetComponent<NewGameMenu>().Select();
                return false;
            case Menu.NewUIState.ArchipelagoConnect:
                GameMaster.GM.UI.transform.GetChild(1).Find("ConnectionMenu").gameObject.GetComponent<ConnectionMenu>().Select();
                return false;
        }

        return true;
    }
}

[HarmonyPatch(typeof(UI))]
[HarmonyPatch(nameof(UI.BButton))]
class UI_BButton_Patch
{
    static bool Prefix()
    {
        switch (Menu.State)
        {
            case Menu.NewUIState.Old:
                return true;
            case Menu.NewUIState.NewGame:
                Menu.State = Menu.NewUIState.Old;
                GameMaster.GM.UI.transform.GetChild(1).Find("NewGameMenu").gameObject.SetActive(false);
                GameMaster.GM.UI.ReturnToMainMenu();
                return false;
            case Menu.NewUIState.ArchipelagoConnect:
                if (ConnectionMenu.Instance.GetComponent<ConnectionMenu>().Typing()) return false;
                Menu.State = Menu.NewUIState.Old;
                GameMaster.GM.UI.transform.GetChild(1).Find("ConnectionMenu").gameObject.SetActive(false);
                GameMaster.GM.UI.ReturnToMainMenu();
                return false;
        }

        return true;
    }
}

[HarmonyPatch(typeof(GameMaster))]
[HarmonyPatch(nameof(GameMaster.ReturnToMain))]
class GameMaster_ReturnToMain_Patch
{
    static void Prefix()
    {
        UIPatch.StopArchipelago();
        if (Archipelago.Enabled && Archipelago.AP.Connected)
        {
            Archipelago.AP.Disconnect();
        }
        Archipelago.Enabled = false;
    }
}