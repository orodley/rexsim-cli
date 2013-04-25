using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Linq;

namespace RexSimulatorCLI
{
    internal class Program
    {
        public  const string TempFileName = "REXSIMCLI_TEMP";
        private const string PidFileName  = "REXSIMCLI_PID";

        public static string TempFileFullPath =
            Path.Combine(Path.GetTempPath(), TempFileName);
        private static string PidFilePathMinusPid = // Needs a better name
            Path.Combine(Path.GetTempPath(), PidFileName);
        private static string PidFileFullPath =     // This too
            PidFilePathMinusPid + Process.GetCurrentProcess().Id;

        public static void Main(string[] args)
        {
            // Check if we're the only instance currently running
            int otherPid = GetOtherInstancePid();

            if (otherPid != -1)
                if (args.Length == 0)
                    Console.WriteLine(@"Another instance is already running with PID: " + otherPid);
                else
                {
                    // Write out args to temp file
                    using (var tempOutFile = new StreamWriter(TempFileFullPath))
                    {
                        tempOutFile.Write(string.Join(" ", args));

                    }

                    /* This is kinda hackish...
                     * The other instance is repeatedly trying to open the file as we're writing to it, so
                     * as soon as we close it, it opens it up again. It doesn't close it until it's
                     * finished writing the returned data out to it, so we'll poll it until we can open it,
                     * then read all the data, print it, and delete the file
                     */

                    Thread.Sleep(50);
                    StreamReader tempInFile = null;

                    var readOutput = false;

                    while (!readOutput)
                    {
                        try
                        {
                            tempInFile = new StreamReader(TempFileFullPath);
                            readOutput = true;
                        }
                        catch
                        {
                            // File is still in use, probably by the other instance
                            Thread.Sleep(10);
                        }
                    }

                    Console.Write(tempInFile.ReadToEnd());
                    tempInFile.Close();
                    File.Delete(TempFileFullPath);
                }
            else
            {
                // Upon CTRL+C, make sure the temp file gets cleaned up
                Console.CancelKeyPress += (s, e) => File.Delete(PidFileFullPath);
                File.Create(PidFileFullPath);
                RexSimulator.Run();
            }
        }

        public static int GetOtherInstancePid()
        {
            var files = Directory.EnumerateFiles(Path.GetTempPath());
            string filename = files.ToList().Find(s =>
                   s.StartsWith(PidFilePathMinusPid));

            if (filename == null)
                return -1;
            else
                return int.Parse(
						filename.Substring(PidFilePathMinusPid.Length));
        }

    }
}
