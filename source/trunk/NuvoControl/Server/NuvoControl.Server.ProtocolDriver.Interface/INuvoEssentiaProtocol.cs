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
            //TODO extract from args the correct command
            _command = ENuvoEssentiaCommands.NoCommand;
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
