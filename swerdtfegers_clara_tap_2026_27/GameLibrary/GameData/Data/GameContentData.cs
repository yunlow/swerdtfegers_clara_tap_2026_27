using activity_00_tap_26_27.Data;
using System.Collections.Generic;

namespace GameLibrary.Data
{
    public class GameContentData
    {
        public List<LocationData> _locationDataTable;
        public List<ShopData> _shopDataTable;
        public Dictionary<string, ItemData> _itemDataTable;
        public List<InventoryEntryData> _inventoryDataTable;
        public List<HeroData> _heroDataTable;
    }
}