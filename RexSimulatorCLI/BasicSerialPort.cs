using System;
using RexSimulator.Hardware.Rex;

namespace RexSimulatorCLI
{
    public class BasicSerialPort
    {
        private readonly SerialIO _serialPort;

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