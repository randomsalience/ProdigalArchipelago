using System;

namespace ProdigalArchipelago;

class TrackerLocation(string name, int id, Func<bool> logic, Func<bool> keyLogic = null)
{
    public string Name = name;
    public int ID = id;
    public Func<bool> Logic = logic;
    public Func<bool> KeyLogic = keyLogic;

    public bool IsUnchecked()
    {
        return Archipelago.AP.IsLocationRandomized(ID) && !GameMaster.GM.Save.Data.Chests.Contains(ID);
    }

    public string GetText(bool regionInLogic)
    {
        if (regionInLogic && Archipelago.AP.HintedLocations.Contains(ID) && (Logic() || (KeyLogic is not null && KeyLogic())))
            return $"<color=#2BFFFF>{Name}</color>";

        if (regionInLogic && Logic())
            return Name;
        
        if (regionInLogic && KeyLogic is not null && KeyLogic())
            return $"<color=#DF7126>{Name}</color>";
        
        return $"<color=#EC374D>{Name}</color>";
    }
}