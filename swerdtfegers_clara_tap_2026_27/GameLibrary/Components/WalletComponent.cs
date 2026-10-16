using activity_00_tap_26_27.Core;
using System;

namespace GameLibrary.Components
{
    public class WalletComponent : Component
    {
        private int _money;

        public WalletComponent(GameObject game_object, int initial_money) : base(game_object)
        {
            _money = initial_money;
        }

        public int GetMoney()
        {
            return _money;
        }

        public void AddMoney(int money_to_add)
        {
            if (money_to_add < 0)
            {
                throw new ArgumentException("money_to_add cannot be negative");
            }

            _money += money_to_add;
        }

        public bool TrySpendMoney(int money_to_spend)
        {
            if (money_to_spend < 0)
            {
                throw new ArgumentException("money_to_spend cannot be negative");
            }

            bool operation_result = false;

            if (money_to_spend <= _money)
            {
                _money -= money_to_spend;
                operation_result = true;
            }

            return operation_result;
        }
    }
}