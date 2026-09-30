using activity_00_tap_26_27.Core;
using activity_00_tap_26_27.Core.Events;
using GameLibrary.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.States
{
    public class MainMenuState : IState
    {
        private readonly GameManager _gameManager;
        private readonly EventManager _eventManager;
        private bool _shouldQuit;
        private readonly IState _iState;

        private LocationComponent _currentLocation;
        private int _selectedChildLocationIndex = -1;

        private readonly List<GameObject> _gameObjectTable = new List<GameObject>();

        public MainMenuState(GameManager gameManager, EventManager eventManager, IState i_state)
        {
            _gameManager = gameManager;
            _eventManager = eventManager;
            _iState = i_state;
        }
        public void Enter()
        {
            _eventManager.RegisterToEvent<GameActionGameEvent>(OnGameAction);
        }

        public void Exit()
        {
            _eventManager.UnregisterFromEvent<GameActionGameEvent>(OnGameAction);
        }

        public void FixedUpdate(float fixed_elapsed_time)
        {
            throw new NotImplementedException();
        }

        public void Update(float elapsed_time)
        {
            throw new NotImplementedException();
        }

        public void RequestQuit()
        {
            _shouldQuit = true;
        }

        public bool GetIsMenuStateActive()
        {
           if (! _shouldQuit)
            {
             return true;
            }
           return false;
        }
        
        public void StartExploration()
        {
            _eventManager.RegisterToEvent<RegisterGameObjectGameEvent>(OnRegisterGameObjectGameEvent);
            _eventManager.RegisterToEvent<UnregisterGameObjectGameEvent>(OnUnregisterGameObjectGameEvent);
            _eventManager.RegisterToEvent<GameActionGameEvent>(OnGameActionGameEvent);

            _eventManager.TriggerDelayedEvent(new LogMessageGameEvent("Game manager started. DELAYED"));
            _eventManager.TriggerEvent(new LogMessageGameEvent("Game manager started."));

            GameObject test_game_object = new GameObject("test_game_objet");

            _eventManager.TriggerDelayedEvent(new RegisterGameObjectGameEvent(test_game_object));

            LocationComponent world_location = CreateLocation("World", _eventManager, null, 0.0f);

            _currentLocation = world_location;

            LocationComponent town_location = CreateLocation("Daisy Town", _eventManager, world_location, 10.0f);
            LocationComponent inn_location = CreateLocation("Laughing Horse Inn", _eventManager, town_location, 5.0f);
            LocationComponent shop_location = CreateLocation("Gun Shop", _eventManager, town_location, 3.0f);

            LocationComponent dungeon_location = CreateLocation("Silver Mine Dungeon", _eventManager, world_location, 15.0f);
            LocationComponent dungeon_level_one_location = CreateLocation("Dungeon level 1", _eventManager, dungeon_location, 2.0f);
            LocationComponent dungeon_level_two_location = CreateLocation("Dungeon level 2", _eventManager, dungeon_level_one_location, 5.0f);
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

        public LocationComponent GetParentLocation()
        {
            return null;
        }

        private void OnGameAction(IGameEvent game_event)
        {
          
        }
        
    }
}
