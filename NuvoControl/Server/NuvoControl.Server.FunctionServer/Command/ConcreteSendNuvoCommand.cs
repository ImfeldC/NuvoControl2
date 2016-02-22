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

using Common.Logging;

using NuvoControl.Common;
using NuvoControl.Common.Configuration;
using NuvoControl.Server.ZoneServer;

namespace NuvoControl.Server.FunctionServer
{
    class ConcreteSendNuvoCommand : ConcreteCommand
    {
        /// <summary>
        /// Logger object.
        /// </summary>
        private static ILog _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Private member to store zone server.
        /// </summary>
        protected IZoneServer _zoneServer = null;


        private SendNuvoCommand _sendNuvoCommand = null;


        public ConcreteSendNuvoCommand(SendNuvoCommand command, IZoneServer zoneServer)
            :base( command )
        {
            _zoneServer = zoneServer;
            if (_zoneServer == null)
            {
                _log.Warn(m => m("Zone Server not available, cannot monitor any zone ..."));
            }
            _sendNuvoCommand = command;
        }

        public override void execCommand(eCommandType cmdType, Function function)
        {
            if (checkCommandType(cmdType))
            {
                LogHelper.Log(LogLevel.Info, String.Format(">>> Execute SendNuvoCommand command on event {0}", cmdType));
                LogHelper.Log(LogLevel.Trace, String.Format("      SendNuvoCommand={0} / Function={1}", _sendNuvoCommand.ToString(), function.ToString()));

                // Send Nuvo command ...
                if (_zoneServer != null)
                {
                    ZoneState newState = new ZoneState();
                    newState.Source = _sendNuvoCommand.SourceId;
                    newState.PowerStatus = _sendNuvoCommand.PowerStatus;
                    if (_sendNuvoCommand.Volume >= 0) newState.Volume = _sendNuvoCommand.Volume;
                    _zoneServer.SetZoneState(_sendNuvoCommand.ZoneId, newState);
                }
                else
                {
                    LogHelper.Log(LogLevel.Error, String.Format("      Execute SendNuvoCommand FAILED!"));
                }

            }
        }

    }
}
