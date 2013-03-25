using System;
using System.Diagnostics;
using System.IO;

namespace RexSimulatorCLI
{
    internal class Program
    {
        public const string TempFileName = "REXSIMCLI_TEMP";

        public static string TempFileFullPath =
            Path.Combine(Path.GetTempPath(), TempFileName);

        public static void Main(string[] args)
        {
            RexSimulator rexSim;

            // Check if we're the only instance currently running
            Process otherInstance = GetOtherInstance();

            if (otherInstance != null)
                if (args.Length == 0)
                    Console.WriteLine(@"Another instance is already running with PID: " + otherInstance.Id);
                else
                {
                    // Write out args to temp file
                    using (StreamWriter tempFile = new StreamWriter(TempFileFullPath))
                    {
                        tempFile.WriteLine(string.Join(" ", args));
                    }
                }
            else
                rexSim = new RexSimulator();
        }

        public static Process GetOtherInstance()
        {
            Process otherInstance = null;
            var current = Process.GetCurrentProcess();

            foreach (var process in Process.GetProcessesByName(current.ProcessName))
                if (process.Id != current.Id)
                    otherInstance = process;
            return otherInstance;
        }

    }
}

