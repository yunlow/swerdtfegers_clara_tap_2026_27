using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.CSV
{
    public class CsvRow
    {
        private readonly int _lineNumber;
        private readonly string[] _cellTable;

        public CsvRow(int line_number, string[] cell_table)
        {
            _lineNumber = line_number;
            _cellTable = cell_table;
        }

        public string GetCellAtIndex(int cell_index)
        {
            return _cellTable[cell_index].Trim();
        }

        public int GetLineNumber()
        {
            return _lineNumber;
        }

        public int GetCellCount()
        {
            return _cellTable.Length;
        }
    }
}
