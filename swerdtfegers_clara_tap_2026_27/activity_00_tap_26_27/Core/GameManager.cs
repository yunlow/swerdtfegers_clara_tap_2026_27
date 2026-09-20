using activity_00_tap_26_27.Events;
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

        public GameManager()
        {
            _eventManager.RegisterToEvent<GameActionGameEvent>(OnGameAction);
        }

        private void SendTranslatedKey(ConsoleKey console_key, EventManager event_manager)
        {
            switch (console_key)
            {
                case ConsoleKey.UpArrow:
                    {
                        event_manager.TriggerEvent(new GameActionGameEvent(GameActionType.NAVIGATE_UP));
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
