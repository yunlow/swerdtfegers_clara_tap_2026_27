using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.Replay
{
    public class ReplayData
    {
        private int _seed;
        private List<Action> _actionTable;
        private int _endFixedStepIndex = -1;
        public ReplayData( int seed)
        {
            _seed = seed;
            _actionTable = new List<Action>();
        }
    }
}
