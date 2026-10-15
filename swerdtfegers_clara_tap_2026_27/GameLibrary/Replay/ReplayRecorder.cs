using activity_00_tap_26_27.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.Replay
{
    public class ReplayRecorder
    {
        private int _seed;
        private EventManager _eventManager;
        private int _fixedStepClock;
        public ReplayRecorder(int seed, EventManager event_manager, int fixed_step_clock)
        {
            _seed = seed;
            _eventManager = event_manager;
            _fixedStepClock = fixed_step_clock;

            _eventManager.RegisterToEvent();
        }
    }
}
