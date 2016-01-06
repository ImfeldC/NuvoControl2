/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
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
    /// <summary>
    /// Delegate used for the event onCommandReceived
    /// </summary>
    /// <param name="sender">This pointer to the sender of the event.</param>
    /// <param name="e">Event information.</param>
    public delegate void ConcreteProtocolEventHandler(
              object sender, ConreteProtocolEventArgs e);

    /// <summary>
    /// Class used in the event onCommandReceived to pass information.
    /// </summary>
    public class ConreteProtocolEventArgs : EventArgs
    {
        private int _deviceId;
        private INuvoEssentiaSingleCommand _command;

        /// <summary>
        /// Get Device Id
        /// </summary>
        public int DeviceId
        {
            get { return _deviceId; }
        }

        /// <summary>
        /// Get Command
        /// </summary>
        public INuvoEssentiaSingleCommand Command
        {
            get { return _command; }
        }

        /// <summary>
        /// Constructor used in case a command has been received.
        /// </summary>
        /// <param name="deviceId">Received Device Id.</param>
        /// <param name="command">Received Command.</param>
        public ConreteProtocolEventArgs(int deviceId, INuvoEssentiaSingleCommand command)
        {
            _deviceId = deviceId;
            _command = command;
        }

    }

    /// <summary>
    /// Public interface, that defines the methods and events which need to be implemented 
    /// by a concrete protocol class. This interface allows sending and receiving commands. 
    /// This layer is responsible to create a telegram for a specific command and to parse 
    /// incoming telegrams to the correct command and its parameters.
    /// The event onCommandReceived is issued in case a command has been received.
    /// </summary>
    public interface IConcreteProtocol
    {

        /// <summary>
        /// The event onCommandReceived is issued in case a command has been received.
        /// </summary>
        event ConcreteProtocolEventHandler onCommandReceived;

        /// <summary>
        /// Opens a connection to the underlying layer.
        /// </summary>
        /// <param name="serialPortConnectInformation"></param>
        void Open(SerialPortConnectInformation serialPortConnectInformation);

        /// <summary>
        /// Close connection.
        /// </summary>
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
