using System.Globalization;
using GameLibrary.Csv;

namespace GameLibrary.Data
{
    public class MineReader
    {
        public static List<MineData> Read(CsvTable csv_table, List<LocationData> location_data_table, List<ShopData> shop_data_table, ContentReport error_report)
        {
            List<MineData> mine_data_table = new List<MineData>();
            HashSet<string> mine_reference_table = new HashSet<string>();

            for (int csv_row_index = 0; csv_row_index < csv_table.GetRowCount(); csv_row_index++)
            {
                int human_readable_row_number = csv_row_index + 2;
                CsvRow row_content = csv_table.GetRowAtIndex(csv_row_index);
                int initial_error_count = error_report.GetErrorCount();

                string location_id = ReaderUtilities.ReadDataAtColumn("LocationId", csv_table, row_content, error_report);
                string minutes_per_strike_read_as_test = ReaderUtilities.ReadDataAtColumn("MinutesPerStrike", csv_table, row_content, error_report);
                string stock_read_as_text = ReaderUtilities.ReadDataAtColumn("Stock", csv_table, row_content, error_report);

                if (string.IsNullOrEmpty(location_id))
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"{location_id} is not a valid reference");
                }

                if (!mine_reference_table.Add(location_id))
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"{location_id} reference already exists.");
                }

                int minutes_per_strike;

                if (int.TryParse(minutes_per_strike_read_as_test, NumberStyles.Integer, CultureInfo.InvariantCulture, out minutes_per_strike))
                {
                    if (minutes_per_strike <= 0)
                    {
                        error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"MinutesPerStrike: {minutes_per_strike} - should be > 0.");
                    }
                }
                else
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"{minutes_per_strike_read_as_test} is not a valid minutes_per_strike, it should be integer.");
                }

                int stock;

                if (int.TryParse(stock_read_as_text, NumberStyles.Integer, CultureInfo.InvariantCulture, out stock))
                {
                    if (stock < 0)
                    {
                        error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"stock: {stock} - should be >= 0.");
                    }
                }
                else
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"{stock_read_as_text} is not a valid stock, it should be integer.");
                }

                if (error_report.GetErrorCount() <= initial_error_count)
                {
                    MineData mine_data = new MineData(location_id, minutes_per_strike, stock);

                    mine_data_table.Add(mine_data);
                }

                bool is_location_found = false;

                for (int location_index = 0; location_index < location_data_table.Count; location_index++)
                {
                    if (location_data_table[location_index]._id == location_id)
                    {
                        is_location_found = true;
                    }
                }

                if (!is_location_found)
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"Mine reference {location_id} not found.");
                }
                else
                {
                    for (int shop_index = 0; shop_index < shop_data_table.Count; shop_index++)
                    {
                        ShopData current_shop_data = shop_data_table[shop_index];

                        if (current_shop_data._locationId == location_id)
                        {
                            error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"{location_id} is a shop, it cannot also be a mine.");
                        }
                    }
                }
            }

            return mine_data_table;
        }
    }
}