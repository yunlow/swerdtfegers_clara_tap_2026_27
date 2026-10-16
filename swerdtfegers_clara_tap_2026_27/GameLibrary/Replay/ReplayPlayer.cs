using activity_00_tap_26_27.Core;
using activity_00_tap_26_27.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.Replay
{
    public class ReplayPlayer
    {
        private ReplayData _replayData;
        private EventManager _eventManager;
        private IFixedStepClock _fixedStepClock;
        private int _currentReplayIndex;
        public ReplayPlayer(ReplayData replay_data, EventManager event_manager, IFixedStepClock fixed_step_clock)
        {
            _replayData = replay_data;
            _eventManager = event_manager;
            _fixedStepClock = fixed_step_clock;

        }

        public void TriggerActionsOfStep(int fixed_step_index, EventManager event_manager)
        {
            while ((_replayData._actionTable.Count > _currentReplayIndex) && (_replayData._actionTable[_currentReplayIndex]._fixedStepIndex <= fixed_step_index))
            {
                GameActionType game_action_type = _replayData._actionTable[_currentReplayIndex]._action;

                if (_replayData._endFixedStepIndex != fixed_step_index)
                {
                    event_manager.TriggerDelayedEvent(new GameActionGameEvent(game_action_type));
                }

                ++_currentReplayIndex;
            }
            event_manager.TriggerEvent(action);
            }
        }

        public bool GetIsFinished(int fixed_step_index)
        {
            bool is_finished = false;

            if (_replayData._endFixedStepIndex >= 0)
            {
                is_finished = _replayData._endFixedStepIndex <= fixed_step_index;
            }
            else
            {
                is_finished = _replayData._actionTable.Count <= 0 || _replayData._actionTable[_replayData._actionTable.Count - 1]._fixedStepIndex < fixed_step_index;
            }

            return is_finished;
        }

        public void FixedUpdate(float fixed_elapsed_time)
        {
            if (!GetIsFinished(_fixedStepClock.GetFixedStepIndex()))
            {
                TriggerActionsOfStep(_fixedStepClock.GetFixedStepIndex(), _eventManager);
            }
        }

    }
}
