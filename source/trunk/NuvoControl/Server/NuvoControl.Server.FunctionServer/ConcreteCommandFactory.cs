/**************************************************************************************************
 * 
 *   Copyright (C) 2016 by Ch. Imfeld. All Rights Reserved.
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.FunctionServer
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NuvoControl.Common.Configuration;
using NuvoControl.Server.ZoneServer;
using NuvoControl.Server.ProtocolDriver.Interface;


namespace NuvoControl.Server.FunctionServer
{
    class ConcreteCommandFactory
    {
        public static IConcreteCommand instantiateConcreteCommand(Command command, IZoneServer zoneServer, Dictionary<int, IAudioDriver> audioDrivers)
        {
            if (typeof(SendMailCommand) == command.GetType())
            {
                return new ConcreteSendMailCommand((SendMailCommand)command);
            }
            else if (typeof(PlaySoundCommand) == command.GetType())
            {
                PlaySoundCommand playSoundCommand = (PlaySoundCommand)command;
                return new ConcretePlaySoundCommand(playSoundCommand, zoneServer, audioDrivers[playSoundCommand.SourceId.ObjectId]);
            }
            else if (typeof(SendNuvoCommand) == command.GetType())
            {
                return new ConcreteSendNuvoCommand((SendNuvoCommand)command, zoneServer);
            }
            return null;
        }
    }
}
