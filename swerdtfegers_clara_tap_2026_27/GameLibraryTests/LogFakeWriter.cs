using GameLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibraryTests
{
    public class LogFakeWriter : ILogWriter
    {
        private string _line;
        public void WriteLine(string line)
        {
            _line = line;
        }
    }
}
