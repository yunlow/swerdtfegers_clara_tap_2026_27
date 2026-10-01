using activity_00_tap_26_27.Core;
using activity_00_tap_26_27.Core.Events;
using activity_00_tap_26_27.Core.Events;
using GameLibrary.Components;
using GameLibrary.States;
using System;
using System.Collections.Generic;
using System.IO;

namespace activity_00_tap_26_27.Core
{
    public class GameManager
    {
        private readonly EventManager _eventManager;
        private readonly List<GameObject> _gameObjectTable = new List<GameObject>();

        private LocationComponent _currentLocation;
        private int _selectedChildLocationIndex = -1;

        private StateMachine _gameFlowStateStateMachine;
        private bool _isMenuStateActive = true;

        private bool _shouldQuit = false;

        public GameManager(EventManager event_manager)
        {
            _eventManager = event_manager;
            _gameFlowStateStateMachine = new StateMachine(_eventManager);
            _gameFlowStateStateMachine.SetInitialState(new MainMenuState(this, _eventManager, _gameFlowStateStateMachine));

            event_manager.RegisterToEvent<RegisterGameObjectGameEvent>(OnRegisterGameObjectGameEvent);
            event_manager.RegisterToEvent<UnregisterGameObjectGameEvent>(OnUnregisterGameObjectGameEvent);

            event_manager.TriggerEvent(new LogMessageGameEvent("Game manager started."));

            GameObject test_game_object = new GameObject("test_game_objet");

            event_manager.TriggerDelayedEvent(new RegisterGameObjectGameEvent(test_game_object));


        }

        public void StartExploration()
        {
            string file_path = Path.GetFullPath("../../../Data/locations.csv");
            if (!File.Exists(file_path))
            {
                Console.WriteLine($"file locations.csv not found at {file_path}");
            }
            else
            {
                try
                {
                    string content = File.ReadAllText(file_path);
                }
                catch (IOException)
                {
                    // Cas réel : le fichier est encore ouvert.
                    Console.WriteLine($"locations.csv cannot be read. Close it in your spreadsheet and try again.");
                }

            }
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

        public int GetSelectedDestinationIndex()
        {
            return _selectedChildLocationIndex;
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
        public void SetSelectedChildLocationIndex(int selected_child_location_index)
        {
            _selectedChildLocationIndex = selected_child_location_index;
        }


        public void SetCurrentLocation(LocationComponent location_component)
        {
            _currentLocation = location_component;
        }
        public void SetIsMenuStateActive(bool is_menu_state_active)
        {
            _isMenuStateActive = is_menu_state_active;
        }
        public bool GetIsMenuStateActive()
        {
            return _isMenuStateActive;
        }
        public void RequestQuit()
        {
            _shouldQuit = true;
        }   

        public bool GetShouldQuit()
        {
            return _shouldQuit;
        }
        public IState GetCurrentGameFlowState()
        {
            return _gameFlowStateStateMachine as IState;
        }
    }
}