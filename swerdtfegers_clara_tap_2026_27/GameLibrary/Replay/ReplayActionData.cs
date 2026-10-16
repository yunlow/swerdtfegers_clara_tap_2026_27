using activity_00_tap_26_27.Core;

namespace GameLibrary.Replay
{
    public class ReplayActionData
    {
        public readonly int _fixedStepIndex;
        public readonly GameActionType _action;

        public ReplayActionData(int fixed_step_index, GameActionType action)
        {
            _fixedStepIndex = fixed_step_index;
            _action = action;
        }
    }
}