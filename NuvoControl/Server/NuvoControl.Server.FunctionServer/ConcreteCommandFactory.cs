using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NuvoControl.Common.Configuration;
using NuvoControl.Server.ZoneServer;


namespace NuvoControl.Server.FunctionServer
{
    class ConcreteCommandFactory
    {
        public static IConcreteCommand instantiateConcreteCommand(Command command, IZoneServer zoneServer)
        {
            if (typeof(SendMailCommand) == command.GetType())
            {
                return new ConcreteSendMailCommand((SendMailCommand)command);
            }
            else if (typeof(PlaySoundCommand) == command.GetType())
            {
                return new ConcretePlaySoundCommand((PlaySoundCommand)command);
            }
            else if (typeof(SendNuvoCommand) == command.GetType())
            {
                return new ConcreteSendNuvoCommand((SendNuvoCommand)command, zoneServer);
            }
            return null;
        }
    }
}
