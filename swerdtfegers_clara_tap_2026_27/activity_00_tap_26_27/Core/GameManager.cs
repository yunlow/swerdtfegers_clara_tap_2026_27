using activity_00_tap_26_27.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace activity_00_tap_26_27.Core
{
    public class GameManager
    {
        private readonly List<GameObject> _gameObjectTable = new List<GameObject>();
        private EventManager _eventManager;

        private bool _shouldQuit = false;

        private GameObject _currentLocation;
        private int _selectedDestinationIndex = -1;

        public GameManager(EventManager event_manager)
        {
            _eventManager = event_manager;
            _eventManager.RegisterToEvent<RegisterGameObjectGameEvent>(OnRegisterGameObject);
            _eventManager.RegisterToEvent<UnregisterGameObjectGameEvent>(OnUnregisterGameObject);
            _eventManager.RegisterToEvent<GameActionGameEvent>(OnGameAction);

            World world = new World(_eventManager);
            _currentLocation = world.BuildWorld();
        }

        private void OnRegisterGameObject(IGameEvent game_event)
        {
            if (game_event is RegisterGameObjectGameEvent register_event)
            {
                GameObject game_object = register_event.GetGameObject();
                if (game_object != null && !_gameObjectTable.Contains(game_object))
                {
                    _gameObjectTable.Add(game_object);
                }
            }
        }

        private void OnUnregisterGameObject(IGameEvent game_event)
        {
            if (game_event is UnregisterGameObjectGameEvent unregister_event)
            {
                GameObject game_object = unregister_event.GetGameObject();
                if (game_object != null)
                {
                    _gameObjectTable.Remove(game_object);
                }
            }
        }

        private void OnGameAction(IGameEvent game_event)
        {
           
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
        public bool GetShouldQuit()
        {
            return _shouldQuit;
        }
    }
}
