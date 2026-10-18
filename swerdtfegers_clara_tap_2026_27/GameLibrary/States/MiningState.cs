using activity_00_tap_26_27.Core;
using activity_00_tap_26_27.Core.Events;
using GameLibrary.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.States
{
    public class MiningState : IState
    {
        private readonly StateMachine _stateMachine;
        private readonly GameManager _gameManager;
        private readonly EventManager _eventManager;
        private readonly MineComponent _mine;
        private readonly Random _gameRandom;

        private bool _isMining;
        public MiningState(StateMachine state_machine, GameManager game_manager, EventManager event_manager, MineComponent mine, Random game_random, int stepsPerMinute)
        {
            _stateMachine = state_machine;
            _gameManager = game_manager;
            _eventManager = event_manager;
            _mine = mine;
            _gameRandom = game_random;

            _isMining = false;

        }
        public void Enter()
        {
            _eventManager.RegisterToEvent<GameActionGameEvent>(OnGameActionGameEvent);
        }
        private void OnGameActionGameEvent(IGameEvent game_event)
        {
            GameActionGameEvent actionEvent = game_event as GameActionGameEvent;
            if (actionEvent == null) return;

            if (actionEvent._gameActionType == GameActionType.CONFIRM)
            {
                if (!_isMining)
                {
                    if (_mine.GetIsActive() == true)
                    {
                        _isMining = true;

                        Console.WriteLine("Mining in progress...");
                    }
                    else
                    {
                        Console.WriteLine("The vein is exhausted.");
                    }
                }
            }
        }

        public void Exit()
        {
            _eventManager.UnregisterFromEvent<GameActionGameEvent>(OnGameActionGameEvent);
        }

        public void FixedUpdate(float fixed_elapsed_time)
        {
           
        }

        public void Update(float elapsed_time)
        {
           
        }
    }
}
