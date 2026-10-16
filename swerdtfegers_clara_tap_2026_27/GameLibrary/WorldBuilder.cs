using activity_00_tap_26_27.Core;
using activity_00_tap_26_27.Core.Events;
using activity_00_tap_26_27.Data;
using GameLibrary.Components;
 
using System.Collections.Generic;

namespace GameLibrary.Data
{
    public class WorldBuilder
    {
        private readonly GameContentData _gameContentData;

        public WorldBuilder(GameContentData game_content_data)
        {
            _gameContentData = game_content_data;
        }

        public LocationComponent Build(EventManager event_manager)
        {
            LocationComponent root_location = null;
            Dictionary<string, GameObject> game_object_table = new Dictionary<string, GameObject>();
            Dictionary<string, LocationComponent> location_component_table = new Dictionary<string, LocationComponent>();

            for (int location_index = 0; location_index < _gameContentData._locationDataTable.Count; location_index++)
            {
                LocationData location_data = _gameContentData._locationDataTable[location_index];
                GameObject location_game_object = new GameObject(location_data._id);
                LocationComponent location_component = new LocationComponent(location_data._name, location_game_object);

                location_game_object.AddComponent(location_component);

                location_component_table.Add(location_data._id, location_component);
                game_object_table.Add(location_data._id, location_game_object);

                event_manager.TriggerEvent(new RegisterGameObjectGameEvent(location_game_object));
                location_game_object.SetIsActive(true);
            }

            for (int location_index = 0; location_index < _gameContentData._locationDataTable.Count; location_index++)
            {
                LocationData location_data = _gameContentData._locationDataTable[location_index];
                LocationComponent location_component = location_component_table[location_data._id];

                if (!string.IsNullOrEmpty(location_data._parentId))
                {
                    LocationComponent parent_location = location_component_table[location_data._parentId];

                    parent_location.AddLocation(location_component, location_data._travelMinutes);
                }
                else
                {
                    root_location = location_component;
                }
            }

            for (int shop_index = 0; shop_index < _gameContentData._shopDataTable.Count; shop_index++)
            {
                string shop_id = _gameContentData._shopDataTable[shop_index]._locationId;
                GameObject shop_game_object = game_object_table[shop_id];
                ShopComponent shop_component = new ShopComponent(_gameContentData._shopDataTable[shop_index], shop_game_object);
                InventoryComponent inventory_component = new InventoryComponent(shop_game_object);

                for (int inventory_data_index = 0; inventory_data_index < _gameContentData._inventoryDataTable.Count; inventory_data_index++)
                {
                    InventoryEntryData current_inventory_data = _gameContentData._inventoryDataTable[inventory_data_index];

                    if (shop_id == current_inventory_data._ownerId)
                    {
                        inventory_component.Add(_gameContentData._itemDataTable[current_inventory_data._itemId], current_inventory_data._quantity);
                    }
                }

                shop_game_object.AddComponent(shop_component);
                shop_game_object.AddComponent(inventory_component);
            }

            return root_location;
        }
    }
}