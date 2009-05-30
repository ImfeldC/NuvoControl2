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
        INuvoEssentiaSingleCommand _command;

        public INuvoEssentiaSingleCommand Command
        {
            get { return _command; }
        }

        public NuvoEssentiaProtocolEventArgs(INuvoEssentiaSingleCommand command)
        {
            _command = command;
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
        /// <param name="command">Nuvo Essentia command (as string). Without starting *-sign and leading '<BR>'sign.</param>
        void SendCommand(string command);

        /// <summary>
        /// Send the single command passed as object to the lower system.
        /// </summary>
        /// <param name="command">Single Nuvo Essentia command.</param>
        void SendCommand(INuvoEssentiaSingleCommand command);

        /// <summary>
        /// Send the multiple command passed as object to the lower system.
        /// </summary>
        /// <param name="command">Multiple Nuvo Essentia command.</param>
        void SendCommand(INuvoEssentiaCommand command);

    }
}
