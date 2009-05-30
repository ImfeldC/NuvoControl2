using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuvoControl.Server.ProtocolDriver.Interface;

namespace NuvoControl.Server.ProtocolDriver
{
    public class NuvoEssentiaCommand : INuvoEssentiaCommand
    {
        private NuvoEssentiaSingleCommand[] _commandList;

        Guid _guid;
        DateTime _createDateTime;
        ENuvoEssentiaCommands _command = ENuvoEssentiaCommands.NoCommand;

        #region Nuvo Essentia Values
        ENuvoEssentiaZones _zoneId = ENuvoEssentiaZones.NoZone;
        ENuvoEssentiaSources _sourceId = ENuvoEssentiaSources.NoSource;
        int _volume = -999;
        #endregion


        public NuvoEssentiaCommand(ENuvoEssentiaCommands command)
        {
            initMembers(command);
        }

        public NuvoEssentiaCommand(ENuvoEssentiaCommands command, ENuvoEssentiaZones zone)
        {
            initMembers(command);
            _zoneId = zone;
        }

        public NuvoEssentiaCommand(ENuvoEssentiaCommands command, ENuvoEssentiaZones zone, ENuvoEssentiaSources source)
        {
            initMembers(command);
            _zoneId = zone;
            _sourceId = source;
        }

        public NuvoEssentiaCommand(ENuvoEssentiaCommands command, ENuvoEssentiaZones zone, ENuvoEssentiaSources source, int volume)
        {
            initMembers(command);
            _zoneId = zone;
            _sourceId = source;
            _volume = volume;
        }


        /// <summary>
        /// Private method to initialize the members.
        /// Call this method in each constructur at start.
        /// </summary>
        private void initMembers(ENuvoEssentiaCommands command)
        {
            _command = command;
            _createDateTime = DateTime.Now;
            _guid = Guid.NewGuid();
        }


        #region INuvoEssentiaCommand Members

        public INuvoEssentiaSingleCommand[] commandList
        {
            get { return _commandList; }
        }

        #endregion

    }
}
