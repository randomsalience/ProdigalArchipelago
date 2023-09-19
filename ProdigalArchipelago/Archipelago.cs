using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Packets;

namespace ProdigalArchipelago
{
    public class Archipelago : MonoBehaviour
    {
        public const string GAME_NAME = "Prodigal";
        public const string VERSION = "0.4.2";
        public const long ID_BASE = 77634425000;

        public const int TESS_ID = 48;
        public const int ARMADEL_ID = 57;

        public const int RUST_KNUCKLE_ID = 5;
        public const int DREAD_HAND_ID = 6;
        public const int IRON_PICK_ID = 9;
        public const int BLESSED_PICK_ID = 10;
        public const int BOLT_ANCHOR_ID = 11;
        public const int HEART_ORE_ID = 70;
        public const int EMPOWERED_HAND_ID = 76;
        public const int FLARE_KNUCKLE_ID = 77;
        public const int AP_ITEM_ID = 101;
        public const int PROGRESSIVE_KNUCKLE_ID = 102;
        public const int PROGRESSIVE_HAND_ID = 103;
        public const int PROGRESSIVE_PICK_ID = 104;
        public const int KEY_ID_START = 105;
        public static readonly int[] BOOTS_IDS = {12, 13, 14, 15, 16};

        public static readonly int[] KEY_SCENES = {5, 12, 6, 8, 10, 9, 13, 11, 24, 16, 19, 17, 27};
        public static readonly string[] KEY_DUNGEONS = {"BONEYARD", "TIDAL MINES", "CROCASINO", "HOWLING BJERG", "CASTLE VANN",
            "MAGMA HEART", "TIME OUT", "LIGHTHOUSE", "CRYSTAL CAVES", "HAUNTED HALL", "SISKA'S WORKSHOP", "BACKROOMS", "PIRATE'S PIER"};

        private static readonly int[] LOCS_BASE = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
            21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48,
            49, 50, 51, 52, 53, 62, 63, 65, 66, 67, 68, 69, 70, 71, 72, 73, 76, 81, 82, 83, 84, 85, 86, 87, 88, 89, 91, 92,
            93, 94, 95, 96, 97, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 125, 126, 127,
            128, 129, 130, 131, 132, 133, 134, 135, 136, 142, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155,
            156, 157, 158, 159, 160, 161, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 177, 178, 179, 180,
            181, 182, 183, 200, 201, 202, 203, 204, 205, 208, 209, 210, 211, 212, 213, 224, 225, 226, 227, 228, 229, 230,
            231, 232, 233, 234, 235, 236, 237, 238, 239};
        private static readonly int[] LOCS_GRELIN = {240, 241, 242, 243};
        private static readonly int[] LOCS_TRADE = {206, 207, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223};
        private const int LOC_ULNI = 223;
        private static readonly int[] LOCS_HIDDEN = {0, 137, 138, 139, 140, 141, 162, 175, 176, 184};
        private static readonly int[] LOCS_DIVE = {98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 124, 244};
        private static readonly int[] LOCS_ENLIGHTENMENT = {64, 74, 75, 77, 78, 79, 80, 90};

        public static bool Enabled;
        public static Archipelago AP;
        private static GameObject Obj;

        private bool _connected;
        public bool Connected
        {
            get { return _connected; }
            set
            {
                _connected = value;
                UIPatch.ErrorIndicator.SetActive(!_connected);
            }
        }

        public string Error = "";
        public int SlotID;
        public ArchipelagoSettings Settings;
        private ArchipelagoSession Session;
        private readonly List<Location> LocationTable = new();
        private int CheatItemsReceived;

        [Serializable]
        public class SaveData
        {
            public ConnectionData Connection;
            public List<(int, long)> ReceivedItemLocations = new();
            public int CheatItemCount = 0;
            public List<int> KeyTotals = new();
        }

        public SaveData Data = new();

        public static void Setup()
        {
            Obj = new();
            DontDestroyOnLoad(Obj);
            AP = Obj.AddComponent<Archipelago>();
        }

        public IEnumerator Connect(ConnectionData cdata, bool reconnect = false)
        {
            Connected = false;

            Session = ArchipelagoSessionFactory.CreateSession(cdata.HostName, cdata.Port);

            var connectTask = Task.Run(Session.ConnectAsync);
            yield return new WaitUntil(() => connectTask.IsCompleted);
            if (!connectTask.IsCompletedSuccessfully)
            {
                Error = $"Failed to connect to Archipelago server at {cdata.HostName}:{cdata.Port}";
                yield break;
            }

            var loginTask = Task.Run(async () => await Session.LoginAsync(GAME_NAME, cdata.SlotName, ItemsHandlingFlags.IncludeStartingInventory, new Version(VERSION), password: cdata.Password));
            yield return new WaitUntil(() => loginTask.IsCompleted);
            if (loginTask.IsFaulted)
            {
                Error = loginTask.Exception.GetBaseException().Message;
                yield break;
            }

            LoginResult result = loginTask.Result;
            if (!result.Successful)
            {
                LoginFailure failure = (LoginFailure)result;
                Error = string.Join("\n", failure.Errors);
                yield break;
            }

            LoginSuccessful login = (LoginSuccessful)result;
            SlotID = login.Slot;
            Session.Socket.SocketClosed += OnSocketClosed;

            if (!reconnect)
            {
                Settings = new ArchipelagoSettings(login.SlotData);
                BuildLocationTable();

                // Scout unchecked locations
                var uncheckedLocationIDs = from location in LocationTable where !location.Checked() select ID_BASE + location.ID;
                var locationInfoTask = Task.Run(async () => await Session.Locations.ScoutLocationsAsync(false, uncheckedLocationIDs.ToArray()));
                yield return new WaitUntil(() => locationInfoTask.IsCompleted);
                if (locationInfoTask.IsFaulted)
                {
                    Error = locationInfoTask.Exception.GetBaseException().Message;
                    yield break;
                }
                
                var locationInfo = locationInfoTask.Result;
                foreach ((var location, var info) in
                    from location in LocationTable
                    join info in locationInfo.Locations
                    on ID_BASE + location.ID equals info.Location
                    select (location, info))
                {
                    location.Item = new ArchipelagoItem(info.Item, Session.Items.GetItemName(info.Item), info.Player, Session.Players.GetPlayerName(info.Player), info.Flags);
                }
            }

            // Sync checked locations
            var checkedLocationIDs = from location in LocationTable where location.Checked() select ID_BASE + location.ID;
            var locationCheckTask = Task.Run(async () => await Session.Locations.CompleteLocationChecksAsync(checkedLocationIDs.ToArray()));
            yield return new WaitUntil(() => locationCheckTask.IsCompleted);
            if (locationCheckTask.IsFaulted)
            {
                Error = locationCheckTask.Exception.GetBaseException().Message;
                yield break;
            }

            Connected = true;
            Error = "";
            Data.Connection = cdata;
            CheatItemsReceived = 0;
        }

        public void Disconnect()
        {
            Session.Socket.DisconnectAsync();
            Session = null;
            Connected = false;
        }

        public void OnSocketClosed(string _)
        {
            if (Connected)
            {
                Connected = false;
                StartCoroutine(TryReconnect());
            }
        }

        private IEnumerator TryReconnect()
        {
            while (!Connected)
            {
                yield return new WaitForSeconds(60);
                yield return Connect(Data.Connection, true);
            }
        }

        private void Update()
        {
            if (Session is not null && Session.Items.Any() && CanReceiveItem())
            {
                var item = Session.Items.DequeueItem();
                if (!Data.ReceivedItemLocations.Contains((item.Player, item.Location)) && item.Location != -1)
                {
                    Data.ReceivedItemLocations.Add((item.Player, item.Location));
                    StartCoroutine(ReceiveItem((int)(item.Item - ID_BASE)));
                }
                else if (item.Location == -1)
                {
                    CheatItemsReceived++;
                    if (CheatItemsReceived >= Data.CheatItemCount)
                    {
                        Data.CheatItemCount = CheatItemsReceived;
                        StartCoroutine(ReceiveItem((int)(item.Item - ID_BASE)));
                    }
                }
            }
        }

        private bool CanReceiveItem()
        {
            return GameMaster.GM.GS == GameMaster.GameState.IN_GAME && !GameMaster.GM.UI.SPEAKING();
        }

        public void InitialPatches()
        {
            for (int i = 0; i < 18; i++)
            {
                GameMaster.GM.Save.Data.Inventory.Add(new SaveSystem.Item());
            }

            GameMaster.GM.Save.Data.CurrentAct = SaveSystem.Act.Act2;
            GameMaster.GM.Save.OverallData.ACHIEVEMENTS[3] = SaveSystem.Quest.QUESTCOMPLETE; // Game beaten
            GameMaster.GM.Save.Data.UnlockedDoors.Add(32); // Siska's Workshop open
            GameMaster.GM.Save.Data.Quests[40] = SaveSystem.Quest.QUESTCOMPLETE; // Backrooms open
            GameMaster.GM.Save.Data.Relationships[TESS_ID].Stage = SaveSystem.NPCData.Stages.STAGE2; // Tess gives HH key
            GameMaster.GM.Save.Data.Quests[54] = SaveSystem.Quest.QUESTCOMPLETE; // Harmonica tune remembered
            GameMaster.GM.Save.Data.Quests[45] = SaveSystem.Quest.QUESTCOMPLETE; // Back of Crystal Caves open
            GameMaster.GM.Save.Data.OverworldState.Add(9); // Skip Revulan dock scene

            if (Settings.SkipOneSmallFavor)
            {
                GameMaster.GM.Save.Data.Quests[2] = SaveSystem.Quest.STAGE16;
            }

            if (Settings.StartWithSpicedHam)
            {
                GameMaster.GM.Save.Data.SLOTA = PlayerCharacter.BUFFS.RUN;
            }

            ColorCheck();

            Data.ReceivedItemLocations.Clear();

            Data.KeyTotals.Clear();
            for (int i = 0; i < KEY_DUNGEONS.Count(); i++)
                Data.KeyTotals.Add(0);
        }

        public bool CollectItem(int locationID)
        {
            if (!IsLocationRandomized(locationID))
                return false;
            
            if (Data.ReceivedItemLocations.Contains((SlotID, ID_BASE + locationID)))
                return true;

            if (Connected)
            {
                StartCoroutine(SendCheck(ID_BASE + locationID));
            }

            GameMaster.GM.Save.Data.Chests.Add(locationID);
            GameMaster.GM.Save.Save();

            foreach (Location location in LocationTable)
            {
                if (location.ID == locationID)
                {
                    if (location.Item.SlotID == SlotID)
                        Data.ReceivedItemLocations.Add((SlotID, locationID));
                    StartCoroutine(ReceiveItem(location.Item.LocalID(), location.Item.Speech(), location.Item.SpriteID()));
                    return true;
                }
            }

            return true;
        }

        private IEnumerator SendCheck(long locationID)
        {
            var task = Task.Run(async () => await Session.Locations.CompleteLocationChecksAsync(locationID));
            yield return new WaitUntil(() => task.IsCompleted);
            if (task.IsCompletedSuccessfully) yield break;

            if (Connected)
            {
                Connected = false;
                StartCoroutine(TryReconnect());
            }
        }

        public int GetLocationItem(int locationID)
        {
            foreach (Location location in LocationTable)
            {
                if (location.ID == locationID)
                {
                    return location.Item.LocalID();
                }
            }

            return 0;
        }

        private IEnumerator ReceiveItem(int id, List<GameMaster.Speech> apSpeech = null, int spriteID = -1)
        {
            if (id == PROGRESSIVE_KNUCKLE_ID)
            {
                if (!GameMaster.GM.Save.Data.Inventory[RUST_KNUCKLE_ID].Acquired)
                {
                    id = RUST_KNUCKLE_ID;
                }
                else
                {
                    id = FLARE_KNUCKLE_ID;
                }
            }

            if (id == PROGRESSIVE_HAND_ID)
            {
                if (!GameMaster.GM.Save.Data.Inventory[DREAD_HAND_ID].Acquired)
                {
                    id = DREAD_HAND_ID;
                }
                else
                {
                    id = EMPOWERED_HAND_ID;
                }
            }

            if (id == PROGRESSIVE_PICK_ID)
            {
                if (!GameMaster.GM.Save.Data.Inventory[IRON_PICK_ID].Acquired)
                {
                    id = IRON_PICK_ID;
                }
                else if (!GameMaster.GM.Save.Data.Inventory[BLESSED_PICK_ID].Acquired)
                {
                    id = BLESSED_PICK_ID;
                }
                else
                {
                    id = BOLT_ANCHOR_ID;
                }
            }

            if (id >= KEY_ID_START && id < KEY_ID_START + KEY_DUNGEONS.Count())
            {
                Data.KeyTotals[id - KEY_ID_START]++;
            }

            if (id != AP_ITEM_ID)
            {
                GameMaster.GM.Save.AddToInventory(id, true);
            }

            if (spriteID == -1)
            {
                spriteID = id;
            }

            ColorCheck();

            bool cutscene = GameMaster.GM.GS == GameMaster.GameState.CUTSCENE;
            GameMaster.GM.PC.CUTSCENE(true);
            GameMaster.GM.PC.EMOTE.ITEM_PICKUP(GameMaster.GM.ItemData.Database[spriteID].ItemSprite, true);
            GameMaster.GM.PC.Anim.SetBool("ITEM", true);

            if (id == HEART_ORE_ID)
            {
                GameMaster.GM.PC.Heal(1);
                GameMaster.GM.Save.Data.Ores++;
                List<GameMaster.Speech> speech = new();
                switch (GameMaster.GM.Save.Data.Ores)
                {
                    case 4:
                        speech.Add(GameMaster.CreateSpeech(46, 0, "A FRAGMENT OF HEART ORE! NOW MY VITALITY WILL BE INCREASED!", "", 0));
                        GameMaster.GM.Save.Data.Ores = 0;
                        if (GameMaster.GM.Save.Data.Quests[39] == SaveSystem.Quest.QUESTCOMPLETE)
                        {
                            GameMaster.GM.BGM.PlayJingle(21);
                        }
                        else
                        {
                            GameMaster.GM.BGM.PlayJingle(20);
                            GameMaster.GM.PC.IncreaseMaxpHP();
                        }
                        break;
                    case 3:
                        speech.Add(GameMaster.CreateSpeech(46, 0, "A FRAGMENT OF HEART ORE! ONLY ONE MORE FRAGMENT TO GO!", "", 0));
                        GameMaster.GM.BGM.PlayJingle(21);
                        break;
                    case 2:
                        speech.Add(GameMaster.CreateSpeech(46, 0, "A FRAGMENT OF HEART ORE! ONLY TWO MORE FRAGMENTS TO GO!", "", 0));
                        GameMaster.GM.BGM.PlayJingle(21);
                        break;
                    case 1:
                        speech.Add(GameMaster.CreateSpeech(46, 0, "A FRAGMENT OF HEART ORE! ONLY THREE MORE FRAGMENTS TO GO!", "", 0));
                        GameMaster.GM.BGM.PlayJingle(21);
                        break;
                }
                GameMaster.GM.UI.InitiateChat(speech, false);
            }
            else
            {
                if (GameMaster.GM.ItemData.Database[id].MAJOR)
                {
                    GameMaster.GM.BGM.PlayJingle(20);
                }
                else
                {
                    GameMaster.GM.BGM.PlayJingle(21);
                }
                switch (id)
                {
                    case 1:
                        GameMaster.GM.Save.AddCurrency(1);
                        break;
                    case 2:
                        GameMaster.GM.Save.AddCurrency(25);
                        break;
                    case 3:
                        GameMaster.GM.Save.AddCurrency(50);
                        break;
                    case 4:
                        GameMaster.GM.Save.AddCurrency(100);
                        break;
                }
                if (id == AP_ITEM_ID)
                {
                    GameMaster.GM.UI.InitiateChat(apSpeech ?? new List<GameMaster.Speech>(), false);
                }
                else
                {
                    GameMaster.GM.UI.InitiateChat(GameMaster.GM.ItemData.Database[id].AboutText, false);
                }
            }

            while (GameMaster.GM.UI.SPEAKING())
            {
                yield return null;
            }

            GameMaster.GM.CUTSCENE(cutscene);
            GameMaster.GM.Save.Save();
        }

        public void Finish()
        {
            StartCoroutine(SendFinish());
        }

        private IEnumerator SendFinish()
        {
            while (true)
                {
                var packet = new StatusUpdatePacket() {
                    Status = ArchipelagoClientState.ClientGoal
                };
                var task = Task.Run(async () => await Session.Socket.SendPacketAsync(packet));
                yield return new WaitUntil(() => task.IsCompleted);
                if (task.IsCompletedSuccessfully) yield break;

                Connected = false;
                while (!Connected)
                {
                    yield return new WaitForSeconds(60);
                    yield return Connect(Data.Connection, true);
                }
            }
        }

        private void BuildLocationTable()
        {
            LocationTable.Clear();
            List<int> locations = new(LOCS_BASE);
            if (Settings.TradingQuest == ArchipelagoSettings.TradingQuestOption.Shuffle)
                locations.AddRange(LOCS_TRADE);
            else if (Settings.TradingQuest == ArchipelagoSettings.TradingQuestOption.Vanilla)
                locations.Add(LOC_ULNI);
            if (Settings.ShuffleGrelinDrops)
                locations.AddRange(LOCS_GRELIN);
            if (Settings.ShuffleHiddenItems)
                locations.AddRange(LOCS_HIDDEN);
            if (Settings.ShuffleDaemonsDive)
                locations.AddRange(LOCS_DIVE);
            if (Settings.ShuffleEnlightenment)
                locations.AddRange(LOCS_ENLIGHTENMENT);

            foreach (int locationID in locations)
            {
                LocationTable.Add(new Location(locationID));
            }
        }

        public bool IsLocationRandomized(int locationID)
        {
            foreach (Location location in LocationTable)
            {
                if (location.ID == locationID)
                    return true;
            }
            return false;
        }

        public int ColorCount()
        {
            int colors = 0;
            foreach (int id in new int[] {43, 44, 53, 74, 75})
            {
                if (GameMaster.GM.Save.Data.Inventory[id].Acquired)
                    colors++;
            }
            return colors;
        }

        public int BlessingCount()
        {
            int blessings = 0;
            foreach (int id in new int[] {89, 90, 91, 92, 93})
            {
                if (GameMaster.GM.Save.Data.Inventory[id].Acquired)
                    blessings++;
            }
            return blessings;
        }

        public void ColorCheck()
        {
            if (ColorCount() >= Settings.ColorsRequired)
            {
                GameMaster.GM.Save.Data.Recolored = true;
            }
        }

        public void StartWarp()
        {
            StartCoroutine(Warp());
        }

        private static IEnumerator Warp()
        {
            GameMaster.GM.UnPauseGame();
            GameMaster.GM.GS = GameMaster.GameState.LOAD;
            GameMaster.GM.Fader.LoadScreen(true);
            while (GameMaster.GM.Fader.Status != 0)
            {
                yield return null;
            }
            if (GameMaster.GM.PC.CrystalKey)
            {
                GameMaster.GM.PC.CrystalKey.GetComponent<Pickup>().KeyBreak(false);
            }
            typeof(GameMaster).GetField("CurrentScene", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(GameMaster.GM, 2);
            var aload = SceneManager.LoadSceneAsync(2);
            typeof(GameMaster).GetField("ALoad", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(GameMaster.GM, aload);
            while (!aload.isDone)
            {
                yield return null;
            }
            GameMaster.GM.PC.transform.position = new Vector3(848, -830, 0);
            GameMaster.GM.PC.ForceLoadCheck();
            MotherBrain.MB.LoadOverworldNPCs();
            GameMaster.GM.Fader.LoadScreen(false);
            while (GameMaster.GM.Fader.Status != 0)
            {
                yield return null;
            }
            GameMaster.GM.UI.US = UI.UIState.NULL;
            GameMaster.GM.GS = GameMaster.GameState.IN_GAME;
            GameMaster.GM.PC.CUTSCENE(false);
        }
    }
}