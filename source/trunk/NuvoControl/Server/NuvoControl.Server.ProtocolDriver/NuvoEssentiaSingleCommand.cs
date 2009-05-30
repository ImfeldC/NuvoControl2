﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuvoControl.Server.ProtocolDriver.Interface;
using AMS.Profile;
using Common.Logging;

namespace NuvoControl.Server.ProtocolDriver
{
    /// <summary>
    /// This class represents a single Nuvo Essentia command.
    /// It has members for all possible values send and/or received from Nuvo Essentia.
    /// Depending on the specified command not all members will be used and set at runtime
    /// to the correct value received from Nuvo Essentia.
    /// Mainly it has several parse and replace methods to build and parse the command string
    /// send to and received from Nuvo Essentia. The members are either used as input data to
    /// replace the placeholders in the out-going command, or to store the information retrieved
    /// from the incoming command
    /// </summary>
    public class NuvoEssentiaSingleCommand : INuvoEssentiaSingleCommand
    {
        private ILog _log;
        private Profile _profile;

        Guid _guid;
        DateTime _createDateTime;
        DateTime _sendDateTime;
        DateTime _receiveDateTime;
        ENuvoEssentiaCommands _command = ENuvoEssentiaCommands.NoCommand;

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
        int _volume = -999;
        int _basslevel = -999;
        int _treblelevel = -999;
        EDIPSwitchOverrideStatus _dipSwitchOverrideStatus = EDIPSwitchOverrideStatus.DIPSwitchOverrideUnknown;
        EVolumeResetStatus _volumeResetStatus = EVolumeResetStatus.VolumeResetUnknown;
        ESourceGroupStatus _sourceGroupStatus = ESourceGroupStatus.SourceGroupUnknown;
        string _firmwareVersion = "";
        #endregion

        #region Outgoing Command Constructors

        public NuvoEssentiaSingleCommand(ENuvoEssentiaCommands command)
        {
            constructMembers();
            initMembers(command);
            _outgoingCommand = buildOutgoingCommand();
        }

        public NuvoEssentiaSingleCommand(ENuvoEssentiaCommands command, ENuvoEssentiaSources source)
        {
            constructMembers();
            initMembers(command);
            _sourceId = source;
            _outgoingCommand = buildOutgoingCommand();
        }

        public NuvoEssentiaSingleCommand(ENuvoEssentiaCommands command, ENuvoEssentiaZones zone)
        {
            constructMembers();
            initMembers(command);
            _zoneId = zone;
            _outgoingCommand = buildOutgoingCommand();
        }

        public NuvoEssentiaSingleCommand(ENuvoEssentiaCommands command, ENuvoEssentiaZones zone, ENuvoEssentiaSources source)
        {
            constructMembers();
            initMembers(command);
            _zoneId = zone;
            _sourceId = source;
            _outgoingCommand = buildOutgoingCommand();
        }

        public NuvoEssentiaSingleCommand(ENuvoEssentiaCommands command, ENuvoEssentiaZones zone, int volume)
        {
            constructMembers();
            initMembers(command);
            _zoneId = zone;
            _volume = volume;
            _outgoingCommand = buildOutgoingCommand();
        }

        public NuvoEssentiaSingleCommand(ENuvoEssentiaCommands command, ENuvoEssentiaZones zone, int basslevel, int treblelevel)
        {
            constructMembers();
            initMembers(command);
            _zoneId = zone;
            _basslevel = basslevel;
            _treblelevel = treblelevel;
            _outgoingCommand = buildOutgoingCommand();
        }

        #endregion

        #region Incoming Command Constructors

        public NuvoEssentiaSingleCommand(string receivedCommand)
        {
            constructMembers();
            ENuvoEssentiaCommands command = searchNuvoEssentiaCommand(receivedCommand);
            initMembers(command);
            _incomingCommand = receivedCommand;
            parseIncomingCommand();
        }

        #endregion

        /// <summary>
        /// This private mathod instantiates the class members.
        /// (e.g. Logger, XML Profile reader, etc.)
        /// </summary>
        private void constructMembers()
        {
            try
            {
                _log = LogManager.GetCurrentClassLogger();
            }
            catch (Exception ex)
            {
                // log is not available
                _log = null;
            }

            try
            {
                // If no filename is provided, it searches the defintions
                // of the commong application xml file (e.g. nuvocontrol.server.protocoldriver.test.dll.config)
                _profile = new Xml("NuvoEssentiaProfile.xml");
            }
            catch (Exception ex)
            {
                 // log is not available
                _profile = null;
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
            _guid = Guid.NewGuid();

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
        /// <param name="command">Command (passed as string)</param>
        /// <returns>Enumeration of the found command. Returns NoCommand if command string isn't available.</returns>
        private ENuvoEssentiaCommands searchNuvoEssentiaCommand(string command)
        {
            string[] sectionNames = _profile.GetSectionNames();
            foreach (string section in sectionNames)
            {
                string incomingCommand = (string)_profile.GetValue(section,"IncomingCommand");
                if (NuvoEssentiaProtocol.compareCommandString(incomingCommand, command))
                {
                    _log.Debug(m => m("Entry found: Command={0}, IncomingCommand={1}, Section={2}", command, incomingCommand, section));
                    return (ENuvoEssentiaCommands)Enum.Parse(typeof(ENuvoEssentiaCommands), section, true);
                }
            }
            // command not found
            return ENuvoEssentiaCommands.NoCommand;
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

        public int CompareTo(object obj)
        {
            return this._guid.CompareTo(((NuvoEssentiaSingleCommand)obj)._guid);
        }

        #endregion


        #region INuvoEssentiaCommand Members

        public Guid Guid
        {
            get { return _guid; }
        }

        public ENuvoEssentiaCommands Command
        {
            get { return _command; }
        }

        public bool Valid
        {
            get { return _command != ENuvoEssentiaCommands.NoCommand; }
        }

        public DateTime SendDateTime
        {
            get { return _sendDateTime; }
            set { _sendDateTime = value; }
        }

        public DateTime ReceiveDateTime
        {
            get { return _receiveDateTime; }
        }

        public DateTime CreatedDateTime
        {
            get { return _createDateTime; }
        }

        public string OutgoingCommandTemplate
        {
            get { return _outgoingCommandTemplate; }
        }

        public string IncomingCommandTemplate
        {
            get { return _incomingCommandTemplate; }
        }

        public string OutgoingCommand
        {
            get { return _outgoingCommand; }
        }

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

        public ENuvoEssentiaZones ZoneId
        {
            get { return _zoneId; }
        }

        public ENuvoEssentiaSources SourceId
        {
            get { return _sourceId; }
        }

        public EZonePowerStatus PowerStatus
        {
            get { return _powerStatus; }
        }

        public EIRCarrierFrequency IrCarrierFrequencySource(ENuvoEssentiaSources source)
        {
            return _irCarrierFrequencySource[(int)source-1];
        }

        public int VolumeLevel
        {
            get { return _volume; }
        }

        public int BassLevel
        {
            get { return _basslevel; }
        }

        public int TrebleLevel
        {
            get { return _treblelevel; }
        }

        public EVolumeResetStatus VolumeResetStatus
        {
            get { return _volumeResetStatus; }
        }

        public EDIPSwitchOverrideStatus DIPSwitchOverrideStatus
        {
            get { return _dipSwitchOverrideStatus; }
        }

        public ESourceGroupStatus SourceGrupStatus
        {
            get { return _sourceGroupStatus; }
        }

        public string FirmwareVersion
        {
            get { return _firmwareVersion; }
        }

        #endregion

        ///
        /// This section contains public and private methods to replace placeholders
        /// in the command string.
        /// 
        /// The following placeholders are known, either in incoming (=I) and/or outgoing (=O) commands:
        /// I O 
        /// x -  aa,bb,cc,dd,ee,ff --> for the IR carrier frequency. Replaced by replacePlaceholderForIRFrequency().
        /// x x  xx  --> Zone Id. Replaced by replacePlaceholderForZone(). Parsed with parseCommandForZone().
        /// x -  ppp --> Zone power status. Replaced by replacePlaceholderForPowerStatus(). Parsed with parseCommandForPowerStatus().
        /// x x  s   --> Source id. Replaced by replacePlaceholderForSource(). Parsed with parseCommandForSource().
        /// x -  q   --> Source group (on/off). Parsed with parseCommandForSourceGroupStatus().
        /// x x  yy  --> Volume level. Replaced by replacePlaceholderWithVolumeLevel(). Parsed with parseCommandForVolumeLevel().
        /// x -  i   --> DIP switch overridden (on/off). Parsed with parseCommandForDIPSwitchOverrideStatus().
        /// x x  uuu  --> Bass EQ level. Replaced by replacePlaceholderWithBassTrebleLevel(). Parsed with parseCommandForBassLevel().
        /// x x  ttt  --> Treble EQ level. Replaced by replacePlaceholderWithBassTrebleLevel(). Parsed with parseCommandForTrebleLevel().
        /// x -  r   --> Volume reset (on/off). Parsed with parseCommandForVolumeResetStatus().
        /// x -  vz.zz --> Firmware version. Parsed with parseCommandForFirmwareVersion().
        /// 
        /// In outgoing commands Replacement methods are used. 
        /// For the incoming commands refer the Parse Section.
        /// 

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
                    case ENuvoEssentiaCommands.SetInitialZoneStatus:
                    case ENuvoEssentiaCommands.VolumeDOWN2db:
                    case ENuvoEssentiaCommands.VolumeUP2db:
                        throw new ProtocolDriverException(string.Format("The command '{0}' is not handled by this single command class. Use the container class instead!", _command));
                        break;

                    // unkown command 
                    default:
                        // replace all knwon replacements so far
                        outgoingCommand = replacePlaceholders(_outgoingCommandTemplate);
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
                string message = string.Format("Not all placeholders have been replaced, for the command {0}", _command );
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
        /// <returns>Result string, placeholders replaced with values.</returns>
        private string replacePlaceholders(string command)
        {
            if (command != null)
            {
                command = replacePlaceholderForZone(command, _zoneId);
                command = replacePlaceholderForSource(command, _sourceId);
                command = replacePlaceholderWithVolumeLevel(command, _volume);
                command = replacePlaceholderWithBassLevel(command, _basslevel);
                command = replacePlaceholderWithTrebleLevel(command, _treblelevel);
                command = replacePlaceholderForPowerStatus(command, _powerStatus);
                command = replacePlaceholderForIRFrequency(command, _irCarrierFrequencySource);
            }
            return command;
        }

        /// <summary>
        /// Replaces the default volume level placeholder with the volume level in the command string.
        /// </summary>
        /// <param name="command">Command string</param>
        /// <param name="volume">Volume level</param>
        /// <returns>Command string with replaced placeholders.</returns>
        private string replacePlaceholderWithVolumeLevel(string command, int volume)
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
        private string replacePlaceholderWithVolumeLevel(string command, int volume, string placeholder)
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
                    _log.Warn(m => m("Replace ERROR: Cannot replace '{0}' in command '{1}', because volume level '{2}' is not in allowed range.", placeholder, command, volume));
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
        private string replacePlaceholderWithBassLevel(string command, int level)
        {
            return replacePlaceholderWithBassTrebleLevel(command, level, "uuu");
        }

        /// <summary>
        /// Replaces the default treble placeholder with the level in the command string.
        /// </summary>
        /// <param name="command">Command string</param>
        /// <param name="level">Treble level</param>
        /// <returns>Command string with replaced placeholders.</returns>
        private string replacePlaceholderWithTrebleLevel(string command, int level)
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
        private string replacePlaceholderWithBassTrebleLevel(string command, int level, string placeholder)
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
                    _log.Warn(m => m("Replace ERROR: Cannot replace '{0}' in command '{1}', because bass or treble level '{2}' is not in allowed range.", placeholder, command, level));
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
        private string replacePlaceholderForPowerStatus(string command, EZonePowerStatus zonepwrstatus)
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
        private string replacePlaceholderForPowerStatus(string command, EZonePowerStatus zonepwrstatus, string placeholder)
        {
            if (command.Contains(placeholder))
            {
                if (zonepwrstatus != EZonePowerStatus.ZoneStatusUnknown)
                {
                    command = command.Replace(placeholder, (zonepwrstatus == EZonePowerStatus.ZoneStatusON ? "ON" : "OFF"));
                }
                else
                {
                    _log.Warn(m => m("Replace ERROR: Cannot replace '{0}' in command '{1}', because Zone status member is not set", placeholder, command));
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
        private string replacePlaceholderForZone(string command, ENuvoEssentiaZones zone)
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
        private string replacePlaceholderForZone(string command, ENuvoEssentiaZones zone, string placeholder)
        {
            if (command.Contains(placeholder))
            {
                if (zone != ENuvoEssentiaZones.NoZone)
                {
                    command = replacePlaceholderWithNumber(command, (int)zone, placeholder);
                }
                else
                {
                    _log.Warn(m => m("Replace ERROR: Cannot replace '{0}' in command '{1}', because Zone member is not set", placeholder, command));
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
        private string replacePlaceholderForIRFrequency(string command, EIRCarrierFrequency[] ircf)
        {
            command = replacePlaceholderForIRFrequency(command, ircf[0], "aa");    // Source 1
            command = replacePlaceholderForIRFrequency(command, ircf[1], "bb");    // Source 2
            command = replacePlaceholderForIRFrequency(command, ircf[2], "cc");    // Source 3
            command = replacePlaceholderForIRFrequency(command, ircf[3], "dd");    // Source 4
            command = replacePlaceholderForIRFrequency(command, ircf[4], "ee");    // Source 5
            command = replacePlaceholderForIRFrequency(command, ircf[5], "ff");    // Source 6
            return command;
        }

        /// <summary>
        /// Replaces the IR carrier frequency placeholder with the IR carrier frequency (=ircf) in the command string.
        /// </summary>
        /// <param name="command">Command string</param>
        /// <param name="ircf">IR Carrier Frequency, either 38kHz or 55kHz</param>
        /// <param name="placeholder">Placeholder for the IR carrier frequency</param>
        /// <returns>Command string with replaced placeholders.</returns>
        private string replacePlaceholderForIRFrequency(string command, EIRCarrierFrequency ircf, string placeholder)
        {
            if (command.Contains(placeholder))
            {
                if (ircf != EIRCarrierFrequency.IRUnknown)
                {
                    command = command.Replace(placeholder, (ircf == EIRCarrierFrequency.IR38kHz ? "38" : "56"));
                }
                else
                {
                    _log.Warn(m => m("Replace ERROR: Cannot replace '{0}' in command '{1}', because IR Carrier Frequency member is not set", placeholder, command));
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
        private string replacePlaceholderForSource(string command, ENuvoEssentiaSources source)
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
        private string replacePlaceholderForSource(string command, ENuvoEssentiaSources source, string placeholder)
        {
            if (command.Contains(placeholder))
            {
                if (source != ENuvoEssentiaSources.NoSource)
                {
                    command = replacePlaceholderWithNumber(command, (int)source, placeholder);
                }
                else
                {
                    _log.Error(m => m("Replace ERROR: Cannot replace '{0}' in command '{1}', because Source member is not set", placeholder, command));
                }
            }
            return command;
        }

        /// <summary>
        /// This method replaces in the command string the occurences of the placeholder.
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
        private string replacePlaceholderWithNumber(string command, int num, string placeholder)
        {
            // build format string
            string format = string.Format("{{0,{0}:D{1}}}",placeholder.Length,placeholder.Length);
            // replace placeholder
            return command.Replace(placeholder, (string.Format(format, num)));
        }

        /// <summary>
        /// This method replaces in the command string the occurences of the placeholder.
        /// It replaces it with the number, passed with num. It fills it up with leading 
        /// zeros if required. In the first place is places a + (plus) or - (minus) sign,
        /// according to the number.
        /// </summary>
        /// <param name="command">Command where to replace the placeholder.</param>
        /// <param name="num">Number which will be put into command.</param>
        /// <param name="placeholder">Placeholder string, this pattern is replaced in the command string.</param>
        /// <returns>Command string with replaced placeholders.</returns>
        private string replacePlaceholderWithNumberConsideringPlusMinus(string command, int num, string placeholder)
        {
            // build format string
            string format = string.Format("{0}{{0,{1}:D{2}}}",(num<0?"":"+"), placeholder.Length-1, placeholder.Length-1);
            // replace placeholder
            return command.Replace(placeholder, (string.Format(format, num)));
        }

        #endregion

        #region Command Parse Section


        /// <summary>
        /// Parses the incoming command string and extracts its values.
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
                            parseCommandForALLIRCarrierFrequency(_incomingCommand);
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
                            _zoneId = parseCommandForZone(_incomingCommand);
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
                            _zoneId = parseCommandForZone(_incomingCommand);
                            // extract power status
                            _powerStatus = parseCommandForPowerStatus(_incomingCommand);
                            // extract source placeholder
                            _sourceId = parseCommandForSource(_incomingCommand);
                            // extract source group status
                            _sourceGroupStatus = parseCommandForSourceGroupStatus(_incomingCommand);
                            // extract volume level
                            _volume = parseCommandForVolumeLevel(_incomingCommand);
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
                            _zoneId = parseCommandForZone(_incomingCommand);
                            // extract DIP Switch Override Status
                            _dipSwitchOverrideStatus = parseCommandForDIPSwitchOverrideStatus(_incomingCommand);
                            // extract bass level
                            _basslevel = parseCommandForBassLevel(_incomingCommand);
                            // extract treble level
                            _treblelevel = parseCommandForTrebleLevel(_incomingCommand);
                            // extract source group status
                            _sourceGroupStatus = parseCommandForSourceGroupStatus(_incomingCommand);
                            // extract Volume Reset Status
                            _volumeResetStatus = parseCommandForVolumeResetStatus(_incomingCommand);
                            break;

                        // read firmware version
                        case ENuvoEssentiaCommands.ReadVersion:
                            _firmwareVersion = parseCommandForFirmwareVersion(_incomingCommand);
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
                            parseCommand(_incomingCommand);
                            _log.Warn(m => m("Warning, for this command '{0}' may not all required parsers are implemented!", _command));
                            break;
                    }
                }
                else
                {
                    _log.Warn(m => m("Cannot parse incoming command, because the underlying command is unkown!"));
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
        /// <param name="command">Command string with placeholders</param>
        private void parseCommand(string command)
        {
            _zoneId = parseCommandForZone(command);
            _sourceId = parseCommandForSource(command);
            _powerStatus = parseCommandForPowerStatus(command);
            _volume = parseCommandForVolumeLevel(command);
            _basslevel = parseCommandForBassLevel(command);
            _treblelevel = parseCommandForTrebleLevel(command);
            _firmwareVersion = parseCommandForFirmwareVersion(command);
            _volumeResetStatus = parseCommandForVolumeResetStatus(command);
            _sourceGroupStatus = parseCommandForSourceGroupStatus(command);
            _dipSwitchOverrideStatus = parseCommandForDIPSwitchOverrideStatus(command);
            parseCommandForALLIRCarrierFrequency(command);
        }

        /// <summary>
        /// Extracts all IR Carrier Frequency out of the recieved command string.
        /// The member _incomingCommandTemplate needs to be set prior.
        /// </summary>
        /// <param name="incomingCommand">Command string received from Nuvo Essentia.</param>
        private void parseCommandForALLIRCarrierFrequency(string incomingCommand)
        {
            _irCarrierFrequencySource[0] = parseCommandForIRCarrierFrequency(incomingCommand, "aa");
            _irCarrierFrequencySource[1] = parseCommandForIRCarrierFrequency(incomingCommand, "bb");
            _irCarrierFrequencySource[2] = parseCommandForIRCarrierFrequency(incomingCommand, "cc");
            _irCarrierFrequencySource[3] = parseCommandForIRCarrierFrequency(incomingCommand, "dd");
            _irCarrierFrequencySource[4] = parseCommandForIRCarrierFrequency(incomingCommand, "ee");
            _irCarrierFrequencySource[5] = parseCommandForIRCarrierFrequency(incomingCommand, "ff");
        }

        /// <summary>
        /// Extracts the IR Carrier Frequency out of the recieved command string,
        /// using the passed pattern.
        /// The member _incomingCommandTemplate needs to be set prior.
        /// </summary>
        /// <param name="incomingCommand">Command string received from Nuvo Essentia.</param>
        /// <param name="placeholder">Pattern to search for in the command template.</param>
        /// <returns>IR Carrier Frequency, extracted out of the command string.</returns>
        private EIRCarrierFrequency parseCommandForIRCarrierFrequency(string incomingCommand, string placeholder)
        {
            string stringIRCarrierFrequency = parseCommand(incomingCommand, _incomingCommandTemplate, placeholder);
            EIRCarrierFrequency irCarrierFrequency;
            switch (stringIRCarrierFrequency)
            {
                case "38":
                    irCarrierFrequency = EIRCarrierFrequency.IR38kHz;
                    break;
                case "56":
                    irCarrierFrequency = EIRCarrierFrequency.IR55kHz;
                    break;
                default:
                    _log.Fatal(m => m("Parse EXCEPTION: Cannot parse IR Carrier Frequency. Wrong command '{0}' received!", stringIRCarrierFrequency));
                    irCarrierFrequency = EIRCarrierFrequency.IRUnknown;
                    break;
            }
            return irCarrierFrequency;
        }

        /// <summary>
        /// Extracts the zone id out of the recieved command string.
        /// The member _incomingCommandTemplate needs to be set prior.
        /// </summary>
        /// <param name="incomingCommand">Command string received from Nuvo Essentia.</param>
        /// <returns>Zone id, extracted out of the command string.</returns>
        private ENuvoEssentiaZones parseCommandForZone(string incomingCommand)
        {
            string stringZone = parseCommand(incomingCommand, _incomingCommandTemplate, "xx");
            try
            {
                return (ENuvoEssentiaZones)Enum.Parse(typeof(ENuvoEssentiaZones), stringZone, true);
            }
            catch( System.ArgumentException ex )
            {
                _log.Fatal(m => m("Parse EXCEPTION: Cannot parse Zone. Wrong zone id '{0}' received! Exception={1}", stringZone, ex));
            }
            return ENuvoEssentiaZones.NoZone;
        }

        /// <summary>
        /// Extracts the source id out of the recieved command string.
        /// The member _incomingCommandTemplate needs to be set prior.
        /// </summary>
        /// <param name="incomingCommand">Command string received from Nuvo Essentia.</param>
        /// <returns>Source id, extracted out of the command string.</returns>
        private ENuvoEssentiaSources parseCommandForSource(string incomingCommand)
        {
            string stringSource = parseCommand(incomingCommand, _incomingCommandTemplate, "s");
            try
            {
                return (ENuvoEssentiaSources)Enum.Parse(typeof(ENuvoEssentiaSources), stringSource, true);
            }
            catch (System.ArgumentException ex)
            {
                _log.Fatal(m => m("Parse EXCEPTION: Cannot parse Source. Wrong source id '{0}' received! Exception={1}", stringSource, ex));
            }
            return ENuvoEssentiaSources.NoSource;
        }

        /// <summary>
        /// Extracts the power status out of the recieved command string.
        /// The member _incomingCommandTemplate needs to be set prior.
        /// </summary>
        /// <param name="incomingCommand">Command string received from Nuvo Essentia.</param>
        /// <returns>Power status, extracted out of the command string.</returns>
        private EZonePowerStatus parseCommandForPowerStatus(string incomingCommand)
        {
            string stringPowerStatus = parseCommand(incomingCommand, _incomingCommandTemplate, "ppp");
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
                default:
                    _log.Fatal(m => m("Parse EXCEPTION: Cannot parse Power Status. Wrong status '{0}' received!", stringPowerStatus));
                    zonePowerStatus = EZonePowerStatus.ZoneStatusUnknown;
                    break;
            }
            return zonePowerStatus;
        }

        /// <summary>
        /// Extracts the volume level out of the recieved command string.
        /// The member _incomingCommandTemplate needs to be set prior.
        /// </summary>
        /// <param name="incomingCommand">Command string received from Nuvo Essentia.</param>
        /// <returns>Volume level, extracted out of the command string.</returns>
        private int parseCommandForVolumeLevel(string incomingCommand)
        {
            string stringVolumeLevel = parseCommand(incomingCommand, _incomingCommandTemplate, "yy");
            if (stringVolumeLevel != "")
            {
                try
                {
                    return Convert.ToInt32(stringVolumeLevel) * -1;
                }
                catch (System.FormatException ex)
                {
                    _log.Fatal(m => m("Parse EXCEPTION: Cannot parse Volume Level. Wrong command '{0}' received! Exception={1}", stringVolumeLevel, ex));
                }
            }
            return -999;    //TODO replace with constant
        }

        /// <summary>
        /// Extracts the bass level out of the recieved command string.
        /// The member _incomingCommandTemplate needs to be set prior.
        /// </summary>
        /// <param name="incomingCommand">Command string received from Nuvo Essentia.</param>
        /// <returns>Bass level, extracted out of the command string.</returns>
        private int parseCommandForBassLevel(string incomingCommand)
        {
            string stringBassLevel = parseCommand(incomingCommand, _incomingCommandTemplate, "uuu");
            if (stringBassLevel != "")
            {
               try
                {
                    return Convert.ToInt32(stringBassLevel);
                }
               catch (System.FormatException ex)
               {
                   _log.Fatal(m => m("Parse EXCEPTION: Cannot parse Bass Level. Wrong command '{0}' received! Exception={1}", stringBassLevel, ex));
               }
            }
            return -999;    //TODO replace with constant
        }

        /// <summary>
        /// Extracts the treble level out of the recieved command string.
        /// The member _incomingCommandTemplate needs to be set prior.
        /// </summary>
        /// <param name="incomingCommand">Command string received from Nuvo Essentia.</param>
        /// <returns>Treble level, extracted out of the command string.</returns>
        private int parseCommandForTrebleLevel(string incomingCommand)
        {
            string stringTrebleLevel = parseCommand(incomingCommand, _incomingCommandTemplate, "ttt");
            if (stringTrebleLevel != "")
            {
               try
                {
                   return Convert.ToInt32(stringTrebleLevel);
                }
               catch (System.FormatException ex)
               {
                   _log.Fatal(m => m("Parse EXCEPTION: Cannot parse Treble Level. Wrong command '{0}' received! Exception={1}", stringTrebleLevel, ex));
               }
            }
            return -999;    //TODO replace with constant
        }

        /// <summary>
        /// Extracts the Volume Reset Status out of the recieved command string.
        /// The member _incomingCommandTemplate needs to be set prior.
        /// </summary>
        /// <param name="incomingCommand">Command string received from Nuvo Essentia.</param>
        /// <returns>Volume Reset Status, extracted out of the command string.</returns>
        private EVolumeResetStatus parseCommandForVolumeResetStatus(string incomingCommand)
        {
            string stringVolumeResetStatus = parseCommand(incomingCommand, _incomingCommandTemplate, "r");
            try
            {
                return (EVolumeResetStatus)Enum.Parse(typeof(EVolumeResetStatus), stringVolumeResetStatus, true);
            }
            catch (System.ArgumentException ex)
            {
                _log.Fatal(m => m("Parse EXCEPTION: Cannot parse Volume Reset Status. Wrong command '{0}' received! Exception={1}", stringVolumeResetStatus, ex));
            }
            return EVolumeResetStatus.VolumeResetUnknown;
        }

        /// <summary>
        /// Extracts the Source Group Status out of the recieved command string.
        /// The member _incomingCommandTemplate needs to be set prior.
        /// </summary>
        /// <param name="incomingCommand">Command string received from Nuvo Essentia.</param>
        /// <returns>Source Group Status, extracted out of the command string.</returns>
        private ESourceGroupStatus parseCommandForSourceGroupStatus(string incomingCommand)
        {
            string stringSourceGroupStatus = parseCommand(incomingCommand, _incomingCommandTemplate, "q");
            try
            {
                return (ESourceGroupStatus)Enum.Parse(typeof(EVolumeResetStatus), stringSourceGroupStatus, true);
            }
            catch (System.ArgumentException ex)
            {
                _log.Fatal(m => m("Parse EXCEPTION: Cannot parse Source Group Status. Wrong command '{0}' received! Exception={1}", stringSourceGroupStatus, ex));
            }
            return ESourceGroupStatus.SourceGroupUnknown;
        }

        /// <summary>
        /// Extracts the DIP Switch Override Status out of the recieved command string.
        /// The member _incomingCommandTemplate needs to be set prior.
        /// </summary>
        /// <param name="incomingCommand">Command string received from Nuvo Essentia.</param>
        /// <returns>DIP Switch Override Status, extracted out of the command string.</returns>
        private EDIPSwitchOverrideStatus parseCommandForDIPSwitchOverrideStatus(string incomingCommand)
        {
            string stringEDIPSwitchOverrideStatus = parseCommand(incomingCommand, _incomingCommandTemplate, "i");
            try
            {
                return (EDIPSwitchOverrideStatus)Enum.Parse(typeof(EVolumeResetStatus), stringEDIPSwitchOverrideStatus, true);
            }
            catch (System.ArgumentException ex)
            {
                _log.Fatal(m => m("Parse EXCEPTION: Cannot parse DIP Switch Override Status. Wrong command '{0}' received! Exception={1}", stringEDIPSwitchOverrideStatus, ex));
            }
            return EDIPSwitchOverrideStatus.DIPSwitchOverrideUnknown;
        }

        /// <summary>
        /// Extracts the firmware version out of the recieved command string.
        /// The member _incomingCommandTemplate needs to be set prior.
        /// </summary>
        /// <param name="incomingCommand">Command string received from Nuvo Essentia.</param>
        /// <returns>Firmware Version, extracted out of the command string.</returns>
        private string parseCommandForFirmwareVersion(string incomingCommand)
        {
            return parseCommand(incomingCommand, _incomingCommandTemplate, "vz.zz");
        }

        /// <summary>
        /// Parses the command string and extracts the content, according to the placeholder
        /// in the command template.
        /// </summary>
        /// <param name="command">Command string to parse.</param>
        /// <param name="commandTemplate">Command Template string.</param>
        /// <param name="placeholder">Placeholder string. Represents the part in the command template, which will be extracted from command string.</param>
        /// <returns>Extracted string out of command string.</returns>
        private static string parseCommand(string command, string commandTemplate, string placeholder)
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
                        result = command.Substring(startindex, placeholder.Length);
                    }
                    catch (System.ArgumentOutOfRangeException ex)
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


    }

}