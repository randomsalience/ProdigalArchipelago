using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace ProdigalArchipelago
{
    public class ConnectionMenu : MonoBehaviour
    {
        public static GameObject Instance;
        public bool NewGame;

        private GameObject BG;
        private List<GameObject> ServerText;
        private List<GameObject> PortText;
        private List<GameObject> SlotText;
        private List<GameObject> PasswordText;
        private List<GameObject> StartText;
        private GameObject UI;
        private GameObject ServerEntry;
        private GameObject PortEntry;
        private GameObject SlotEntry;
        private GameObject PasswordEntry;
        private GameObject ErrorText;
        private int Selected = 0;
        private bool Delay = false;
        private bool Frozen = false;

        private static readonly Color TextColor = new Color32(130, 109, 95, 255);

        public static void Create()
        {
            Instance = new GameObject("ConnectionMenu");
            Instance.SetActive(false);
            var connectionMenu = Instance.AddComponent<ConnectionMenu>();
            Instance.transform.parent = GameMaster.GM.UI.transform.GetChild(1);
            Instance.transform.localPosition = new Vector3(0, 0, 0);
            connectionMenu.Setup();
        }

        private void Setup()
        {
            UI = new GameObject("ConnectionSetupUI");
            UI.AddComponent<EventSystem>();
            UI.AddComponent<InputSystemUIInputModule>();
            var canvas = UI.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingLayerName = "UI";
            canvas.sortingOrder = 3;
            var canvasScaler = UI.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1280, 720);
            UI.transform.SetParent(transform, false);
            UI.transform.localPosition = new Vector3(0, 0, 0);

            BG = new GameObject("ConnectionSetupBG");
            var bgSprite = BG.AddComponent<SpriteRenderer>();
            bgSprite.sprite = SpriteManager.ConnectionSetupBGSprite;
            bgSprite.sortingOrder = 1;
            bgSprite.sortingLayerName = "UI";
            BG.transform.parent = transform;
            BG.transform.localPosition = new Vector3(0, 0, 0);

            ServerText = Menu.CreateTextObjects("Server", 6, transform, -14, 42, TextColor);
            PortText = Menu.CreateTextObjects("Port", 4, transform, -8, 22, TextColor);
            SlotText = Menu.CreateTextObjects("Slot", 4, transform, -8, 2, TextColor);
            PasswordText = Menu.CreateTextObjects("Password", 8, transform, -20, -18, TextColor);
            StartText = Menu.CreateTextObjects("Start", 5, transform, -11, -38, TextColor);

            ServerEntry = CreateInputField("ServerEntry", 124, 0);
            PortEntry = CreateInputField("PortEntry", 24, 5);
            SlotEntry = CreateInputField("SlotEntry", -76, 16);
            PasswordEntry = CreateInputField("PasswordEntry", -176, 0);

            PortEntry.GetComponent<InputField>().contentType = InputField.ContentType.IntegerNumber;
            PasswordEntry.GetComponent<InputField>().contentType = InputField.ContentType.Password;

            ErrorText = new GameObject("ErrorText");
            var text = ErrorText.AddComponent<Text>();
            text.alignment = TextAnchor.UpperCenter;
            text.fontSize = 24;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.color = Color.red;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            ErrorText.transform.SetParent(UI.transform, false);
            ErrorText.transform.localPosition = new Vector3(0, -300, 0);
        }

        private void OnEnable()
        {
            Menu.RenderText(ServerText, "SERVER");
            Menu.RenderText(PortText, "PORT");
            Menu.RenderText(SlotText, "SLOT");
            Menu.RenderText(PasswordText, "PASSWORD");
            Menu.RenderText(StartText, "START");

            if (NewGame)
            {
                ServerEntry.GetComponent<InputField>().text = "archipelago.gg";
                PortEntry.GetComponent<InputField>().text = "";
                SlotEntry.GetComponent<InputField>().text = "";
                PasswordEntry.GetComponent<InputField>().text = "";
            }
            else
            {
                var cdata = Archipelago.AP.Data.Connection;
                ServerEntry.GetComponent<InputField>().text = cdata.HostName;
                PortEntry.GetComponent<InputField>().text = cdata.Port.ToString();
                SlotEntry.GetComponent<InputField>().text = cdata.SlotName;
                PasswordEntry.GetComponent<InputField>().text = cdata.Password;
            }

            ErrorText.GetComponent<Text>().text = "";

            StartCoroutine(ActivateInput());
        }

        private IEnumerator ActivateInput()
        {
            yield return new WaitForEndOfFrame();
            ChangeSelection(NewGame ? 0 : 4);
        }

        private void Update()
        {
            if (Delay || Frozen) return;

            if (InputManager.IM.UI_DIR.y > 0)
            {
                ChangeSelection(Selected - 1);
                StartCoroutine(StartDelay());
            }
            if (InputManager.IM.UI_DIR.y < 0)
            {
                ChangeSelection(Selected + 1);
                StartCoroutine(StartDelay());
            }
        }

        private IEnumerator StartDelay()
        {
            Delay = true;
            yield return new WaitForSeconds(0.2f);
            Delay = false;
        }

        private GameObject CreateInputField(string name, int y, int characterLimit)
        {
            var entry = new GameObject(name);
            entry.transform.SetParent(UI.transform, false);
            entry.transform.localPosition = new Vector3(0, y, 0);
            entry.AddComponent<CanvasRenderer>();
            var text = entry.AddComponent<Text>();
            text.alignment = TextAnchor.UpperCenter;
            text.fontSize = 24;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.color = Color.black;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            var input = entry.AddComponent<InputField>();
            input.textComponent = text;
            input.characterLimit = characterLimit;
            input.interactable = true;
            var fitter = entry.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            return entry;
        }

        private void ChangeSelection(int selected)
        {
            SetSelectedColor(TextColor);
            Selected = Math.Clamp(selected, 0, 4);
            var entry = Selected switch
            {
                0 => ServerEntry,
                1 => PortEntry,
                2 => SlotEntry,
                3 => PasswordEntry,
                _ => null,
            };
            entry?.GetComponent<InputField>()?.ActivateInputField();
            entry?.GetComponent<InputField>()?.Select();
            UI.GetComponent<EventSystem>().SetSelectedGameObject(entry);
            SetSelectedColor(new Color(0.37f, 0.80f, 0.89f));
        }

        private void SetSelectedColor(Color color)
        {
            var text = Selected switch
            {
                0 => ServerText,
                1 => PortText,
                2 => SlotText,
                3 => PasswordText,
                _ => StartText,
            };
            foreach (var letter in text)
            {
                letter.GetComponent<SpriteRenderer>().color = color;
            }
        }

        public void Select()
        {
            if (Typing()) return;

            string server = ServerEntry.GetComponent<InputField>().text;
            int.TryParse(PortEntry.GetComponent<InputField>().text, out int port);
            string slot = SlotEntry.GetComponent<InputField>().text;
            string password = PasswordEntry.GetComponent<InputField>().text;
            ConnectionData cdata = new(server, port, slot, password);
            StartCoroutine(Connect(cdata));
        }

        private IEnumerator Connect(ConnectionData cdata)
        {
            Frozen = true;
            ErrorText.GetComponent<Text>().text = "";
            yield return Archipelago.AP.Connect(cdata);
            if (Archipelago.AP.Connected)
            {
                if (NewGame)
                {
                    Menu.StartArchipelagoGame();
                }
                else
                {
                    Menu.LoadArchipelagoGame();
                }
            }
            else
            {
                ErrorText.GetComponent<Text>().text = Archipelago.AP.Error;
            }
            Frozen = false;
        }

        public bool Typing()
        {
            return Frozen || Selected != 4;
        }
    }
}