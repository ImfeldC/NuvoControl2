using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NuvoControl.Common.Configuration;


namespace NuvoControl.Server.FunctionServer
{
    class ConcreteCommandFactory
    {
        public static IConcreteCommand instantiateConcreteCommand(Command command)
        {
            if (typeof(SendMailCommand) == command.GetType())
            {
                return new ConcreteSendMailCommand((SendMailCommand)command);
            }
            else if (typeof(PlaySoundCommand) == command.GetType())
            {
                return new ConcretePlaySoundCommand((PlaySoundCommand)command);
            }
            return null;
        }
    }
}
