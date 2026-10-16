using activity_00_tap_26_27.Data;

using GameLibrary.CSV;
using System.Collections.Generic;
using System.Globalization;

namespace GameLibrary.Data
{
    public class InventoryReader
    {
        public static List<InventoryEntryData> Read(CsvTable csv_table, List<ShopData> shop_data_table, List<HeroData> hero_data_table, Dictionary<string, ItemData> item_data_table, ContentReport error_report)
        {
            List<InventoryEntryData> inventory_entry_data_table = new List<InventoryEntryData>();

            for (int csv_row_index = 0; csv_row_index < csv_table.GetRowCount(); csv_row_index++)
            {
                int human_readable_row_number = csv_row_index + 2;
                CsvRow row_content = csv_table.GetRowAtIndex(csv_row_index);
                int initial_error_count = error_report.GetErrorCount();

                string owner_id = ReaderUtilities.ReadDataAtColumn("OwnerId", csv_table, row_content, error_report);
                string item_id = ReaderUtilities.ReadDataAtColumn("ItemId", csv_table, row_content, error_report);
                string quantity_read_as_text = ReaderUtilities.ReadDataAtColumn("Quantity", csv_table, row_content, error_report);

                int quantity = -1;

                if (string.IsNullOrEmpty(owner_id))
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"{owner_id} is not a valid owner reference");
                }

                if (string.IsNullOrEmpty(item_id))
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"{item_id} is not a valid item reference");
                }

                if (!string.IsNullOrEmpty(quantity_read_as_text))
                {

                    if (int.TryParse(quantity_read_as_text, NumberStyles.Integer, CultureInfo.InvariantCulture, out quantity))
                    {
                        if (quantity < 0)
                        {
                            error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"Item quantity: {quantity} - should be >= 0");
                        }
                    }
                    else
                    {
                        error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"{quantity_read_as_text} is not a valid quantity, it should be integer.");
                    }
                }
                else
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"Quantity is empty.");
                }

                if (error_report.GetErrorCount() <= initial_error_count)
                {
                    InventoryEntryData inventory_entry_data = new InventoryEntryData(owner_id, item_id, quantity);

                    inventory_entry_data_table.Add(inventory_entry_data);
                }

                bool is_item_found = item_data_table.ContainsKey(item_id);

                if (!is_item_found)
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"Item reference {item_id} not found.");
                }

                bool is_owner_found = false;

                for (int shop_index = 0; shop_index < shop_data_table.Count; shop_index++)
                {
                    if (shop_data_table[shop_index]._locationId == owner_id)
                    {
                        is_owner_found = true;
                    }
                }

                for (int hero_index = 0; hero_index < hero_data_table.Count; hero_index++)
                {
                    if (hero_data_table[hero_index]._id == owner_id)
                    {
                        is_owner_found = true;
                    }
                }

                if (!is_owner_found)
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"Item owner reference {owner_id} not found.");
                }
            }

            return inventory_entry_data_table;
        }
    }
}