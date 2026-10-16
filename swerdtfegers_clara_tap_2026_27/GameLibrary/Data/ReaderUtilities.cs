
using GameLibrary.CSV;

namespace GameLibrary.Data
{
    public static class ReaderUtilities
    {
        public static string ReadDataAtColumn(string column_name, CsvTable csv_table, CsvRow csv_row, ContentReport error_report)
        {
            string read_data = "";
            int column_index = csv_table.GetColumnIndex(column_name);

            if (column_index < 0)
            {
                error_report.AddError(csv_table.GetFileName(), 0, $"missing {column_name} columns");
            }
            else
            {
                read_data = csv_row.GetCellAtIndex(column_index);
            }

            return read_data;
        }
    }
}