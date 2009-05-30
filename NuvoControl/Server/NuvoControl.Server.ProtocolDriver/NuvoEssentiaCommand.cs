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

        public NuvoEssentiaCommand(ENuvoEssentiaCommands command, ENuvoEssentiaZones zone, ENuvoEssentiaSources source, int volume)
        {
            initMembers();
            _zoneId = zone;
            _sourceId = source;
            _volume = volume;
            addCommand(command);
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
                // combined commands -> TurnZoneON, SetVolume and SetSource.
                case ENuvoEssentiaCommands.SetInitialZoneStatus:
                    _commandList.Enqueue(new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.TurnZoneON,_zoneId));
                    _commandList.Enqueue(new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.SetVolume, _zoneId, _volume));
                    _commandList.Enqueue(new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.SetSource, _zoneId, _sourceId));
                    break;

                // combined commands -> not handled by the single command class
                case ENuvoEssentiaCommands.VolumeDOWN2db:
                case ENuvoEssentiaCommands.VolumeUP2db:
                    _commandList.Enqueue(new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ReadStatusCONNECT, _zoneId));
                    _commandList.Enqueue(new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.SetVolume, _zoneId, _volume)); //TODO use current volume to set new volume level
                    break;

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
