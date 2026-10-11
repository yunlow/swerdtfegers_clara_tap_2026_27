using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.CSV
{
    public class ContentReport
    {
       

        private class ErrorData
        {
            public string _fileName;
            public int _fileLine;
            public string _message;
        }

        private readonly List<ErrorData> _errorTable = new List<ErrorData>();

        public void AddError(string file_name, int line_number, string v)
        {
            _errorTable.Add(new ErrorData { _fileName = file_name, _fileLine = line_number, _message = v });
        }
        public int GetErrorCount()
        {
            return _errorTable.Count;
        }
        public string GetErrorTextAtIndex(int error_index)
        {
            ErrorData error_data = _errorTable[error_index];

            return $"{error_data._fileName}, line {error_data._fileLine}: {error_data._message}";
        }

    }
}
