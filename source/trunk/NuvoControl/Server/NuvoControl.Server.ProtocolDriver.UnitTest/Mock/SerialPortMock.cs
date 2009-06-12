/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.ProtocolDriver.UnitTest
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
using NuvoControl.Server.ProtocolDriver.Interface;
using Common.Logging;

namespace NuvoControl.Server.ProtocolDriver.Test.Mock
{
    class SerialPortMock : ISerialPort
    {
        private ILog _log = LogManager.GetCurrentClassLogger();
        private string _writeText = "";

        #region ISerialPort Members

        public event SerialPortEventHandler onDataReceived;

        public void Open(SerialPortConnectInformation serialPortConnectInformation)
        {
            _log.Debug(m => m("Open called."));
        }

        public void Close()
        {
            _log.Debug(m => m("Close called."));
        }

        public bool IsOpen
        {
            get 
            {
                _log.Debug(m => m("IsOpen called.")); 
                return true; 
            }
            
        }

        public void Write(string text)
        {
            _log.Debug(m => m("Write called."));
            WriteText = text;
        }

        #endregion

        //
        // Mock Specific Methods
        //

        public void passDataToTestClass( string msg )
        {
            //raise the event, and pass data to next layer
            if (onDataReceived != null)
            {
                onDataReceived(this,
                  new SerialPortEventArgs(msg));
            }
        }

        public string WriteText
        {
            get { return _writeText; }
            set { _writeText = value; }
        }

    
    }
}
