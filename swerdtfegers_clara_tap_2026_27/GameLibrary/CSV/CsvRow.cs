using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.CSV
{
    public class CsvRow
    {
        private int _lineNumber;
        private string[] _cells;

        public CsvRow(int line_number, string[] cells)
        {
            _lineNumber = line_number;
            _cells = cells;
        }
    }
}
