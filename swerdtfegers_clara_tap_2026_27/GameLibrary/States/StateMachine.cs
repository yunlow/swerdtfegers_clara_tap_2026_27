using activity_00_tap_26_27.Core.Events;


namespace GameLibrary.States
{
    public class StateMachine
    {
        private readonly EventManager _event_manager;

        private IState _currentState;

        public StateMachine(EventManager event_manager)
        {
            _event_manager = event_manager;
        }

        public void SetInitialState(IState initial_state)
        {
            _event_manager.TriggerEvent(new LogMessageGameEvent($"Initial state set to {initial_state.GetType().Name}"));
            _currentState = initial_state;
            _currentState.Enter();
        }

        public void ChangeState(IState new_state)
        {
            _event_manager.TriggerEvent(new LogMessageGameEvent($"{_currentState.GetType().Name} changed state to {new_state.GetType().Name}"));
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

        public IState GetCurrentState()
        {
            return _currentState;
        }
    }
}