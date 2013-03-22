using System;
using System.IO;
using System.Text;

using RexSimulator.Hardware.Rex;

namespace RexSimulatorCLI
{
	public class BasicSerialPort
	{
		private SerialIO mSerialPort;

		public BasicSerialPort (SerialIO port)
		{
			this.mSerialPort = port;

			mSerialPort.SerialDataTransmitted +=
                new EventHandler<SerialIO.SerialEventArgs>(mSerialPort_SerialDataTransmitted);
		}

        /// <summary>
        /// Write data received from the serial port to stdout
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		void mSerialPort_SerialDataTransmitted(object sender, SerialIO.SerialEventArgs e)
		{
            Console.Write((char)e.Data);
		}
	}
}