using activity_00_tap_26_27.Core;
using activity_00_tap_26_27.Core.Events;
using activity_00_tap_26_27.Core.Events;
using System.Collections.Generic;
using GameLibrary.Components;
using GameLibrary.States;

namespace activity_00_tap_26_27.Core
{
    public class GameManager
    {
        private readonly EventManager _eventManager;
        private readonly List<GameObject> _gameObjectTable = new List<GameObject>();

        private LocationComponent _currentLocation;
        private int _selectedChildLocationIndex = -1;

        private StateMachine _gameFlowStateStateMachine;

        private bool _shouldQuit = false;

        public GameManager(EventManager event_manager, StateMachine game_flow_state_state_machine)
        {
            _eventManager = event_manager;
            _gameFlowStateStateMachine = game_flow_state_state_machine;

           
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

        public void SetCurrentLocation(LocationComponent location_component)
        {
            _currentLocation = location_component;
        }

        public bool GetShouldQuit()
        {
            return _shouldQuit;
        }
    }
}