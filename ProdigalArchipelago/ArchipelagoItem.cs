using System.Collections.Generic;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;

namespace ProdigalArchipelago
{
    public class ArchipelagoItem
    {
        public long ID;
        public string Name;
        public int SlotID;
        public string SlotName;
        public ItemFlags Classification;

        public ArchipelagoItem(NetworkItem item, bool received)
        {
            ID = item.Item;
            Name = Archipelago.AP.Session.Items.GetItemName(ID);
            SlotID = received ? Archipelago.AP.SlotID : item.Player;
            SlotName = Archipelago.AP.Session.Players.GetPlayerName(SlotID);
            Classification = item.Flags;
        }

        public int LocalID()
        {
            if (SlotID == Archipelago.AP.SlotID)
            {
                return (int)(ID - Archipelago.ID_BASE);
            }
            else
            {
                return Archipelago.AP_ITEM_ID;
            }
        }

        public List<GameMaster.Speech> Speech()
        {
            string kind = Classification switch
            {
                ItemFlags.Advancement => "P",
                ItemFlags.NeverExclude => "U",
                ItemFlags.Trap => "T",
                _ => "F",
            };

            return new List<GameMaster.Speech>
            {
                GameMaster.CreateSpeech(46, 0, $"FOUND @{kind}{Name.ToUpper()}@*FOR {SlotName.ToUpper()}!", "", 0)
            };
        }

        public int SpriteID()
        {
            if (ID >= Archipelago.ID_BASE && ID < Archipelago.ID_BASE + GameMaster.GM.ItemData.Database.Count)
            {
                return (int)(ID - Archipelago.ID_BASE);
            }
            else
            {
                return Archipelago.AP_ITEM_ID;
            }
        }
    }
}