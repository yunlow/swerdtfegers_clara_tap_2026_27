using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using activity_00_tap_26_27.Core;

namespace activity_00_tap_26_27.Data
{
    public class LocationData
    {
        
            public readonly string _id;
            public readonly string _name;
            public readonly string _parentId;
            public readonly int _travelMinutes;

            public LocationData(string id, string name, string parent_id, int travel_minutes)
            {
                _id = id;
                _name = name;
                _parentId = parent_id;
                _travelMinutes = travel_minutes;
            }
        }
    
}
