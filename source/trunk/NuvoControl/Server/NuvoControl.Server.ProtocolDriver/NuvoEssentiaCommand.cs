using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuvoControl.Server.ProtocolDriver.Interface;

namespace NuvoControl.Server.ProtocolDriver
{
    public class NuvoEssentiaCommand : INuvoEssentiaCommand
    {
        private Queue<INuvoEssentiaSingleCommand> _commandList = new Queue<INuvoEssentiaSingleCommand>();

        Guid _guid;
        DateTime _createDateTime;

        #region Nuvo Essentia Values
        ENuvoEssentiaZones _zoneId = ENuvoEssentiaZones.NoZone;
        ENuvoEssentiaSources _sourceId = ENuvoEssentiaSources.NoSource;
        int _volume = -999;
        #endregion


        public NuvoEssentiaCommand(ENuvoEssentiaZones zone)
        {
            initMembers();
            _zoneId = zone;
        }

        public NuvoEssentiaCommand(ENuvoEssentiaZones zone, ENuvoEssentiaSources source)
        {
            initMembers();
            _zoneId = zone;
            _sourceId = source;
        }

        public NuvoEssentiaCommand(ENuvoEssentiaZones zone, ENuvoEssentiaSources source, int volume)
        {
            initMembers();
            _zoneId = zone;
            _sourceId = source;
            _volume = volume;
        }


        /// <summary>
        /// Private method to initialize the members.
        /// Call this method in each constructur at start.
        /// </summary>
        private void initMembers()
        {
            _commandList.Clear();
            _createDateTime = DateTime.Now;
            _guid = Guid.NewGuid();
        }


        #region INuvoEssentiaCommand Members

        public Queue<INuvoEssentiaSingleCommand> commandList
        {
            get { return _commandList; }
        }

        public void addCommand(ENuvoEssentiaCommands command)
        {
            switch( command )
            {
                // combined commands -> not handled by the single command class
                case ENuvoEssentiaCommands.SetInitialZoneStatus:
                case ENuvoEssentiaCommands.VolumeDOWN2db:
                case ENuvoEssentiaCommands.VolumeUP2db:

                case ENuvoEssentiaCommands.NoCommand:
                    break;

                // default, assume single command and add them to the list
                default:
                    _commandList.Enqueue(new NuvoEssentiaSingleCommand(command));
                    break;
            }
        }

        #endregion
    }
}
