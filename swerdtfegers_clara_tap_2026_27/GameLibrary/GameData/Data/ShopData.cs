using System;
using System.Collections.Generic;
using System.Text;

namespace activity_00_tap_26_27.Data
{
    public class ShopData
    {
        public string _locationId;
        public int _openingHour;
        public int _closingHour;

        public ShopData(string location_id, int opening_hour, int closing_hour)
        {
            _locationId = location_id;
            _openingHour = opening_hour;
            _closingHour = closing_hour;
        }
    }
}
