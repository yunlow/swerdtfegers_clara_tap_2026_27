using activity_00_tap_26_27.Core;
using activity_00_tap_26_27.Core.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.Replay
{
    public class ReplayRecorder
    {
        private int _seed;
        private List<ReplayActionData> _replayActionTable = new List<ReplayActionData>();
        private EventManager _eventManager;
        private IFixedStepClock _fixedStepClock;
        public ReplayRecorder(int seed, EventManager event_manager, IFixedStepClock fixed_step_clock)
        {
            _seed = seed;
            _eventManager = event_manager;
            _fixedStepClock = fixed_step_clock;

            if (event_manager != null)
            {
                event_manager.RegisterToEvent<GameActionGameEvent>(OnGameActionGameEvent);
            }
        }

        private void OnGameActionGameEvent(IGameEvent game_event)
        {
            GameActionGameEvent game_action_game_event = game_event as GameActionGameEvent;

            Record(_fixedStepClock.GetFixedStepIndex(), game_action_game_event._gameActionType);
        }

        public void Record(int fixed_step_index, GameActionType action)
        {
            ReplayActionData replay_action_data = new ReplayActionData(fixed_step_index, action);

            _replayActionTable.Add(replay_action_data);
        }

        public void StopRecording()
        {
            _eventManager.UnregisterFromEvent<GameActionGameEvent>(OnGameActionGameEvent);
        }

        public int GetActionCount()
        {
            return _replayActionTable.Count;
        }

        public string GetCsvText(int end_fixed_step_index)
        {
            StringBuilder csv_text_string_builder = new StringBuilder(GetCsvTextWithoutEnd());

            if (end_fixed_step_index > -1)
            {
                csv_text_string_builder.AppendLine($"{end_fixed_step_index};END;");
            }

            return csv_text_string_builder.ToString();
        }

        public string GetCsvTextWithoutEnd()
        {
            StringBuilder csv_text_string_builder = new StringBuilder();

            csv_text_string_builder.AppendLine("Step;Type;Value");
            csv_text_string_builder.AppendLine($"0;SEED;{_seed}");

            for (int replay_action_index = 0; replay_action_index < _replayActionTable.Count; replay_action_index++)
            {
                ReplayActionData current_replay_action_data = _replayActionTable[replay_action_index];

                csv_text_string_builder.AppendLine($"{current_replay_action_data._fixedStepIndex};ACTION;{current_replay_action_data._action}");
            }

            return csv_text_string_builder.ToString();
        }
    }

}
}
