using System;
using System.Collections.Generic;
using System.Text;

namespace activity_00_tap_26_27.Data
{
    public class ItemData
    {
        public string _id;
        public string _name;
        
        public int _price;
        public int _quantity;


        public ItemData(string item_id, string item_name, int price)
        {
            _id = item_id;
            _name = item_name;
           
            _price = price;
            
            
           
        }
    }
}
