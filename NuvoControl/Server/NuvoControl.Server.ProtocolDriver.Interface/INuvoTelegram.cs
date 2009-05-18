using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuvoControl.Server.ProtocolDriver.Interface
{

    public delegate void NuvoTelegramEventHandler(
              object sender, NuvoTelegramEventArgs e);

    public class NuvoTelegramEventArgs : EventArgs
    {
        string _msg;

        public string Message
        {
            get { return _msg; }
        }

        public NuvoTelegramEventArgs(string msg)
        {
            _msg = msg;
        }

    }


    public interface INuvoTelegram
    {

        event NuvoTelegramEventHandler onTelegramReceived;

        void Open(SerialPortConnectInformation serialPortConnectInformation);

        void Close();

        void SendTelegram(string telegram);

    }
}
