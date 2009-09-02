using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuvoControl.Common.Configuration;

namespace NuvoControl.Server.FunctionServer
{
    class ConcreteAlarmFunction : ConcreteFunction
    {
        AlarmFunction _function;

        public ConcreteAlarmFunction( AlarmFunction function )
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
