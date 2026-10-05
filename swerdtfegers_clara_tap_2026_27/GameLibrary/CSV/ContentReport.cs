using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary.CSV
{
    public class ContentReport
    {
        private List<string> _errors;

        public ContentReport()
        {
            _errors = new List<string>();
        }

        public void AddError(string file_name, int line_number, string v)
        {
            _errors.Add($"Error in file '{file_name}' at line {line_number}: {v}");
        }
        public int GetErrorCount()
        {
            return _errors.Count;
        }
        public string GetErrorTextAtIndex(int error_index)
        {
            return _errors[error_index];
        }

    }
}
