using System;

namespace ProdigalArchipelago
{
    class TrackerLocation
    {
        public string Name;
        public int ID;
        public Func<bool> Logic;
        public Func<bool> KeyLogic;

        public TrackerLocation(string name, int id, Func<bool> logic, Func<bool> keyLogic = null)
        {
            Name = name;
            ID = id;
            Logic = logic;
            KeyLogic = keyLogic;
        }

        public bool IsUnchecked()
        {
            return Archipelago.AP.IsLocationRandomized(ID) && !GameMaster.GM.Save.Data.Chests.Contains(ID);
        }

        public string GetText(bool regionInLogic)
        {
            if (regionInLogic && Logic())
                return Name;
            
            if (regionInLogic && KeyLogic is not null && KeyLogic())
                return $"<color=#DF7126>{Name}</color>";
            
            return $"<color=#EC374D>{Name}</color>";
        }
    }
}