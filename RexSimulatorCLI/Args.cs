using System;
using System.Collections.Generic;
using System.Linq;

namespace RexSimulatorCLI
{
    class Args
    {
        private List<string> _argsList;

        public Args(string argsString)
        {
            _argsList = argsString.Split(' ').ToList();
        }
    }
}