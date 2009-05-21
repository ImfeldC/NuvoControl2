using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuvoControl.Server.ProtocolDriver.Interface;
using Common.Logging;
using System.Collections;
using AMS.Profile;

namespace NuvoControl.Server.ProtocolDriver
{
    class NuvoEssentiaProtocol : INuvoEssentiaProtocol
    {
        private ILog _log = LogManager.GetCurrentClassLogger();
        private Profile _profile;

        private INuvoTelegram _serialPort;

        public NuvoEssentiaProtocol(INuvoTelegram nuvoTelegram)
        {
            _profile = new Xml("E:\\ImfeldC-NuvoControl\\source\\trunk\\NuvoControl\\Server\\NuvoControl.Server.ProtocolDriver\\NuvoEssentiaProfile.xml");
            _log.Debug(m => m("Open profile file: {0}", _profile.Name));

            if (nuvoTelegram == null)
                _serialPort = new NuvoTelegram(null);
            else
                _serialPort = nuvoTelegram;
            _serialPort.onTelegramReceived += new NuvoTelegramEventHandler(_serialPort_onTelegramReceived);
        }

        void _serialPort_onTelegramReceived(object sender, NuvoTelegramEventArgs e)
        {
            throw new NotImplementedException();
        }


        #region INuvoEssentiaProtocol Members

        public event NuvoEssentiaProtocolEventHandler onCommandReceived;

        public void Open(SerialPortConnectInformation serialPortConnectInformation)
        {
            _serialPort.Open(serialPortConnectInformation);
        }

        public void Close()
        {
            _serialPort.Close();
        }

        public void SendCommand(string command)
        {
            searchNuvoEssentiaCommandForOutgoingCommand(command);
        }

        public void SendCommand(NuvoEssentiaCommand command)
        {
            throw new NotImplementedException();
        }

        #endregion

        /// <summary>
        /// Converts the command string to a Nuvo Essentia command.
        /// </summary>
        /// <param name="command">Nuvo Essentia command string.</param>
        /// <returns></returns>
        private NuvoEssentiaCommand convertString2NuvoEssentiaCommand(string command)
        {
            return new NuvoEssentiaCommand(ENuvoEssentiaCommands.NoCommand);
        }


        private ENuvoEssentiaCommands searchNuvoEssentiaCommandForOutgoingCommand(string outgoingCommand)
        {
            string[] sectionNames = _profile.GetSectionNames();
            foreach (string section in sectionNames)
            {
                string[] sectionEntries = _profile.GetEntryNames(section);
                foreach (string entry in sectionEntries)
                {
                    if ( ((string)_profile.GetValue(section, entry)).Equals(outgoingCommand))
                    {
                        _log.Debug(m => m("Entry found: Entry={0}, Section={1}", entry, section));
                        return (ENuvoEssentiaCommands)Enum.Parse(typeof(ENuvoEssentiaCommands), section, true);
                    }
                }
            }
            return ENuvoEssentiaCommands.NoCommand;
        }

    }
}
