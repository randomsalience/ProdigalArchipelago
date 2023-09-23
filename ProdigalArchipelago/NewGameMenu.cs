using System.Collections.Generic;
using UnityEngine;

namespace ProdigalArchipelago
{
    public class NewGameMenu : MonoBehaviour
    {
        private enum GameType
        {
            Normal,
            Archipelago,
        }

        public static GameObject Instance;
        
        private GameObject Bg;
        private List<GameObject> NormalText;
        private List<GameObject> ArchipelagoText;
        private GameObject Selector;
        private GameType Selected;

        public static void Create()
        {
            Instance = new GameObject("NewGameMenu");
            Instance.SetActive(false);
            var newGameMenu = Instance.AddComponent<NewGameMenu>();
            Instance.transform.parent = GameMaster.GM.UI.transform.GetChild(1);
            Instance.transform.localPosition = new Vector3(0, 0, 0);
            newGameMenu.Setup();
        }

        private void Setup()
        {
            Bg = new GameObject("NewGameBG");
            var bgSprite = Bg.AddComponent<SpriteRenderer>();
            bgSprite.sprite = SpriteManager.GameChoiceBGSprite;
            bgSprite.sortingOrder = 1;
            bgSprite.sortingLayerName = "UI";
            Bg.transform.parent = transform;
            Bg.transform.localPosition = new Vector3(0, 0, 0);

            NormalText = Menu.CreateTextObjects("Normal", 6, transform, -36, 6, new Color32(235, 223, 193, 255));
            ArchipelagoText = Menu.CreateTextObjects("Archipelago", 11, transform, -36, -6, new Color32(235, 223, 193, 255));

            Selector = new GameObject("GameTypeSelector");
            var selectorSprite = Selector.AddComponent<SpriteRenderer>();
            selectorSprite.sprite = SpriteManager.ArrowSprite;
            selectorSprite.sortingOrder = 2;
            selectorSprite.sortingLayerName = "UI";
            Selector.transform.parent = transform;
            Selector.transform.localPosition = new Vector3(-45, 6, 0);

            Selected = GameType.Normal;
        }

        private void Start()
        {
            Menu.RenderText(NormalText, "NORMAL");
            Menu.RenderText(ArchipelagoText, "ARCHIPELAGO");
        }

        private void Update()
        {
            if (InputManager.IM.UI_DIR.y > 0)
            {
                Selected = GameType.Normal;
                Selector.transform.localPosition = new Vector3(-45, 6, 0);
            }
            else if (InputManager.IM.UI_DIR.y < 0)
            {
                Selected = GameType.Archipelago;
                Selector.transform.localPosition = new Vector3(-45, -6, 0);
            }
        }

        public void Select()
        {
            switch (Selected)
            {
                case GameType.Normal:
                    Archipelago.Enabled = false;
                    Menu.StartNormalGame();
                    break;
                case GameType.Archipelago:
                    Archipelago.Enabled = true;
                    Archipelago.AP.Data = new();
                    Menu.ArchipelagoConnect(true);
                    break;
            }
        }
    }
}