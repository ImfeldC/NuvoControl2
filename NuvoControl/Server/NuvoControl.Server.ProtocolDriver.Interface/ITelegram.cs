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
    /// Delegate used for the event onTelegramReceived
    /// </summary>
    /// <param name="sender">Ths pointer to the sender of the event.</param>
    /// <param name="e">Event information, for the event onTelegramReceived</param>
    public delegate void TelegramEventHandler(
              object sender, TelegramEventArgs e);

    /// <summary>
    /// Telegram event argument class.
    /// Used to pass information with the event onTelegramReceived
    /// </summary>
    public class TelegramEventArgs : EventArgs
    {
        /// <summary>
        /// Message received from the underlying layer,
        /// </summary>
        string _msg;

        /// <summary>
        /// Get message.
        /// </summary>
        public string Message
        {
            get { return _msg; }
        }

        /// <summary>
        /// Constructor for the telegram event argument class.
        /// Used in case a telegram has been received.
        /// </summary>
        /// <param name="msg">Received telegram</param>
        public TelegramEventArgs(string msg)
        {
            _msg = msg;
        }

    }

    /// <summary>
    /// Public interface, that defines the methods and events which need to be implemented 
    /// by the telegram layer. This interface allows sending and receiving single telegrams. 
    /// This layer is responsible to add and remove start and end characters. If available it 
    /// would also calculate and check an available checksum.
    /// The event onTelegramReceived is issued in case a telegram has been received.
    /// </summary>
    public interface ITelegram
    {

        /// <summary>
        /// The event onTelegramReceived is issued in case a telegram has been received.
        /// </summary>
        event TelegramEventHandler onTelegramReceived;
        
        /// <summary>
        /// Opens a connection to the underlaying layer.
        /// </summary>
        /// <param name="serialPortConnectInformation">Serial port information to connect.</param>
        void Open(SerialPortConnectInformation serialPortConnectInformation);

        /// <summary>
        /// Closes a connection.
        /// </summary>
        void Close();

        /// <summary>
        /// Send a telegram.
        /// </summary>
        /// <param name="telegram">Telegram text, which shall be sent.</param>
        void SendTelegram(string telegram);

    }
}
