using System;
using System.IO;

using RexSimulator.Hardware.Rex;

namespace RexSimulatorCLI
{
    public class BasicSerialPort : Panel
    {
        private readonly SerialIO _serialPort;

        public BasicSerialPort (SerialIO port, Stream s) : base(s)
        {
            _serialPort = port;
            _serialPort.SerialDataTransmitted += serialPort_SerialDataTransmitted;
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