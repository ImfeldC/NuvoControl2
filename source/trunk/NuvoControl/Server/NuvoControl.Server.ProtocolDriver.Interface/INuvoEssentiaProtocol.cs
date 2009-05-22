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
        NuvoEssentiaCommand _command;

        public NuvoEssentiaCommand Command
        {
            get { return _command; }
        }

        public NuvoEssentiaProtocolEventArgs(NuvoEssentiaCommand command)
        {
            _command = command;
        }

    }

    public class NuvoEssentiaCommand
    {
        DateTime _createDateTime;
        DateTime _sendDateTime;
        DateTime _receiveDateTime;
        ENuvoEssentiaCommands _command;

        public NuvoEssentiaCommand(ENuvoEssentiaCommands command)
        {
            _command = command;
            _createDateTime = DateTime.Now;
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
    }

    public interface INuvoEssentiaProtocol
    {

        event NuvoEssentiaProtocolEventHandler onCommandReceived;

        void Open(SerialPortConnectInformation serialPortConnectInformation);

        void Close();

        /// <summary>
        /// Send the command passed as string to the lower system.
        /// If the command string cannot be converted into a valid
        /// Nuvo Essentia command, nothing is sent to the lower 
        /// system.
        /// </summary>
        /// <param name="command">Nuvo Essentia command (as string). Without starting *-sign.</param>
        void SendCommand(string command);

        /// <summary>
        /// Send the command passed as object to the lower system.
        /// </summary>
        /// <param name="command">Nuvo Essentia command.</param>
        void SendCommand(NuvoEssentiaCommand command);
    
    }
}
