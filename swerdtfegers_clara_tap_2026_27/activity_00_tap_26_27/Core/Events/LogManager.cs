using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace activity_00_tap_26_27.Core.Events
{
    public class LogManager
    {
        private EventManager _eventManager;
       
        public LogManager(EventManager event_manager)
        {
            _eventManager = event_manager;

            _eventManager.RegisterToEvent<LogMessageGameEvent>(OnLogMessage);
        }

        private void OnLogMessage(IGameEvent game_event)
        {
            string log_entry = $"[{DateTime.Now}] {((LogMessageGameEvent)game_event).GetMessage()}";
            File.AppendAllText("my_file.txt", log_entry);


        }
        public void Log(string message)
        {
            _eventManager.TriggerEvent(new LogMessageGameEvent(message));
        }
    }
}
