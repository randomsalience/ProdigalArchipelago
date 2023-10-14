using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using HarmonyLib;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Packets;

namespace ProdigalArchipelago;

public class Archipelago : MonoBehaviour
{
    public const string GAME_NAME = "Prodigal";
    public const string VERSION = "0.4.2";
    public const long ID_BASE = 77634425000;

    public const int CAROLINE_ID = 5;
    public const int TESS_ID = 48;
    public const int ARMADEL_ID = 57;

    private static readonly int[] LOCS_BASE = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
        21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48,
        49, 50, 51, 52, 53, 62, 63, 65, 66, 67, 68, 69, 70, 71, 72, 73, 76, 81, 82, 83, 84, 85, 86, 87, 88, 89, 91, 92,
        93, 94, 95, 96, 97, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 125, 126, 127,
        128, 129, 130, 131, 132, 133, 134, 135, 136, 142, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155,
        156, 157, 158, 159, 160, 161, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 177, 178, 179, 180,
        181, 182, 183, 200, 201, 202, 203, 204, 205, 208, 209, 210, 211, 212, 213, 224, 225, 226, 227, 228, 229, 230,
        231, 232, 233, 234, 235, 236, 237, 238, 239];
    private static readonly int[] LOCS_GRELIN = [240, 241, 242, 243];
    private static readonly int[] LOCS_TRADE = [206, 207, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223];
    private const int LOC_ULNI = 223;
    private static readonly int[] LOCS_HIDDEN = [0, 137, 138, 139, 140, 141, 162, 175, 176, 184];
    private static readonly int[] LOCS_CASTLE = [54, 55, 56, 57, 58, 59, 60, 61, 248];
    private static readonly int[] LOCS_DIVE = [98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 124, 244];
    private static readonly int[] LOCS_ENLIGHTENMENT = [64, 74, 75, 77, 78, 79, 80, 90];
    public static readonly int[] LOCS_SECRET_SHOP = [245, 246, 247];

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
    public string SlotName;
    public ArchipelagoSettings Settings;
    public ArchipelagoSession Session;
    public Dictionary<string, object> SlotData;
    private System.Random Random;
    private readonly SortedDictionary<int, ArchipelagoItem> LocationTable = [];
    public int[] SecretShopPrices = new int[3];
    private int CheatItemsReceived;
    public bool IsBjergCastle;

    [Serializable]
    public class SaveData
    {
        public ConnectionData Connection;
        public int Seed = 0;
        public List<(int, long)> ReceivedItemLocations = [];
        public int CheatItemCount = 0;
        public List<int> KeyTotals = [];
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
        SlotData = login.SlotData;
        SlotName = cdata.SlotName;
        int seed = ArchipelagoSettings.GetOrDefault(SlotData, "seed", 0);

        if (Data.Seed != 0 && Data.Seed != seed)
        {
            Error = "The server's seed does not match the save file's seed\nMake sure you're connecting with the right save file";
            Disconnect();
            yield break;
        }

        Data.Seed = seed;

        if (!reconnect)
        {
            Settings = new ArchipelagoSettings(SlotData);
            BuildLocationTable();
            MapTracker.SetupTracker();
            IsBjergCastle = false;

            // Scout unchecked locations
            var uncheckedLocationIDs = from locationID in LocationTable.Keys where !LocationChecked(locationID) select ID_BASE + locationID;
            Task<LocationInfoPacket> locationInfoTask = Task.Run(async () => await Session.Locations.ScoutLocationsAsync(false, uncheckedLocationIDs.ToArray()));
            yield return new WaitUntil(() => locationInfoTask.IsCompleted);
            if (locationInfoTask.IsFaulted)
            {
                Error = locationInfoTask.Exception.GetBaseException().Message;
                yield break;
            }
            
            var locationInfo = locationInfoTask.Result;
            foreach (var item in locationInfo.Locations)
            {
                int locationID = (int)(item.Location - ID_BASE);
                LocationTable[locationID] = new ArchipelagoItem(item, false);
            }
        }

        // Sync checked locations
        var checkedLocationIDs = from locationID in LocationTable.Keys where LocationChecked(locationID) select ID_BASE + locationID;
        var locationCheckTask = Task.Run(async () => await Session.Locations.CompleteLocationChecksAsync(checkedLocationIDs.ToArray()));
        yield return new WaitUntil(() => locationCheckTask.IsCompleted);
        if (locationCheckTask.IsFaulted)
        {
            Error = locationCheckTask.Exception.GetBaseException().Message;
            yield break;
        }

        // Sync collected locations
        foreach (long locationID in Session.Locations.AllLocationsChecked)
        {
            int chestID = (int)(locationID - ID_BASE);
            if (!GameMaster.GM.Save.Data.Chests.Contains(chestID))
                GameMaster.GM.Save.Data.Chests.Add(chestID);
        }

        // Connection successful
        Session.Socket.SocketClosed += OnSocketClosed;
        Session.Locations.CheckedLocationsUpdated += OnCheckedLocationsUpdated;
        Connected = true;
        Error = "";
        Data.Connection = cdata;
        CheatItemsReceived = 0;
        TrapControl.Activate();
        Randomize();
        GameMaster.GM.Save.Save();
    }

    public void Disconnect()
    {
        Session.Socket.DisconnectAsync();
        Session = null;
        Connected = false;
        TrapControl.Deactivate();
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
        if (Session is not null && Session.Items.Any() && NormalGameState())
        {
            var item = Session.Items.DequeueItem();
            if (!Data.ReceivedItemLocations.Contains((item.Player, item.Location)) && item.Location != -1 && item.Location != -2)
            {
                Data.ReceivedItemLocations.Add((item.Player, item.Location));
                StartCoroutine(ReceiveItem(new ArchipelagoItem(item, true)));
            }
            else if (item.Location == -1 || item.Location == -2)
            {
                CheatItemsReceived++;
                if (CheatItemsReceived > Data.CheatItemCount)
                {
                    Data.CheatItemCount = CheatItemsReceived;
                    StartCoroutine(ReceiveItem(new ArchipelagoItem(item, true), item.Location == -2));
                }
            }
        }
    }

    public static bool NormalGameState()
    {
        return GameMaster.GM.GS == GameMaster.GameState.IN_GAME &&
            GameMaster.GM.PC.ACTIVE_CHARACTER == PlayerCharacter.CHARACTER.ORAN &&
            GameMaster.GM.PC.PlayerState == PlayerCharacter.MovementState.Normal &&
            !GameMaster.GM.UI.SPEAKING();
    }

    public void InitialPatches()
    {
        for (int i = 0; i < 19; i++)
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
        GameMaster.GM.Save.Data.Relationships[CAROLINE_ID].Stage = SaveSystem.NPCData.Stages.STAGE0;
        GameMaster.GM.Save.Data.Quests[22] = SaveSystem.Quest.QUESTCOMPLETE; // Skip statue activation text

        if (Settings.SkipOneSmallFavor)
        {
            GameMaster.GM.Save.Data.Quests[2] = SaveSystem.Quest.STAGE16;
        }

        if (Settings.StartWithSpicedHam)
        {
            GameMaster.GM.Save.Data.SLOTA = PlayerCharacter.BUFFS.RUN;
        }
        if (Settings.StartWithWingedBoots)
        {
            GameMaster.GM.Save.Data.BootSlot = PlayerCharacter.Boots.Winged;
        }

        if (Settings.AltarToVar)
        {
            GameMaster.GM.Save.Data.Quests[36] = SaveSystem.Quest.QUESTCOMPLETE;
        }
        if (Settings.AltarToZolei)
        {
            GameMaster.GM.Save.Data.Quests[37] = SaveSystem.Quest.QUESTCOMPLETE;
        }
        if (Settings.AltarToRaem)
        {
            GameMaster.GM.Save.Data.Quests[38] = SaveSystem.Quest.QUESTCOMPLETE;
        }
        if (Settings.AltarToHate)
        {
            GameMaster.GM.Save.Data.Quests[39] = SaveSystem.Quest.QUESTCOMPLETE;
        }
        if (Settings.CurseOfFrailty)
        {
            GameMaster.GM.Save.Data.Quests[84] = SaveSystem.Quest.QUESTCOMPLETE;
        }
        if (Settings.CurseOfFamine)
        {
            GameMaster.GM.Save.Data.Quests[85] = SaveSystem.Quest.QUESTCOMPLETE;
        }
        if (Settings.CurseOfWind)
        {
            GameMaster.GM.Save.Data.Quests[86] = SaveSystem.Quest.QUESTCOMPLETE;
        }
        if (Settings.CurseOfRust)
        {
            GameMaster.GM.Save.Data.Quests[87] = SaveSystem.Quest.QUESTCOMPLETE;
        }
        if (Settings.CurseOfBlooms)
        {
            GameMaster.GM.Save.Data.Quests[89] = SaveSystem.Quest.QUESTCOMPLETE;
        }
        if (Settings.CurseOfFlames)
        {
            GameMaster.GM.Save.Data.Quests[90] = SaveSystem.Quest.QUESTCOMPLETE;
        }
        if (Settings.CurseOfHorns)
        {
            GameMaster.GM.Save.Data.Quests[91] = SaveSystem.Quest.QUESTCOMPLETE;
        }
        if (Settings.CurseOfCrowns)
        {
            GameMaster.GM.Save.Data.Quests[92] = SaveSystem.Quest.QUESTCOMPLETE;
            GameMaster.GM.Save.Data.Married = true;
            GameMaster.GM.Save.Data.Wife = NPC.Name.Revulan;
        }

        ColorCheck();

        Data.ReceivedItemLocations.Clear();

        Data.KeyTotals.Clear();
        for (int i = 0; i < Key.Keys.Count(); i++)
            Data.KeyTotals.Add(0);
    }

    public bool CollectItem(int locationID)
    {
        ArchipelagoItem item = GetLocationItem(locationID);
        if (item is null)
            return false;
        
        if (Data.ReceivedItemLocations.Contains((SlotID, ID_BASE + locationID)))
            return true;

        if (Connected)
        {
            StartCoroutine(SendCheck(ID_BASE + locationID));
        }

        GameMaster.GM.Save.Data.Chests.Add(locationID);
        if (item.SlotID == SlotID)
        {
            Data.ReceivedItemLocations.Add((SlotID, locationID));
        }
        StartCoroutine(ReceiveItem(item));
        
        GameMaster.GM.Save.Save();
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

    public ArchipelagoItem GetLocationItem(int locationID)
    {
        if (!LocationTable.TryGetValue(locationID, out var item))
            return null;
        return item;
    }

    private IEnumerator ReceiveItem(ArchipelagoItem item, bool skipDisplay = false)
    {
        Item localItem = item.LocalItem();

        bool cutscene = GameMaster.GM.GS == GameMaster.GameState.CUTSCENE;
        if (!skipDisplay)
        {
            GameMaster.GM.PC.CUTSCENE(true);
            GameMaster.GM.PC.EMOTE.ITEM_PICKUP(item.Sprite(false), true);
            GameMaster.GM.PC.Anim.SetBool("ITEM", true);
        }

        if (item.SlotID == SlotID)
        {
            bool rusted = localItem switch
            {
                Item.BlessedPick => GameMaster.GM.Save.Data.Quests[87] == SaveSystem.Quest.QUESTCOMPLETE,
                Item.BoltAnchor => GameMaster.GM.Save.Data.Quests[87] == SaveSystem.Quest.QUESTCOMPLETE,
                _ => false, 
            };

            if (!rusted)
            {
                GameMaster.GM.Save.AddToInventory((int)localItem, true);
            }

            if (localItem.IsKey())
            {
                Data.KeyTotals[localItem.ToKeyID()]++;
            }

            if (localItem == Item.WeaponChain)
            {
                GameMaster.GM.PC.Anim.SetBool("WEPCHAIN", true);
            }

            if (localItem == Item.SilveredScarf)
            {
                GameMaster.GM.PC.EquipScarf();
            }

            ColorCheck();

            if (localItem == Item.HeartOre)
            {
                GameMaster.GM.PC.Heal(1);
                List<GameMaster.Speech> speech = [];
                int jingle = 21;
                if (GameMaster.GM.Save.Data.Quests[39] == SaveSystem.Quest.QUESTCOMPLETE)
                {
                    speech.Add(GameMaster.CreateSpeech(46, 0, "A FRAGMENT OF HEART ORE CRUMBLES AND DISAPPEARS.", "", 0));
                }
                else
                {
                    GameMaster.GM.Save.Data.Ores++;
                    switch (GameMaster.GM.Save.Data.Ores)
                    {
                        case 4:
                            speech.Add(GameMaster.CreateSpeech(46, 0, "A FRAGMENT OF HEART ORE! NOW MY VITALITY WILL BE INCREASED!", "", 0));
                            GameMaster.GM.Save.Data.Ores = 0;
                            jingle = 20;
                            GameMaster.GM.PC.IncreaseMaxpHP();
                            break;
                        case 3:
                            speech.Add(GameMaster.CreateSpeech(46, 0, "A FRAGMENT OF HEART ORE! ONLY ONE MORE FRAGMENT TO GO!", "", 0));
                            break;
                        case 2:
                            speech.Add(GameMaster.CreateSpeech(46, 0, "A FRAGMENT OF HEART ORE! ONLY TWO MORE FRAGMENTS TO GO!", "", 0));
                            break;
                        case 1:
                            speech.Add(GameMaster.CreateSpeech(46, 0, "A FRAGMENT OF HEART ORE! ONLY THREE MORE FRAGMENTS TO GO!", "", 0));
                            break;
                    }
                }
                if (!skipDisplay)
                {
                    GameMaster.GM.BGM.PlayJingle(jingle);
                    GameMaster.GM.UI.InitiateChat(speech, false);
                }
            }
            else
            {
                if (!skipDisplay)
                {
                    if (localItem.Data().MAJOR && !rusted)
                    {
                        GameMaster.GM.BGM.PlayJingle(20);
                    }
                    else
                    {
                        GameMaster.GM.BGM.PlayJingle(21);
                    }

                    if (!rusted)
                    {
                        GameMaster.GM.UI.InitiateChat(localItem.Data().AboutText, false);
                    }
                    else
                    {
                        GameMaster.GM.UI.InitiateChat([
                            GameMaster.CreateSpeech(46, 0, "IT'S TOO DAMAGED TO USE. . .", "", 0),
                        ], false);
                    }
                }

                switch (localItem)
                {
                    case Item.Gold1:
                        GameMaster.GM.Save.AddCurrency(1);
                        break;
                    case Item.Gold10:
                        GameMaster.GM.Save.AddCurrency(10);
                        break;
                    case Item.Gold20:
                        GameMaster.GM.Save.AddCurrency(20);
                        break;
                    case Item.Gold100:
                        GameMaster.GM.Save.AddCurrency(100);
                        break;
                }
            }
        }
        else if (!skipDisplay)
        {
            GameMaster.GM.BGM.PlayJingle(21);
            GameMaster.GM.UI.InitiateChat(item.Speech(), false);
        }

        if (localItem.IsTrap())
        {
            TrapControl.TC.NewTrap(localItem.ToTrapType());
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

    public void OnCheckedLocationsUpdated(ReadOnlyCollection<long> newCheckedLocations)
    {
        foreach (long locationID in newCheckedLocations)
        {
            int chestID = (int)(locationID - ID_BASE);
            if (!GameMaster.GM.Save.Data.Chests.Contains(chestID))
                GameMaster.GM.Save.Data.Chests.Add(chestID);
        }
    }

    private void BuildLocationTable()
    {
        List<int> locations = new(LOCS_BASE);
        if (Settings.TradingQuest == ArchipelagoSettings.TradingQuestOption.Shuffle)
            locations.AddRange(LOCS_TRADE);
        else if (Settings.TradingQuest == ArchipelagoSettings.TradingQuestOption.Vanilla)
            locations.Add(LOC_ULNI);
        if (Settings.ShuffleGrelinDrops)
            locations.AddRange(LOCS_GRELIN);
        if (Settings.ShuffleHiddenItems)
            locations.AddRange(LOCS_HIDDEN);
        if (Settings.ShuffleBjergCastle)
            locations.AddRange(LOCS_CASTLE);
        if (Settings.ShuffleDaemonsDive)
            locations.AddRange(LOCS_DIVE);
        if (Settings.ShuffleEnlightenment)
            locations.AddRange(LOCS_ENLIGHTENMENT);
        if (Settings.ShuffleSecretShop)
            locations.AddRange(LOCS_SECRET_SHOP);

        LocationTable.Clear();
        foreach (int locationID in locations)
        {
            LocationTable[locationID] = null;
        }
    }

    public bool IsLocationRandomized(int locationID)
    {
        return LocationTable.ContainsKey(locationID);
    }

    public bool LocationChecked(int locationID)
    {
        return GameMaster.GM.Save.Data.Chests.Contains(locationID);
    }

    public int ColorCount()
    {
        return ItemExtension.AllColors().Count(item => item.Acquired());
    }

    public int BlessingCount()
    {
        return ItemExtension.AllBlessings().Count(item => item.Acquired());
    }

    public void ColorCheck()
    {
        if (ColorCount() >= Settings.ColorsRequired)
        {
            GameMaster.GM.Save.Data.Recolored = true;
        }
    }

    public string PickHint()
    {
        if (Item.IronPick.Acquired())
        {
            return "@CIN YOUR POCKET@";
        }
        try
        {
            var player = (string)SlotData["pick_hint_player"];
            var location = (string)SlotData["pick_hint_location"];
            if (player == SlotName)
                return location.ToUpper();
            return $"AT @C{location.ToUpper()} IN {player.ToUpper()}'s WORLD@";
        }
        catch (Exception)
        {
            return "SOMEWHERE";
        }
    }

    public void AddToChat(List<GameMaster.Speech> chat, bool question)
    {
        StartCoroutine(ChatWhenReady(chat, question));
    }

    private IEnumerator ChatWhenReady(List<GameMaster.Speech> chat, bool question)
    {
        while (GameMaster.GM.UI.SPEAKING())
        {
            yield return null;
        }
        GameMaster.GM.UI.InitiateChat(chat, question);
    }

    public Key CurrentDungeonKey()
    {
        int currentScene = (int)AccessTools.Field(typeof(GameMaster), "CurrentScene").GetValue(GameMaster.GM);
        if (currentScene == Key.BjergCastle.Scene && IsBjergCastle)
            return Key.BjergCastle;
        foreach (Key key in Key.Keys)
        {
            if (key.Scene == currentScene)
                return key;
        }
        return null;
    }

    public void StartWarp()
    {
        StartCoroutine(Warp());
    }

    private IEnumerator Warp()
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
        GameMaster.GM.UI.Fishing(false);
        GameMaster.GM.PC.RecoverTemp();
        GameMaster.GM.UI.StartMines(false);
        AccessTools.Field(typeof(GameMaster), "CurrentScene").SetValue(GameMaster.GM, 2);
        var aload = SceneManager.LoadSceneAsync(2);
        AccessTools.Field(typeof(GameMaster), "ALoad").SetValue(GameMaster.GM, aload);
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

    private void Randomize()
    {
        Random = new(Data.Seed);
        if (Settings.ShuffleSecretShop)
        {
            RandomizeSecretShopPrices();
        }
        RandomizeTrapAppearances();
    }

    private void RandomizeSecretShopPrices()
    {
        for (int i = 0; i < 3; i++)
        {
            ArchipelagoItem item = GetLocationItem(LOCS_SECRET_SHOP[i]);
            if (item is null)
            {
                SecretShopPrices[i] = 0;
            }
            else
            {
                SecretShopPrices[i] = item.Classification switch
                {
                    ItemFlags.Advancement => Random.Next(150, 300),
                    ItemFlags.NeverExclude => Random.Next(50, 200),
                    ItemFlags.Trap => Random.Next(0, 20),
                    _ => Random.Next(0, 100),
                };
            }
        }
    }

    private void RandomizeTrapAppearances()
    {
        List<Item> majorItems = ItemExtension.MajorItems();
        foreach ((_, var item) in LocationTable)
        {
            Item trapSpriteID = majorItems[Random.Next(majorItems.Count)];
            if (item is not null)
                item.TrapSpriteID = trapSpriteID;
        }
    }
}