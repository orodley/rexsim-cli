using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RexSimulatorCLI
{
    public delegate void InputRecievedHandler(ConsoleKeyInfo info);

    public abstract class Panel
    {
        private StreamWriter _streamWriter;
        public readonly Stream BaseStream;

        public event InputRecievedHandler InputRecieved;

        protected Panel()
        {
            BaseStream = new MemoryStream();
            _streamWriter = new StreamWriter(BaseStream);
        }

        public void Write(char c)
        {
            _streamWriter.Write(c);
            _streamWriter.Flush();
        }

        public void SendInput(ConsoleKeyInfo info)
        {
            if (InputRecieved != null)
            {
                InputRecieved(info);
            }
        }
    }
}
