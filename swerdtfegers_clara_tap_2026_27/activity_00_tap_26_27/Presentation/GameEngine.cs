
using activity_00_tap_26_27.Core;
using activity_00_tap_26_27.Core.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static System.Collections.Specialized.BitVector32;

namespace activity_00_tap_26_27.Presentation
{
    public class GameEngine
    {
        private const float FIXED_FRAME_TIME = 20 / 1000.0f;

        private EventManager _eventManager;
        private LogManager _logManager;
        private GameManager _gameManager;
       
        private readonly Stopwatch _stopwatch = new Stopwatch();

       

        private readonly ConsoleRenderManager _renderManager = new ConsoleRenderManager();


     

        public void Run()
        {
            _stopwatch.Start();

            float lag = 0.0f;

            float last_time = GetCurrentTime();

            while (!_gameManager.GetShouldQuit())
            {
                float loop_start_time = GetCurrentTime();
                float elapsed_time = loop_start_time - last_time;

                lag += elapsed_time;

                ProcessInput();

                while(lag >= FIXED_FRAME_TIME)
                {
                    FixedUpdate(FIXED_FRAME_TIME);
                    lag -= FIXED_FRAME_TIME;
                }

          
                
               

                Update(elapsed_time);

                Render();
                
                _gameManager.Update(elapsed_time);
                _gameManager.FixedUpdate(FIXED_FRAME_TIME);

                _eventManager.ProcessDelayedEvents();

                last_time = loop_start_time;

                _gameManager.GetShouldQuit();
            }

            _logManager.Log("GameEngine.Run() - Game loop has exited.");

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

        private void FixedUpdate(float fixed_elapsed_time)
        {

        }

        private void Update(float elapsed_time)
        {
           
        }

        private void Render()
        {
            _renderManager.Draw(0,0, "Game in progress...\n", ConsoleColor.Magenta, ConsoleColor.Black);
            _renderManager.Render();
        }

        private float GetCurrentTime()
        {
            return _stopwatch.ElapsedMilliseconds / 1000.0f;
        }
    }
}