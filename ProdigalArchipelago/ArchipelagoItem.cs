using System.Collections.Generic;
using Archipelago.MultiClient.Net.Enums;

namespace ProdigalArchipelago
{
    public class ArchipelagoItem
    {
        public long ID;
        public string Name;
        public int SlotID;
        public string SlotName;
        public ItemFlags Classification;

        public ArchipelagoItem(long id, string name, int slotID, string slotName, ItemFlags classification)
        {
            ID = id;
            Name = name;
            SlotID = slotID;
            SlotName = slotName;
            Classification = classification;
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
            if (SlotID == Archipelago.AP.SlotID)
            {
                return -1;
            }

            if (ID >= Archipelago.ID_BASE && ID < Archipelago.ID_BASE + GameMaster.GM.ItemData.Database.Count)
            {
                int id = (int)(ID - Archipelago.ID_BASE);

                if (id == Archipelago.PROGRESSIVE_PICK_ID)
                {
                    return Archipelago.IRON_PICK_ID;
                }
                if (id == Archipelago.PROGRESSIVE_KNUCKLE_ID)
                {
                    return Archipelago.RUST_KNUCKLE_ID;
                }
                if (id == Archipelago.PROGRESSIVE_HAND_ID)
                {
                    return Archipelago.DREAD_HAND_ID;
                }

                return id;
            }
            else
            {
                return Archipelago.AP_ITEM_ID;
            }
        }
    }
}