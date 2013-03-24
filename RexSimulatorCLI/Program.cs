using System;

namespace RexSimulatorCLI
{
	class Program
	{
        private static RexSimulator _rexSim;

		public static void Main (string[] args)
		{
            //Set up console
            Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);

			_rexSim = new RexSimulator();
		}
	}
}