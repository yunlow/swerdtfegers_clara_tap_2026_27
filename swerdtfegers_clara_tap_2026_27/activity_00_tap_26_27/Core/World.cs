using activity_00_tap_26_27.Core.Components;
using activity_00_tap_26_27.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace activity_00_tap_26_27.Core
{
    public class World
    {
        private readonly EventManager _eventManager;

        public World(EventManager event_manager)
        {
            _eventManager = event_manager;
        }

        public GameObject BuildWorld()
        {
            GameObject world = CreateLocation("World");
            GameObject daisy_town = CreateLocation("Daisy Town");
            GameObject inn = CreateLocation("Laughing Horse Inn");
            GameObject shop = CreateLocation("Gun Shop");
            GameObject dungeon = CreateLocation("Silver Mine Dungeon");
            GameObject level_1 = CreateLocation("Dungeon Level 1");
            GameObject level_2 = CreateLocation("Dungeon Level 2");

            LinkLocations(world, daisy_town, 10.0f);
            LinkLocations(world, dungeon, 15.0f);

            LinkLocations(daisy_town, inn, 2.0f);
            LinkLocations(daisy_town, shop, 3.0f);

            LinkLocations(dungeon, level_1, 5.0f);
            LinkLocations(level_1, level_2, 8.0f);

            return world;
        }

        private GameObject CreateLocation(string name)
        {
            GameObject obj = new GameObject(name);
            LocationComponent loc = new LocationComponent(obj, name);
            obj.AddComponent(loc);
            obj.SetIsActive(true);

            _eventManager.TriggerDelayedEvent(new RegisterGameObjectGameEvent(obj));

            return obj;
        }

        private void LinkLocations(GameObject a, GameObject b, float duration)
        {
            LocationComponent comp_a = a.GetComponent<LocationComponent>();
            LocationComponent comp_b = b.GetComponent<LocationComponent>();

            comp_a.AddConnection(b, duration);
            comp_b.AddConnection(a, duration);
        }
    }
}
