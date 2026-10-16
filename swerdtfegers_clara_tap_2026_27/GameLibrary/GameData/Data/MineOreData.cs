namespace GameLibrary.Data
{
    public class MineOreData
    {
        public readonly string _locationId;
        public readonly string _itemId;
        public readonly int _weight;

        public MineOreData(string location_id, string item_id, int weight)
        {
            _locationId = location_id;
            _itemId = item_id;
            _weight = weight;
        }
    }
}