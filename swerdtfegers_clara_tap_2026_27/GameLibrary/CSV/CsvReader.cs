using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.CSV
{
    public class CsvReader
    {
        
            public static CsvTable Read(string file_name, string content, ContentReport error_report)
            {
                CsvTable result_table = new CsvTable(file_name);
                string[] file_line_table = content.Split('\n');

                for (int line_index = 0; line_index < file_line_table.Length; line_index++)
                {
                    string file_line = file_line_table[line_index];

                    if (!string.IsNullOrEmpty(file_line))
                    {
                        string[] cell_table = file_line.Split(';');

                        if (result_table.GetRowHeaderTable() == null)
                        {
                            result_table.SetHeader(cell_table);
                        }
                        else if (result_table.GetRowHeaderTable().Count < cell_table.Length)
                        {
                            error_report.AddError(file_name, line_index, "Too many column");
                        }
                        else if (result_table.GetRowHeaderTable().Count > cell_table.Length)
                        {
                            error_report.AddError(file_name, line_index, "Missing column");
                        }
                        else
                        {
                            result_table.AddRow(new CsvRow(line_index + 1, cell_table));
                        }
                    }
                }

                return result_table;
            }
        }
    }

