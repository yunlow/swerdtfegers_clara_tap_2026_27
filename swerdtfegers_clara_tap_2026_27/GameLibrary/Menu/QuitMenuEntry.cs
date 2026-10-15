using activity_00_tap_26_27.Core;

namespace GameLibrary.Menu
{
    public class QuitMenuEntry : IMenuEntry
    {
        private const string TEXT = "Quit (or press Esc)";

        private readonly GameManager _gameManager;

        public QuitMenuEntry(GameManager game_manager)
        {
            _gameManager = game_manager;
        }

        public string GetText()
        {
            return TEXT;
        }

        public void Execute()
        {
            _gameManager.RequestQuit();
        }
    }
}
