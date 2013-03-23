using System.Globalization;
using System.Timers;
using RexSimulator.Hardware;

namespace RexSimulatorCLI.Panels
{
    public class SSD : Panel
    {
        private Timer _updateTimer;
        private RexBoard _rexBoard;

        public SSD(RexBoard board)
        {
            _rexBoard = board;

            _updateTimer = new Timer();
            _updateTimer.Elapsed += _updateTimer_Elapsed;
            _updateTimer.Enabled = true;
        }

        void _updateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SetCursorPosition(0, 0);
            Write("Left: " + _rexBoard.Parallel.LeftSSDOut.ToString(CultureInfo.InvariantCulture) + "\n");
            SetCursorPosition(0, 1);
            Write("Right: " + _rexBoard.Parallel.RightSSDOut.ToString(CultureInfo.InvariantCulture) + "\n");
        }
    }
}
