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
        ENuvoEssentiaCommands _command;

        int _id;
        string _outgoingCommand;
        string _incomingCommand;

        ENuvoEssentiaZones _zoneId = ENuvoEssentiaZones.NoZone;
        ENuvoEssentiaSources _sourceId = ENuvoEssentiaSources.NoSource;
        EZonePowerStatus _powerStatus = EZonePowerStatus.ZoneStatusUnkown;
        EIRCarrierFrequency[] _irCarrierFrequencySource = new EIRCarrierFrequency[6];

        public NuvoEssentiaCommand(ENuvoEssentiaCommands command)
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


        /// <summary>
        /// Replaces the placeholders in the input command with its corresponding
        /// values. Returns a string containing the values.
        /// </summary>
        /// <param name="command">Command string with placeholders</param>
        /// <returns>Result string, placeholders replaced with values.</returns>
        private string replacePlaceholders(string command)
        {
            // Placeholder: IR Frequency
            command = replacePlaceholderForIRFrequency(command, _irCarrierFrequencySource[0], "aa");
            command = replacePlaceholderForIRFrequency(command, _irCarrierFrequencySource[1], "bb");
            command = replacePlaceholderForIRFrequency(command, _irCarrierFrequencySource[2], "cc");
            command = replacePlaceholderForIRFrequency(command, _irCarrierFrequencySource[3], "dd");
            command = replacePlaceholderForIRFrequency(command, _irCarrierFrequencySource[4], "ee");
            command = replacePlaceholderForIRFrequency(command, _irCarrierFrequencySource[5], "ff");

            return command;
        }

        private string replacePlaceholderForIRFrequency( string command, EIRCarrierFrequency ircf, string placeholder)
        {
            if (command.Contains(placeholder))
            {
                if (ircf != EIRCarrierFrequency.IRUnknown)
                {
                    command = command.Replace(placeholder, (ircf == EIRCarrierFrequency.IR38kHz ? "38" : "55"));
                }
                else
                {
                    _log.Warn(m => m("Replace ERROR: Cannot replace '{0}' because IR Carrier Frequency is not set", placeholder) );
                }
            }
            return command;
        }

        private string replacePlaceholderForSource(string command, ENuvoEssentiaSources source, string placeholder)
        {
            if (command.Contains(placeholder))
            {
                if (source != ENuvoEssentiaSources.NoSource)
                {
                    command = command.Replace(placeholder, (string.Format("{0,1}",(int)source)));
                }
                else
                {
                    _log.Warn(m => m("Replace ERROR: Cannot replace '{0}' because Source is not set", placeholder));
                }
            }
            return command;
        }
    }

}
