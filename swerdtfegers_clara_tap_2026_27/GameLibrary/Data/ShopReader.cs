using GameLibrary.CSV;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace activity_00_tap_26_27.Data
{
    public class ShopReader
    {
        public static List<ShopData> Read(CsvTable csv_table, List<LocationData> location_data_table, ContentReport error_report)
        {
            List<ShopData> shop_data_table = new List<ShopData>();
            HashSet<string> shop_reference_table = new HashSet<string>();

            for (int csv_row_index = 0; csv_row_index < csv_table.GetRowCount(); csv_row_index++)
            {
                int human_readable_row_number = csv_row_index + 2;
                CsvRow row_content = csv_table.GetRowAtIndex(csv_row_index);

                bool has_error = false;

                string shop_id = row_content.GetCellAtIndex(0);
                int shop_opening_hour = -1;
                int shop_closing_hour = -1;

                if (string.IsNullOrEmpty(shop_id))
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"{shop_id} is not a valid reference");
                    has_error = true;
                }

                if (!shop_reference_table.Add(shop_id))
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"{shop_id} reference already exists.");
                    has_error = true;
                }

                if (int.TryParse(row_content.GetCellAtIndex(1), NumberStyles.Integer, CultureInfo.InvariantCulture, out shop_opening_hour))
                {
                    if ((shop_opening_hour < 0) || (shop_opening_hour > 23))
                    {
                        error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"Shop opening hour: {shop_opening_hour} - should be >= 0 && <= 24.");
                        has_error = true;
                    }
                }
                else
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"{row_content.GetCellAtIndex(1)} is not a valid opening hour, it should be integer.");
                }

                if (int.TryParse(row_content.GetCellAtIndex(2), NumberStyles.Integer, CultureInfo.InvariantCulture, out shop_closing_hour))
                {
                    if ((shop_closing_hour < 0) || (shop_closing_hour > 23))
                    {
                        error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"Shop closing hour: {shop_closing_hour} - should be >= 0 && <= 24.");
                        has_error = true;
                    }
                }
                else
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"{row_content.GetCellAtIndex(2)} is not a valid closing hour, it should be integer.");
                }

                if (shop_opening_hour >= shop_closing_hour)
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"shop opening hours {shop_opening_hour} should be lower than shop closing hour{shop_closing_hour}.");
                }

                if (!has_error)
                {
                    ShopData shop_data = new ShopData(shop_id, shop_opening_hour, shop_closing_hour);

                    shop_data_table.Add(shop_data);
                }

                bool is_location_found = false;

                for (int location_index = 0; location_index < location_data_table.Count; location_index++)
                {
                    if (location_data_table[location_index]._id == shop_id)
                    {
                        is_location_found = true;
                    }
                }

                if (!is_location_found)
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"Shop reference {shop_id} not found.");
                }
            }

            return shop_data_table;
        }
    }

}
