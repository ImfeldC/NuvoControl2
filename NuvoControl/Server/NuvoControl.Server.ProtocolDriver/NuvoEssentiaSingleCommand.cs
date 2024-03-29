﻿/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
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
using AMS.Profile;
using Common.Logging;
using NuvoControl.Common;

/*! 
 * \page pagePlaceHolders Protocol Driver Configuration Placeholders
 *
 * This page explains the placeholders used to identify values in the command templates.
 *
 * The command templates are configured in the file \ref NuvoEssentiaProfile.xml
 *
 * The following placeholders are known, either in incoming (=I) and/or outgoing (=O) commands:
 *
 * <table>
 * <tr> 
 *     <th><b> I </b></th> 
 *     <th><b> O </b></th> 
 *     <th><b> Placeholder </b></th> 
 *     <th><b> Field </b></th> 
 *     <th><b> Description </b></th>
 * </tr><tr>
 *     <td>x</td>
 *     <td>-</td>
 *     <td>aa,bb,cc,dd,ee,ff</td>
 *     <td>for the IR carrier frequency.</td>
 *     <td>Replaced by NuvoControl.Server.ProtocolDriver.NuvoEssentiaSingleCommand::replacePlaceholderForIRFrequency() .</td>
 * </tr><tr>
 *     <td>x</td>
 *     <td>x</td>
 *     <td>xx</td>
 *     <td>Zone Id.</td>
 *     <td>Replaced by NuvoControl.Server.ProtocolDriver.NuvoEssentiaSingleCommand::replacePlaceholderForZone(). 
 *         Parsed with NuvoControl.Server.ProtocolDriver.NuvoEssentiaSingleCommand::parseCommandForZone().</td>
 * </tr><tr>
 *     <td>x</td>
 *     <td>-</td>
 *     <td>ppp</td>
 *     <td>Zone power status.</td>
 *     <td>Replaced by NuvoControl.Server.ProtocolDriver.NuvoEssentiaSingleCommand::replacePlaceholderForPowerStatus(). 
 *         Parsed with NuvoControl.Server.ProtocolDriver.NuvoEssentiaSingleCommand::parseCommandForPowerStatus().</td>
 * </tr><tr>
 *     <td>x</td>
 *     <td>x</td>
 *     <td>s</td>
 *     <td>Source id.</td>
 *     <td>Replaced by NuvoControl.Server.ProtocolDriver.NuvoEssentiaSingleCommand::replacePlaceholderForSource(). 
 *         Parsed with NuvoControl.Server.ProtocolDriver.NuvoEssentiaSingleCommand::parseCommandForSource().</td>
 * </tr><tr>
 *     <td>x</td>
 *     <td>-</td>
 *     <td>q</td>
 *     <td>Source group (on/off).</td>
 *     <td>Parsed with NuvoControl.Server.ProtocolDriver.NuvoEssentiaSingleCommand::parseCommandForSourceGroupStatus().</td>
 * </tr><tr>
 *     <td>x</td>
 *     <td>x</td>
 *     <td>yy</td>
 *     <td>Volume level.</td>
 *     <td>Replaced by NuvoControl.Server.ProtocolDriver.NuvoEssentiaSingleCommand::replacePlaceholderWithVolumeLevel(). 
 *         Parsed with NuvoControl.Server.ProtocolDriver.NuvoEssentiaSingleCommand::parseCommandForVolumeLevel().</td>
 * </tr><tr>
 *     <td>x</td>
 *     <td>-</td>
 *     <td>i</td>
 *     <td>DIP switch overridden (on/off).</td>
 *     <td>Parsed with NuvoControl.Server.ProtocolDriver.NuvoEssentiaSingleCommand::parseCommandForDIPSwitchOverrideStatus().</td>
 * </tr><tr>
 *     <td>x</td>
 *     <td>x</td>
 *     <td>uuu</td>
 *     <td>Bass EQ level.</td>
 *     <td>Replaced by NuvoControl.Server.ProtocolDriver.NuvoEssentiaSingleCommand::replacePlaceholderWithBassTrebleLevel(). 
 *         Parsed with NuvoControl.Server.ProtocolDriver.NuvoEssentiaSingleCommand::parseCommandForBassLevel().</td>
 * </tr><tr>
 *     <td>x</td>
 *     <td>x</td>
 *     <td>ttt</td>
 *     <td>Treble EQ level.</td>
 *     <td>Replaced by NuvoControl.Server.ProtocolDriver.NuvoEssentiaSingleCommand::replacePlaceholderWithBassTrebleLevel(). 
 *         Parsed with NuvoControl.Server.ProtocolDriver.NuvoEssentiaSingleCommand::parseCommandForTrebleLevel().</td>
 * </tr><tr>
 *     <td>x</td>
 *     <td>-</td>
 *     <td>r</td>
 *     <td>Volume reset (on/off).</td>
 *     <td>Parsed with NuvoControl.Server.ProtocolDriver.NuvoEssentiaSingleCommand::parseCommandForVolumeResetStatus().</td>
 * </tr><tr>
 *     <td>x</td>
 *     <td>-</td>
 *     <td>vz.zz</td>
 *     <td>Firmware version.</td>
 *     <td>Parsed with NuvoControl.Server.ProtocolDriver.NuvoEssentiaSingleCommand::parseCommandForFirmwareVersion().</td>
 * </tr>
 * </table>
 * 
 */

namespace NuvoControl.Server.ProtocolDriver
{
    /// <summary>
    /// This class represents a single Nuvo Essentia command.
    /// 
    /// This class has members to parse or replace for all possible values send and/or received from Nuvo Essentia.
    /// Depending on the specified command not all members will be used and set at runtime
    /// to a value received from Nuvo Essentia.
    /// Mainly it has several parse and replace methods to build and parse the command string
    /// send to and received from Nuvo Essentia. The members are either used as input data to
    /// replace the placeholders in the out-going command, or to store the information retrieved
    /// from the incoming command
    /// 
    /// This class contains public and private methods to replace placeholders
    /// in the command string.
    /// 
    /// The allowed placeholders are explained in more detail on the page \ref pagePlaceHolders "Placeholders"
    /// 
    /// In outgoing commands Replacement methods are used.
    /// For the incoming commands refer the Parse Section.
    /// 
    /// </summary>
    public class NuvoEssentiaSingleCommand : INuvoEssentiaSingleCommand
    {
        #region Common Logger
        /// <summary>
        /// Common logger object. Requires the using directive <c>Common.Logging</c>. See 
        /// <see cref="LogManager"/> for more information.
        /// </summary>
        private ILog _log = LogManager.GetCurrentClassLogger();
        #endregion

        private string _profileFilename = null;
        private Profile _profile;

        SimpleId _simpleId;
        DateTime _createDateTime;
        DateTime _sendDateTime;
        DateTime _receiveDateTime;
        ENuvoEssentiaCommands _command = ENuvoEssentiaCommands.NoCommand;

        /// <summary>
        /// Public constant defining the maximum value of the Nuvo Essentia volume level.
        /// Each value matching the following ruls is ok:
        /// VOLUME_MINVALUE is less or equal to 'value' is less or equal to VOLUMEMAXLEVEL
        /// </summary>
        public const int NUVOESSENTIA_VOLUME_MAXVALUE = 0;

        /// <summary>
        /// Public constant defining the minimum value of the Nuvo Essentia volume level.
        /// Each value matching the following ruls is ok:
        /// VOLUME_MINVALUE is less or equal to 'value' is less or equal to VOLUMEMAXLEVEL
        /// </summary>
        public const int NUVOESSENTIA_VOLUME_MINVALUE = -79;



        #region Profile Values (read from xml file)
        int _id;
        string _outgoingCommand;
        string _outgoingCommandTemplate;
        string _incomingCommand;
        string _incomingCommandTemplate;
        #endregion

        #region Nuvo Essentia Values
        ENuvoEssentiaZones _zoneId = ENuvoEssentiaZones.NoZone;
        ENuvoEssentiaSources _sourceId = ENuvoEssentiaSources.NoSource;
        EZonePowerStatus _powerStatus = EZonePowerStatus.ZoneStatusUnknown;
        EIRCarrierFrequency[] _irCarrierFrequencySource = new EIRCarrierFrequency[6];
        int _volume = ZoneState.VALUE_UNDEFINED;
        int _basslevel = ZoneState.VALUE_UNDEFINED;
        int _treblelevel = ZoneState.VALUE_UNDEFINED;
        EDIPSwitchOverrideStatus _dipSwitchOverrideStatus = EDIPSwitchOverrideStatus.DIPSwitchOverrideUnknown;
        EVolumeResetStatus _volumeResetStatus = EVolumeResetStatus.VolumeResetUnknown;
        ESourceGroupStatus _sourceGroupStatus = ESourceGroupStatus.SourceGroupUnknown;
        string _firmwareVersion = "";
        #endregion

        // Is used in case, we need to transfer this class as object
        // through the MSMQ.
        //public NuvoEssentiaSingleCommand()
        //{
        //    constructMembers();
        //    initMembers(ENuvoEssentiaCommands.NoCommand);
        //}

        #region Outgoing Command Constructors

        /// <summary>
        /// Constructor to build an outgoing command.
        /// This constructor is only useable for paramaterless commands. Use other constructors if you need to pass 
        /// parameters, e.g. the zone id, etc.
        /// </summary>
        /// <param name="command">NuvoEssentia command to build.</param>
        public NuvoEssentiaSingleCommand(ENuvoEssentiaCommands command)
        {
            constructMembers();
            initMembers(command);
            _outgoingCommand = buildOutgoingCommand();
        }

        /// <summary>
        /// Constructor to build an outgoing command.
        /// </summary>
        /// <param name="command">NuvoEssentia command to build.</param>
        /// <param name="source">Source id, which shall be used in the outgoing command.</param>
        public NuvoEssentiaSingleCommand(ENuvoEssentiaCommands command, ENuvoEssentiaSources source)
        {
            constructMembers();
            initMembers(command);
            _sourceId = source;
            _outgoingCommand = buildOutgoingCommand();
        }

        /// <summary>
        /// Constructor to build an outgoing command.
        /// </summary>
        /// <param name="command">NuvoEssentia command to build.</param>
        /// <param name="zone">Zone id, which shall be used in the outgoing command.</param>
        public NuvoEssentiaSingleCommand(ENuvoEssentiaCommands command, ENuvoEssentiaZones zone)
        {
            constructMembers();
            initMembers(command);
            _zoneId = zone;
            _outgoingCommand = buildOutgoingCommand();
        }

        /// <summary>
        /// Constructor to build an outgoing command.
        /// </summary>
        /// <param name="command">NuvoEssentia command to build.</param>
        /// <param name="zone">Zone id, which shall be used in the outgoing command.</param>
        /// <param name="source">Source id, which shall be used in the outgoing command.</param>
        public NuvoEssentiaSingleCommand(ENuvoEssentiaCommands command, ENuvoEssentiaZones zone, ENuvoEssentiaSources source)
        {
            constructMembers();
            initMembers(command);
            _zoneId = zone;
            _sourceId = source;
            _outgoingCommand = buildOutgoingCommand();
        }

        /// <summary>
        /// Constructor to build an outgoing command.
        /// </summary>
        /// <param name="command">NuvoEssentia command to build.</param>
        /// <param name="zone">Zone id, which shall be used in the outgoing command.</param>
        /// <param name="volume">Volume Level, which shall be used in the outgoing command.</param>
        public NuvoEssentiaSingleCommand(ENuvoEssentiaCommands command, ENuvoEssentiaZones zone, int volume)
        {
            constructMembers();
            initMembers(command);
            _zoneId = zone;
            _volume = NuvoEssentiaCommand.limitVolume2NuvoEssentia(volume);
            _outgoingCommand = buildOutgoingCommand();
        }

        /// <summary>
        /// Constructor to build an outgoing command.
        /// </summary>
        /// <param name="command">NuvoEssentia command to build.</param>
        /// <param name="zone">Zone id, which shall be used in the outgoing command.</param>
        /// <param name="basslevel">Bass Level, which shall be used in the outgoing command.</param>
        /// <param name="treblelevel">Treble Level, which shall be used in the outgoing command.</param>
        public NuvoEssentiaSingleCommand(ENuvoEssentiaCommands command, ENuvoEssentiaZones zone, int basslevel, int treblelevel)
        {
            constructMembers();
            initMembers(command);
            _zoneId = zone;
            _basslevel = basslevel;
            _treblelevel = treblelevel;
            _outgoingCommand = buildOutgoingCommand();
        }

        /// <summary>
        /// Constructor to build an outgoing command.
        /// </summary>
        /// <param name="command">NuvoEssentia command to build.</param>
        /// <param name="zone">Zone is, which shall be used in the outgoing command.</param>
        /// <param name="source">Source id, which shall be used in the outgoing command.</param>
        /// <param name="volume">Volume Level, which shall be used in the outgoing command.</param>
        /// <param name="basslevel">Bass Level, which shall be used in the outgoing command.</param>
        /// <param name="treblelevel">Treble Level, which shall be used in the outgoing command.</param>
        public NuvoEssentiaSingleCommand(ENuvoEssentiaCommands command, ENuvoEssentiaZones zone, ENuvoEssentiaSources source, int volume, int basslevel, int treblelevel)
        {
            constructMembers();
            initMembers(command);
            _zoneId = zone;
            _sourceId = source;
            _volume = NuvoEssentiaCommand.limitVolume2NuvoEssentia(volume);
            _basslevel = basslevel;
            _treblelevel = treblelevel;
            _outgoingCommand = buildOutgoingCommand();
        }

        /// <summary>
        /// Constructor to build an outgoing command.
        /// </summary>
        /// <param name="command">NuvoEssentia command to build.</param>
        /// <param name="zone">Zone id, which shall be used in the outgoing command.</param>
        /// <param name="source">Source id, which shall be used in the outgoing command.</param>
        /// <param name="volume">Volume Level, which shall be used in the outgoing command.</param>
        /// <param name="basslevel">Bass Level, which shall be used in the outgoing command.</param>
        /// <param name="treblelevel">Treble Level, which shall be used in the outgoing command.</param>
        /// <param name="powerStatus">Power Status, which shall be used in the outgoing command.</param>
        /// <param name="ircf">Array of IR Carrier Frequency, which shall be used in the outgoing command.</param>
        /// <param name="dipSwitchOverrideStatus">DIP Switch Override Status, which shall be used in the outgoing command.</param>
        /// <param name="volumeResetStatus">Volume Reset Status, which shall be used in the outgoing command.</param>
        /// <param name="sourceGroupStatus">Source Group Status, which shall be used in the outgoing command.</param>
        /// <param name="firmwareVersion">Firmware Version, which shall be used in the outgoing command.</param>
        public NuvoEssentiaSingleCommand(
            ENuvoEssentiaCommands command, ENuvoEssentiaZones zone, 
            ENuvoEssentiaSources source, int volume, int basslevel, int treblelevel,
            EZonePowerStatus powerStatus, EIRCarrierFrequency[] ircf,
            EDIPSwitchOverrideStatus dipSwitchOverrideStatus, EVolumeResetStatus volumeResetStatus, 
            ESourceGroupStatus sourceGroupStatus, string firmwareVersion)
        {
            constructMembers();
            initMembers(command);
            _zoneId = zone;
            _sourceId = source;
            _powerStatus = powerStatus;
            _irCarrierFrequencySource = ircf;
            _volume = NuvoEssentiaCommand.limitVolume2NuvoEssentia(volume);
            _basslevel = basslevel;
            _treblelevel = treblelevel;
            _dipSwitchOverrideStatus = dipSwitchOverrideStatus;
            _volumeResetStatus = volumeResetStatus;
            _sourceGroupStatus = sourceGroupStatus;
            _firmwareVersion = firmwareVersion;
            _outgoingCommand = buildOutgoingCommand();
        }

        #endregion

        #region Incoming Command Constructors

        /// <summary>
        /// Constructor to build an incoming command.
        /// If the command has been succesfully parsed, the values can be asked via the member accessors, 
        /// like ZoneId, VolumeLevel, etc.
        /// </summary>
        /// <param name="commandString">Command string received from NuvoEssentia.</param>
        public NuvoEssentiaSingleCommand(string commandString)
        {
            constructMembers();
            ENuvoEssentiaCommands command = searchNuvoEssentiaCommandWithIncomingCommand(commandString);
            if (command != ENuvoEssentiaCommands.NoCommand)
            {
                // Incoming command found
                _incomingCommand = commandString;
                initMembers(command);
                parseIncomingCommand();
            }
            else
            {
                // Search for outgoing command
                command = searchNuvoEssentiaCommandWithOutgoingCommand(commandString);
                if (command != ENuvoEssentiaCommands.NoCommand)
                {
                    // Outgoing command found
                    _outgoingCommand = commandString;
                    initMembers(command);
                    parseCommand(_outgoingCommand,_outgoingCommandTemplate);
                }
            }
        }

        #endregion

        /// <summary>
        /// This private mathod instantiates the class members.
        /// (e.g. Logger, XML Profile reader, etc.)
        /// </summary>
        private void constructMembers()
        {
            _profileFilename = Properties.Settings.Default.NuvoEssentiaProfileFile; /* "NuvoEssentiaProfile.xml" */

            try
            {
                // If no filename is provided, it searches the defintions
                // of the common application xml file (e.g. nuvocontrol.server.protocoldriver.test.dll.config)
                _profile = new Xml( _profileFilename );
            }
            catch (Exception ex)
            {
                 // profile is not available
                _profile = null;
                throw ex;
            }
        }

        /// <summary>
        /// Private method to initialize the members.
        /// Call this method in each constructur at start.
        /// </summary>
        private void initMembers(ENuvoEssentiaCommands command)
        {
            _command = command;
            _createDateTime = DateTime.Now;
            _simpleId = SimpleId.NewGuid();

            for (int i = 0; i < _irCarrierFrequencySource.Length; i++)
            {
                _irCarrierFrequencySource[i] = EIRCarrierFrequency.IRUnknown;
            }

            // Load profile for this command
            _id = Convert.ToInt32((string)_profile.GetValue(command.ToString(), "Id"));
            _outgoingCommandTemplate = (string)_profile.GetValue(command.ToString(), "OutgoingCommand");
            _incomingCommandTemplate = (string)_profile.GetValue(command.ToString(), "IncomingCommand");
        }

        /// <summary>
        /// Searches in the profile the command passed as string the corresponding incoming template.
        /// </summary>
        /// <param name="commandString">Command (passed as string)</param>
        /// <returns>Enumeration of the found command. Returns NoCommand if command string isn't available.</returns>
        private static ENuvoEssentiaCommands searchNuvoEssentiaCommandWithIncomingCommand(string commandString)
        {
            Profile profile = new Xml(Properties.Settings.Default.NuvoEssentiaProfileFile);
            string[] sectionNames = profile.GetSectionNames();
            foreach (string section in sectionNames)
            {
                string incomingCommand = (string)profile.GetValue(section,"IncomingCommand");
                if (NuvoEssentiaSingleCommand.compareCommandString(incomingCommand, commandString))
                {
                    LogManager.GetCurrentClassLogger().Debug(m => m("Entry found: Command={0}, IncomingCommand={1}, Section={2}", commandString, incomingCommand, section));
                    return (ENuvoEssentiaCommands)Enum.Parse(typeof(ENuvoEssentiaCommands), section, true);
                }
            }
            // command not found
            return ENuvoEssentiaCommands.NoCommand;
        }

        /// <summary>
        /// Searches in the profile the command passed as string.
        /// This can either be an incomming- or outgoing-command.
        /// </summary>
        /// <param name="command">Command (passed as string)</param>
        /// <returns>Enumeration of the found command. Returns NoCommand if command string isn't available.</returns>
        public static ENuvoEssentiaCommands searchNuvoEssentiaCommandWithOutgoingCommand(string command)
        {
            Profile profile = new Xml(Properties.Settings.Default.NuvoEssentiaProfileFile);
            string[] sectionNames = profile.GetSectionNames();
            foreach (string section in sectionNames)
            {
                if (compareCommandString((string)profile.GetValue(section, "OutgoingCommand"), command))
                {
                    LogManager.GetCurrentClassLogger().Debug(m => m("Entry found: Command={0}, Section={1}", command, section));
                    return (ENuvoEssentiaCommands)Enum.Parse(typeof(ENuvoEssentiaCommands), section, true);
                }
            }
            // command not found
            return ENuvoEssentiaCommands.NoCommand;
        }

        /// <summary>
        /// Compares the received command string with a configured command string.
        /// The comparision takes into account, that ...
        /// - Placeholders in the configured command string are set with the specific value in the received command string
        /// - Depending on the zone status the received command string has another length then expected
        /// </summary>
        /// <param name="configuredCommand">Configured command string.</param>
        /// <param name="receivedCommand">Received command string.</param>
        /// <returns>True of the received command string is equal to the configured command string.</returns>
        public static bool compareCommandString(string configuredCommand, string receivedCommand)
        {
            // The strings are equal, when ...
            // a) The configured string is equal to the received string OR
            // b) The received string contains PWRON (=Power ON) and 
            //    the length of the received string is 1 smaller than the received string
            //    (This is because the configured string contains 3 placeholders, for either OFF or ON)
            // c) The format of the received string is equal to the received string
            //    (don't compare lower characters, because they are placeholders)

            // Check Length first ...
            if ((configuredCommand.Length == receivedCommand.Length) ||
                (receivedCommand.Contains("PWRON") && (configuredCommand.Length - 1 == receivedCommand.Length))
              )
            {
                // replace PWRON with PWR_ON, this allows future comparison on character level (both strings have the same size)
                receivedCommand = receivedCommand.Replace("PWRON", "PWRXON");

                // Compare each character
                for (int i = 0; i < receivedCommand.Length; i++)
                {
                    if ((receivedCommand[i] != configuredCommand[i]) &&
                        (!Char.IsLetter(configuredCommand[i]) || Char.IsUpper(configuredCommand[i])))
                    {
                        // Format missmatch
                        return false;
                    }
                }
            }
            else
            {
                // Size missmatch
                return false;
            }

            // Comparison ok
            return true;
        }


        /// <summary>
        /// Method to check the outgoing command. Are alle placeholders replaced?
        /// </summary>
        /// <param name="command">Command string to check</param>
        /// <returns>true, if all placeholders have been replaced. false, if not.</returns>
        private bool checkOutgoingCommand(string command)
        {
            foreach (char c in command)
            {
                if (Char.IsLetter(c) && Char.IsLower(c))
                {
                    // not all placeholders replaced.
                    return false;
                }
            }
            return true;
        }


        #region IComparable Members

        /// <summary>
        /// Method required for the interface IComparable.
        /// It compares the GUID. If the GUID is the same in both objects, thsi method returns true.
        /// </summary>
        /// <param name="obj">Object to compare to.</param>
        /// <returns>True, if the objects are identical.</returns>
        public int CompareTo(object obj)
        {
            string id1 = this._simpleId.ToString();
            string id2 = ((NuvoEssentiaSingleCommand)obj)._simpleId.ToString();
            return ( String.Compare(id1, id2) );
        }

        #endregion

        #region INuvoEssentiaCommand Members

        /// <summary>
        /// See base interface INuvoEssentiaSingleCommand for more information.
        /// </summary>
        public SimpleId Guid
        {
            get { return _simpleId; }
        }

        /// <summary>
        /// See base interface INuvoEssentiaSingleCommand for more information.
        /// </summary>
        public ENuvoEssentiaCommands Command
        {
            get { return _command; }
        }

        /// <summary>
        /// See base interface INuvoEssentiaSingleCommand for more information.
        /// </summary>
        public bool Valid
        {
            get { return _command != ENuvoEssentiaCommands.NoCommand; }
        }

        /// <summary>
        /// See base interface INuvoEssentiaSingleCommand for more information.
        /// </summary>
        public bool Finished
        {
            get { return _receiveDateTime.Year > 1; }
        }

        /// <summary>
        /// See base interface INuvoEssentiaSingleCommand for more information.
        /// </summary>
        public DateTime SendDateTime
        {
            get { return _sendDateTime; }
            set { _sendDateTime = value; }
        }

        /// <summary>
        /// See base interface INuvoEssentiaSingleCommand for more information.
        /// </summary>
        public DateTime ReceiveDateTime
        {
            get { return _receiveDateTime; }
        }

        /// <summary>
        /// See base interface INuvoEssentiaSingleCommand for more information.
        /// </summary>
        public DateTime CreatedDateTime
        {
            get { return _createDateTime; }
        }

        /// <summary>
        /// See base interface INuvoEssentiaSingleCommand for more information.
        /// </summary>
        public string OutgoingCommandTemplate
        {
            get { return _outgoingCommandTemplate; }
        }

        /// <summary>
        /// See base interface INuvoEssentiaSingleCommand for more information.
        /// </summary>
        public string IncomingCommandTemplate
        {
            get { return _incomingCommandTemplate; }
        }

        /// <summary>
        /// See base interface INuvoEssentiaSingleCommand for more information.
        /// </summary>
        public string OutgoingCommand
        {
            get { return _outgoingCommand; }
        }

        /// <summary>
        /// See base interface INuvoEssentiaSingleCommand for more information.
        /// </summary>
        public string IncomingCommand
        {
            get
            {
                return _incomingCommand;
            }
            set
            {
                _incomingCommand = value;
                _receiveDateTime = DateTime.Now;
                parseIncomingCommand();
            }
        }

        /// <summary>
        /// See base interface INuvoEssentiaSingleCommand for more information.
        /// </summary>
        public ENuvoEssentiaZones ZoneId
        {
            get { return _zoneId; }
        }

        /// <summary>
        /// See base interface INuvoEssentiaSingleCommand for more information.
        /// </summary>
        public ENuvoEssentiaSources SourceId
        {
            get { return _sourceId; }
        }

        /// <summary>
        /// See base interface INuvoEssentiaSingleCommand for more information.
        /// </summary>
        public EZonePowerStatus PowerStatus
        {
            get { return _powerStatus; }
        }

        /// <summary>
        /// See base interface INuvoEssentiaSingleCommand for more information.
        /// </summary>
        public EIRCarrierFrequency IrCarrierFrequencySource(ENuvoEssentiaSources source)
        {
            return _irCarrierFrequencySource[(int)source-1];
        }

        /// <summary>
        /// See base interface INuvoEssentiaSingleCommand for more information.
        /// </summary>
        public int VolumeLevel
        {
            get { return _volume; }
            set
            {
                _volume = NuvoEssentiaCommand.limitVolume2NuvoEssentia(value); 
                _outgoingCommand = buildOutgoingCommand();
            }
        }

        /// <summary>
        /// See base interface INuvoEssentiaSingleCommand for more information.
        /// </summary>
        public int BassLevel
        {
            get { return _basslevel; }
        }

        /// <summary>
        /// See base interface INuvoEssentiaSingleCommand for more information.
        /// </summary>
        public int TrebleLevel
        {
            get { return _treblelevel; }
        }

        /// <summary>
        /// See base interface INuvoEssentiaSingleCommand for more information.
        /// </summary>
        public EVolumeResetStatus VolumeResetStatus
        {
            get { return _volumeResetStatus; }
        }

        /// <summary>
        /// See base interface INuvoEssentiaSingleCommand for more information.
        /// </summary>
        public EDIPSwitchOverrideStatus DIPSwitchOverrideStatus
        {
            get { return _dipSwitchOverrideStatus; }
        }

        /// <summary>
        /// See base interface INuvoEssentiaSingleCommand for more information.
        /// </summary>
        public ESourceGroupStatus SourceGrupStatus
        {
            get { return _sourceGroupStatus; }
        }

        /// <summary>
        /// See base interface INuvoEssentiaSingleCommand for more information.
        /// </summary>
        public string FirmwareVersion
        {
            get { return _firmwareVersion; }
        }

        #endregion

        #region Command Replace Section

        /// <summary>
        /// Builds the outgoing command string, for this command.
        /// Replaces all placeholders with its corresponding values. 
        /// </summary>
        /// <returns>Outgoing command string.</returns>
        private string buildOutgoingCommand()
        {
            string outgoingCommand="";
            if (_command != ENuvoEssentiaCommands.NoCommand)
            {
                switch (_command)
                {
                    // commands without parameter
                    case ENuvoEssentiaCommands.ReadStatusSOURCEIR:
                    case ENuvoEssentiaCommands.RestoreDefaultSOURCEIR:
                    case ENuvoEssentiaCommands.TurnALLZoneOFF:
                    case ENuvoEssentiaCommands.RampVolumeALLZoneDOWN:
                    case ENuvoEssentiaCommands.RampVolumeALLZoneUP:
                    case ENuvoEssentiaCommands.StopRampVolumeALLZone:
                    case ENuvoEssentiaCommands.MuteALLZoneOFF:
                    case ENuvoEssentiaCommands.MuteALLZoneON:
                    case ENuvoEssentiaCommands.ReadVersion:
                        // No replacement required (command has not parameters)
                        outgoingCommand = _outgoingCommandTemplate;
                        break;

                    // commands with zone parameter only
                    case ENuvoEssentiaCommands.ReadStatusCONNECT:
                    case ENuvoEssentiaCommands.ReadStatusZONE:
                    case ENuvoEssentiaCommands.TurnZoneON:
                    case ENuvoEssentiaCommands.TurnZoneOFF:
                    case ENuvoEssentiaCommands.RampVolumeDOWN:
                    case ENuvoEssentiaCommands.RampVolumeUP:
                    case ENuvoEssentiaCommands.StopRampVolume:
                    case ENuvoEssentiaCommands.MuteOFF:
                    case ENuvoEssentiaCommands.MuteON:
                    case ENuvoEssentiaCommands.SetSourceGroupOFF:
                    case ENuvoEssentiaCommands.SetSourceGroupON:
                    case ENuvoEssentiaCommands.SetVolumeResetOFF:
                    case ENuvoEssentiaCommands.SetVolumeResetON:
                    case ENuvoEssentiaCommands.SetKeypadLockOFF:
                    case ENuvoEssentiaCommands.SetKeypadLockON:
                        // replace zone placeholder
                        outgoingCommand = replacePlaceholderForZone(_outgoingCommandTemplate, _zoneId);
                        break;

                    // commands with source parameter
                    case ENuvoEssentiaCommands.SetSOURCEIR38:
                    case ENuvoEssentiaCommands.SetSOURCEIR56:
                        // replace source placeholder
                        outgoingCommand = replacePlaceholderForSource(outgoingCommand, _sourceId);
                        break;

                    // commands with zone and source parameter 
                    case ENuvoEssentiaCommands.SetSource:
                        // replace zone placeholder
                        outgoingCommand = replacePlaceholderForZone(_outgoingCommandTemplate, _zoneId);
                        // replace source placeholder
                        outgoingCommand = replacePlaceholderForSource(outgoingCommand, _sourceId);
                        break;

                    // commands with zone and volume parameter 
                    case ENuvoEssentiaCommands.SetVolume:
                        // replace zone placeholder
                        outgoingCommand = replacePlaceholderForZone(_outgoingCommandTemplate, _zoneId);
                        // replace volume level
                        outgoingCommand = replacePlaceholderWithVolumeLevel(outgoingCommand, _volume);
                        break;

                    // commands with zone and bass parameter 
                    case ENuvoEssentiaCommands.SetBassLevel:
                        // replace zone placeholder
                        outgoingCommand = replacePlaceholderForZone(_outgoingCommandTemplate, _zoneId);
                        // replace bass level
                        outgoingCommand = replacePlaceholderWithBassLevel(outgoingCommand, _basslevel);
                        break;

                    // commands with zone and treble parameter 
                    case ENuvoEssentiaCommands.SetTrebleLevel:
                        // replace zone placeholder
                        outgoingCommand = replacePlaceholderForZone(_outgoingCommandTemplate, _zoneId);
                        // replace treble level
                        outgoingCommand = replacePlaceholderWithTrebleLevel(outgoingCommand, _treblelevel);
                        break;

                    // incoming command ONLY
                    case ENuvoEssentiaCommands.ExternalMuteActivated:
                    case ENuvoEssentiaCommands.ExternalMuteDeactivated:
                    case ENuvoEssentiaCommands.ErrorInCommand:
                        outgoingCommand = "";
                        _log.Error(m => m("Error, cannot build outgoing command string for the command '{0}'!", _command));
                        break;

                    // combined commands -> not handled by the single command class
                    case ENuvoEssentiaCommands.SetZoneStatus:
                    case ENuvoEssentiaCommands.GetZoneStatus:
                        throw new ProtocolDriverException(string.Format("The command '{0}' is not handled by this single command class. Use the container class instead!", _command));

                    // unkown command 
                    default:
                        // replace all knwon replacements so far
                        outgoingCommand = replacePlaceholders(_outgoingCommandTemplate,_zoneId,_sourceId,_volume,_basslevel,_treblelevel,_powerStatus,_irCarrierFrequencySource);
                        _log.Warn(m => m("Warning, for this command '{0}' may not all required replacements implemented!", _command));
                        break;
                }
            }
            else
            {
                _log.Warn(m => m("Cannot build outgoing command, because command is unkown!"));
            }

            if (!checkOutgoingCommand(outgoingCommand))
            {
                string message = string.Format("Not all placeholders have been replaced, for the command '{0}'", _command );
                _log.Warn(m => m(message));
                throw new ProtocolDriverException(message);
            }

            return outgoingCommand;
        }

        /// <summary>
        /// Replaces the placeholders in the input command with its corresponding
        /// values. Returns a string containing the values.
        /// This method executes all known replacements.
        /// </summary>
        /// <param name="command">Command string with placeholders</param>
        /// <param name="zoneId">Zone Id, to replace in template command string.</param>
        /// <param name="sourceId">Source Id, to replace in template command string.</param>
        /// <param name="volume">Volume Level, to replace in template command string.</param>
        /// <param name="bassLevel">Bass Level, to replace in template command string.</param>
        /// <param name="trebleLevel">Treble Level, to replace in template command string.</param>
        /// <param name="powerStatus">Power Status, to replace in template command string.</param>
        /// <param name="irCarrierFrequencySource">Array of IR Carrier Frequency, to replace in template command string.</param>
        /// <returns>Result string, placeholders replaced with values.</returns>
        static public string replacePlaceholders(string command, ENuvoEssentiaZones zoneId, ENuvoEssentiaSources sourceId, int volume, int bassLevel, int trebleLevel, EZonePowerStatus powerStatus, EIRCarrierFrequency[] irCarrierFrequencySource)
        {
            if (command != null)
            {
                command = replacePlaceholderForZone(command, zoneId);
                command = replacePlaceholderForSource(command, sourceId);
                command = replacePlaceholderWithVolumeLevel(command, volume);
                command = replacePlaceholderWithBassLevel(command, bassLevel);
                command = replacePlaceholderWithTrebleLevel(command, trebleLevel);
                command = replacePlaceholderForPowerStatus(command, powerStatus);
                command = replacePlaceholderForIRFrequency(command, irCarrierFrequencySource);
            }
            return command;
        }

        /// <summary>
        /// Replaces the placeholders in the input command with its corresponding
        /// values. Returns a string containing the values.
        /// This method executes all known replacements.
        /// </summary>
        /// <param name="command">Template command string with placeholders</param>
        /// <param name="zoneId">Zone Id, to replace in template command string.</param>
        /// <param name="sourceId">Source Id, to replace in template command string.</param>
        /// <param name="volume">Volume Level, to replace in template command string.</param>
        /// <param name="bassLevel">Bass Level, to replace in template command string.</param>
        /// <param name="trebleLevel">Treble Level, to replace in template command string.</param>
        /// <param name="powerStatus">Power Status, to replace in template command string.</param>
        /// <param name="irCarrierFrequencySource">Array of IR Carrier Frequency, to replace in template command string.</param>
        /// <param name="sourceGroupStatus">Source Group Status, to replace in template command string.</param>
        /// <param name="volumeResetStatus">Volume Reset Status, to replace in template command string.</param>
        /// <param name="dipSwitchOverrideStatus">DIP Switch Overrids Status, to replace in template command string.</param>
        /// <param name="version">Version, to replace in template command string.</param>
        /// <returns>Result string, placeholders replaced with values.</returns>
        static public string replacePlaceholders(string command, 
            ENuvoEssentiaZones zoneId, ENuvoEssentiaSources sourceId, int volume, int bassLevel, int trebleLevel, 
            EZonePowerStatus powerStatus, EIRCarrierFrequency[] irCarrierFrequencySource,
            ESourceGroupStatus sourceGroupStatus, EVolumeResetStatus volumeResetStatus, EDIPSwitchOverrideStatus dipSwitchOverrideStatus, string version)
        {
            if (command != null)
            {
                command = replacePlaceholderForZone(command, zoneId);
                command = replacePlaceholderForSource(command, sourceId);
                command = replacePlaceholderWithVolumeLevel(command, volume);
                command = replacePlaceholderWithBassLevel(command, bassLevel);
                command = replacePlaceholderWithTrebleLevel(command, trebleLevel);
                command = replacePlaceholderForPowerStatus(command, powerStatus);
                command = replacePlaceholderForIRFrequency(command, irCarrierFrequencySource);

                command = replacePlaceholderForSourceGroupStatus(command, sourceGroupStatus);
                command = replacePlaceholderForVolumeResetStatus(command, volumeResetStatus);
                command = replacePlaceholderForDIPSwitchOverrideStatus(command, dipSwitchOverrideStatus);
                command = replacePlaceholderForVersion(command, version);
            }
            return command;
        }

        /// <summary>
        /// Replaces the default volume level placeholder with the volume level in the command string.
        /// </summary>
        /// <param name="command">Command string</param>
        /// <param name="volume">Volume level</param>
        /// <returns>Command string with replaced placeholders.</returns>
        static private string replacePlaceholderWithVolumeLevel(string command, int volume)
        {
            return replacePlaceholderWithVolumeLevel(command, volume, "yy");
        }

        /// <summary>
        /// Replaces the volume level placeholder with the volume level in the command string.
        /// </summary>
        /// <param name="command">Command string</param>
        /// <param name="volume">Volume level</param>
        /// <param name="placeholder">Placeholder for the volume level</param>
        /// <returns>Command string with replaced placeholders.</returns>
        static private string replacePlaceholderWithVolumeLevel(string command, int volume, string placeholder)
        {
            // Ignore 'minus' sign.
            volume = Math.Abs(volume);
            if (command.Contains(placeholder))
            {
                // level below max in dB: -00 to -79 dB (include lead 0 for all single-digit values)
                if (volume >= 0 && volume <= 79)
                {
                    command = replacePlaceholderWithNumber(command, volume, placeholder);
                }
                else
                {
                    LogManager.GetCurrentClassLogger().Warn(m => m("Replace ERROR: Cannot replace '{0}' in command '{1}', because volume level '{2}' is not in allowed range.", placeholder, command, volume));
                }
            }
            return command;
        }

        /// <summary>
        /// Replaces the default bass placeholder with the level in the command string.
        /// </summary>
        /// <param name="command">Command string</param>
        /// <param name="level">Bass level</param>
        /// <returns>Command string with replaced placeholders.</returns>
        static private string replacePlaceholderWithBassLevel(string command, int level)
        {
            return replacePlaceholderWithBassTrebleLevel(command, level, "uuu");
        }

        /// <summary>
        /// Replaces the default treble placeholder with the level in the command string.
        /// </summary>
        /// <param name="command">Command string</param>
        /// <param name="level">Treble level</param>
        /// <returns>Command string with replaced placeholders.</returns>
        static private string replacePlaceholderWithTrebleLevel(string command, int level)
        {
            return replacePlaceholderWithBassTrebleLevel(command, level, "ttt");
        }

        /// <summary>
        /// Replaces the bass or treble placeholder with the level in the command string.
        /// </summary>
        /// <param name="command">Command string</param>
        /// <param name="level">Bass or treble level</param>
        /// <param name="placeholder">Placeholder for the bass or treble level</param>
        /// <returns>Command string with replaced placeholders.</returns>
        static private string replacePlaceholderWithBassTrebleLevel(string command, int level, string placeholder)
        {
            if (command.Contains(placeholder))
            {
                // The documentation of Essentia is not clear, it states two value ranges !!!!!
                // A) EQ level, dB, –8 to +0 (flat) to +8 in 1 dB increments
                // B) EQ level, dB, –12 to +0 (flat) to +12 in 2 dB increments. USE LEAD "0" IN TENS PLACE FOR VALUE LESS THAN 10.
                if (level >= -12 && level <= 12)
                {
                    command = replacePlaceholderWithNumberConsideringPlusMinus(command, level, placeholder);
                }
                else
                {
                    LogManager.GetCurrentClassLogger().Warn(m => m("Replace ERROR: Cannot replace '{0}' in command '{1}', because bass or treble level '{2}' is not in allowed range.", placeholder, command, level));
                }
            }
            return command;
        }

        /// <summary>
        /// Replaces the default power status placeholder with the power status in the command string.
        /// </summary>
        /// <param name="command">Command string</param>
        /// <param name="zonepwrstatus">Zone power status</param>
        /// <returns>Command string with replaced placeholders.</returns>
        static private string replacePlaceholderForPowerStatus(string command, EZonePowerStatus zonepwrstatus)
        {
            return replacePlaceholderForPowerStatus(command, zonepwrstatus, "ppp");
        }

        /// <summary>
        /// Replaces the power status placeholder with the power status in the command string.
        /// </summary>
        /// <param name="command">Command string</param>
        /// <param name="zonepwrstatus">Zone power status</param>
        /// <param name="placeholder">Placeholder for the zone id</param>
        /// <returns>Command string with replaced placeholders.</returns>
        static private string replacePlaceholderForPowerStatus(string command, EZonePowerStatus zonepwrstatus, string placeholder)
        {
            if (command.Contains(placeholder))
            {
                if (zonepwrstatus != EZonePowerStatus.ZoneStatusUnknown)
                {
                    command = command.Replace(placeholder, (zonepwrstatus == EZonePowerStatus.ZoneStatusON ? "ON" : "OFF"));
                }
                else
                {
                    LogManager.GetCurrentClassLogger().Warn(m => m("Replace ERROR: Cannot replace '{0}' in command '{1}', because Zone status member is not set", placeholder, command));
                }
            }
            return command;
        }

        /// <summary>
        /// Replaces the default zone placeholder with the zone id in the command string.
        /// </summary>
        /// <param name="command">Command string</param>
        /// <param name="zone">Zone Id</param>
        /// <returns>Command string with replaced placeholders.</returns>
        static private string replacePlaceholderForZone(string command, ENuvoEssentiaZones zone)
        {
            return replacePlaceholderForZone(command, zone, "xx");
        }

        /// <summary>
        /// Replaces the zone placeholder with the zone id in the command string.
        /// </summary>
        /// <param name="command">Command string</param>
        /// <param name="zone">Zone Id</param>
        /// <param name="placeholder">Placeholder for the zone id</param>
        /// <returns>Command string with replaced placeholders.</returns>
        static private string replacePlaceholderForZone(string command, ENuvoEssentiaZones zone, string placeholder)
        {
            if (command.Contains(placeholder))
            {
                if (zone != ENuvoEssentiaZones.NoZone)
                {
                    command = replacePlaceholderWithNumber(command, (int)zone, placeholder);
                }
                else
                {
                    LogManager.GetCurrentClassLogger().Warn(m => m("Replace ERROR: Cannot replace '{0}' in command '{1}', because Zone member is not set", placeholder, command));
                }
            }
            return command;
        }

        /// <summary>
        /// Replaces the default IR carrier frequency placeholder with the IR carrier frequency (=ircf) in the command string.
        /// </summary>
        /// <param name="command">Command string</param>
        /// <param name="ircf">IR Carrier Frequency, either 38kHz or 55kHz</param>
        /// <returns>Command string with replaced placeholders.</returns>
        static private string replacePlaceholderForIRFrequency(string command, EIRCarrierFrequency[] ircf)
        {
            command = replacePlaceholderForIRFrequency(command, (ircf.Length >= 1 ? ircf[0] : EIRCarrierFrequency.IRUnknown), "aa");    // Source 1
            command = replacePlaceholderForIRFrequency(command, (ircf.Length >= 2 ? ircf[1] : EIRCarrierFrequency.IRUnknown), "bb");    // Source 2
            command = replacePlaceholderForIRFrequency(command, (ircf.Length >= 3 ? ircf[2] : EIRCarrierFrequency.IRUnknown), "cc");    // Source 3
            command = replacePlaceholderForIRFrequency(command, (ircf.Length >= 4 ? ircf[3] : EIRCarrierFrequency.IRUnknown), "dd");    // Source 4
            command = replacePlaceholderForIRFrequency(command, (ircf.Length >= 5 ? ircf[4] : EIRCarrierFrequency.IRUnknown), "ee");    // Source 5
            command = replacePlaceholderForIRFrequency(command, (ircf.Length >= 6 ? ircf[5] : EIRCarrierFrequency.IRUnknown), "ff");    // Source 6
            return command;
        }

        /// <summary>
        /// Replaces the IR carrier frequency placeholder with the IR carrier frequency (=ircf) in the command string.
        /// </summary>
        /// <param name="command">Command string</param>
        /// <param name="ircf">IR Carrier Frequency, either 38kHz or 55kHz</param>
        /// <param name="placeholder">Placeholder for the IR carrier frequency</param>
        /// <returns>Command string with replaced placeholders.</returns>
        static private string replacePlaceholderForIRFrequency(string command, EIRCarrierFrequency ircf, string placeholder)
        {
            if (command.Contains(placeholder))
            {
                if (ircf != EIRCarrierFrequency.IRUnknown)
                {
                    command = command.Replace(placeholder, (ircf == EIRCarrierFrequency.IR38kHz ? "38" : "56"));
                }
                else
                {
                    LogManager.GetCurrentClassLogger().Warn(m => m("Replace ERROR: Cannot replace '{0}' in command '{1}', because IR Carrier Frequency member is not set", placeholder, command));
                }
            }
            return command;
        }

        /// <summary>
        /// Replaces the default source placeholder with the source id in the command string.
        /// </summary>
        /// <param name="command">Command string</param>
        /// <param name="source">Source Id</param>
        /// <returns>Command string with replaced placeholders.</returns>
        static private string replacePlaceholderForSource(string command, ENuvoEssentiaSources source)
        {
            return replacePlaceholderForSource(command, source, "s");
        }

        /// <summary>
        /// Replaces the source placeholder with the source id in the command string.
        /// </summary>
        /// <param name="command">Command string</param>
        /// <param name="source">Source Id</param>
        /// <param name="placeholder">Placeholder for the source id</param>
        /// <returns>Command string with replaced placeholders.</returns>
        static private string replacePlaceholderForSource(string command, ENuvoEssentiaSources source, string placeholder)
        {
            if (command.Contains(placeholder))
            {
                if (source != ENuvoEssentiaSources.NoSource)
                {
                    command = replacePlaceholderWithNumber(command, (int)source, placeholder);
                }
                else
                {
                    LogManager.GetCurrentClassLogger().Error(m => m("Replace ERROR: Cannot replace '{0}' in command '{1}', because Source member is not set", placeholder, command));
                }
            }
            return command;
        }

        /// <summary>
        /// This static method replaces in the command string the occurences of the placeholder.
        /// It replaces it with the number, passed with num. It fills it up with leading 
        /// zeros if required.
        /// If the number is negative an addtional - (minus) sign is added to the output
        /// command string. If this is not required, use the method replacePlaceholderWithNumberConsideringPlusMinus
        /// instead.
        /// </summary>
        /// <param name="command">Command where to replace the placeholder.</param>
        /// <param name="num">Number which will be put into command.</param>
        /// <param name="placeholder">Placeholder string, this pattern is replaced in the command string.</param>
        /// <returns>Command string with replaced placeholders.</returns>
        static private string replacePlaceholderWithNumber(string command, int num, string placeholder)
        {
            // build format string
            string format = string.Format("{{0,{0}:D{1}}}",placeholder.Length,placeholder.Length);
            // replace placeholder
            return command.Replace(placeholder, (string.Format(format, num)));
        }

        /// <summary>
        /// This static method replaces in the command string the occurences of the placeholder.
        /// It replaces it with the number, passed with num. It fills it up with leading 
        /// zeros if required. In the first place is places a + (plus) or - (minus) sign,
        /// according to the number.
        /// </summary>
        /// <param name="command">Command where to replace the placeholder.</param>
        /// <param name="num">Number which will be put into command.</param>
        /// <param name="placeholder">Placeholder string, this pattern is replaced in the command string.</param>
        /// <returns>Command string with replaced placeholders.</returns>
        static private string replacePlaceholderWithNumberConsideringPlusMinus(string command, int num, string placeholder)
        {
            // build format string
            string format = string.Format("{0}{{0,{1}:D{2}}}",(num<0?"":"+"), placeholder.Length-1, placeholder.Length-1);
            // replace placeholder
            return command.Replace(placeholder, (string.Format(format, num)));
        }

        /// <summary>
        /// Replaces the default source group status placeholder with the source group status in the command string.
        /// </summary>
        /// <param name="command">Command string</param>
        /// <param name="sourceGroupStatus">Source Group Status</param>
        /// <returns>Command string with replaced placeholders.</returns>
        static private string replacePlaceholderForSourceGroupStatus(string command, ESourceGroupStatus sourceGroupStatus)
        {
            return replacePlaceholderForSourceGroupStatus(command, sourceGroupStatus, "q");
        }

        /// <summary>
        /// Replaces the source group status placeholder with the source group status in the command string.
        /// </summary>
        /// <param name="command">Command string</param>
        /// <param name="sourceGroupStatus">source group status</param>
        /// <param name="placeholder">Placeholder for the source group status</param>
        /// <returns>Command string with replaced placeholders.</returns>
        static private string replacePlaceholderForSourceGroupStatus(string command, ESourceGroupStatus sourceGroupStatus, string placeholder)
        {
            if (command.Contains(placeholder))
            {
                if (sourceGroupStatus != ESourceGroupStatus.SourceGroupUnknown)
                {
                    command = replacePlaceholderWithNumber(command, (int)sourceGroupStatus, placeholder);
                }
                else
                {
                    LogManager.GetCurrentClassLogger().Error(m => m("Replace ERROR: Cannot replace '{0}' in command '{1}', because Source Group Member Status member is not set", placeholder, command));
                }
            }
            return command;
        }

        /// <summary>
        /// Replaces the default volume reset status placeholder with the volume reset status in the command string.
        /// </summary>
        /// <param name="command">Command string</param>
        /// <param name="volumeResetStatus">Volume Reset Status</param>
        /// <returns>Command string with replaced placeholders.</returns>
        static private string replacePlaceholderForVolumeResetStatus(string command, EVolumeResetStatus volumeResetStatus)
        {
            return replacePlaceholderForVolumeResetStatus(command, volumeResetStatus, "r");
        }

        /// <summary>
        /// Replaces the volume reset status placeholder with the volume reset status in the command string.
        /// </summary>
        /// <param name="command">Command string</param>
        /// <param name="volumeResetStatus">volume reset status</param>
        /// <param name="placeholder">Placeholder for the volume reset status</param>
        /// <returns>Command string with replaced placeholders.</returns>
        static private string replacePlaceholderForVolumeResetStatus(string command, EVolumeResetStatus volumeResetStatus, string placeholder)
        {
            if (command.Contains(placeholder))
            {
                if (volumeResetStatus != EVolumeResetStatus.VolumeResetUnknown)
                {
                    command = replacePlaceholderWithNumber(command, (int)volumeResetStatus, placeholder);
                }
                else
                {
                    LogManager.GetCurrentClassLogger().Error(m => m("Replace ERROR: Cannot replace '{0}' in command '{1}', because Volume Reset Status Member member is not set", placeholder, command));
                }
            }
            return command;
        }

        /// <summary>
        /// Replaces the default DIP Switch Override Status placeholder with the DIP Switch Override Status in the command string.
        /// </summary>
        /// <param name="command">Command string</param>
        /// <param name="dipSwitchOverrideStatus">DIP Switch Override Status</param>
        /// <returns>Command string with replaced placeholders.</returns>
        static private string replacePlaceholderForDIPSwitchOverrideStatus(string command, EDIPSwitchOverrideStatus dipSwitchOverrideStatus)
        {
            return replacePlaceholderForDIPSwitchOverrideStatus(command, dipSwitchOverrideStatus, "i");
        }

        /// <summary>
        /// Replaces the DIP Switch Override Status placeholder with the DIP Switch Override Status in the command string.
        /// </summary>
        /// <param name="command">Command string</param>
        /// <param name="dipSwitchOverrideStatus">DIP Switch Override Status</param>
        /// <param name="placeholder">Placeholder for the DIP Switch Override Status</param>
        /// <returns>Command string with replaced placeholders.</returns>
        static private string replacePlaceholderForDIPSwitchOverrideStatus(string command, EDIPSwitchOverrideStatus dipSwitchOverrideStatus, string placeholder)
        {
            if (command.Contains(placeholder))
            {
                if (dipSwitchOverrideStatus != EDIPSwitchOverrideStatus.DIPSwitchOverrideUnknown)
                {
                    command = replacePlaceholderWithNumber(command, (int)dipSwitchOverrideStatus, placeholder);
                }
                else
                {
                    LogManager.GetCurrentClassLogger().Error(m => m("Replace ERROR: Cannot replace '{0}' in command '{1}', because DIP Switch Override Status Member member is not set", placeholder, command));
                }
            }
            return command;
        }

        /// <summary>
        /// Replaces the version in the command string.
        /// </summary>
        /// <param name="command">Command string</param>
        /// <param name="version">Version string</param>
        /// <returns>Command string with replaced placeholders.</returns>
        static private string replacePlaceholderForVersion(string command, string version)
        {
            return command.Replace("vz.zz", version );
        }


        #endregion

        #region Command Parse Section


        /// <summary>
        /// Parses the incoming command string and extracts its values.
        /// It uses only the incoming command template, this allows to optimze
        /// the parse algorithm. We call only the parse methods which are part
        /// of the command.
        /// On the other hand the method parseCommand() calls all possible
        /// parse methods independent of the underlying command.
        /// </summary>
        private void parseIncomingCommand()
        {
            if (_incomingCommand != "")
            {
                if (_command != ENuvoEssentiaCommands.NoCommand)
                {
                    switch (_command)
                    {
                        // commands with IR carrie frequency parameter
                        case ENuvoEssentiaCommands.ReadStatusSOURCEIR:
                        case ENuvoEssentiaCommands.RestoreDefaultSOURCEIR:
                        case ENuvoEssentiaCommands.SetSOURCEIR38:
                        case ENuvoEssentiaCommands.SetSOURCEIR56:
                            parseCommandForALLIRCarrierFrequency(_incomingCommand,_incomingCommandTemplate);
                            break;

                        // commands without parameter
                        case ENuvoEssentiaCommands.TurnALLZoneOFF:
                        case ENuvoEssentiaCommands.RampVolumeALLZoneDOWN:
                        case ENuvoEssentiaCommands.RampVolumeALLZoneUP:
                        case ENuvoEssentiaCommands.StopRampVolumeALLZone:
                        case ENuvoEssentiaCommands.MuteALLZoneOFF:
                        case ENuvoEssentiaCommands.MuteALLZoneON:
                            // No extraction (incoming command has not parameters)
                            break;

                        // commands with zone parameter only
                        case ENuvoEssentiaCommands.StopRampVolume:
                        case ENuvoEssentiaCommands.SetKeypadLockOFF:
                        case ENuvoEssentiaCommands.SetKeypadLockON:
                            // extract zone id
                            _zoneId = parseCommandForZone(_incomingCommand,_incomingCommandTemplate);
                            break;

                        // commands with zone, Power Status, Source, Source Group, Volume Level
                        case ENuvoEssentiaCommands.ReadStatusCONNECT:
                        case ENuvoEssentiaCommands.TurnZoneON:
                        case ENuvoEssentiaCommands.TurnZoneOFF:
                        case ENuvoEssentiaCommands.SetSource:
                        case ENuvoEssentiaCommands.SetVolume:
                        case ENuvoEssentiaCommands.RampVolumeDOWN:
                        case ENuvoEssentiaCommands.RampVolumeUP:
                        case ENuvoEssentiaCommands.MuteOFF:
                        case ENuvoEssentiaCommands.MuteON:
                            // extract zone id
                            _zoneId = parseCommandForZone(_incomingCommand,_incomingCommandTemplate);
                            // extract power status
                            _powerStatus = parseCommandForPowerStatus(_incomingCommand,_incomingCommandTemplate);
                            // extract source placeholder
                            _sourceId = parseCommandForSource(_incomingCommand,_incomingCommandTemplate);
                            // extract source group status
                            _sourceGroupStatus = parseCommandForSourceGroupStatus(_incomingCommand,_incomingCommandTemplate);
                            // extract volume level
                            _volume = parseCommandForVolumeLevel(_incomingCommand,_incomingCommandTemplate);
                            break;

                        // commands with zone, DIP Switch Override, Bass-, Treble Level, Source Group, Volume Reset
                        case ENuvoEssentiaCommands.ReadStatusZONE:
                        case ENuvoEssentiaCommands.SetBassLevel:
                        case ENuvoEssentiaCommands.SetTrebleLevel:
                        case ENuvoEssentiaCommands.SetSourceGroupOFF:
                        case ENuvoEssentiaCommands.SetSourceGroupON:
                        case ENuvoEssentiaCommands.SetVolumeResetOFF:
                        case ENuvoEssentiaCommands.SetVolumeResetON:
                            // extract zone id
                            _zoneId = parseCommandForZone(_incomingCommand,_incomingCommandTemplate);
                            // extract DIP Switch Override Status
                            _dipSwitchOverrideStatus = parseCommandForDIPSwitchOverrideStatus(_incomingCommand,_incomingCommandTemplate);
                            // extract bass level
                            _basslevel = parseCommandForBassLevel(_incomingCommand,_incomingCommandTemplate);
                            // extract treble level
                            _treblelevel = parseCommandForTrebleLevel(_incomingCommand,_incomingCommandTemplate);
                            // extract source group status
                            _sourceGroupStatus = parseCommandForSourceGroupStatus(_incomingCommand,_incomingCommandTemplate);
                            // extract Volume Reset Status
                            _volumeResetStatus = parseCommandForVolumeResetStatus(_incomingCommand,_incomingCommandTemplate);
                            break;

                        // read firmware version
                        case ENuvoEssentiaCommands.ReadVersion:
                            _firmwareVersion = parseCommandForFirmwareVersion(_incomingCommand,_incomingCommandTemplate);
                            break;

                        // incoming command ONLY
                        case ENuvoEssentiaCommands.ExternalMuteActivated:
                        case ENuvoEssentiaCommands.ExternalMuteDeactivated:
                            // No parsing (incoming command has not parameters)
                            _log.Debug(m => m("External mute has changed '{0}'!", _command));
                            break;

                        case ENuvoEssentiaCommands.ErrorInCommand:
                            _log.Error(m => m("Error, received error command '{0}' from Nuvo Essentia!", _command));
                            break;

                        // unkown command 
                        default:
                            // replace all knwon replacements so far
                            parseCommand(_incomingCommand,_incomingCommandTemplate);
                            _log.Warn(m => m("Warning, for this command '{0}' may not all required parsers are implemented!", _command));
                            break;
                    }
                }
                else
                {
                    _log.Warn(m => m("Cannot parse incoming command, because the underlying command is unkown! (_incomingCommand={0})", _incomingCommand));
                }
            }
            else
            {
                _log.Warn(m => m("Cannot parse incoming command, because command is empty!"));
            }
        }


        /// <summary>
        /// Replaces the placeholders in the input command with its corresponding
        /// values. Returns a string containing the values.
        /// This method executes all known replacements.
        /// </summary>
        /// <param name="commandString">Command string with placeholders</param>
        /// <param name="commandStringTemplate">Command Template, expected for this command.</param>
        private void parseCommand(string commandString, string commandStringTemplate)
        {
            _zoneId = parseCommandForZone(commandString, commandStringTemplate);
            _sourceId = parseCommandForSource(commandString, commandStringTemplate);
            _powerStatus = parseCommandForPowerStatus(commandString, commandStringTemplate);
            _volume = parseCommandForVolumeLevel(commandString, commandStringTemplate);
            _basslevel = parseCommandForBassLevel(commandString, commandStringTemplate);
            _treblelevel = parseCommandForTrebleLevel(commandString, commandStringTemplate);
            _firmwareVersion = parseCommandForFirmwareVersion(commandString, commandStringTemplate);
            _volumeResetStatus = parseCommandForVolumeResetStatus(commandString, commandStringTemplate);
            _sourceGroupStatus = parseCommandForSourceGroupStatus(commandString, commandStringTemplate);
            _dipSwitchOverrideStatus = parseCommandForDIPSwitchOverrideStatus(commandString, commandStringTemplate);
            parseCommandForALLIRCarrierFrequency(commandString, commandStringTemplate);
        }

        /// <summary>
        /// Extracts all IR Carrier Frequency out of the recieved command string.
        /// The member _incomingCommandTemplate needs to be set prior.
        /// </summary>
        /// <param name="commandString">Command string received from Nuvo Essentia.</param>
        /// <param name="commandStringTemplate">Command Template, expected for this command.</param>
        private void parseCommandForALLIRCarrierFrequency(string commandString, string commandStringTemplate)
        {
            _irCarrierFrequencySource[0] = parseCommandForIRCarrierFrequency(commandString, commandStringTemplate, "aa");
            _irCarrierFrequencySource[1] = parseCommandForIRCarrierFrequency(commandString, commandStringTemplate, "bb");
            _irCarrierFrequencySource[2] = parseCommandForIRCarrierFrequency(commandString, commandStringTemplate, "cc");
            _irCarrierFrequencySource[3] = parseCommandForIRCarrierFrequency(commandString, commandStringTemplate, "dd");
            _irCarrierFrequencySource[4] = parseCommandForIRCarrierFrequency(commandString, commandStringTemplate, "ee");
            _irCarrierFrequencySource[5] = parseCommandForIRCarrierFrequency(commandString, commandStringTemplate, "ff");
        }

        /// <summary>
        /// Extracts the IR Carrier Frequency out of the recieved command string,
        /// using the passed pattern.
        /// The member _incomingCommandTemplate needs to be set prior.
        /// </summary>
        /// <param name="commandString">Command string received from Nuvo Essentia.</param>
        /// <param name="commandStringTemplate">Command Template, expected for this command.</param>
        /// <param name="placeholder">Pattern to search for in the command template.</param>
        /// <returns>IR Carrier Frequency, extracted out of the command string.</returns>
        static private EIRCarrierFrequency parseCommandForIRCarrierFrequency(string commandString, string commandStringTemplate, string placeholder)
        {
            string stringIRCarrierFrequency = parseCommand(commandString, commandStringTemplate, placeholder);
            EIRCarrierFrequency irCarrierFrequency;
            switch (stringIRCarrierFrequency)
            {
                case "38":
                    irCarrierFrequency = EIRCarrierFrequency.IR38kHz;
                    break;
                case "56":
                    irCarrierFrequency = EIRCarrierFrequency.IR56kHz;
                    break;
                case "":
                    irCarrierFrequency = EIRCarrierFrequency.IRUnknown;
                    break;
                default:
                    LogManager.GetCurrentClassLogger().Fatal(m => m("Parse EXCEPTION: Cannot parse IR Carrier Frequency. Wrong command '{0}' received!", stringIRCarrierFrequency));
                    irCarrierFrequency = EIRCarrierFrequency.IRUnknown;
                    break;
            }
            return irCarrierFrequency;
        }

        /// <summary>
        /// Extracts the zone id out of the recieved command string.
        /// The member _incomingCommandTemplate needs to be set prior.
        /// </summary>
        /// <param name="commandString">Command string received from Nuvo Essentia.</param>
        /// <param name="commandStringTemplate">Command Template, expected for this command.</param>
        /// <returns>Zone id, extracted out of the command string.</returns>
        static private ENuvoEssentiaZones parseCommandForZone(string commandString, string commandStringTemplate)
        {
            string stringZone = parseCommand(commandString, commandStringTemplate, "xx");
            if (stringZone != "")
            {
                try
                {
                    return (ENuvoEssentiaZones)Enum.Parse(typeof(ENuvoEssentiaZones), stringZone, true);
                }
                catch (System.ArgumentException ex)
                {
                    LogManager.GetCurrentClassLogger().Fatal(m => m("Parse EXCEPTION: Cannot parse Zone. Wrong zone id '{0}' received! Exception={1}", stringZone, ex));
                }
            }
            return ENuvoEssentiaZones.NoZone;
        }

        /// <summary>
        /// Extracts the source id out of the recieved command string.
        /// The member _incomingCommandTemplate needs to be set prior.
        /// </summary>
        /// <param name="commandString">Command string received from Nuvo Essentia.</param>
        /// <param name="commandStringTemplate">Command Template, expected for this command.</param>
        /// <returns>Source id, extracted out of the command string.</returns>
        static private ENuvoEssentiaSources parseCommandForSource(string commandString, string commandStringTemplate)
        {
            string stringSource = parseCommand(commandString, commandStringTemplate, "s");
            if (stringSource != "")
            {
                try
                {
                    return (ENuvoEssentiaSources)Enum.Parse(typeof(ENuvoEssentiaSources), stringSource, true);
                }
                catch (System.ArgumentException ex)
                {
                    LogManager.GetCurrentClassLogger().Fatal(m => m("Parse EXCEPTION: Cannot parse Source. Wrong source id '{0}' received! Exception={1}", stringSource, ex));
                }
            }
            return ENuvoEssentiaSources.NoSource;
        }

        /// <summary>
        /// Extracts the power status out of the recieved command string.
        /// The member _incomingCommandTemplate needs to be set prior.
        /// </summary>
        /// <param name="commandString">Command string received from Nuvo Essentia.</param>
        /// <param name="commandStringTemplate">Command Template, expected for this command.</param>
        /// <returns>Power status, extracted out of the command string.</returns>
        static private EZonePowerStatus parseCommandForPowerStatus(string commandString, string commandStringTemplate)
        {
            string stringPowerStatus = parseCommand(commandString, commandStringTemplate, "ppp");
            EZonePowerStatus zonePowerStatus;
            switch (stringPowerStatus)
            {
                case "OFF":
                    zonePowerStatus = EZonePowerStatus.ZoneStatusOFF;
                    break;
                case "ON":
                case " ON":
                    zonePowerStatus = EZonePowerStatus.ZoneStatusON;
                    break;
                case "":
                    zonePowerStatus = EZonePowerStatus.ZoneStatusUnknown;
                    break;
                default:
                    LogManager.GetCurrentClassLogger().Fatal(m => m("Parse EXCEPTION: Cannot parse Power Status. Wrong status '{0}' received!", stringPowerStatus));
                    zonePowerStatus = EZonePowerStatus.ZoneStatusUnknown;
                    break;
            }
            return zonePowerStatus;
        }

        /// <summary>
        /// Extracts the volume level out of the recieved command string.
        /// The member _incomingCommandTemplate needs to be set prior.
        /// </summary>
        /// <param name="commandString">Command string received from Nuvo Essentia.</param>
        /// <param name="commandStringTemplate">Command Template, expected for this command.</param>
        /// <returns>Volume level, extracted out of the command string.</returns>
        static private int parseCommandForVolumeLevel(string commandString, string commandStringTemplate)
        {
            string stringVolumeLevel = parseCommand(commandString, commandStringTemplate, "yy");
            if (stringVolumeLevel != "")
            {
                try
                {
                    return Convert.ToInt32(stringVolumeLevel) * -1;
                }
                catch (System.FormatException ex)
                {
                    LogManager.GetCurrentClassLogger().Fatal(m => m("Parse EXCEPTION: Cannot parse Volume Level. Wrong command '{0}' received! Exception={1}", stringVolumeLevel, ex));
                }
            }
            return ZoneState.VALUE_UNDEFINED;
        }

        /// <summary>
        /// Extracts the bass level out of the recieved command string.
        /// The member _incomingCommandTemplate needs to be set prior.
        /// </summary>
        /// <param name="commandString">Command string received from Nuvo Essentia.</param>
        /// <param name="commandStringTemplate">Command Template, expected for this command.</param>
        /// <returns>Bass level, extracted out of the command string.</returns>
        static private int parseCommandForBassLevel(string commandString, string commandStringTemplate)
        {
            string stringBassLevel = parseCommand(commandString, commandStringTemplate, "uuu");
            if (stringBassLevel != "")
            {
               try
                {
                    return Convert.ToInt32(stringBassLevel);
                }
               catch (System.FormatException ex)
               {
                   LogManager.GetCurrentClassLogger().Fatal(m => m("Parse EXCEPTION: Cannot parse Bass Level. Wrong command '{0}' received! Exception={1}", stringBassLevel, ex));
               }
            }
            return ZoneState.VALUE_UNDEFINED;
        }

        /// <summary>
        /// Extracts the treble level out of the recieved command string.
        /// The member _incomingCommandTemplate needs to be set prior.
        /// </summary>
        /// <param name="commandString">Command string received from Nuvo Essentia.</param>
        /// <param name="commandStringTemplate">Command Template, expected for this command.</param>
        /// <returns>Treble level, extracted out of the command string.</returns>
        static private int parseCommandForTrebleLevel(string commandString, string commandStringTemplate)
        {
            string stringTrebleLevel = parseCommand(commandString, commandStringTemplate, "ttt");
            if (stringTrebleLevel != "")
            {
               try
                {
                   return Convert.ToInt32(stringTrebleLevel);
                }
               catch (System.FormatException ex)
               {
                   LogManager.GetCurrentClassLogger().Fatal(m => m("Parse EXCEPTION: Cannot parse Treble Level. Wrong command '{0}' received! Exception={1}", stringTrebleLevel, ex));
               }
            }
            return ZoneState.VALUE_UNDEFINED;
        }

        /// <summary>
        /// Extracts the Volume Reset Status out of the recieved command string.
        /// The member _incomingCommandTemplate needs to be set prior.
        /// </summary>
        /// <param name="commandString">Command string received from Nuvo Essentia.</param>
        /// <param name="commandStringTemplate">Command Template, expected for this command.</param>
        /// <returns>Volume Reset Status, extracted out of the command string.</returns>
        static private EVolumeResetStatus parseCommandForVolumeResetStatus(string commandString, string commandStringTemplate)
        {
            string stringVolumeResetStatus = parseCommand(commandString, commandStringTemplate, "r");
            if (stringVolumeResetStatus != "")
            {
                try
                {
                    return (EVolumeResetStatus)Enum.Parse(typeof(EVolumeResetStatus), stringVolumeResetStatus, true);
                }
                catch (System.ArgumentException ex)
                {
                    LogManager.GetCurrentClassLogger().Fatal(m => m("Parse EXCEPTION: Cannot parse Volume Reset Status. Wrong command '{0}' received! Exception={1}", stringVolumeResetStatus, ex));
                }
            }
            return EVolumeResetStatus.VolumeResetUnknown;
        }

        /// <summary>
        /// Extracts the Source Group Status out of the recieved command string.
        /// The member _incomingCommandTemplate needs to be set prior.
        /// </summary>
        /// <param name="commandString">Command string received from Nuvo Essentia.</param>
        /// <param name="commandStringTemplate">Command Template, expected for this command.</param>
        /// <returns>Source Group Status, extracted out of the command string.</returns>
        static private ESourceGroupStatus parseCommandForSourceGroupStatus(string commandString, string commandStringTemplate)
        {
            string stringSourceGroupStatus = parseCommand(commandString, commandStringTemplate, "q");
            if (stringSourceGroupStatus != "")
            {
                try
                {
                    return (ESourceGroupStatus)Enum.Parse(typeof(EVolumeResetStatus), stringSourceGroupStatus, true);
                }
                catch (System.ArgumentException ex)
                {
                    LogManager.GetCurrentClassLogger().Fatal(m => m("Parse EXCEPTION: Cannot parse Source Group Status. Wrong command '{0}' received! Exception={1}", stringSourceGroupStatus, ex));
                }
            }
            return ESourceGroupStatus.SourceGroupUnknown;
        }

        /// <summary>
        /// Extracts the DIP Switch Override Status out of the recieved command string.
        /// The member _incomingCommandTemplate needs to be set prior.
        /// </summary>
        /// <param name="commandString">Command string received from Nuvo Essentia.</param>
        /// <param name="commandStringTemplate">Command Template, expected for this command.</param>
        /// <returns>DIP Switch Override Status, extracted out of the command string.</returns>
        static private EDIPSwitchOverrideStatus parseCommandForDIPSwitchOverrideStatus(string commandString, string commandStringTemplate)
        {
            string stringEDIPSwitchOverrideStatus = parseCommand(commandString, commandStringTemplate, "i");
            if (stringEDIPSwitchOverrideStatus != "")
            {
                try
                {
                    return (EDIPSwitchOverrideStatus)Enum.Parse(typeof(EVolumeResetStatus), stringEDIPSwitchOverrideStatus, true);
                }
                catch (System.ArgumentException ex)
                {
                    LogManager.GetCurrentClassLogger().Fatal(m => m("Parse EXCEPTION: Cannot parse DIP Switch Override Status. Wrong command '{0}' received! Exception={1}", stringEDIPSwitchOverrideStatus, ex));
                }
            }
            return EDIPSwitchOverrideStatus.DIPSwitchOverrideUnknown;
        }

        /// <summary>
        /// Extracts the firmware version out of the recieved command string.
        /// The member _incomingCommandTemplate needs to be set prior.
        /// </summary>
        /// <param name="commandString">Command string received from Nuvo Essentia.</param>
        /// <param name="commandStringTemplate">Command Template, expected for this command.</param>
        /// <returns>Firmware Version, extracted out of the command string.</returns>
        static private string parseCommandForFirmwareVersion(string commandString, string commandStringTemplate)
        {
            return parseCommand(commandString, commandStringTemplate, "vz.zz");
        }

        /// <summary>
        /// Parses the command string and extracts the content, according to the placeholder
        /// in the command template.
        /// </summary>
        /// <param name="command">Command string to parse.</param>
        /// <param name="commandTemplate">Command Template string.</param>
        /// <param name="placeholder">Placeholder string. Represents the part in the command template, which will be extracted from command string.</param>
        /// <returns>Extracted string out of command string.</returns>
        static private string parseCommand(string command, string commandTemplate, string placeholder)
        {
            string result="";
            if (commandTemplate != null && commandTemplate.Contains(placeholder) && placeholder.Length > 0)
            {
                // consider special case of power status, which can be either ON or OFF
                if (commandTemplate.Contains("ppp"))
                {
                    // template contains power status, replace 'ON' status with ' ON'
                    // this ensures that the command string has the same length as the template
                    command = command.Replace("ON", " ON");
                }
                // extract string in received command, according to the command template
                int startindex = commandTemplate.IndexOf(placeholder);
                if (startindex >= 0)
                {
                    try
                    {
                        //TODO handle case if the command is "?" (in case of an error)
                        result = command.Substring(startindex, placeholder.Length);
                    }
                    catch (System.ArgumentOutOfRangeException)
                    {
                        result = "";
                    }
                }
                else
                {
                    result = "";
                }
            }
            else
            {
                // placeholder is not in the command template
                // It's not possible to parse the command string.
                result = "";
            }
            return result;
        }

        #endregion


        /// <summary>
        /// Public override for the <c>ToString</c> method.
        /// </summary>
        /// <returns>String representing the content of this object.</returns>
        public override string ToString()
        {
            return String.Format("Command={0}, ZoneId={1}, PowerStatus={2}, Valid={3}, Finished={4}, Id={5}.",Command, ZoneId, PowerStatus, Valid, Finished, Guid);
        }
    }

}
