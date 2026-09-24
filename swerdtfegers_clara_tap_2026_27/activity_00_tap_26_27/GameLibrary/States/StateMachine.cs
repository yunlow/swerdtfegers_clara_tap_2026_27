namespace activity_00_tap_26_27.States
{
    public class StateMachine
    {
        private IState _currentState;

        public void SetInitialState(IState initial_state)
        {
            _currentState = initial_state;
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