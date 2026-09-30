using activity_00_tap_26_27.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.States
{
    public class ExploringState : IState
    {
        private readonly GameManager _gameManager;
        private bool shouldQuit;

        public ExploringState(GameManager game_manager)
        {
            _gameManager = game_manager;
        }
        public void Enter()
        {
            throw new NotImplementedException();
        }

        public void Exit()
        {
            throw new NotImplementedException();
        }

        public void FixedUpdate(float fixed_elapsed_time)
        {
            throw new NotImplementedException();
        }

        public void Update(float elapsed_time)
        {
            throw new NotImplementedException();
        }
    }
}
