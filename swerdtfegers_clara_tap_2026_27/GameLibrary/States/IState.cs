namespace GameLibrary.States
{
    public interface IState
    {
        void Enter();
        void Update(float elapsed_time);
        void FixedUpdate(float fixed_elapsed_time);
        void Exit();
    }
}