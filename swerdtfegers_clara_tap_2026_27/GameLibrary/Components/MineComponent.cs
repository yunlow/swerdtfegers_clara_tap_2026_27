using activity_00_tap_26_27.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.Components
{
    public class MineComponent : Component
    {
        private List<int> _weights;
        private int _totalWeight;
        private int _stock;
        private int _minuteper;
        private GameObject _gameObject;
        public MineComponent(GameObject game_object) : base(game_object)
        {
            _gameObject = game_object;

            _totalWeight = 0;

            foreach (int weight in _weights)
            {
                _totalWeight += weight;
            }
        }
        public int PickOreIndex(int roll)
        {
            if (roll < 0 || roll >= _totalWeight)
            {
                throw new ArgumentOutOfRangeException(nameof(roll), "Roll must be within [0, total weight).");
            }

            int cumulative = 0;
            for (int i = 0; i < _weights.Count; i++)
            {
                cumulative += _weights[i];
                if (roll < cumulative)
                {
                    return i;
                }
            }
            return _weights.Count - 1;
        }

        public int DrawOre(Random random)
        {
            int roll = random.Next(0, _totalWeight);
            return PickOreIndex(roll);
        }
    }
}
