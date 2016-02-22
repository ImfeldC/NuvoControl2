using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NuvoControl.Common.Configuration;
using NuvoControl.Server.ZoneServer;
using NuvoControl.Server.ProtocolDriver.Interface;


namespace NuvoControl.Server.FunctionServer
{
    class ConcreteFunctionFactory
    {
        public static IConcreteFunction instantiateConcreteFuntion(Function function, IZoneServer zoneServer, Dictionary<int, IAudioDriver> audioDrivers)
        {
            if (typeof(SleepFunction) == function.GetType())
            {
                return new ConcreteSleepFunction((SleepFunction)function, zoneServer, audioDrivers);
            }
            else if(typeof(AlarmFunction) == function.GetType())
            {
                return new ConcreteAlarmFunction((AlarmFunction)function, zoneServer, audioDrivers);
            }
            return null;
        }
    }
}
