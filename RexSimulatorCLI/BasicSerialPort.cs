using System;
using System.IO;
using System.Threading;
using RexSimulator.Hardware;
using RexSimulator.Hardware.Rex;

namespace RexSimulatorCLI.Panels
{
    public class BasicSerialPort
    {
        private readonly SerialIO _serialPort;
        private readonly RexBoard _rexBoard;

        public BasicSerialPort (SerialIO port)
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
            Console.Write((char)e.Data);
        }
    }
}