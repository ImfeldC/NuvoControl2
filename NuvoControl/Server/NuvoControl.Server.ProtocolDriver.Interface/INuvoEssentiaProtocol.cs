using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuvoControl.Server.ProtocolDriver.Interface
{
    public delegate void NuvoEssentiaProtocolEventHandler(
              object sender, NuvoEssentiaProtocolEventArgs e);

    public class NuvoEssentiaProtocolEventArgs : EventArgs
    {
        string _msg;

        public string Message
        {
            get { return _msg; }
        }

        public NuvoEssentiaProtocolEventArgs(string msg)
        {
            _msg = msg;
        }

    }

    public class NuvoEssentiaCommand
    {
        ENuvoEssentiaCommands _command;

        public NuvoEssentiaCommand(ENuvoEssentiaCommands command)
        {
            _command = command;
        }

        public NuvoEssentiaCommand(NuvoEssentiaProtocolEventArgs args)
        {
            _command = 0;
        }
    }

    interface INuvoEssentiaProtocol
    {

        event NuvoEssentiaProtocolEventHandler onCommandReceived;

        void Open(SerialPortConnectInformation serialPortConnectInformation);

        void Close();

        void SendCommand(string command);

        void SendCommand(NuvoEssentiaCommand command);
    
    }
}
