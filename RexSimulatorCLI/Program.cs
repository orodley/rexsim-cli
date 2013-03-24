using System;
using System.Diagnostics;

namespace RexSimulatorCLI
{
	class Program
	{
		public static void Main (string[] args)
		{
            RexSimulator rexSim;
            
            // Check if we're the only instance currently running
		    Process otherInstance = null;
            var current = Process.GetCurrentProcess();

            foreach (var process in Process.GetProcessesByName(current.ProcessName))
                if (process.Id != current.Id)
                    otherInstance = process;

		    if (otherInstance != null && args.Length == 0)
		        Console.WriteLine(@"Another instance is already running with PID: " + otherInstance.Id);
            else
                rexSim = new RexSimulator();
		}
	}
}