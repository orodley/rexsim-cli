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
				/*if (mRunning && mRamForm.Breakpoints.Contains(mRexBoard.CPU.PC)) //stop the CPU if a breakpoint has been hit
				{
					this.Invoke(new Action(runButton.PerformClick));
					continue;
				}*/
				
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
						
						/*for (long i = 0; i < mSlowdownCount / 1000000000; i++) ; //dirty cpu cycle-wasting loop                        

                        long diff = (long)mLastClockRate - TARGET_CLOCK_RATE;

                        mSlowdownCount += diff;

                        Thread.Sleep(0); //swap thread out to allow other processes to run.*/
					}
				}
			}
		}

		public void Step()
		{
			while (!mRexBoard.Tick()) ;
		}
	}
}
