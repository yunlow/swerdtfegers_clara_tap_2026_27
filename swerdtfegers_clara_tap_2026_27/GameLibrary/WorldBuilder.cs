using activity_00_tap_26_27.Core;
using GameLibrary.Components;
using System;
using System.Collections.Generic;
using System.Text;
using activity_00_tap_26_27.Core.Events;
using activity_00_tap_26_27;
using activity_00_tap_26_27.Data;



namespace GameLibrary
{
    public class WorldBuilder
    {
        private readonly List<LocationData> _locationDataTable;
        private readonly List<ShopData> _shopDataTable;
        private readonly List<ItemData> _itemDataTable;

        public WorldBuilder(List<LocationData> location_data_table, List<ShopData> shop_data_table, List<ItemData> item_data_table)
        {
            _locationDataTable = location_data_table;
            _shopDataTable = shop_data_table;
            _itemDataTable = item_data_table;
        }

        public LocationComponent Build(EventManager event_manager)
        {
            LocationComponent root_location = null;
            Dictionary<string, GameObject> game_object_table = new Dictionary<string, GameObject>();
            Dictionary<string, LocationComponent> location_component_table = new Dictionary<string, LocationComponent>();

            for (int location_index = 0; location_index < _locationDataTable.Count; location_index++)
            {
                LocationData location_data = _locationDataTable[location_index];
                GameObject location_game_object = new GameObject(location_data._id);
                LocationComponent location_component = new LocationComponent(location_data._name);

                location_game_object.AddComponent(location_component);

                location_component_table.Add(location_data._id, location_component);
                game_object_table.Add(location_data._id, location_game_object);

                event_manager.TriggerEvent(new RegisterGameObjectGameEvent(location_game_object));
                location_game_object.SetIsActive(true);
            }

            for (int location_index = 0; location_index < _locationDataTable.Count; location_index++)
            {
                LocationData location_data = _locationDataTable[location_index];
                LocationComponent location_component = location_component_table[location_data._id];

                if (!string.IsNullOrEmpty(location_data._parentId))
                {
                    LocationComponent parent_location = location_component_table[location_data._parentId];

                    parent_location.AddConnection(location_component, location_data._travelMinutes);
                }
                else
                {
                    root_location = location_component;
                }
            }

            for (int shop_index = 0; shop_index < _shopDataTable.Count; shop_index++)
            {
                GameObject shop_game_object = game_object_table[_shopDataTable[shop_index]._locationId];

                shop_game_object.AddComponent(new ShopComponent(_shopDataTable[shop_index], _itemDataTable, shop_game_object));
            }

            return root_location;
        }
    }
}
