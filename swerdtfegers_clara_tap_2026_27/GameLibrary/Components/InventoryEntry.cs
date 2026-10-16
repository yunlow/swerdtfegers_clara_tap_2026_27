using activity_00_tap_26_27.Data;
using GameLibrary.Data;

namespace GameLibrary.Components
{
    public class InventoryEntry
    {
        private ItemData _item;
        private int _quantity;

        public InventoryEntry(ItemData item_data, int quantity)
        {
            _item = item_data;
            _quantity = quantity;
        }

        public ItemData GetItem()
        {
            return _item;
        }

        public int GetQuantity()
        {
            return _quantity;
        }

        public void SetQuantity(int quantity)
        {
            _quantity = quantity;
        }
    }
}