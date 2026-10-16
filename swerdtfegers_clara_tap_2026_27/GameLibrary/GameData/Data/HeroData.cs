using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.Data
{
    public class HeroData
    {
        public string _id;
        public string _name;
        public int _health;
        public int _gold;

        public HeroData(string id, string name, int health, int gold)
        {
            _id = id;
            _name = name;
            _health = health;
            _gold = gold;
        }
    }
}
