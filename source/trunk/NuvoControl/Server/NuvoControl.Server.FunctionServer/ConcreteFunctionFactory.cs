using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuvoControl.Common.Configuration;

namespace NuvoControl.Server.FunctionServer
{
    class ConcreteFunctionFactory
    {
        public static IConcreteFunction instantiateConcreteFuntion(Function function)
        {
            if (typeof(SleepFunction) == function.GetType())
            {
                return new ConcreteSleepFunction((SleepFunction)function);
            }
            else if(typeof(AlarmFunction) == function.GetType())
            {
                return new ConcreteAlarmFunction((AlarmFunction)function);
            }
            return null;
        }
    }
}
