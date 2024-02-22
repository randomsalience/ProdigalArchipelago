using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using Archipelago.MultiClient.Net.MessageLog.Messages;

namespace ProdigalArchipelago;

public class ArchipelagoConsole : MonoBehaviour
{
    public static GameObject Instance;

    private static InputAction ConsoleAction;
    private static GameMaster.GameState PreviousGameState;
    private static UI.UIState PreviousUIState;

    private GameObject BG;
    private GameObject Canvas;
    private GameObject TextBox;
    private GameObject InputBox;
    private InputAction SubmitAction;

    public static void Create()
    {
        Instance = new("Console");
        Instance.SetActive(false);
        Instance.transform.SetParent(GameMaster.GM.UI.transform);
        Instance.AddComponent<ArchipelagoConsole>().Setup();

        ConsoleAction = new();
        ConsoleAction.AddBinding("<Keyboard>/f1");
        ConsoleAction.Enable();
        ConsoleAction.started += OnConsoleToggled;
    }

    private static void OnConsoleToggled(InputAction.CallbackContext _)
    {
        if (Instance.activeSelf)
        {
            Instance.SetActive(false);
            GameMaster.GM.GS = PreviousGameState;
            GameMaster.GM.UI.US = PreviousUIState;
            if (PreviousGameState == GameMaster.GameState.IN_GAME)
            {
                Time.timeScale = 1.0f;
                GameMaster.GM.TimeMulti = 1;
            }
        }
        else if (Archipelago.Enabled && Archipelago.AP.Session is not null)
        {
            PreviousGameState = GameMaster.GM.GS;
            PreviousUIState = GameMaster.GM.UI.US;
            Instance.SetActive(true);
            GameMaster.GM.GS = GameMaster.GameState.UI;
            GameMaster.GM.UI.US = UI.UIState.LOCKED;
            Time.timeScale = 0.0f;
            GameMaster.GM.TimeMulti = 0;
        }
    }

    private void Setup()
    {
        BG = new("ConsoleBG");
        BG.transform.SetParent(transform);
        var bgSprite = BG.AddComponent<SpriteRenderer>();
        bgSprite.sprite = ResourceManager.ConsoleBGSprite;
        bgSprite.sortingLayerName = "UI";
        bgSprite.sortingOrder = 20;

        Canvas = new("ConsoleCanvas");
        Canvas.transform.SetParent(transform);
        var canvas = Canvas.AddComponent<Canvas>();
        Canvas.AddComponent<EventSystem>();
        Canvas.AddComponent<InputSystemUIInputModule>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingLayerName = "UI";
        canvas.sortingOrder = 21;
        var canvasScaler = Canvas.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1280, 720);

        TextBox = new("ConsoleText");
        TextBox.transform.SetParent(Canvas.transform);
        TextBox.transform.localPosition = new Vector3(0, 30, 0);
        var text = TextBox.AddComponent<Text>();
        text.color = Color.white;
        text.fontSize = ResourceManager.GetFontSize();
        text.font = ResourceManager.GetFont();
        text.lineSpacing = 1.25f;
        var textRect = TextBox.GetComponent<RectTransform>();
        textRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1200);
        textRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 540);

        InputBox = new("ConsoleInput");
        InputBox.transform.SetParent(Canvas.transform);
        InputBox.transform.localPosition = new Vector3(0, -340, 0);
        InputBox.AddComponent<CanvasRenderer>();
        var inputText = InputBox.AddComponent<Text>();
        inputText.color = Color.white;
        inputText.fontSize = ResourceManager.GetFontSize();
        inputText.font = ResourceManager.GetFont();
        var input = InputBox.AddComponent<InputField>();
        input.textComponent = inputText;
        input.interactable = true;
        var inputRect = InputBox.GetComponent<RectTransform>();
        inputRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1200);

        SubmitAction = new();
        SubmitAction.AddBinding("<Keyboard>/enter");
        SubmitAction.started += Submit;
    }

    private void OnEnable()
    {
        StartCoroutine(ActivateInput());
        SubmitAction.Enable();
    }

    private void OnDisable()
    {
        SubmitAction.Disable();
    }

    private IEnumerator ActivateInput()
    {
        yield return new WaitForEndOfFrame();
        var input = InputBox.GetComponent<InputField>();
        input.ActivateInputField();
        input.Select();
    }

    private void Update()
    {
        var text = TextBox.GetComponent<Text>();
        var messages = from msg in Archipelago.AP.MessageLog select MessageString(msg);
        int firstIndex = 0;
        text.text = string.Join('\n', messages);
        while (text.preferredHeight > 540)
        {
            firstIndex++;
            text.text = string.Join('\n', messages.Skip(firstIndex));
        }
    }

    private string MessageString(LogMessage message)
    {
        var parts = from part in message.Parts select
            $"<color=#{part.Color.R:X2}{part.Color.G:X2}{part.Color.B:X2}>{part.Text}</color>";
        return string.Join("", parts);
    }

    private void Submit(InputAction.CallbackContext _)
    {
        var input = InputBox.GetComponent<InputField>();
        Archipelago.AP.SendMessageToServer(input.text);
        input.text = "";
        StartCoroutine(ActivateInput());
    }

    public void UpdateFontSize()
    {
        TextBox.GetComponent<Text>().fontSize = ResourceManager.GetFontSize();
        TextBox.GetComponent<Text>().font = ResourceManager.GetFont();
        InputBox.GetComponent<Text>().fontSize = ResourceManager.GetFontSize();
        InputBox.GetComponent<Text>().font = ResourceManager.GetFont();
    }
}