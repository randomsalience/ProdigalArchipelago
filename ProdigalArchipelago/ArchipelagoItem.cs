using System.Collections.Generic;
using UnityEngine;
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
        public Item TrapSpriteID;

        public ArchipelagoItem(NetworkItem item, bool received)
        {
            ID = item.Item;
            Name = Archipelago.AP.Session.Items.GetItemName(ID);
            SlotID = received ? Archipelago.AP.SlotID : item.Player;
            SlotName = Archipelago.AP.Session.Players.GetPlayerName(SlotID);
            Classification = item.Flags;
        }

        public Item LocalItem()
        {
            if (SlotID == Archipelago.AP.SlotID)
            {
                Item item = (Item)(ID - Archipelago.ID_BASE);
                return item.NonProgressive();
            }
            else
            {
                return Item.ArchipelagoItem;
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

        public Sprite Sprite(bool disguiseTraps)
        {
            Item spriteItem;
            
            if (SlotID == Archipelago.AP.SlotID)
                spriteItem = LocalItem();
            else if (ID >= Archipelago.ID_BASE && ID < Archipelago.ID_BASE + GameMaster.GM.ItemData.Database.Count)
                spriteItem = (Item)(ID - Archipelago.ID_BASE);
            else
                spriteItem = Item.ArchipelagoItem;
            
            if (Classification == ItemFlags.Trap)
            {
                if (disguiseTraps)
                {
                    spriteItem = TrapSpriteID;
                    if (SlotID == Archipelago.AP.SlotID)
                    {
                        spriteItem = spriteItem.NonProgressive();
                    }
                }
                else
                {
                    return null;
                }
            }

            return spriteItem.Data().ItemSprite;
        }
    }
}