using activity_00_tap_26_27.Core;
using activity_00_tap_26_27.Core.Components;
using activity_00_tap_26_27.Core.Events;
using activity_00_tap_26_27.Core.Components;
using activity_00_tap_26_27.Core.Events;
using System.Collections.Generic;

namespace activity_00_tap_26_27.Core
{
    public class GameManager
    {
        private readonly EventManager _eventManager;
        private readonly List<GameObject> _gameObjectTable = new List<GameObject>();

        private LocationComponent _currentLocation;
        private int _selectedChildLocationIndex = -1;

        private bool _shouldQuit = false;

        public GameManager(EventManager event_manager)
        {
            _eventManager = event_manager;

            event_manager.RegisterToEvent<RegisterGameObjectGameEvent>(OnRegisterGameObjectGameEvent);
            event_manager.RegisterToEvent<UnregisterGameObjectGameEvent>(OnUnregisterGameObjectGameEvent);
            event_manager.RegisterToEvent<GameActionGameEvent>(OnGameActionGameEvent);

            event_manager.TriggerDelayedEvent(new LogMessageGameEvent("Game manager started. DELAYED"));
            event_manager.TriggerEvent(new LogMessageGameEvent("Game manager started."));

            GameObject test_game_object = new GameObject("test_game_objet");

            event_manager.TriggerDelayedEvent(new RegisterGameObjectGameEvent(test_game_object));

            LocationComponent world_location = CreateLocation("World", event_manager, null, 0.0f);

            _currentLocation = world_location;

            LocationComponent town_location = CreateLocation("Daisy Town", event_manager, world_location, 10.0f);
            LocationComponent inn_location = CreateLocation("Laughing Horse Inn", event_manager, town_location, 5.0f);
            LocationComponent shop_location = CreateLocation("Gun Shop", event_manager, town_location, 3.0f);

            LocationComponent dungeon_location = CreateLocation("Silver Mine Dungeon", event_manager, world_location, 15.0f);
            LocationComponent dungeon_level_one_location = CreateLocation("Dungeon level 1", event_manager, dungeon_location, 2.0f);
            LocationComponent dungeon_level_two_location = CreateLocation("Dungeon level 2", event_manager, dungeon_level_one_location, 5.0f);
        }

        private LocationComponent CreateLocation(string location_name, EventManager event_manager, LocationComponent parent_location, float distance_to_parent)
        {
            GameObject game_object = new GameObject(location_name);
            LocationComponent location_component = new LocationComponent(location_name);

            game_object.AddComponent(location_component);

            if (parent_location != null)
            {
                parent_location.AddConnection(location_component, distance_to_parent);
            }

            event_manager.TriggerDelayedEvent(new RegisterGameObjectGameEvent(game_object));

            return location_component;
        }

        private void OnGameActionGameEvent(IGameEvent game_event)
        {
            GameActionGameEvent game_action_game_event = game_event as GameActionGameEvent;

            _eventManager.TriggerEvent(new LogMessageGameEvent($"Command {game_action_game_event._gameActionType} received!"));

            if (game_action_game_event._gameActionType == GameActionType.NAVIGATE_DOWN)
            {
                _selectedChildLocationIndex = (_selectedChildLocationIndex + 1) % _currentLocation.GetDestinationCount();
            }
            else if (game_action_game_event._gameActionType == GameActionType.NAVIGATE_UP)
            {
                _selectedChildLocationIndex = (_selectedChildLocationIndex - 1 + _currentLocation.GetDestinationCount()) % _currentLocation.GetDestinationCount();
            }
            else if (game_action_game_event._gameActionType == GameActionType.CONFIRM)
            {
                Connection selected_connection = _currentLocation.GetDestinationAtIndex(_selectedChildLocationIndex);
                _eventManager.TriggerEvent(new LogMessageGameEvent($"Location changed from {_currentLocation.GetName()} to {selected_connection.GetDestinationLocation().GetName()} - time taken: {selected_connection.GetTimeToReachDestination()}."));
                _currentLocation = selected_connection.GetDestinationLocation();
            }
            else if (game_action_game_event._gameActionType == GameActionType.CANCEL)
            {
                _selectedChildLocationIndex = -1;
            }
            else if (game_action_game_event._gameActionType == GameActionType.QUIT)
            {
                _shouldQuit = true;
            }
        }

        public int GetSelectedDestinationIndex()
        {
            return _selectedChildLocationIndex;
        }

        private void OnRegisterGameObjectGameEvent(IGameEvent game_event)
        {
            RegisterGameObjectGameEvent register_game_object_game_event = game_event as RegisterGameObjectGameEvent;

            _gameObjectTable.Add(register_game_object_game_event._gameObject);
        }

        private void OnUnregisterGameObjectGameEvent(IGameEvent game_event)
        {
            UnregisterGameObjectGameEvent unregister_game_object_game_event = game_event as UnregisterGameObjectGameEvent;

            _gameObjectTable.Remove(unregister_game_object_game_event._gameObject);
        }

        public void FixedUpdate(float fixed_elapsed_time)
        {
            for (int object_index = 0; object_index < _gameObjectTable.Count; object_index++)
            {
                GameObject game_object = _gameObjectTable[object_index];

                if (game_object.GetIsActive())
                {
                    game_object.FixedUpdate(fixed_elapsed_time);
                }
            }
        }

        public void Update(float elapsed_time)
        {
            for (int object_index = 0; object_index < _gameObjectTable.Count; object_index++)
            {
                GameObject game_object = _gameObjectTable[object_index];

                if (game_object.GetIsActive())
                {
                    game_object.Update(elapsed_time);
                }
            }
        }

        public LocationComponent GetCurrentLocation()
        {
            return _currentLocation;
        }

        public bool GetShouldQuit()
        {
            return _shouldQuit;
        }
    }
}