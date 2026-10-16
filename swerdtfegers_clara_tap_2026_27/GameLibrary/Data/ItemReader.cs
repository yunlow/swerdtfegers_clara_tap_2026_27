using activity_00_tap_26_27.Data;
using GameLibrary.Csv;
using GameLibrary.CSV;
using System.Collections.Generic;
using System.Globalization;

namespace GameLibrary.Data
{
    public static class ItemReader
    {
        public static Dictionary<string, ItemData> Read(CsvTable csv_table, ContentReport error_report)
        {
            Dictionary<string, ItemData> item_data_table = new Dictionary<string, ItemData>();

            for (int csv_row_index = 0; csv_row_index < csv_table.GetRowCount(); csv_row_index++)
            {
                int human_readable_row_number = csv_row_index + 2;
                CsvRow row_content = csv_table.GetRowAtIndex(csv_row_index);
                int initial_error_count = error_report.GetErrorCount();

                string item_id = ReaderUtilities.ReadDataAtColumn("Id", csv_table, row_content, error_report);
                string item_name = ReaderUtilities.ReadDataAtColumn("Name", csv_table, row_content, error_report);
                string price_read_as_text = ReaderUtilities.ReadDataAtColumn("Price", csv_table, row_content, error_report);

                int price = -1;

                if (string.IsNullOrEmpty(item_id))
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"{item_id} is not a valid reference");
                }

                if (item_data_table.ContainsKey(item_id))
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"{item_id} reference already exists.");
                }

                if (string.IsNullOrEmpty(item_name))
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"{item_name} is not a valid name");
                }

                if (!string.IsNullOrEmpty(price_read_as_text))
                {
                    if (int.TryParse(price_read_as_text, NumberStyles.Integer, CultureInfo.InvariantCulture, out price))
                    {
                        if (price < 0)
                        {
                            error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"Item price: {price} - should be >= 0");
                        }
                    }
                    else
                    {
                        error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"{price_read_as_text} is not a valid price, it should be integer.");
                    }
                }
                else
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"Price is empty.");
                }

                if (error_report.GetErrorCount() <= initial_error_count)
                {
                    ItemData item_data = new ItemData(item_id, item_name, price);

                    item_data_table.Add(item_id, item_data);
                }
            }

            return item_data_table;
        }
    }
}