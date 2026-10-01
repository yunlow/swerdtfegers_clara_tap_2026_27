using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.CSV
{
    public class CsvTable
    {
        private string file_name;
        private string _header;

        public CsvTable(string file_name)
        {
            this.file_name = file_name;
        }

        public void AddRow(CsvRow csv_row)
        {
            throw new NotImplementedException();
        }

        public int GetColumnCount()
        {
            throw new NotImplementedException();
        }

        public int GetColumnIndex(string column_name)
        {
            throw new NotImplementedException();
        }

        public int GetRowCount()
        {
            throw new NotImplementedException();
        }

        public void GetRowAtIndex(int row_index, out CsvRow csvRow)
        {
            throw new NotImplementedException();
        }


        public bool GetHasHeader()
        {
            throw new NotImplementedException();
        }

        public void SetHeader(string[] cells)
        {
            throw new NotImplementedException();
        }
    }
}
