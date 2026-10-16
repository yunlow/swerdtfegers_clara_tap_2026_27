using activity_00_tap_26_27.Core;
using activity_00_tap_26_27.Core.Events;
using GameLibrary.Csv;
using GameLibrary.CSV;
using GameLibrary.Events;
using GameLibrary.Replay;
using GameLibrary.States;

namespace GameLibrary.Menu
{
    public class LoadSaveMenuEntry : IMenuEntry
    {
        private readonly string _saveName;
        private readonly GameManager _gameManager;
        private readonly StateMachine _stateMachine;
        private readonly EventManager _eventManager;

        public LoadSaveMenuEntry(string save_name, StateMachine state_machine, GameManager game_manager, EventManager event_manager)
        {
            _saveName = save_name;
            _gameManager = game_manager;
            _stateMachine = state_machine;
            _eventManager = event_manager;
        }

        public string GetText()
        {
            return $"Load {_saveName}";
        }

        public void Execute()
        {
            if (!_saveName.Contains("(damaged)"))
            {
                ContentReport error_report = new ContentReport();
                CsvTable csv_table = CsvReader.Read(_saveName, _gameManager.GetSaveTable()[_saveName], error_report);
                ReplayData replay_data = ReplayReader.Read(csv_table, error_report);

                _gameManager.LoadReplay(replay_data);
                _stateMachine.ChangeState(new ExploringState(_stateMachine, _gameManager, _eventManager));
            }
        }
    }
}