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

    /// <summary>
    /// 
    /// </summary>
    public interface INuvoTelegram
    {

        event NuvoTelegramEventHandler onTelegramReceived;

        void Open(SerialPortConnectInformation serialPortConnectInformation);

        void Close();

        void SendTelegram(string telegram);

    }
}
