using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using RexSimulator.Hardware;

namespace RexSimulatorCLI
{
    public class RexSimulator
    {
        private const long TargetClockRate = 4000000;

        private readonly RexBoard _rexBoard;
        private readonly Thread _cpuWorker;
        private readonly Thread _inputWorker;

        private BasicSerialPort _serialPort1;
        //private BasicSerialPort mSerialPort2;
                
        private long _lastTickCount = 0;
        private DateTime _lastTickCountUpdate = DateTime.Now;
        public double LastClockRate = TargetClockRate;
        private double _lastClockRateSmoothed = TargetClockRate;
        private bool _throttleCpu = true;
        
        private bool _running = true;
        private bool _stepping = false;

        public RexSimulator ()
        {
            _rexBoard = new RexBoard();

            //Load WRAMPmon into ROM
            Stream wmon =
                new MemoryStream(Encoding.ASCII.GetBytes(
                    File.ReadAllText(Path.Combine("Resources", "monitor.srec"))));
            _rexBoard.LoadSrec(wmon);
            wmon.Close();
            
            //Set up the worker threads
            _cpuWorker = new Thread(CPUWorker);
            _inputWorker = new Thread(InputWorker);

            // Set up the timer
            // Qualified name is used since System.Threading also contains a class called "Timer"
            var timer = new System.Timers.Timer();
            timer.Elapsed += timer_Elapsed;

            timer.Enabled = true;

            //Set up system interfaces
            _serialPort1 = new BasicSerialPort(_rexBoard.Serial1);

            _cpuWorker.Start();
            _inputWorker.Start();
        }

        private void CPUWorker()
        {
            int stepCount = 0;
            int stepsPerSleep = 0;
            
            while (true)
            {
                if (_running)
                {
                    Step();
                    _running ^= _stepping; //stop the CPU running if this is only supposed to do a single step.
                    
                    //Slow the processor down if need be
                    if (_throttleCpu)
                    {
                        if (stepCount++ >= stepsPerSleep)
                        {
                            stepCount -= stepsPerSleep;
                            Thread.Sleep(5);
                            int diff = (int)LastClockRate - (int)TargetClockRate;
                            stepsPerSleep -= diff / 10000;
                            stepsPerSleep = Math.Min(Math.Max(0, stepsPerSleep), 1000000);
                        }
                    }
                }
            }
        }

        private void InputWorker()
        {
            while (true)
            {
                if (_running && Console.KeyAvailable)
                {
                    var keyPress = Console.ReadKey(true);

                    // Handle Ctrl+A [somekey]
                    if (keyPress.Modifiers.HasFlag(ConsoleModifiers.Control) && keyPress.Key == ConsoleKey.A)
                    {
                        switch (Console.ReadKey(true).KeyChar)
                        {
                            case 's': // Sending an S-Record
                                var filename = Console.ReadLine();
                                var uploadFileWorker = new Thread(UploadFileWorker);
                                uploadFileWorker.Start(filename);
                                break;
                            // 1-8: Toggling a switch
                            case '1': ToggleSwitch(0); break;
                            case '2': ToggleSwitch(1); break;
                            case '3': ToggleSwitch(2); break;
                            case '4': ToggleSwitch(3); break;
                            case '5': ToggleSwitch(4); break;
                            case '6': ToggleSwitch(5); break;
                            case '7': ToggleSwitch(6); break;
                            case '8': ToggleSwitch(7); break;
                        }
                    }
                    else
                        _rexBoard.Serial1.SendAsync(keyPress.KeyChar);
                }
            }
        }

        /// <summary>
        /// Toggle the "switchNum"th switch from the left
        /// </summary>
        /// <param name="switchNum"></param>
        private void ToggleSwitch(int switchNum)
        {
            _rexBoard.Parallel.Switches ^= (128u >> switchNum);
        }

        /// <summary>
        /// Sends a file through the serial port.
        /// </summary>
        private void UploadFileWorker(object filename)
        {
            var reader = new StreamReader((string)filename);

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                foreach (char c in line)
                {
                    _rexBoard.Serial1.Send(c);
                }
                _rexBoard.Serial1.Send('\n');
            }

            reader.Close();
        }

        /// <summary>
        /// Recalculate the simulated CPU clock rate.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Elapsed(object sender, EventArgs e)
        {
            long ticksSinceLastUpdate = _rexBoard.TickCounter - _lastTickCount;
            TimeSpan timeSinceLastUpdate = DateTime.Now.Subtract(_lastTickCountUpdate);
            _lastTickCount = _rexBoard.TickCounter;
            _lastTickCountUpdate = DateTime.Now;

            double rate = 0.5;
            LastClockRate = ticksSinceLastUpdate / timeSinceLastUpdate.TotalSeconds;
            _lastClockRateSmoothed = _lastClockRateSmoothed * (1.0 - rate) + LastClockRate * rate;

            Console.Title = string.Format("REX Board Simulator: Clock Rate: {0:0.000} MHz ({1:000}%)", _lastClockRateSmoothed / 1e6, _lastClockRateSmoothed * 100 / TargetClockRate);
        }

        public void Step()
        {
            while (!_rexBoard.Tick()) { }
        }
    }
}
