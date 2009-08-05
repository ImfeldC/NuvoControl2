/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.ProtocolDriver.Interface
 *   Author:         Ch.Imfeld
 *   Creation Date:  6/12/2009 11:02:29 PM
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 6/12/2009 11:02:29 PM, Ch.Imfeld: Initial implementation.
 * 
 **************************************************************************************************/


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
        private int _deviceId;
        private INuvoEssentiaSingleCommand _command;

        public int DeviceId
        {
            get { return _deviceId; }
        }
        public INuvoEssentiaSingleCommand Command
        {
            get { return _command; }
        }

        public NuvoEssentiaProtocolEventArgs(int deviceId, INuvoEssentiaSingleCommand command)
        {
            _deviceId = deviceId;
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
        /// <param name="command">Nuvo Essentia command (as string). Without starting *-sign and leading 'carriage return'sign.</param>
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
