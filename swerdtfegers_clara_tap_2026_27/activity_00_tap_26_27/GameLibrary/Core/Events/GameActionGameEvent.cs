using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace activity_00_tap_26_27.Core.Events
{
    public class GameActionGameEvent : IGameEvent
    {
        public readonly GameActionType _gameActionType;

        public GameActionGameEvent(GameActionType game_action_type)
        {
            _gameActionType = game_action_type;
        }
    }
}
