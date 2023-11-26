using System.Collections.Generic;
using UnityEngine;

namespace ProdigalArchipelago;

public class StatsScreen : MonoBehaviour
{
    public static StatsScreen Instance;

    List<GameObject> TimeText;
    List<GameObject> ItemsText;
    List<GameObject> PickText;
    List<GameObject> HandText;
    List<GameObject> EyeText;
    List<GameObject> LariatText;
    List<GameObject> KnuckleText;
    List<GameObject> FlareText;
    List<GameObject> DeathsText;
    List<GameObject> WarpsText;
    List<GameObject> FallsText;
    List<GameObject> KillsText;
    List<GameObject> DamageTakenText;
    List<GameObject> KeysBrokenText;

    public static void Create()
    {
        GameObject obj = new("StatsScreen");
        obj.SetActive(false);
        obj.transform.SetParent(GameMaster.GM.UI.transform.GetChild(1));
        Instance = obj.AddComponent<StatsScreen>();
        var renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = ResourceManager.StatsScreenBGSprite;
        renderer.sortingLayerName = "UI";
        renderer.sortingOrder = 8;
    }

    private void Awake()
    {
        Color32 textColor = new(130, 109, 95, 255);
        TimeText = Menu.CreateTextObjects("TimeText", 13, transform, -96, 56, textColor);
        ItemsText = Menu.CreateTextObjects("ItemsText", 13, transform, 0, 56, textColor);
        PickText = Menu.CreateTextObjects("PickText", 13, transform, -96, 44, textColor);
        HandText = Menu.CreateTextObjects("HandText", 13, transform, -96, 32, textColor);
        EyeText = Menu.CreateTextObjects("EyeText", 12, transform, -96, 20, textColor);
        LariatText = Menu.CreateTextObjects("LariatText", 15, transform, 0, 44, textColor);
        KnuckleText = Menu.CreateTextObjects("KnuckleText", 16, transform, 0, 32, textColor);
        FlareText = Menu.CreateTextObjects("FlareText", 14, transform, 0, 20, textColor);
        DeathsText = Menu.CreateTextObjects("DeathsText", 10, transform, -96, 8, textColor);
        WarpsText = Menu.CreateTextObjects("WarpsText", 9, transform, -96, -4, textColor);
        FallsText = Menu.CreateTextObjects("FallsText", 9, transform, -96, -16, textColor);
        KillsText = Menu.CreateTextObjects("KillsText", 9, transform, 0, 8, textColor);
        DamageTakenText = Menu.CreateTextObjects("DamageTaken", 16, transform, 0, -4, textColor);
        KeysBrokenText = Menu.CreateTextObjects("KeysBrokenText", 15, transform, 0, -16, textColor);
    }

    private void Update()
    {
        Menu.RenderText(TimeText, $"TIME {HMS(Archipelago.AP.Stats.FinishTime)}");
        Menu.RenderText(ItemsText, $"ITEMS {Archipelago.AP.Stats.ItemsCollected:D3}/{Archipelago.AP.Stats.ItemsTotal:D3}");
        Menu.RenderText(PickText, $"PICK {HMS(Archipelago.AP.Stats.PickTime)}");
        Menu.RenderText(HandText, $"HAND {HMS(Archipelago.AP.Stats.HandTime)}");
        Menu.RenderText(EyeText, $"EYE {HMS(Archipelago.AP.Stats.EyeTime)}");
        Menu.RenderText(LariatText, $"LARIAT {HMS(Archipelago.AP.Stats.LariatTime)}");
        Menu.RenderText(KnuckleText, $"KNUCKLE {HMS(Archipelago.AP.Stats.KnuckleTime)}");
        Menu.RenderText(FlareText, $"FLARE {HMS(Archipelago.AP.Stats.FlareTime)}");
        Menu.RenderText(DeathsText, $"DEATHS {Cap(Archipelago.AP.Stats.DeathCount)}");
        Menu.RenderText(WarpsText, $"WARPS {Cap(Archipelago.AP.Stats.WarpCount)}");
        Menu.RenderText(FallsText, $"FALLS {Cap(Archipelago.AP.Stats.FallCount)}");
        Menu.RenderText(KillsText, $"KILLS {Cap(Archipelago.AP.Stats.KillCount)}");
        Menu.RenderText(DamageTakenText, $"DAMAGE TAKEN {Cap(Archipelago.AP.Stats.DamageTaken)}");
        Menu.RenderText(KeysBrokenText, $"KEYS BROKEN {Cap(Archipelago.AP.Stats.KeysBroken)}");
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private string HMS(int time)
    {
        int hours = time / 3600;
        int minutes = (time - 3600 * hours) / 60;
        int seconds = time - 3600 * hours - 60 * minutes;
        return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
    }

    private int Cap(int value)
    {
        return value > 999 ? 999 : value;
    }
}