using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuvoControl.Common.Configuration;

namespace NuvoControl.Server.FunctionServer
{
    class ConcreteSleepFunction : ConcreteFunction
    {
        SleepFunction _function;

        public ConcreteSleepFunction(SleepFunction function)
        {
            _function = function;
        }

        public override Function Function
        {
            get
            {
                return _function;
            }
        }
    }
}
