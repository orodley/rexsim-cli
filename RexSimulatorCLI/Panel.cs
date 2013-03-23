using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RexSimulatorCLI
{
    public abstract class Panel
    {
        private StreamWriter _stream;

        protected Panel(Stream s)
        {
            _stream = new StreamWriter(s);
        }

        public void Write(char c)
        {
            _stream.Write(c);
            _stream.Flush();
        }

        public void Write(string s)
        {
            _stream.Write(s);
            _stream.Flush();
        }
    }
}
