using GameLibrary.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace activity_00_tap_26_27.Core
{
    public class Connection
    {
        private LocationComponent _destinationLocation;
        private float _timeToReachDestination;

        public Connection(LocationComponent destination_location, float time_to_reach_destination)
        {
            _destinationLocation = destination_location;
            _timeToReachDestination = time_to_reach_destination;
        }

        public LocationComponent GetDestinationLocation()
        {
            return _destinationLocation;
        }

        public float GetTimeToReachDestination()
        {
            return _timeToReachDestination;
        }
    }
}
