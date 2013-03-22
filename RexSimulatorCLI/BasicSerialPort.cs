using System;
using System.IO;
using System.Text;

using RexSimulator.Hardware.Rex;

namespace RexSimulatorCLI
{
	public class BasicSerialPort
	{
		private SerialIO mSerialPort;
		private StringBuilder mRecvBuffer;

		public BasicSerialPort (SerialIO port)
		{
			this.mSerialPort = port;
			this.mRecvBuffer = new StringBuilder();

			mSerialPort.SerialDataTransmitted += new EventHandler<SerialIO.SerialEventArgs>(mSerialPort_SerialDataTransmitted);

		}

		void mSerialPort_SerialDataTransmitted(object sender, SerialIO.SerialEventArgs e)
		{
			Console.Write((char)e.Data);
			/*lock (mRecvBuffer)
			{
				//mSerialPort.AckRecv();
				mRecvBuffer.Append((char)e.Data);
			}*/
		}
	}
}
