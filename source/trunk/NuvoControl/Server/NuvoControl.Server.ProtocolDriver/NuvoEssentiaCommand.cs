/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.ProtocolDriver
 *   Author:         Ch.Imfeld
 *   Creation Date:  6/12/2009 11:02:29 PM
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 6/12/2009 11:02:29 PM, Ch.Imfeld: Initial implementation.
 * 
 **************************************************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuvoControl.Server.ProtocolDriver.Interface;
using NuvoControl.Common;
using Common.Logging;

namespace NuvoControl.Server.ProtocolDriver
{
    /// <summary>
    /// Class to handle multiple Nuvo Essentia commands.
    /// See class <see cref="INuvoEssentiaSingleCommand"/> for more information.
    /// </summary>
    public class NuvoEssentiaCommand : INuvoEssentiaCommand
    {
        /// <summary>
        /// Private member list, which holds the single Nuvo Essentia commands.
        /// </summary>
        private List<INuvoEssentiaSingleCommand> _commandList = new List<INuvoEssentiaSingleCommand>();

        Guid _guid;
        DateTime _createDateTime;

        #region Nuvo Essentia Values
        ENuvoEssentiaZones _zoneId = ENuvoEssentiaZones.NoZone;
        ENuvoEssentiaSources _sourceId = ENuvoEssentiaSources.NoSource;
        int _volume = ZoneState.VALUE_UNDEFINED;
        #endregion

        #region Constructors
        /// <summary>
        /// Public constructor, setting the Zone id.
        /// </summary>
        /// <param name="zone">Zone id.</param>
        public NuvoEssentiaCommand(ENuvoEssentiaZones zone)
        {
            initMembers();
            _zoneId = zone;
        }

        /// <summary>
        /// Public constructor, setting the Zone and Source id.
        /// </summary>
        /// <param name="zone">Zone id.</param>
        /// <param name="source">Source id.</param>
        public NuvoEssentiaCommand(ENuvoEssentiaZones zone, ENuvoEssentiaSources source)
        {
            initMembers();
            _zoneId = zone;
            _sourceId = source;
        }

        /// <summary>
        /// Public constructor, setting the Zone id, Source id and volume level.
        /// </summary>
        /// <param name="zone">Zone id.</param>
        /// <param name="source">Source id.</param>
        /// <param name="volume">Volume Level.</param>
        public NuvoEssentiaCommand(ENuvoEssentiaZones zone, ENuvoEssentiaSources source, int volume)
        {
            initMembers();
            _zoneId = zone;
            _sourceId = source;
            _volume = limitVolume2NuvoEssentia(volume);
        }

        /// <summary>
        /// Public constructor, setting the Command and Zone id.
        /// </summary>
        /// <param name="command">Nuvo Essentia Command.</param>
        /// <param name="zone">Zone Id.</param>
        public NuvoEssentiaCommand(ENuvoEssentiaCommands command, ENuvoEssentiaZones zone)
        {
            initMembers();
            _zoneId = zone;
            addCommand(command);
        }

        /// <summary>
        /// Public constructor, setting the Command, Zone Id, Source Id and Volume level.
        /// </summary>
        /// <param name="command">Nuvo Essentia Command.</param>
        /// <param name="zone">Zone Id.</param>
        /// <param name="source">Source Id.</param>
        /// <param name="volume">Volume Level.</param>
        public NuvoEssentiaCommand(ENuvoEssentiaCommands command, ENuvoEssentiaZones zone, ENuvoEssentiaSources source, int volume)
        {
            initMembers();
            _zoneId = zone;
            _sourceId = source;
            _volume = limitVolume2NuvoEssentia(volume);
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

        /// <summary>
        /// Public static method to adjust the volume level. It checks the allowed range for the Nuvo Essentia.
        /// If the value is outside of the range of <c>NUVOESSENTIA_VOLUME_MINVALUE</c>, 
        /// and <c>NUVOESSENTIA_VOLUME_MAXVALUE</c>, it is set to the corresponding boundary value.
        /// </summary>
        /// <param name="volume">Volume Level.</param>
        /// <returns>Volume Level, adjusted to the allowed range of Nuvo Essentia.</returns>
        public static int limitVolume2NuvoEssentia(int volume)
        {
            return (volume<NuvoEssentiaSingleCommand.NUVOESSENTIA_VOLUME_MINVALUE?NuvoEssentiaSingleCommand.NUVOESSENTIA_VOLUME_MINVALUE:(volume>NuvoEssentiaSingleCommand.NUVOESSENTIA_VOLUME_MAXVALUE?NuvoEssentiaSingleCommand.NUVOESSENTIA_VOLUME_MAXVALUE:volume));
        }

        /// <summary>
        /// Public static method to calculate the volume level - based on volume level received from
        /// NuvoControl Server - for a Nuvo Essentia device.
        /// L  = Laustärke NuvoControl
        /// LE = Laustärke Essentia
        /// L  = ((LE + 79) * 100) / 79
        /// LE = ((L * 79) / 100) - 79
        /// </summary>
        /// <param name="volume">Volume Level, from NuvoControl</param>
        /// <returns>Volume Level, for NuvoEssentia.</returns>
        public static int calcVolume2NuvoEssentia(int volume)
        {
            //TODO: Use constants for calculation
            double d = ((volume * 79.0) / 100.0);
            int i = (int)Math.Round(d); 
            return i - 79;
        }

        /// <summary>
        /// Public static method to calculate the volume level - based on volume level received from
        /// NuvoControl Server - for a Nuvo Essentia device.
        /// L  = Laustärke NuvoControl
        /// LE = Laustärke Essentia
        /// L  = ((LE + 79) * 100) / 79
        /// LE = ((L * 79) / 100) - 79
        /// </summary>
        /// <param name="volume">Volume Level, from NuvoControl</param>
        /// <returns>Volume Level, for NuvoEssentia.</returns>
        public static int calcVolume2NuvoControl(int volume)
        {
            //TODO: Use constants for calculation
            return ((volume + 79) * 100) / 79;
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
                case ENuvoEssentiaCommands.SetZoneStatus:
                    _commandList.Add(new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.TurnZoneON,_zoneId));
                    _commandList.Add(new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.SetVolume, _zoneId, _volume));
                    _commandList.Add(new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.SetSource, _zoneId, _sourceId));
                    break;

                // combined command --> Two commands => ReadStatusCONNECT and ReadStatusZONE.
                case ENuvoEssentiaCommands.GetZoneStatus:
                    _commandList.Add(new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ReadStatusCONNECT, _zoneId));
                    _commandList.Add(new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ReadStatusZONE, _zoneId));
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
