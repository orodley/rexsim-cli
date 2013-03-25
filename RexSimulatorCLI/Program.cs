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
            Process otherInstance = getOtherInstance();

            if (otherInstance != null && args.Length == 0)
                Console.WriteLine(@"Another instance is already running with PID: " + otherInstance.Id);
            else //New process; start the simulator
                rexSim = new RexSimulator();
	}

        public static Process getOtherInstance()
        {
            Process otherInstance = null;
            var current = Process.GetCurrentProcess();

            foreach (var process in Process.GetProcessesByName(curent.ProcessName))
                if (process.Id != current.Id)
                    otherInstance = process;

            return otherInstance;
        }
    }
}
