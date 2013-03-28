using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RexSimulator.Hardware;

namespace RexSimulatorCLI
{
    /// <summary>
    /// Handles arg parsing
    /// 
    /// To add a new option, first write the method that will perform the required
    /// action for that option. Everything in Output gets written out to standard
    /// out, so put any desired output from the option there
    /// 
    /// Then add the long and short forms of the option to _longOptionMap and
    /// _shortOptionMap (initialized in the constructor)
    /// </summary>
    class Args
    {
        private const string InvalidInputMessage  = @"Invalid args: "; // TODO: Better message
        private const string UnknownOptionMessage = @"Unknown option: ";

        private readonly List<string> _argsList;
        private readonly RexBoard _rexBoard;
        public string Output { get; private set; }

        private readonly Dictionary<string, Action> _shortOptionMap;
        private readonly Dictionary<string, Action> _longOptionMap;

        /// <summary>
        /// Given a string containing args and a RexBoard, work out the appropriate
        /// response to be printed based on those args, and store it in Output
        /// </summary>
        /// <param name="argsString"></param>
        /// <param name="board"></param>
        public Args(string argsString, RexBoard board)
        {
            _argsList = argsString.Split(' ').ToList();
            _rexBoard = board;

            _shortOptionMap = new Dictionary<string, Action>
                {
                    {"r", _outputRegisters},
                    {"t", _toggleCpuThrottling},
                };

            _longOptionMap = new Dictionary<string, Action>
                {
                    
                    {"registers",         _outputRegisters},
                    {"toggle-throttling", _toggleCpuThrottling},
                };

            Output = ""; /* Initialize to "" so that non-output-setting options don't
                            cause a NullReferenceException */
            

            // There should only be one argument starting with "-" or "--"...
            if (_argsList.Count(_startsWithHyphen) != 1 ||
                // ...and it should be at the start
                _argsList.FindIndex(_startsWithHyphen) != 0)
            {
                Output = InvalidInputMessage + string.Join(" ", _argsList);
                return;
            }

            int substringStart;
            Dictionary<string, Action> longOrShortMap;

            if (_argsList[0].StartsWith("--")) // Long option
            {
                substringStart = 2;
                longOrShortMap = _longOptionMap;
            }
            else                                    // Short option
            {
                substringStart = 1;
                longOrShortMap = _shortOptionMap;
            }

            Action action;
            if (longOrShortMap.TryGetValue(_argsList[0].Substring(substringStart), out action))
                action();
            else
                Output = UnknownOptionMessage + _argsList[0];
        }

        private bool _startsWithHyphen(string s) { return s.StartsWith("-"); }

        private void _outputRegisters()
        {
            var sb = new StringBuilder();

            for (int register = 0; register < 16; register++)
                sb.Append(string.Format("${0:D2}: {1:X8}\n",
                                        register, _rexBoard.CPU.mGpRegisters[(uint)register]));
            Output = sb.ToString();
        }

        private void _toggleCpuThrottling()
        {
            RexSimulator.ThrottleCpu = !RexSimulator.ThrottleCpu;
        }
    }
}