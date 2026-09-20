using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace activity_00_tap_26_27.Core.Events
{
    public class RegisterGameObjectGameEvent : IGameEvent
    {
        private readonly GameObject _gameObject;

        public RegisterGameObjectGameEvent(GameObject game_object)
        {
            _gameObject = game_object;
        }

        public GameObject GetGameObject()
        {
            return _gameObject;
        }
    }
}
