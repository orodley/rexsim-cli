using System;

using RexSimulator.Hardware;

namespace RexSimulatorCLI
{
	class Program
	{
		private static RexSimulator mRexSim;

		public static void Main (string[] args)
		{
			mRexSim = new RexSimulator();
		}
	}
}