using System;
using System.Collections.Generic;
using System.Text;

namespace activity_00_tap_26_27.Data
{
    public class ItemData
    {
        public string _id;
        public string _name;
        public string _shopId;
        public int _price;
        public int _quantity;

        public ItemData(string item_id, string item_name, string shop_id, int price, int quantity)
        {
            _id = item_id;
            _name = item_name;
            _shopId = shop_id;
            _price = price;
            _quantity = quantity;
        }
    }
}
