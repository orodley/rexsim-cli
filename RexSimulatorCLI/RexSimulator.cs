using System;
using System.IO;
using System.Text;
using System.Threading;
using RexSimulator.Hardware;

namespace RexSimulatorCLI
{
	public class RexSimulator
	{
		private const long TARGET_CLOCK_RATE = 4000000;

		private RexBoard mRexBoard;
		private Thread mWorker;

		private BasicSerialPort mSerialPort1;
		//private BasicSerialPort mSerialPort2;
				
		private long mLastTickCount = 0;
		private DateTime mLastTickCountUpdate = DateTime.Now;
		public double mLastClockRate = TARGET_CLOCK_RATE;
		private double mLastClockRateSmoothed = TARGET_CLOCK_RATE;
		private bool mThrottleCpu = true;
		
		private bool mRunning = true;
		private bool mStepping = false;

		public RexSimulator ()
		{
			mRexBoard = new RexBoard();

			//Load WRAMPmon into ROM
			Stream wmon = new MemoryStream(ASCIIEncoding.ASCII.GetBytes(File.ReadAllText(Path.Combine("Resources", "monitor.srec"))));
			mRexBoard.LoadSrec(wmon);
			wmon.Close();
			
			//Set up the worker thread
			mWorker = new Thread(new ThreadStart(Worker));

            // Set up the timer
            // Qualified name is used since System.Threading also contains a class called "Timer"
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += timer_Elapsed;

            timer.Enabled = true;

			//Set up system interfaces
			mSerialPort1 = new BasicSerialPort(mRexBoard.Serial1);

			mWorker.Start();
		}

		private void Worker()
		{
			int stepCount = 0;
			int stepsPerSleep = 0;
			
			while (true)
			{				
				if (mRunning)
				{
					this.Step();
					mRunning ^= mStepping; //stop the CPU running if this is only supposed to do a single step.
					
					//Slow the processor down if need be
					if (mThrottleCpu)
					{
						if (stepCount++ >= stepsPerSleep)
						{
							stepCount -= stepsPerSleep;
							Thread.Sleep(5);
							int diff = (int)mLastClockRate - (int)TARGET_CLOCK_RATE;
							stepsPerSleep -= diff / 10000;
							stepsPerSleep = Math.Min(Math.Max(0, stepsPerSleep), 1000000);
						}
					}
				}
			}
		}

        /// <summary>
        /// Recalculate the simulated CPU clock rate.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Elapsed(object sender, EventArgs e)
        {
            long ticksSinceLastUpdate = mRexBoard.TickCounter - mLastTickCount;
            TimeSpan timeSinceLastUpdate = DateTime.Now.Subtract(mLastTickCountUpdate);
            mLastTickCount = mRexBoard.TickCounter;
            mLastTickCountUpdate = DateTime.Now;

            double rate = 0.5;
            mLastClockRate = ticksSinceLastUpdate / timeSinceLastUpdate.TotalSeconds;
            mLastClockRateSmoothed = mLastClockRateSmoothed * (1.0 - rate) + mLastClockRate * rate;
        }

		public void Step()
		{
			while (!mRexBoard.Tick()) ;
		}
	}
}
