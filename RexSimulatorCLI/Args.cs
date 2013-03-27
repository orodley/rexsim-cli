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
    /// Then add the long and short forms of the option to the switches in the
    /// constructor. All you have to do in the case for your option is call your method
    /// </summary>
    class Args
    {
        private const string InvalidInputMessage  = @"Invalid args: "; // TODO: Better message
        private const string UnknownOptionMessage = @"Unknown option: ";

        private readonly List<string> _argsList;
        private readonly RexBoard _rexBoard;
        public string Output { get; private set; }

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

            Output = ""; /* Initialize to "" so that non-output-setting options don't
                            cause a NullReferenceException */

            // There should only be one argument starting with "-" or "--"...
            if (_argsList.Count(_startsWithHyphen) != 1 ||
                // ...and it should be at the start
                _argsList.FindIndex(_startsWithHyphen) != 0)
            {
                Output = InvalidInputMessage + string.Join(" ", _argsList);
            }
            else if (_argsList[0].StartsWith("--")) // Long option
                switch (_argsList[0].Substring(2))
                {
                    case "registers":
                        _outputRegisters();
                        break;
                    case "toggle-throttling":
                        _toggleCpuThrottling();
                        break;
                    default:
                        Output = UnknownOptionMessage + _argsList[0];
                        break;
                }
            else                                    // Short option
            {
                switch (_argsList[0].Substring(1))
                {
                    case "r":
                        _outputRegisters();
                        break;
                    case "t":
                        _toggleCpuThrottling();
                        break;
                    default:
                        Output = UnknownOptionMessage + _argsList[0];
                        break;
                }
            }
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