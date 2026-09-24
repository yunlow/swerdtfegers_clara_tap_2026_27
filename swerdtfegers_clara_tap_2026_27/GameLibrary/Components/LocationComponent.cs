using activity_00_tap_26_27.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLibrary.Components
{
    public class LocationComponent : Component
    {
        private string _locationName;
        private readonly List<Connection> _connectionTable = new List<Connection>();



        public LocationComponent(string location_name)
        {
            _locationName = location_name;
        }

        public void AddConnection(LocationComponent location_component, float travel_duration)
        {
            _connectionTable.Add(new Connection(location_component, travel_duration));
        }

        public void CreateLocation(string location_name)
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

      
    }
}
