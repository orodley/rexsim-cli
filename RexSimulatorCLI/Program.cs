using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace RexSimulatorCLI
{
    internal class Program
    {
        public const string TempFileName = "REXSIMCLI_TEMP";

        public static string TempFileFullPath =
            Path.Combine(Path.GetTempPath(), TempFileName);

        public static void Main(string[] args)
        {
            // Check if we're the only instance currently running
            Process otherInstance = GetOtherInstance();

            if (otherInstance != null)
                if (args.Length == 0)
                    Console.WriteLine(@"Another instance is already running with PID: " + otherInstance.Id);
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

                    bool readOutput = false;
 
                    while (!readOutput)
                    {
                        try
                        {
                            tempInFile = new StreamReader(TempFileFullPath);
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                        finally
                        {
                            Console.Write(tempInFile.ReadToEnd());
                            tempInFile.Close();
                            File.Delete(TempFileFullPath);

                            readOutput = true;
                        }
                    }
                }
            else
                new RexSimulator();
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

