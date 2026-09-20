using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace activity_00_tap_26_27.Core.Events
{
    public class GameActionGameEvent : IGameEvent
    {
        private readonly GameActionType _actionType;

        public GameActionGameEvent(GameActionType action_type)
        {
            _actionType = action_type;
        }

        public GameActionType GetActionType()
        {
            return _actionType;
        }
    }
}
