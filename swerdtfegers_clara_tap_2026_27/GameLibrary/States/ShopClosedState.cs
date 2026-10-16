using activity_00_tap_26_27.Core;
using activity_00_tap_26_27.Core.Events;
using GameLibrary.Components;
using GameLibrary.Events;

namespace GameLibrary.States
{
    public class ShopClosedState : IState
    {
        private readonly StateMachine _stateMachine;
        private readonly ShopComponent _shopComponent;
        private readonly InventoryComponent _inventoryComponent;
        private readonly GameManager _gameManager;
        private readonly EventManager _eventManager;

        public ShopClosedState(StateMachine state_machine, ShopComponent shop_component, InventoryComponent inventory_component, GameManager game_manager, EventManager event_manager)
        {
            _stateMachine = state_machine;
            _shopComponent = shop_component;
            _inventoryComponent = inventory_component;
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
                _gameManager.SetCurrentLocation(_shopComponent.GetGameObject().GetComponent<LocationComponent>().GetParentLocation());
                _stateMachine.ChangeState(new ExploringState(_stateMachine, _gameManager, _eventManager));
            }
        }

        public void Update(float elapsed_time)
        {

        }

        public void FixedUpdate(float fixed_elapsed_time)
        {
            if (_shopComponent.GetIsOpen())
            {
                _stateMachine.ChangeState(new ShopOpenState(_stateMachine, _shopComponent, _inventoryComponent, _gameManager, _eventManager));
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

        public bool GetIsReplaying()
        {
            return _gameManager.GetIsReplaying();
        }

        public int GetReplaySpeed()
        {
            return _gameManager.GetReplaySpeed();
        }
    }
}