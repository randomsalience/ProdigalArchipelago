namespace ProdigalArchipelago
{
    public class Location
    {
        public int ID;
        public ArchipelagoItem Item;

        public Location(int id)
        {
            ID = id;
        }

        public bool Checked()
        {
            return GameMaster.GM.Save.Data.Chests.Contains(ID);
        }
    }
}