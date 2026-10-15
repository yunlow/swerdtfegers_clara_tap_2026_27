using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.Replay
{
    public class ReplayData
    {
        public readonly int _seed;
        public readonly List<Action> _actionTable;
        public readonly int _endFixedStepIndex = -1;
        public ReplayData( int seed, List<ReplayActionData> action_table, int end_fixed_step_index)
        {
            _seed = seed;
            _actionTable = new List<Action>();
            _endFixedStepIndex = end_fixed_step_index;
        }
    }
}
