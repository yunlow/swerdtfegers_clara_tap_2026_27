using activity_00_tap_26_27.Core.Events;

namespace GameLibrary.Events
{
    public class SaveRequestedGameEvent : IGameEvent
    {
        public readonly string _saveText;

        public SaveRequestedGameEvent(string save_text)
        {
            _saveText = save_text;
        }
    }
}