using System.Collections.Generic;
using System.Diagnostics;
using System;
using activity_00_tap_26_27.Components;
using activity_00_tap_26_27.Events;

namespace activity_00_tap_26_27
{
    public class GameEngine
    {
        private const float FIXED_FRAME_TIME = 20 / 1000.0f;

        private EventManager _eventManager;
        private LogManager _logManager;
        private GameManager _gameManager;
       
        private readonly Stopwatch _stopwatch = new Stopwatch();

       

        private readonly ConsoleRenderManager _renderManager = new ConsoleRenderManager();


        private bool _shouldQuit = false;

        public void Run()
        {
            _stopwatch.Start();

            float lag = 0.0f;

            float last_time = GetCurrentTime();

            while (!_shouldQuit)
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

                _eventManager.ProcessEvents();
                _eventManager.TriggerDelayedEvents(elapsed_time);

                Update(elapsed_time);

                Render();
                
                _gameManager.Update(elapsed_time);
                _gameManager.FixedUpdate(FIXED_FRAME_TIME);

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

                

                if (player_command.Key == ConsoleKey.Escape)
                {
                    _shouldQuit = true;
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
            _renderManager.Draw(0,0, "Game in progress...\n", ConsoleColor.Magenta);
            _renderManager.Render();
        }

        private float GetCurrentTime()
        {
            return _stopwatch.ElapsedMilliseconds / 1000.0f;
        }
    }
}