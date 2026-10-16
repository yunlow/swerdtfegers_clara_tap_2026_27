using GameLibrary.CSV;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace activity_00_tap_26_27.Data
{
    public class LocationReader
    {
        public static List<LocationData> Read(CsvTable csv_table, ContentReport error_report)
        {
            Dictionary<string, int> location_id_table = new Dictionary<string, int>();
            List<LocationData> location_data_table = new List<LocationData>();

            for (int csv_row_index = 0; csv_row_index < csv_table.GetRowCount(); csv_row_index++)
            {
                int human_readable_row_number = csv_row_index + 2;
                CsvRow row_content = csv_table.GetRowAtIndex(csv_row_index);

                bool has_error = false;

                string location_id = row_content.GetCellAtIndex(0);
                string location_name = row_content.GetCellAtIndex(1);
                string location_parent_id = row_content.GetCellAtIndex(2);
                int location_travel_minutes = -1;

                if (string.IsNullOrEmpty(location_id))
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"{location_id} is not a valid reference");
                    has_error = true;
                }

                if (!location_id_table.TryAdd(location_id, human_readable_row_number))
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"{location_id} ID is already present.");
                    has_error = true;
                }

                if (string.IsNullOrEmpty(location_name))
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"{location_name} is not a valid name");
                    has_error = true;
                }

                if (!string.IsNullOrEmpty(location_parent_id))
                {
                    if (int.TryParse(row_content.GetCellAtIndex(3), NumberStyles.Integer, CultureInfo.InvariantCulture, out location_travel_minutes))
                    {
                        if (location_travel_minutes < 0)
                        {
                            error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"Travel time has to be >= 0.");
                            has_error = true;
                        }
                    }
                    else
                    {
                        error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"{row_content.GetCellAtIndex(3)} is not a valid location travel minute, it should be integer.");
                    }
                }

                if (!has_error)
                {
                    LocationData location_data = new LocationData(location_id, location_name, location_parent_id, location_travel_minutes);

                    location_data_table.Add(location_data);
                }
            }

            int root_count = 0;

            for (int location_data_index = 0; location_data_index < location_data_table.Count; location_data_index++)
            {
                string parent_id = location_data_table[location_data_index]._parentId;

                if (!string.IsNullOrEmpty(parent_id))
                {
                    if (!location_id_table.ContainsKey(parent_id))
                    {
                        error_report.AddError(csv_table.GetFileName(), location_id_table[location_data_table[location_data_index]._id], $"{parent_id} not found.");
                    }
                }
                else
                {
                    if (root_count <= 0)
                    {
                        root_count++;
                    }
                    else
                    {
                        error_report.AddError(csv_table.GetFileName(), location_id_table[location_data_table[location_data_index]._id], $"there is already a root node.");
                    }
                }
            }

            return location_data_table;
        }
    }

}
