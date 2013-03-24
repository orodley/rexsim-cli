using System;
using System.IO;

namespace RexSimulatorCLI
{
	class Program
	{
        private static RexSimulator _rexSim;

	    //public static TextWriter stdout;

		public static void Main (string[] args)
		{
            //Set up console
            Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
            /*
            stdout = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true, NewLine = "\n" };
            Console.SetOut(stdout);
            */

			_rexSim = new RexSimulator();
		}
	}
}