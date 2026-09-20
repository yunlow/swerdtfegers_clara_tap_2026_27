using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace activity_00_tap_26_27.Core
{
    public class LocationLink
    {
        private readonly GameObject _destination;
        private readonly float _duration;

        public LocationLink(GameObject destination, float duration)
        {
            _destination = destination;
            _duration = duration;
        }

        public GameObject GetDestination()
        {
            return _destination;
        }

        public float GetDuration()
        {
            return _duration;
        }
    }
}
