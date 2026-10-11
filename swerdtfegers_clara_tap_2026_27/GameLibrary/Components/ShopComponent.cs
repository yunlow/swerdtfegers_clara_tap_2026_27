using activity_00_tap_26_27.Core;
using activity_00_tap_26_27.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.Components
{
    public class ShopComponent : Component
    {
        private readonly float GAME_MINUTES_PER_SECOND = 10.0f;

        private ShopData _shopData;
        private List<ItemData> _itemDataTable;
        private float _timeInMinutes;

        public ShopComponent(ShopData shop_data, List<ItemData> item_data_table, GameObject game_object) : base()
        {
            _shopData = shop_data;
            _itemDataTable = item_data_table;
            _timeInMinutes = 8 * 60;
        }

        public int GetHour()
        {
            return (int)MathF.Floor(_timeInMinutes / 60.0f);
        }

        public int GetMinute()
        {
            return (int)MathF.Floor(_timeInMinutes % 60);
        }

        public override void FixedUpdate(float elapsed_fixed_time)
        {
            _timeInMinutes += elapsed_fixed_time * GAME_MINUTES_PER_SECOND;
            _timeInMinutes %= 24 * 60;
        }

        public ShopData GetShopData()
        {
            return _shopData;
        }

        public bool GetIsOpen()
        {
            return GetHour() >= _shopData._openingHour && GetHour() <= _shopData._closingHour;
        }

        public int GetItemCount()
        {
            return _itemDataTable.Count;
        }

        public ItemData GetItemAtIndex(int item_index)
        {
            return _itemDataTable[item_index];
        }

        public int GetQuantityAtIndex(int item_index)
        {
            return _itemDataTable[item_index]._quantity;
        }

        public bool TryRemoveOneAtIndex(int item_index)
        {
            if (_itemDataTable[item_index]._quantity > 0)
            {
                _itemDataTable[item_index]._quantity -= 1;

                return true;
            }

            return false;
        }

        
    }
}
