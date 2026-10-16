using activity_00_tap_26_27.Core;
using System.Collections.Generic;

namespace GameLibrary.Components
{
    public class LocationComponent : Component
    {
        private string _locationName;
        private LocationComponent _parentLocation;
        private readonly List<Connection> _connectionTable = new List<Connection>();

        public LocationComponent(string location_name, GameObject game_object) : base(game_object)
        {
            _locationName = location_name;
        }

        public string GetName()
        {
            return _locationName;
        }

        public int GetDestinationCount()
        {
            return _connectionTable.Count;
        }

        public Connection GetDestinationAtIndex(int location_index)
        {
            return _connectionTable[location_index];
        }

        public void AddLocation(LocationComponent location_component, float travel_duration)
        {
            _connectionTable.Add(new Connection(location_component, travel_duration));
            location_component.SetParentLocation(this);
        }

        private void SetParentLocation(LocationComponent parent_location)
        {
            _parentLocation = parent_location;
        }

        public LocationComponent GetParentLocation()
        {
            return _parentLocation;
        }
    }
}