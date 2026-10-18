using activity_00_tap_26_27.Core;
using GameLibrary;
using GameLibrary.States;
using System;
using System.Collections.Generic;
using System.Text;

namespace activity_00_tap_26_27.Render
{
    public class MainMenuRenderer
    {
        private GameManager _gameManager;
        public MainMenuRenderer(GameManager game_manager, ConsoleRenderManager render_manager)
        {
            _gameManager = game_manager;
        }

        public void Render(IState state, ConsoleRenderManager render_manager)
        {
            IState menu_state = state;
            if (menu_state != null)
            {
                
                render_manager.Draw(0, 0, "Main Menu", ConsoleColor.White, ConsoleColor.Black);
                render_manager.Draw(0, 1, "1. Start Game", ConsoleColor.White, ConsoleColor.Black);
                render_manager.Draw(0, 2, "2. Options", ConsoleColor.White, ConsoleColor.Black);
                render_manager.Draw(0, 3, "3. Exit", ConsoleColor.White, ConsoleColor.Black);
                render_manager.Render();
            }
        }
    }
}
