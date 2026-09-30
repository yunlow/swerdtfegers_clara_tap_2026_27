using activity_00_tap_26_27.Core.Events;

namespace GameLibrary.States
{
    public class StateMachine
    {
        private IState _currentState;
        private EventManager _eventManager;

        public StateMachine(EventManager event_manager)
        {
            
            _eventManager = event_manager;

            _eventManager.RegisterToEvent<LogMessageGameEvent>(OnLogMessageGameEvent);

        }

        private void OnLogMessageGameEvent(IGameEvent game_event)
        {
            if (_currentState != null)
            {
                _eventManager.TriggerEvent(new LogMessageGameEvent($"Current State: {_currentState.GetType().Name}"));
            }
        }

        public void SetInitialState(IState initial_state)
        {
            _currentState = initial_state;
           _eventManager.TriggerEvent(new LogMessageGameEvent($"Initial State: {_currentState.GetType().Name}"));
            _currentState.Enter();
        }

        public void ChangeState(IState new_state)
        {
            _currentState.Exit();
            _currentState = new_state;
            _currentState.Enter();
        }

        public void Update(float elapsed_time)
        {
            _currentState.Update(elapsed_time);
        }

        public void FixedUpdate(float fixed_elapsed_time)
        {
            _currentState.FixedUpdate(fixed_elapsed_time);
        }
    }
}