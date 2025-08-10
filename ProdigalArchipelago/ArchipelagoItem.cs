using System.Collections.Generic;
using UnityEngine;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Models;

namespace ProdigalArchipelago;

public class ArchipelagoItem
{
    public long ID;
    public string Name;
    public int SlotID;
    public string SlotName;
    public ItemFlags Classification;
    public Item TrapSpriteID;

    public ArchipelagoItem(ItemInfo item, bool received)
    {
        ID = item.ItemId;
        try
        {
            Name = Archipelago.AP.Session.Items.GetItemName(ID, item.ItemGame);
        }
        catch
        {
            Name = "an item";
        }
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
        string kind = "F";
        if ((Classification & ItemFlags.Advancement) == ItemFlags.Advancement)
        {
            kind = "P";
        }
        else if ((Classification & ItemFlags.NeverExclude) == ItemFlags.NeverExclude)
        {
            kind = "U";
        }
        else if (Classification == ItemFlags.Trap)
        {
            kind = "T";
        }
        if (Classification == (ItemFlags.Advancement | ItemFlags.NeverExclude))
        {
            kind = "I";
        }

        return [GameMaster.CreateSpeech(46, 0, $"FOUND @{kind}{UIPatch.Sanitize(Name)}@*FOR {UIPatch.Sanitize(SlotName)}!", "", 0)];
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