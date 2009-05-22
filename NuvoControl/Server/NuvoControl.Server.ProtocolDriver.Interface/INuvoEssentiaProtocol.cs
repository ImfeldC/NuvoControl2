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
        INuvoEssentiaCommand _command;

        public INuvoEssentiaCommand Command
        {
            get { return _command; }
        }

        public NuvoEssentiaProtocolEventArgs(INuvoEssentiaCommand command)
        {
            _command = command;
        }

    }

    public interface INuvoEssentiaCommand : IComparable
    {
        Guid Guid { get; }
        ENuvoEssentiaCommands Command { get; }
        bool Valid { get; }

        DateTime CreatedDateTime { get; }
        DateTime SendDateTime { get; set; }
        DateTime ReceiveDateTime { get; set; }

        string OutgoingCommand { get; }
        string IncomingCommand { get; }
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
        void SendCommand(INuvoEssentiaCommand command);
    
    }
}
