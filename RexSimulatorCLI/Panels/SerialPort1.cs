using System;
using System.IO;
using System.Threading;
using RexSimulator.Hardware;
using RexSimulator.Hardware.Rex;

namespace RexSimulatorCLI.Panels
{
    public class SerialPort1 : Panel
    {
        private readonly SerialIO _serialPort;
        private readonly RexBoard _rexBoard;

        public SerialPort1 (SerialIO port, RexBoard board)
        {
            _serialPort = port;
            _rexBoard = board;

            _serialPort.SerialDataTransmitted += serialPort_SerialDataTransmitted;
            base.InputRecieved += BasicSerialPort_InputRecieved;
        }

        void BasicSerialPort_InputRecieved(ConsoleKeyInfo info)
        {
            if (info.Modifiers.HasFlag(ConsoleModifiers.Control) && info.Key == ConsoleKey.A)
            {
                switch (Console.ReadKey(true).KeyChar)
                {
                    case 's': // Sending an S-Record
                        Write("Enter .srec to send: ");
                        var filename = Console.ReadLine();
                        var uploadFileWorker = new Thread(UploadFileWorker);
                        uploadFileWorker.Start(filename);
                        break;
                }
            }
            else
                _rexBoard.Serial1.SendAsync(info.KeyChar);
        }

        /// <summary>
        /// Sends a file through the serial port.
        /// </summary>
        private void UploadFileWorker(object filename)
        {
            if (!File.Exists((string) filename))
            {
                return;
            }

            var reader = new StreamReader((string)filename);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                foreach (char c in line)
                {
                    _rexBoard.Serial1.Send(c);
                }
                _rexBoard.Serial1.Send('\n');
            }
            reader.Close();
        }

        /// <summary>
        /// Write data received from the serial port to stdout
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void serialPort_SerialDataTransmitted(object sender, SerialIO.SerialEventArgs e)
        {
            Write((char)e.Data);
        }
    }
}