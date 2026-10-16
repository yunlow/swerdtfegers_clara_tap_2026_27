using activity_00_tap_26_27.Core;
using activity_00_tap_26_27.Core.Events;
using GameLibrary.Events;
using GameLibrary.Menu;
using System.Collections.Generic;

namespace GameLibrary.States
{
    public class MainMenuState : IState
    {
        private GameManager _gameManager;
        private StateMachine _stateMachine;
        private EventManager _eventManager;

        private int _selectedItemIndex = -1;

        private readonly List<IMenuEntry> _menuItemTable = new List<IMenuEntry>();

        public MainMenuState(StateMachine state_machine, GameManager game_manager, EventManager event_manager)
        {
            _stateMachine = state_machine;
            _gameManager = game_manager;
            _eventManager = event_manager;
        }

        public void Enter()
        {
            _eventManager.RegisterToEvent<GameActionGameEvent>(OnGameActionGameEvent);
            _menuItemTable.Add(new NewGameMenuEntry(_gameManager, _stateMachine, _eventManager));

            foreach ((string key, string value) in _gameManager.GetSaveTable())
            {
                _menuItemTable.Add(new LoadSaveMenuEntry(key, _stateMachine, _gameManager, _eventManager));
            }

            _menuItemTable.Add(new ReplaySpeedMenuEntry(_gameManager));
            _menuItemTable.Add(new QuitMenuEntry(_gameManager));
        }

        private void OnGameActionGameEvent(IGameEvent game_event)
        {
            GameActionGameEvent game_action_game_event = game_event as GameActionGameEvent;

            if (game_action_game_event._gameActionType == GameActionType.CONFIRM)
            {
                if ((_selectedItemIndex >= 0) && (_selectedItemIndex < _menuItemTable.Count))
                {
                    _menuItemTable[_selectedItemIndex].Execute();
                }
            }
            else if (game_action_game_event._gameActionType == GameActionType.NAVIGATE_UP)
            {
                _selectedItemIndex = (_selectedItemIndex - 1 + _menuItemTable.Count) % _menuItemTable.Count;
            }
            else if (game_action_game_event._gameActionType == GameActionType.NAVIGATE_DOWN)
            {
                _selectedItemIndex = (_selectedItemIndex + 1) % _menuItemTable.Count;
            }
            else if (game_action_game_event._gameActionType == GameActionType.QUIT)
            {
                _gameManager.RequestQuit();
            }
        }

        public List<IMenuEntry> GetMenuItemTable()
        {
            return _menuItemTable;
        }

        public void Update(float elapsed_time)
        {

        }

        public void FixedUpdate(float fixed_elapsed_time)
        {

        }

        public void Exit()
        {
            _eventManager.UnregisterFromEvent<GameActionGameEvent>(OnGameActionGameEvent);
        }

        public int GetSelectedItemIndex()
        {
            return _selectedItemIndex;
        }
    }
}