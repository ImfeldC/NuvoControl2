using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuvoControl.Server.Simulator
{
    class SimulatorException : Exception
    {
        public SimulatorException(string message)
            : base(message)
        {
        }
    }
}
