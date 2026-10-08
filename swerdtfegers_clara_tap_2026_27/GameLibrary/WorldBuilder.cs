using activity_00_tap_26_27.Core;
using GameLibrary.Components;
using System;
using System.Collections.Generic;
using System.Text;
using activity_00_tap_26_27.Core.Events;
using activity_00_tap_26_27;



namespace GameLibrary
{
    public class WorldBuilder
    {
        public static Dictionary<string, LocationComponent> Build(List<LocationData> location_datas)
        {
            Dictionary<string, LocationComponent> location_table = new Dictionary<string, LocationComponent>();

            
            for (int i = 0; i < location_datas.Count; i++)
            {
                LocationData data = location_datas[i];

                GameObject game_object = new GameObject(data._name);
                LocationComponent location_component = new LocationComponent(data._id, data._name);
                game_object.AddComponent(location_component);

                location_table.Add(data._id, location_component);
            }

            for (int i = 0; i < location_datas.Count; i++)
            {
                LocationData data = location_datas[i];

                if (data._parentId.Length > 0)
                {
                    if (location_table.ContainsKey(data._id) == false || location_table.ContainsKey(data._parentId) == false)
                    {
                        throw new ArgumentException("Programmer error: Invalid location or parent ID in WorldBuilder.");
                    }

                    LocationComponent child = location_table[data._id];
                    LocationComponent parent = location_table[data._parentId];

                    ConnectLocations(parent, child, data._travelMinutes);
                }
            }

            return location_table;
        }

        private static void ConnectLocations(LocationComponent parent, LocationComponent child, int travel_minutes)
        {
            parent.AddNeighbor(child, travel_minutes);
            child.AddNeighbor(parent, travel_minutes);
        }
    }
}
