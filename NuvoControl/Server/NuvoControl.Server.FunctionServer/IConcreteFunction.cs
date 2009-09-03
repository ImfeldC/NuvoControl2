using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuvoControl.Common.Configuration;

namespace NuvoControl.Server.FunctionServer
{
    interface IConcreteFunction
    {
        /// <summary>
        /// Returns the underlying configuration data for this function object.
        /// </summary>
        Function Function { get; }

        /// <summary>
        /// This method is periodically called, to re-calculate the current function.
        /// If required, this method sends commands to the zones, to change their state.
        /// </summary>
        /// <param name="aktTime">Actual date and time, as base for the calculation.</param>
        void calculateFunction(DateTime aktTime);

    }
}
