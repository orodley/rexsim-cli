using System;
using System.Timers;
using RexSimulator.Hardware;

namespace RexSimulatorCLI.Panels
{
    class Registers : Panel
    {
        private readonly RexBoard _rexBoard;
        private readonly Timer _updateTimer;
        private string _formatString;

        public Registers(RexBoard rexBoard)
        {
            _rexBoard = rexBoard;

            _formatString = "${0:D2}: {1}\n";

            _updateTimer = new Timer();
            _updateTimer.Elapsed += _updateTimer_Elapsed;
            _updateTimer.Enabled = true;

            InputRecieved += Registers_InputRecieved;
        }

        private void _updateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            for (uint register = 0; register < 16; register++)
            {
                SetCursorPosition(0, (int)register);
                Write(string.Format(_formatString, register, _rexBoard.CPU.mGpRegisters[register]));
            }
        }

        private void Registers_InputRecieved(ConsoleKeyInfo keypress)
        {
            if (keypress.Modifiers.HasFlag(ConsoleModifiers.Control))
                switch (keypress.Key)
                {
                    case ConsoleKey.H: // Switch the display to hex
                        _formatString = "${0:D2}: 0x{1:X8}";
                        break;
                    case ConsoleKey.D: // Switch the display to decimal (default)
                        _formatString = "${0:D2}: {1}\n";
                        break;
                }
        }
    }
}
