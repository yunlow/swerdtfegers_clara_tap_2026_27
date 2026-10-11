using GameLibrary.CSV;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace activity_00_tap_26_27.Data
{
    public class ItemReader
    {
        public static List<ItemData> Read(CsvTable csv_table, List<ShopData> shop_data_table, ContentReport error_report)
        {
            List<ItemData> item_data_table = new List<ItemData>();
            HashSet<string> item_reference_table = new HashSet<string>();

            for (int csv_row_index = 0; csv_row_index < csv_table.GetRowCount(); csv_row_index++)
            {
                int human_readable_row_number = csv_row_index + 2;
                CsvRow row_content = csv_table.GetRowAtIndex(csv_row_index);
                bool has_error = false;

                string item_id = row_content.GetCellAtIndex(0);
                string item_name = row_content.GetCellAtIndex(1);
                string shop_id = row_content.GetCellAtIndex(2);
                int price = -1;
                int quantity = -1;

                if (string.IsNullOrEmpty(item_id))
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"{item_id} is not a valid reference");
                    has_error = true;
                }

                if (!item_reference_table.Add(item_id))
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"{item_id} reference already exists.");
                    has_error = true;
                }

                if (string.IsNullOrEmpty(item_name))
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"{item_name} is not a valid name");
                    has_error = true;
                }

                if (int.TryParse(row_content.GetCellAtIndex(3), NumberStyles.Integer, CultureInfo.InvariantCulture, out price))
                {
                    if (price < 0)
                    {
                        error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"Item price: {price} - should be >= 0");
                        has_error = true;
                    }
                }
                else
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"{row_content.GetCellAtIndex(3)} is not a valid price, it should be integer.");
                }

                if (int.TryParse(row_content.GetCellAtIndex(4), NumberStyles.Integer, CultureInfo.InvariantCulture, out quantity))
                {
                    if (quantity < 0)
                    {
                        error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"quantity >= 0");
                        has_error = true;
                    }
                }
                else
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number,
                                    $"{row_content.GetCellAtIndex(4)} is not a valid quantity, it should be integer.");
                }

                if (!has_error)
                {
                    ItemData item_data = new ItemData(item_id, item_name, shop_id, price, quantity);

                    item_data_table.Add(item_data);
                }

                bool is_shop_found = false;

                for (int shop_index = 0; shop_index < shop_data_table.Count; shop_index++)
                {
                    if (shop_data_table[shop_index]._locationId == shop_id)
                    {
                        is_shop_found = true;
                    }
                }

                if (!is_shop_found)
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"Shop reference {shop_id} not found.");
                }
            }

            return item_data_table;
        }
    }

}
