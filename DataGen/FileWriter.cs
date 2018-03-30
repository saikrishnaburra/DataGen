using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataGen
{
    public class FileWriter
    {
        public long _maxRecordsPerFile;
        private StreamWriter _sw;
        private readonly string _fileNameFormat;
        private readonly string _dir;
        private int _currChunk;
        private long _count;
        private string _header;
        
        public FileWriter(string dir, string fileNameForamt, string header, long maxRecordsPerFile)
        {
            _currChunk = 0;
            _fileNameFormat = fileNameForamt;
            _dir = dir;
            _header = header;
            _sw = new StreamWriter(Path.Combine(dir, string.Format(_fileNameFormat, _currChunk++)));
            _sw.WriteLine(header);
            _count = 0;
            _maxRecordsPerFile = maxRecordsPerFile;
        }

        public void Write(string line)
        {
            _sw.WriteLine(line);
            _count++;
            if (_count % _maxRecordsPerFile == 0)
            {
                _sw.Close();
                _sw = new StreamWriter(Path.Combine(_dir, string.Format(_fileNameFormat, _currChunk++)));
                _sw.WriteLine(_header);
            }
        }

        public void Close()
        {
            _sw.Close();
            Console.WriteLine("Wrote {0} records in {1}", _count, _fileNameFormat);
        }
    }
}
