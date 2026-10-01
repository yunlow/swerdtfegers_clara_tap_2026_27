using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.CSV
{
    public class CsvReader
    {
        public CsvTable Read(string file_name, string content, ContentReport report)
        {
            CsvTable csv_table = new CsvTable(file_name);
            string[] line_table = content.Split('\n');
            for (int line_index = 0; line_index < line_table.Length; line_index++)
            {
                string line = line_table[line_index].Trim();
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
                        report.AddError(file_name, line_number, $"{cells.Length} values found, " +
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
