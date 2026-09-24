using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace activity_00_tap_26_27.Core.Events
{
    public class LogMessageGameEvent : IGameEvent
    {
        private string _message;

        public LogMessageGameEvent(string message)
        {
            _message = message;
        }

        public string GetMessage()
        {
            return _message;
        }
    }
}
