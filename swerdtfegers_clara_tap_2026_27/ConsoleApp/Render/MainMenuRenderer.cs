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
            throw new NotImplementedException();
        }
    }
}
