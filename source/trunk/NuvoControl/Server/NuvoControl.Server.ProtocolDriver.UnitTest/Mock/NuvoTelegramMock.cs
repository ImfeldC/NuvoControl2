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


        #region INuvoTelegram Members

        public event NuvoTelegramEventHandler onTelegramReceived;

        public void Close()
        {
            _log.Debug(m => m("Close called."));
        }

        public void Open(SerialPortConnectInformation serialPortConnectInformation)
        {
            _log.Debug(m => m("Open called. {0}", serialPortConnectInformation.ToString()));
        }

        public void SendTelegram(string telegram)
        {
            _log.Debug(m => m("Send called. {0}", telegram));
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
        }


    }
}
