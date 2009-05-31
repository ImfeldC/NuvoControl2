using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuvoControl.Server.ProtocolDriver.Interface;

namespace NuvoControl.Server.ProtocolDriver
{
    public class NuvoEssentiaCommand : INuvoEssentiaCommand
    {
        private List<INuvoEssentiaSingleCommand> _commandList = new List<INuvoEssentiaSingleCommand>();

        Guid _guid;
        DateTime _createDateTime;

        #region Nuvo Essentia Values
        ENuvoEssentiaZones _zoneId = ENuvoEssentiaZones.NoZone;
        ENuvoEssentiaSources _sourceId = ENuvoEssentiaSources.NoSource;
        int _volume = -999;
        #endregion

        #region Constructors
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
        #endregion

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

        /// <summary>
        /// See description of INuvoEssentiaCommand
        /// </summary>
        public INuvoEssentiaSingleCommand getNextCommand(INuvoEssentiaSingleCommand prevCommand)
        {
            if (_commandList.Count <= 0)
            {
                throw new ProtocolDriverException(string.Format("The command list is empty, cannot return next command! {0}", prevCommand));
            }

            if (prevCommand != null)
            {
                if (_commandList.Contains(prevCommand))
                {
                    int index = _commandList.IndexOf(prevCommand);
                    if ((index >= 0) && (index + 1 < _commandList.Count))
                    {
                        // next command found
                        return _commandList[index + 1];
                    }
                    else
                    {
                        // next command not found, return null
                        return null;
                    }
                }
                else
                {
                    throw new ProtocolDriverException(string.Format("The command is not part of the command list, cannot return next command! {0}", prevCommand));
                }
            }
            else
            {
                // return first command in list
                return _commandList[0];
            }
        }

        /// <summary>
        /// See description of INuvoEssentiaCommand
        /// </summary>
        public void addCommand(ENuvoEssentiaCommands command)
        {
            switch( command )
            {
                // combined commands -> TurnZoneON, SetVolume and SetSource.
                case ENuvoEssentiaCommands.SetInitialZoneStatus:
                    _commandList.Add(new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.TurnZoneON,_zoneId));
                    _commandList.Add(new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.SetVolume, _zoneId, _volume));
                    _commandList.Add(new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.SetSource, _zoneId, _sourceId));
                    break;

                case ENuvoEssentiaCommands.NoCommand:
                    break;

                // default, single command and add them to the list
                default:
                    _commandList.Add(new NuvoEssentiaSingleCommand(command));
                    break;
            }
        }

        #endregion


    }
}
