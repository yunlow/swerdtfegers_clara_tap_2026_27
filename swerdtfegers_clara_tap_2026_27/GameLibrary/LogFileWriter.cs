using GameLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameLibrary
{
    public class LogFileWriter : ILogWriter
    {
        

        public void WriteLine(string line)
        {
            Console.WriteLine(line);
        }
    }
}
