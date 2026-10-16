using activity_00_tap_26_27.Core;
using activity_00_tap_26_27.Core.Events;
using GameLibrary;
using GameLibrary.Events;
using System;
using System.IO;

namespace ConsoleGame
{
    public class SaveFileLoader
    {
        private const string SEARCH_PATTERN = "*.csv";

        private readonly string _savesDirectoryPath;
        private readonly GameManager _gameManager;
        private readonly EventManager _eventManager;

        public SaveFileLoader(string saves_directory_path, GameManager game_manager, EventManager event_manager)
        {
            _savesDirectoryPath = saves_directory_path;
            _gameManager = game_manager;
            _eventManager = event_manager;
        }

        public void LoadAllSaves()
        {
            if (!Directory.Exists(_savesDirectoryPath))
            {
                return;
            }

            string[] file_paths = Directory.GetFiles(_savesDirectoryPath, SEARCH_PATTERN);

            // GetFiles order not guaranteed: sort alphabetically.
            Array.Sort(file_paths, StringComparer.Ordinal);

            for (int file_index = 0; file_index < file_paths.Length; file_index++)
            {
                LoadSave(file_paths[file_index]);
            }
        }

        private void LoadSave(string file_path)
        {
            string file_name = Path.GetFileName(file_path);

            try
            {
                _gameManager.AddSave(file_name, File.ReadAllText(file_path));
            }
            catch (IOException exception)
            {
                // Locked file
                _eventManager.TriggerEvent(new LogMessageGameEvent($"{file_name} cannot be read: {exception.Message}"));
            }
        }
    }
}
