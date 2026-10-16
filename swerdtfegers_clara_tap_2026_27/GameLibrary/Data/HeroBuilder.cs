using activity_00_tap_26_27.Core;
using activity_00_tap_26_27.Core.Events;
using GameLibrary.Components;
using GameLibrary.Events;

namespace GameLibrary.Data
{
    public class HeroBuilder
    {
        private readonly GameContentData _gameContentData;

        public HeroBuilder(GameContentData game_content_data)
        {
            _gameContentData = game_content_data;
        }

        public GameObject Build(EventManager event_manager)
        {
            GameObject hero_game_object = null;

            for (int hero_index = 0; hero_index < _gameContentData._heroDataTable.Count; hero_index++)
            {
                HeroData hero_data = _gameContentData._heroDataTable[hero_index];

                hero_game_object = new GameObject(hero_data._id);

                HeroComponent hero_component = new HeroComponent(hero_game_object, hero_data._name, hero_data._id);
                InventoryComponent inventory_component = new InventoryComponent(hero_game_object);
                WalletComponent wallet_component = new WalletComponent(hero_game_object, hero_data._gold);
                HealthComponent health_component = new HealthComponent(hero_game_object, hero_data._health);

                hero_game_object.AddComponent(hero_component);
                hero_game_object.AddComponent(inventory_component);

                for (int inventory_data_index = 0; inventory_data_index < _gameContentData._inventoryDataTable.Count; inventory_data_index++)
                {
                    InventoryEntryData current_inventory_data = _gameContentData._inventoryDataTable[inventory_data_index];

                    if (hero_data._id == current_inventory_data._ownerId)
                    {
                        inventory_component.Add(_gameContentData._itemDataTable[current_inventory_data._itemId], current_inventory_data._quantity);
                    }
                }
                hero_game_object.AddComponent(wallet_component);
                hero_game_object.AddComponent(health_component);

                event_manager.TriggerEvent(new RegisterGameObjectGameEvent(hero_game_object));
                hero_game_object.SetIsActive(true);
            }

            return hero_game_object;
        }
    }
}