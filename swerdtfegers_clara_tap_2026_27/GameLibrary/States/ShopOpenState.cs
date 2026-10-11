using activity_00_tap_26_27.Core;
using activity_00_tap_26_27.Core.Events;
using GameLibrary.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.States
{
    public class ShopOpenState : IState
    {
        private readonly StateMachine _stateMachine;
        private readonly ShopComponent _shopComponent;
        private readonly GameManager _gameManager;
        private readonly EventManager _eventManager;

        private int _selectedItemIndex = -1;
        private string _currentSpeech = "";

        public ShopOpenState(StateMachine state_machine, ShopComponent shop_component, GameManager game_manager, EventManager event_manager)
        {
            _stateMachine = state_machine;
            _shopComponent = shop_component;
            _gameManager = game_manager;
            _eventManager = event_manager;
        }

        public void Enter()
        {
            _eventManager.RegisterToEvent<GameActionGameEvent>(OnGameActionGameEvent);
        }

        private void OnGameActionGameEvent(IGameEvent game_event)
        {
            GameActionGameEvent game_action_game_event = game_event as GameActionGameEvent;

            if ((game_action_game_event._gameActionType == GameActionType.CANCEL) || (game_action_game_event._gameActionType == GameActionType.QUIT))
            {
                _gameManager.SetCurrentLocation(null);
                _stateMachine.ChangeState(new ExploringState(_stateMachine, _gameManager, _eventManager));
            }
            else if (game_action_game_event._gameActionType == GameActionType.NAVIGATE_UP)
            {
                _selectedItemIndex = (_selectedItemIndex - 1 + _shopComponent.GetItemCount()) % _shopComponent.GetItemCount();
            }
            else if (game_action_game_event._gameActionType == GameActionType.NAVIGATE_DOWN)
            {
                _selectedItemIndex = (_selectedItemIndex + 1) % _shopComponent.GetItemCount();
            }
            else if (game_action_game_event._gameActionType == GameActionType.CONFIRM)
            {
                if ((_selectedItemIndex >= 0) && (_selectedItemIndex < _shopComponent.GetItemCount()))
                {
                    if (_shopComponent.TryRemoveOneAtIndex(_selectedItemIndex))
                    {
                        _currentSpeech = $"You bought {_shopComponent.GetItemAtIndex(_selectedItemIndex)._name} for {_shopComponent.GetItemAtIndex(_selectedItemIndex)._price}$";
                    }
                }
            }
        }

        public void Update(float elapsed_time)
        {

        }

        public void FixedUpdate(float fixed_elapsed_time)
        {
            if (!_shopComponent.GetIsOpen())
            {
                _stateMachine.ChangeState(new ShopClosedState(_stateMachine, _shopComponent, _gameManager, _eventManager));
            }
        }

        public void Exit()
        {
            _eventManager.UnregisterFromEvent<GameActionGameEvent>(OnGameActionGameEvent);
        }

        public ShopComponent GetShopComponent()
        {
            return _shopComponent;
        }

        public int GetSelectedItemIndex()
        {
            return _selectedItemIndex;
        }

        public string GetCurrentSpeech()
        {
            return _currentSpeech;
        }
    }

}
