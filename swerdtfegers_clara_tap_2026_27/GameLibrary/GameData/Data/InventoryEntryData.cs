using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.Data
{
    public class InventoryEntryData
    {
        public string _ownerId;
        public string _itemId;
        public int _quantity;

        public InventoryEntryData(string owner_id, string item_id, int quantity)
        {
            _ownerId = owner_id;
            _itemId = item_id;
            _quantity = quantity;
        }

    }
}
