using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuvoControl.Server.ProtocolDriver.Interface;
using AMS.Profile;
using Common.Logging;

namespace NuvoControl.Server.ProtocolDriver
{
    public class NuvoEssentiaCommand : INuvoEssentiaCommand
    {
        private ILog _log = LogManager.GetCurrentClassLogger();

        Guid _guid;
        DateTime _createDateTime;
        DateTime _sendDateTime;
        DateTime _receiveDateTime;
        ENuvoEssentiaCommands _command = ENuvoEssentiaCommands.NoCommand;

        int _id;
        string _outgoingCommand;
        string _incomingCommand;

        ENuvoEssentiaZones _zoneId = ENuvoEssentiaZones.NoZone;
        ENuvoEssentiaSources _sourceId = ENuvoEssentiaSources.NoSource;
        EZonePowerStatus _powerStatus = EZonePowerStatus.ZoneStatusUnkown;
        EIRCarrierFrequency[] _irCarrierFrequencySource = new EIRCarrierFrequency[6];
        int _volume = -999;
        int _basslevel = -999;
        int _treblelevel = -999;

        public NuvoEssentiaCommand(ENuvoEssentiaCommands command)
        {
            initMembers(command);
        }

        public NuvoEssentiaCommand(ENuvoEssentiaCommands command, ENuvoEssentiaSources source)
        {
            initMembers(command);
            _sourceId = source;
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

        public NuvoEssentiaCommand(ENuvoEssentiaCommands command, int volume)
        {
            initMembers(command);
            _volume = volume;
        }

        public NuvoEssentiaCommand(ENuvoEssentiaCommands command, int basslevel, int treblelevel)
        {
            initMembers(command);
            _basslevel = basslevel;
            _treblelevel = treblelevel;
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

            //TODO replace hard-coded path
            Profile profile = new Xml("E:\\ImfeldC-NuvoControl\\source\\trunk\\NuvoControl\\Server\\NuvoControl.Server.ProtocolDriver\\NuvoEssentiaProfile.xml");

            // Load profile for this command
            _id = Convert.ToInt32((string)profile.GetValue(command.ToString(), "Id"));
            _outgoingCommand = (string)profile.GetValue(command.ToString(), "OutgoingCommand");
            _incomingCommand = (string)profile.GetValue(command.ToString(), "IncomingCommand");
        }


        #region IComparable Members

        public int CompareTo(object obj)
        {
            return this._guid.CompareTo(((NuvoEssentiaCommand)obj)._guid);
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
            set { _receiveDateTime = value; }
        }

        public DateTime CreatedDateTime
        {
            get { return _createDateTime; }
        }

        public string OutgoingCommand
        {
            get { return _outgoingCommand; }
        }

        public string IncomingCommand
        {
            get { return _incomingCommand; }
        }

        #endregion


        #region Replace Section
        ///
        /// This section contains public and private methods to replace placeholders
        /// in the command string.
        /// 
        /// The following placeholders are known, either in incoming (=I) and/or outgoing (=O) commands:
        /// I O 
        /// x -  aa,bb,cc,dd,ee,ff --> for the IR carrier frequency. Replaced by replacePlaceholderForIRFrequency()
        /// x x  xx  --> Zone Id. Replaced by replacePlaceholderForZone()
        /// x -  ppp --> Zone power status. Replaced by replacePlaceholderForPowerStatus()
        /// x x  s   --> Source id. Replaced by replacePlaceholderForSource()
        /// x -  q   --> Source group (on/off). 
        /// x x  yy  --> Volume level. Replaced by replacePlaceholderWithVolumeLevel()
        /// x -  p   --> DIP switch overridden (on/off).
        /// x x  uuu  --> Bass EQ level. Replaced by replacePlaceholderWithBassTrebleLevel()
        /// x x  ttt  --> Treble EQ level. Replaced by replacePlaceholderWithBassTrebleLevel()
        /// x -  r   --> Volume reset (on/off)
        /// x -  vz.zz --> Firmware version 
        /// 
        /// Only replacements in outgoing commands are executed. For the incoming commands refer the Parse Section.
        /// 

        /// <summary>
        /// Replaces the placeholders in the input command with its corresponding
        /// values. Returns a string containing the values.
        /// </summary>
        /// <param name="command">Command string with placeholders</param>
        /// <returns>Result string, placeholders replaced with values.</returns>
        private string replacePlaceholders(string command)
        {
            command = replacePlaceholderForZone(command, _zoneId, "xx");
            command = replacePlaceholderForSource(command, _sourceId, "s");
            command = replacePlaceholderWithVolumeLevel(command, _volume, "yy");
            command = replacePlaceholderWithBassTrebleLevel(command, _basslevel, "uuu");
            command = replacePlaceholderWithBassTrebleLevel(command, _treblelevel, "ttt");
            return command;
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
                if (zonepwrstatus != EZonePowerStatus.ZoneStatusUnkown)
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
                    command = command.Replace(placeholder, (ircf == EIRCarrierFrequency.IR38kHz ? "38" : "55"));
                }
                else
                {
                    _log.Warn(m => m("Replace ERROR: Cannot replace '{0}' in command '{1}', because IR Carrier Frequency member is not set", placeholder, command));
                }
            }
            return command;
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
                    _log.Warn(m => m("Replace ERROR: Cannot replace '{0}' in command '{1}', because Source member is not set", placeholder, command));
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
    }

}
