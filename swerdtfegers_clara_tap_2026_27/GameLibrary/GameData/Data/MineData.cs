namespace GameLibrary.Data
{
    public class MineData
    {
        public readonly string _locationId;
        public readonly int _minutesPerStrike;
        public readonly int _stock;

        public MineData(string location_id, int minutes_per_strike, int stock)
        {
            _locationId = location_id;
            _minutesPerStrike = minutes_per_strike;
            _stock = stock;
        }
    }
}