using activity_00_tap_26_27.Core;

namespace GameLibrary.Menu
{
    public class ReplaySpeedMenuEntry : IMenuEntry
    {
        private int _replaySpeedIndex = 2;
        private readonly int[] _replaySpeedTable = { 1, 5, 20, 100 };
        private GameManager _gameManager;

        public ReplaySpeedMenuEntry(GameManager game_manager)
        {
            _gameManager = game_manager;
            _gameManager.SetReplaySpeed(_replaySpeedTable[_replaySpeedIndex]);
        }

        public string GetText()
        {
            return $"Replay speed x{_replaySpeedTable[_replaySpeedIndex]}";
        }

        public void Execute()
        {
            _replaySpeedIndex = (_replaySpeedIndex + 1) % _replaySpeedTable.Length;
            _gameManager.SetReplaySpeed(_replaySpeedTable[_replaySpeedIndex]);
        }
    }
}