using activity_00_tap_26_27.Data;
using GameLibrary.Csv;
using GameLibrary.CSV;
using System.Collections.Generic;
using System.Globalization;

namespace GameLibrary.Data
{
    public class HeroReader
    {
        public static List<HeroData> Read(CsvTable csv_table, List<LocationData> location_data_table, ContentReport error_report)
        {
            List<HeroData> hero_data_table = new List<HeroData>();
            HashSet<string> reference_table = new HashSet<string>();

            if (csv_table.GetRowCount() < 1)
            {
                error_report.AddError(csv_table.GetFileName(), -1, $"No hero defined.");
            }

            for (int csv_row_index = 0; csv_row_index < csv_table.GetRowCount(); csv_row_index++)
            {
                int human_readable_row_number = csv_row_index + 2;
                CsvRow row_content = csv_table.GetRowAtIndex(csv_row_index);
                int initial_error_count = error_report.GetErrorCount();

                string id = ReaderUtilities.ReadDataAtColumn("Id", csv_table, row_content, error_report);
                string name = ReaderUtilities.ReadDataAtColumn("Name", csv_table, row_content, error_report);
                string health_read_as_text = ReaderUtilities.ReadDataAtColumn("Health", csv_table, row_content, error_report);
                string gold_read_as_text = ReaderUtilities.ReadDataAtColumn("Gold", csv_table, row_content, error_report);

                int health = -1;
                int gold = -1;

                if (string.IsNullOrEmpty(id))
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"{id} is not a valid reference");
                }

                if (!reference_table.Add(id))
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"{id} reference already exists.");
                }

                if (string.IsNullOrEmpty(name))
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"{name} is not a valid name");
                }

                if (!string.IsNullOrEmpty(health_read_as_text))
                {
                    if (int.TryParse(health_read_as_text, NumberStyles.Integer, CultureInfo.InvariantCulture, out health))
                    {
                        if (health <= 0)
                        {
                            error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"Hero’s health: {health} - should be >= 0");
                        }
                    }
                    else
                    {
                        error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"{health_read_as_text} is not a valid health, it should be integer.");
                    }
                }
                else
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"Health is empty.");
                }

                if (!string.IsNullOrEmpty(gold_read_as_text))
                {
                    if (int.TryParse(gold_read_as_text, NumberStyles.Integer, CultureInfo.InvariantCulture, out gold))
                    {
                        if (gold < 0)
                        {
                            error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"Hero’s gold: {gold} - should be > 0");
                        }
                    }
                    else
                    {
                        error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"{gold_read_as_text} is not a valid Gold, it should be integer.");
                    }
                }
                else
                {
                    error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"Gold is empty.");
                }

                if (error_report.GetErrorCount() <= initial_error_count)
                {
                    HeroData hero_data = new HeroData(id, name, health, gold);

                    hero_data_table.Add(hero_data);

                    if (hero_data_table.Count > 1)
                    {
                        error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"Too many heroes {hero_data_table.Count}, only one is accepted for now.");
                    }
                }

                for (int location_index = 0; location_index < location_data_table.Count; location_index++)
                {
                    if (location_data_table[location_index]._id == id)
                    {
                        error_report.AddError(csv_table.GetFileName(), human_readable_row_number, $"Hero is using the id of an already existing location: {id}.");
                    }
                }
            }


            return hero_data_table;
        }
    }
}