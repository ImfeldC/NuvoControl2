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

        private INuvoTelegram _serialPort;

        // static?
        private Hashtable _OutgoingCommand;
        private Hashtable _IncomingCommand;

        private void initHashtables()
        {
            Profile m_profile;
            m_profile = new Xml();
            _log.Debug(m=>m("Open settings file {0}", m_profile.Name));

            _OutgoingCommand.Add(ENuvoEssentiaCommands.ReadStatusSOURCEIR, "IRSETSR");
            _IncomingCommand.Add(ENuvoEssentiaCommands.ReadStatusSOURCEIR, "IRSET:aa,bb,cc,dd,ee,ff");
        }

        public NuvoEssentiaProtocol(INuvoTelegram nuvoTelegram)
        {
            initHashtables();

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
            throw new NotImplementedException();
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
    }
}
