using activity_00_tap_26_27.Core;
using activity_00_tap_26_27.Core.Events;
using activity_00_tap_26_27.Data;
using activity_00_tap_26_27.Render;
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
       
        private readonly Stopwatch _stopwatch = new Stopwatch();

       
        private Dictionary<Type, IScreenRenderer> _screenRenderers = new Dictionary<Type, IScreenRenderer>();

        private readonly ConsoleRenderManager _renderManager = new ConsoleRenderManager();


     public GameEngine()
        {
            _screenRenderers.Add(typeof(MainMenuState), new MainMenuRenderer(_gameManager));
            _screenRenderers.Add(typeof(ExploringState), new ExploringRenderer(_gameManager));

            
        }

        public void Run()
        {
            _stopwatch.Start();

            float last_time = GetCurrentTime();
            float lag = 0.0f;

            _eventManager.TriggerEvent(new LogMessageGameEvent("Game engine started."));

            ContentReport content_report = new ContentReport();

            List<LocationData> location_data_table = null;
            List<ShopData> shop_data_table = null;
            List<ItemData> item_data_table = null;

            {
                string locations_content = GetFileContent("locations.csv", content_report);

                if (locations_content != null)
                {
                    CsvTable location_csv_table = CsvReader.Read("locations.csv", locations_content, content_report);

                    if (location_csv_table != null)
                    {
                        location_data_table = LocationReader.Read(location_csv_table, content_report);
                    }
                }
            }

            {
                string shops_content = GetFileContent("shops.csv", content_report);

                if (shops_content != null)
                {
                    CsvTable shop_csv_table = CsvReader.Read("shops.csv", shops_content, content_report);

                    if (shop_csv_table != null)
                    {
                        shop_data_table = ShopReader.Read(shop_csv_table, location_data_table, content_report);
                    }
                }
            }

            {
                string items_content = GetFileContent("items.csv", content_report);

                if (items_content != null)
                {
                    CsvTable item_csv_table = CsvReader.Read("items.csv", items_content, content_report);

                    if (item_csv_table != null)
                    {
                        item_data_table = ItemReader.Read(item_csv_table, shop_data_table, content_report);
                    }
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
                _gameManager = new GameManager(_eventManager);

                _screenRenderers.Add(typeof(MainMenuState), new MainMenuRenderer(_gameManager));
                _screenRenderers.Add(typeof(ExploringState), new ExploringRenderer(_gameManager));
                _screenRenderers.Add(typeof(ShopOpenState), new ShopOpenRenderer());
                _screenRenderers.Add(typeof(ShopClosedState), new ShopClosedRenderer());

                while (!_gameManager.GetShouldQuit())
                {
                    float loop_start_time = GetCurrentTime();
                    float elapsed_time = loop_start_time - last_time;

                    lag += elapsed_time;

                    ProcessInput();

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