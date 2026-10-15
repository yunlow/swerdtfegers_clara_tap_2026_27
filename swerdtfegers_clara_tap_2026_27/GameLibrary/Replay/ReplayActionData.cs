using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.Replay
{
    public class ReplayActionData
    {
        private int _fixedStepIndex;
        public ReplayActionData(int fixed_step_index, Action action)
        {
            _fixedStepIndex = fixed_step_index;
        }
    }
}
