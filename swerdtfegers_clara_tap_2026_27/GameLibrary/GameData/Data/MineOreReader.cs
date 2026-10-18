using GameLibrary.CSV;
using System.Collections.Generic;
using System.Globalization;


namespace GameLibrary.Data
{
    public class MineOreReader
    {
        public static List<MineOreData> Read(CsvTable csv_table, List<MineData> mine_data_table, Dictionary<string, ItemData> item_data_table, ContentReport error_report)
        {
            List<MineOreData> mine_ore_data_table = new List<MineOreData>();

            for (int csv_row_index = 0; csv_row_index < csv_table.GetRowCount(); csv_row_index++)
            {
                int human_readable_row_number = csv_row_index + 2;
                CsvRow row_content = csv_table.GetRowAtIndex(csv_row_index);
                int initial_error_count = error_report.GetErrorCount();

                string location_id = ReaderUtilities.ReadDataAtColumn("LocationId", csv_table, row_content, error_report);
                string item_id = ReaderUtilities.ReadDataAtColumn("ItemId", csv_table, row_content, error_report);
                string weight_read_as_text = ReaderUtilities.ReadDataAtColumn("Weight", csv_table, row_content, error_report);

                if (string.IsNullOrEmpty(location_id))
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"{location_id} is not a valid reference");
                }

                if (string.IsNullOrEmpty(item_id))
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"{item_id} is not a valid reference");
                }

                int weight;

                if (int.TryParse(weight_read_as_text, NumberStyles.Integer, CultureInfo.InvariantCulture, out weight))
                {
                    if (weight < 0)
                    {
                        error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"weight: {weight} - should be >= 0.");
                    }
                }
                else
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"{weight_read_as_text} is not a valid weight, it should be integer.");
                }

                if (error_report.GetErrorCount() <= initial_error_count)
                {
                    MineOreData mine_ore_data = new MineOreData(location_id, item_id, weight);

                    mine_ore_data_table.Add(mine_ore_data);
                }

                if (!item_data_table.ContainsKey(item_id))
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"{item_id} is unknown item.");
                }

                bool is_location_found = false;

                for (int location_index = 0; location_index < mine_data_table.Count; location_index++)
                {
                    if (mine_data_table[location_index]._locationId == location_id)
                    {
                        is_location_found = true;
                    }
                }

                if (!is_location_found)
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"Mine reference {location_id} not found.");
                }
            }

            return mine_ore_data_table;
        }
        
    }
}