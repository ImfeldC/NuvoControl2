using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NuvoControl.Common.Configuration;
using NuvoControl.Server.ZoneServer;


namespace NuvoControl.Server.FunctionServer
{
    class ConcreteFunctionFactory
    {
        public static IConcreteFunction instantiateConcreteFuntion(Function function, IZoneServer zoneServer)
        {
            if (typeof(SleepFunction) == function.GetType())
            {
                return new ConcreteSleepFunction((SleepFunction)function, zoneServer);
            }
            else if(typeof(AlarmFunction) == function.GetType())
            {
                return new ConcreteAlarmFunction((AlarmFunction)function, zoneServer);
            }
            return null;
        }
    }
}
