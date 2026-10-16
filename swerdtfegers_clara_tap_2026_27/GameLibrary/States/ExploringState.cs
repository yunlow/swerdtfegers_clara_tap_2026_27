using activity_00_tap_26_27.Core;
using activity_00_tap_26_27.Core.Events;
using GameLibrary.Components;
using GameLibrary.Events;

namespace GameLibrary.States
{
    public class ExploringState : IState
    {
        private StateMachine _stateMachine;
        private EventManager _eventManager;
        private GameManager _gameManager;

        public ExploringState(StateMachine state_machine, GameManager game_manager, EventManager event_manager)
        {
            _stateMachine = state_machine;
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

            _eventManager.TriggerEvent(new LogMessageGameEvent($"Command {game_action_game_event._gameActionType} received!"));

            if (game_action_game_event._gameActionType == GameActionType.NAVIGATE_DOWN)
            {
                int selected_destination_index = _gameManager.GetSelectedDestinationIndex();

                _gameManager.SetSelectedChildLocationIndex((selected_destination_index + 1) % _gameManager.GetCurrentLocation().GetDestinationCount());
            }
            else if (game_action_game_event._gameActionType == GameActionType.NAVIGATE_UP)
            {
                int selected_destination_index = _gameManager.GetSelectedDestinationIndex();
                int destination_count = _gameManager.GetCurrentLocation().GetDestinationCount();

                _gameManager.SetSelectedChildLocationIndex((selected_destination_index - 1 + destination_count) % destination_count);
            }
            else if (game_action_game_event._gameActionType == GameActionType.CONFIRM)
            {
                int selected_destination_index = _gameManager.GetSelectedDestinationIndex();

                if (selected_destination_index != -1)
                {
                    LocationComponent current_location = _gameManager.GetCurrentLocation();
                    Connection selected_connection = current_location.GetDestinationAtIndex(selected_destination_index);

                    LocationComponent destination_location = selected_connection.GetDestinationLocation();

                    _eventManager.TriggerEvent(new LogMessageGameEvent($"Location changed from {current_location.GetName()} to {destination_location.GetName()} - time taken: {selected_connection.GetTimeToReachDestination()}."));
                    _gameManager.SetCurrentLocation(destination_location);

                    ShopComponent shop_component = destination_location.GetGameObject().GetComponent<ShopComponent>();

                    if (shop_component != null)
                    {
                        InventoryComponent inventory_component = destination_location.GetGameObject().GetComponent<InventoryComponent>();

                        if (shop_component.GetIsOpen())
                        {
                            _stateMachine.ChangeState(new ShopOpenState(_stateMachine, shop_component, inventory_component, _gameManager, _eventManager));
                        }
                        else
                        {
                            _stateMachine.ChangeState(new ShopClosedState(_stateMachine, shop_component, inventory_component, _gameManager, _eventManager));
                        }
                    }
                }
            }
            else if (game_action_game_event._gameActionType == GameActionType.CANCEL)
            {
                _gameManager.SetSelectedChildLocationIndex(-1);
            }
            else if (game_action_game_event._gameActionType == GameActionType.QUIT)
            {
                LocationComponent parent_location = _gameManager.GetCurrentLocation().GetParentLocation();

                if (parent_location != null)
                {
                    _gameManager.SetCurrentLocation(parent_location);
                }
                else
                {
                    _gameManager.EndGame();
                    _stateMachine.ChangeState(new MainMenuState(_stateMachine, _gameManager, _eventManager));
                }
            }
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

        public LocationComponent GetCurrentLocation()
        {
            return _gameManager.GetCurrentLocation();
        }

        public int GetSelectedDestinationIndex()
        {
            return _gameManager.GetSelectedDestinationIndex();
        }

        public bool GetIsReplaying()
        {
            return _gameManager.GetIsReplaying();
        }

        public GameObject GetHeroObject()
        {
            return _gameManager.GetHeroObject();
        }

        public int GetReplaySpeed()
        {
            return _gameManager.GetReplaySpeed();
        }
    }
}