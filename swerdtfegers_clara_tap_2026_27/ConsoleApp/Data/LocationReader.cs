using GameLibrary.CSV;
using System;
using System.Collections.Generic;
using System.Text;

namespace activity_00_tap_26_27.Data
{
    public class LocationReader
    {
        public CsvTable Read(CsvTable csv_table, ContentReport report)
        {
            List<CsvRow> row_list = csv_table.GetRowsList();
            for (int line_index = 0; line_index < row_list.Count; line_index++)
            {
                string line = row_list.Trim();
                int line_number = line_index + 1;
                if (line.Length > 0)
                {
                    string[] cells = line.Split(';');
                    if (csv_table.GetHasHeader() == false)
                    {
                        csv_table.SetHeader(cells);
                    }
                    else if (cells.Length != csv_table.GetColumnCount())
                    {
                        report.AddError(csv_table.GetFileName(), line_number, $"{cells.Length} values found, " +
                       $"{csv_table.GetColumnCount()} expected.");
                    }
                    else
                    {
                        csv_table.AddRow(new CsvRow(line_number, cells));
                    }
                }
            }
            return csv_table;
        }

    }
}
