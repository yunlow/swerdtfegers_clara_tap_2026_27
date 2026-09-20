using activity_00_tap_26_27.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace activity_00_tap_26_27.Core
{
    public class World
    {
        private readonly EventManager _eventManager;

        public World(EventManager event_manager)
        {
            _eventManager = event_manager;
        }

    }
}
