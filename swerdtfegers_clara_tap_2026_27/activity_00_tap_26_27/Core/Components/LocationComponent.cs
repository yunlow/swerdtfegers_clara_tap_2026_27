using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace activity_00_tap_26_27.Core.Components
{
    public class LocationComponent : Component
    {
        private readonly string _locationName;
        private readonly List<LocationLink> _links = new List<LocationLink>();

        public LocationComponent(GameObject owner, string location_name)
        {
            _locationName = location_name;
        }

        public string GetLocationName()
        {
            return _locationName;
        }

        public void AddLink(GameObject destination, float duration)
        {
            _links.Add(new LocationLink(destination, duration));
        }

        public int GetDestinationCount()
        {
            return _links.Count;
        }

        public LocationLink GetDestinationAtIndex(int index)
        {
            if (index >= 0 && index < _links.Count)
            {
                return _links[index];
            }
            return null;
        }
    }
}
