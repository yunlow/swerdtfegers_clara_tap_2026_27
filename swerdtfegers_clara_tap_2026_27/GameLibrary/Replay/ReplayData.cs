using System.Collections.Generic;

namespace GameLibrary.Replay
{
    public class ReplayData
    {
        public readonly int _seed;
        public readonly List<ReplayActionData> _actionTable;
        public readonly int _endFixedStepIndex = -1;

        public ReplayData(int seed, List<ReplayActionData> action_table, int end_fixed_step_index)
        {
            _seed = seed;
            _actionTable = action_table;
            _endFixedStepIndex = end_fixed_step_index;
        }
    }
}