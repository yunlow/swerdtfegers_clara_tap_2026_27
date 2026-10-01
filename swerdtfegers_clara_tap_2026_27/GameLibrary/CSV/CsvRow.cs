using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.CSV
{
    public class CsvRow
    {
        private int line_number;
        private string[] cells;

        public CsvRow(int line_number, string[] cells)
        {
            this.line_number = line_number;
            this.cells = cells;
        }
    }
}
