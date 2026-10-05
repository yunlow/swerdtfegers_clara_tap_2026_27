using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.CSV
{
    public class CsvTable
    {
        private string _fileName;
        private string[] _header;
        private List<CsvRow> _rows;

        public CsvTable(string file_name)
        {
            _fileName = file_name;
            _rows = new List<CsvRow>();
        }

        public List<CsvRow> GetRowsList()
        {
            return _rows;
        }

        public void AddRow(CsvRow csv_row)
        {
            _rows.Add(csv_row);
        }
        
        public string GetFileName()
        {
            return _fileName;
        }

        public int GetColumnCount()
        {
            if (_header == null)
            {
                return 0;
            }
            return _header.Length;
        }

        public int GetColumnIndex(string column_name)
        {
            if (_header == null)
            {
                return -1;
            }

            for (int i = 0; i < _header.Length; i++)
            {
                if (_header[i] == column_name)
                {
                    return i;
                }
            }
            return -1;
        }

        public int GetRowCount()
        {
            return _rows.Count;
        }

        public void GetRowAtIndex(int row_index, out CsvRow csvRow)
        {
            csvRow = _rows[row_index];
        }


        public bool GetHasHeader()
        {
            if(_header == null)
            {
                return false;
            }
            return true;
        }

        public void SetHeader(string[] cells)
        {
            for(int i = 0; i < cells.Length; i++)
            {
                cells[i] = cells[i].Trim();
            }
            
        }
    }
}
