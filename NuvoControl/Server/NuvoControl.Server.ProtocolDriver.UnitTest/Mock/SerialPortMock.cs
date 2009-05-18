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
        }

        #endregion


        public void passDataToTestClass( string msg )
        {
            //raise the event, and pass data to next layer
            if (onDataReceived != null)
            {
                onDataReceived(this,
                  new SerialPortEventArgs(msg));
            }
        }
    }
}
