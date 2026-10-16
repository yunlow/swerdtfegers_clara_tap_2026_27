using activity_00_tap_26_27.Core;
using activity_00_tap_26_27.Core.Events;
using GameLibrary;
using GameLibrary.Events;
using System;
using System.Globalization;
using System.IO;

namespace ConsoleGame
{
    public class SaveFileWriter
    {
        private const string SAVE_PREFIX = "save";
        private const string CRASH_PREFIX = "crash";
        private const string FILE_EXTENSION = ".csv";
        private const string DATE_FORMAT = "yyyy_MM_dd_HH_mm_ss";

        private readonly string _savesDirectoryPath;
        private readonly GameManager _gameManager;
        private readonly EventManager _eventManager;

        public SaveFileWriter(string saves_directory_path, GameManager game_manager, EventManager event_manager)
        {
            _savesDirectoryPath = saves_directory_path;
            _gameManager = game_manager;
            _eventManager = event_manager;

            Directory.CreateDirectory(_savesDirectoryPath);
            _eventManager.RegisterToEvent<SaveRequestedGameEvent>(OnSaveRequestedGameEvent);
        }

        public void WriteCrashSave(string save_text)
        {
            if (string.IsNullOrEmpty(save_text))
            {
                return;
            }

            try
            {
                WriteNewFile(CRASH_PREFIX, save_text);
            }
            catch (IOException)
            {
                // Disk refuses, let the exception go up.
            }
        }

        private void OnSaveRequestedGameEvent(IGameEvent game_event)
        {
            SaveRequestedGameEvent save_event = game_event as SaveRequestedGameEvent;

            try
            {
                string file_name = WriteNewFile(SAVE_PREFIX, save_event._saveText);

                _gameManager.AddSave(file_name, save_event._saveText);
            }
            catch (IOException exception)
            {
                _eventManager.TriggerEvent(new LogMessageGameEvent($"The save could not be written: {exception.Message}"));
            }
        }

        private string WriteNewFile(string prefix, string content)
        {
            string date_text = DateTime.Now.ToString(DATE_FORMAT, CultureInfo.InvariantCulture);
            string file_name = prefix + "_" + date_text + FILE_EXTENSION;
            int copy_number = 2;

            while (File.Exists(Path.Combine(_savesDirectoryPath, file_name)))
            {
                file_name = prefix + "_" + date_text + "_" + copy_number + FILE_EXTENSION;
                copy_number++;
            }

            File.WriteAllText(Path.Combine(_savesDirectoryPath, file_name), content);

            return file_name;
        }
    }
}
