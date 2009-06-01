using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuvoControl.Server.ProtocolDriver.Interface;
using Common.Logging;

namespace NuvoControl.Server.ProtocolDriver.Test.Mock
{
    class NuvoTelegramMock : INuvoTelegram
    {
        private ILog _log = LogManager.GetCurrentClassLogger();

        private bool _closeMethodCalled;
        private bool _openMethodCalled;
        SerialPortConnectInformation _serialPortConnectInformation;
        private string _telegram;
        private List<string> _telegramList = new List<string>();


        #region INuvoTelegram Members

        public event NuvoTelegramEventHandler onTelegramReceived;

        public void Close()
        {
            _log.Debug(m => m("Close called."));
            _closeMethodCalled = true;
        }

        public void Open(SerialPortConnectInformation serialPortConnectInformation)
        {
            _log.Debug(m => m("Open called. {0}", serialPortConnectInformation.ToString()));
            _openMethodCalled = true;
            _serialPortConnectInformation = serialPortConnectInformation;
        }

        public void SendTelegram(string telegram)
        {
            _log.Debug(m => m("Send called. {0}", telegram));
            _telegram = telegram;
            _telegramList.Add(telegram);
        }


        #endregion

        //
        // Mock Specific Methods
        //

        public void passDataToTestClass(string msg)
        {
            //raise the event, and pass data to next layer
            if (onTelegramReceived != null)
            {
                onTelegramReceived(this,
                  new NuvoTelegramEventArgs(msg));
            }
            _telegramList.Add(msg);
        }

        public bool CloseMethodCalled
        {
            get { return _closeMethodCalled; }
        }
        public bool OpenMethodCalled
        {
            get { return _openMethodCalled; }
        }
        public SerialPortConnectInformation SerialPortConnectInformation
        {
            get { return _serialPortConnectInformation; }
        }
        public string Telegram
        {
            get { return _telegram; }
        }
        public List<string> TelegramList
        {
            get { return _telegramList; }
        }


    }
}
