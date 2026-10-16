using activity_00_tap_26_27.Core;
using activity_00_tap_26_27.Core.Events;
using activity_00_tap_26_27.Data;
using activity_00_tap_26_27.Render;
using ConsoleGame;
using GameLibrary;
using GameLibrary.CSV;
using GameLibrary.States;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using static System.Collections.Specialized.BitVector32;

namespace activity_00_tap_26_27
{
    public class GameEngine
    {
        private const float FIXED_FRAME_TIME = 20 / 1000.0f;

        private EventManager _eventManager;
        private LogManager _logManager;
        private GameManager _gameManager;
        private WorldBuilder _worldBuilder;

        private readonly Stopwatch _stopwatch = new Stopwatch();
        private SaveFileWriter _saveFileWriter;
        private SaveFileLoader _saveFileLoader;

        private Dictionary<Type, IScreenRenderer> _screenRenderers = new Dictionary<Type, IScreenRenderer>();

        private readonly ConsoleRenderManager _renderManager = new ConsoleRenderManager();
        private static readonly string ROOT_DIRECTORY_PATH = "../../../";
        private static readonly string SAVES_DIRECTORY_PATH = $"{ROOT_DIRECTORY_PATH}Saves/";
        private static readonly string DATA_DIRECTORY_PATH = $"{ROOT_DIRECTORY_PATH}Data/";
        private static readonly string LOG_FILE_PATH = $"{ROOT_DIRECTORY_PATH}debug.log";


        public GameEngine()
        {
            _screenRenderers.Add(typeof(MainMenuState), new MainMenuRenderer(_gameManager));
            _screenRenderers.Add(typeof(ExploringState), new ExploringRenderer(_gameManager));

            _logManager = new LogManager(_eventManager, new LogFileWriter());

        }
        private CsvTable ReadCsvTableFromFile(string file_name, ContentReport content_report)
        {
            CsvTable csv_table = null;
            string file_content = GetFileContent(file_name, content_report);

            if (file_content != null)
            {
                csv_table = CsvReader.Read(file_name, file_content, content_report);
            }

            return csv_table;
        }

        public void Run()
        {
            _stopwatch.Start();

            float last_time = GetCurrentTime();
            float lag = 0.0f;

            _eventManager.TriggerEvent(new LogMessageGameEvent("Game engine started."));

            ContentReport content_report = new ContentReport();

            GameContentData game_content_data = new GameContentData();

            {
                CsvTable location_csv_table = ReadCsvTableFromFile("locations.csv", content_report);

                if (location_csv_table != null)
                {
                    game_content_data._locationDataTable = LocationReader.Read(location_csv_table, content_report);
                }
            }

            {
                CsvTable shop_csv_table = ReadCsvTableFromFile("shops.csv", content_report);

                if (shop_csv_table != null)
                {
                    game_content_data._shopDataTable = ShopReader.Read(shop_csv_table, game_content_data._locationDataTable, content_report);
                }
            }

            {
                CsvTable item_csv_table = ReadCsvTableFromFile("items.csv", content_report);

                if (item_csv_table != null)
                {
                    game_content_data._itemDataTable = ItemReader.Read(item_csv_table, content_report);
                }
            }

            {
                CsvTable hero_csv_table = ReadCsvTableFromFile("heroes.csv", content_report);

                if (hero_csv_table != null)
                {
                    game_content_data._heroDataTable = HeroReader.Read(hero_csv_table, game_content_data._locationDataTable, content_report);
                }
            }

            {
                CsvTable inventory_csv_table = ReadCsvTableFromFile("inventories.csv", content_report);

                if (inventory_csv_table != null)
                {
                    game_content_data._inventoryDataTable = InventoryReader.Read(inventory_csv_table, game_content_data._shopDataTable, game_content_data._heroDataTable, game_content_data._itemDataTable, content_report);
                }
            }

            if (content_report.GetErrorCount() > 0)
            {
                for (int error_index = 0; error_index < content_report.GetErrorCount(); error_index++)
                {
                    _eventManager.TriggerEvent(new LogMessageGameEvent(content_report.GetErrorTextAtIndex(error_index)));
                    Console.WriteLine(content_report.GetErrorTextAtIndex(error_index));
                }
            }
            else
            {
                _gameManager = new GameManager(_eventManager, game_content_data);

                _saveFileWriter = new SaveFileWriter(SAVES_DIRECTORY_PATH, _gameManager, _eventManager);
                _saveFileLoader = new SaveFileLoader(SAVES_DIRECTORY_PATH, _gameManager, _eventManager);
                _saveFileLoader.LoadAllSaves();

                _gameManager.ShowMenu();

                _screenRenderers.Add(typeof(MainMenuState), new MainMenuRenderer(_gameManager));
                _screenRenderers.Add(typeof(ExploringState), new ExploringRenderer(_gameManager));
                _screenRenderers.Add(typeof(ShopOpenState), new ShopOpenRenderer());
                _screenRenderers.Add(typeof(ShopClosedState), new ShopClosedRenderer());

                try
                {
                    while (!_gameManager.GetShouldQuit())
                    {
                        float loop_start_time = GetCurrentTime();
                        float elapsed_time = loop_start_time - last_time;

                        ProcessInput();

                        if (_gameManager.GetIsReplaying())
                        {
                            elapsed_time *= _gameManager.GetReplaySpeed();
                        }

                        lag += elapsed_time;

                        while (lag >= FIXED_FRAME_TIME)
                        {
                            FixedUpdate(FIXED_FRAME_TIME);

                            _eventManager.ProcessDelayedEvents();

                            lag -= FIXED_FRAME_TIME;
                        }

                        Update(elapsed_time);

                        Render();

                        last_time = loop_start_time;
                    }
                }
                catch (Exception exception)
                {
                    _saveFileWriter.WriteCrashSave(_gameManager.GetSaveText());
                    _eventManager.TriggerEvent(new LogMessageGameEvent(($"The game crashed: {exception.Message}")));
                    throw;
                }
            }

        }


        private void ProcessInput()
        {
            while (Console.KeyAvailable)
            {
                ConsoleKeyInfo player_command = Console.ReadKey(true);

                bool is_valid_command = player_command.Key == ConsoleKey.UpArrow ||
                                        player_command.Key == ConsoleKey.DownArrow ||
                                        player_command.Key == ConsoleKey.Enter ||
                                        player_command.Key == ConsoleKey.Escape;

                GameActionType action = GameActionType.QUIT;

                if (ConsoleKey.UpArrow == player_command.Key)
                {
                    action = GameActionType.NAVIGATE_UP;
                }
                else if (ConsoleKey.DownArrow == player_command.Key)
                {
                    action = GameActionType.NAVIGATE_DOWN;
                }
                else if (ConsoleKey.Enter == player_command.Key)
                {
                    action = GameActionType.CONFIRM;
                }
                else if (ConsoleKey.Escape == player_command.Key)
                {
                    action = GameActionType.CANCEL;
                }

                if(is_valid_command)
                {
                    _eventManager.TriggerEvent(new GameActionGameEvent(action));
                }
            }
        }

        private static string GetFileContent(string file_name, ContentReport content_report)
        {
            string file_content = null;
            string file_path = $"../../../Data/{file_name}";

            try
            {
                file_content = File.ReadAllText(file_path);
            }
            catch (FileNotFoundException exception)
            {
                content_report.AddError(file_name, -1, $"File not found: {exception.Message} at location {file_path}");
                // Console.WriteLine($"File not found: {exception.Message}");
            }
            catch (Exception exception)
            {
                content_report.AddError(file_name, -1, $"An unexpected error happened: {exception.Message}");
                // Console.WriteLine($"An unexpected error happened: {exception.Message}");
            }

            return file_content;
        }


        private void FixedUpdate(float fixed_elapsed_time)
        {
            _gameManager.FixedUpdate(fixed_elapsed_time);
        }

        private void Update(float elapsed_time)
        {
            _gameManager.Update(elapsed_time);
        }

        private void Render()
        {
            _renderManager.Draw(0,0, "Game in progress...\n", ConsoleColor.Magenta, ConsoleColor.Black);
            _renderManager.Render(); //obligatoire de le mettre car donne la base avant d'être changé par le reste

            IScreenRenderer current_renderer = _screenRenderers[_gameManager.GetCurrentGameFlowState().GetType()];
            
            current_renderer.Render(_gameManager.GetCurrentGameFlowState(), _renderManager);
        }

        private float GetCurrentTime()
        {
            return _stopwatch.ElapsedMilliseconds / 1000.0f;
        }
    }
}