using activity_00_tap_26_27.Core;
using activity_00_tap_26_27.Core.Events;
using GameLibrary.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.Menu
{
    public class NewGameMenuEntry
    {
        private GameManager _gameManager;
        private StateMachine _stateMachine;
        private EventManager _eventManager;

        public NewGameMenuEntry(GameManager game_manager, StateMachine state_machine, EventManager event_manager)
        {
            _gameManager = game_manager;
            _stateMachine = state_machine;
            _eventManager = event_manager;
        }

        public string GetText()
        {
            return "New Game";
        }

        public void Execute()
        {
            _gameManager.StartNewGame((int)DateTime.Now.Ticks);
            _stateMachine.ChangeState(new ExploringState(_stateMachine, _gameManager, _eventManager));
        }
    }
}
