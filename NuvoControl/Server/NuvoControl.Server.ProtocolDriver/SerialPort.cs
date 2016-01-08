/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.ProtocolDriver
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
using System.IO.Ports;
using NuvoControl.Server.ProtocolDriver.Interface;
using Common.Logging;
using System.Timers;

namespace NuvoControl.Server.ProtocolDriver
{
    public class SerialPort : ISerialPort
    {
        #region Common Logger
        /// <summary>
        /// Common logger object. Requires the using directive <c>Common.Logging</c>. See 
        /// <see cref="LogManager"/> for more information.
        /// </summary>
        private ILog _log = LogManager.GetCurrentClassLogger();
        #endregion

        /// <summary>
        /// Private members
        /// </summary>
        private System.IO.Ports.SerialPort _comPort; 
        private SerialPortConnectInformation _serialPortConnectInformation;
        private bool _limitedEnvironment = false;       // true, if "mono" framework is detected!
        private bool _readIntervalTimerRunning = false; // true, if read interval timer has been started

        /// <summary>
        /// Private member to hold the timer used to send a 'ping' to the device.
        /// </summary>
        private System.Timers.Timer _timerPing = new System.Timers.Timer();


        /// <summary>
        /// Default constructor. 
        /// Includes a check of the running environment; sets _LimitedEnvironment
        /// </summary>
        public SerialPort()
        {
            _log.Trace(m => m("Serial port instantiated!"));
            try
            {
                // NOTE: The following properties are not implemented in the "mono" framework
                // See "Limitations" in http://www.mono-project.com/archived/howtosystemioports/
                // Not implemented under Mono (Raspberry Pi)
                System.IO.Ports.SerialPort serialPort = new System.IO.Ports.SerialPort();
                bool discardNull = serialPort.DiscardNull;
                // if this command executes w/o exception, we are running on a "full" .NET environment (not "mono" framework)
                _limitedEnvironment = false;
            }
            catch (System.NotImplementedException exc)
            {
                // Limited environment detected!
                _limitedEnvironment = true;
            }

            // Enable read intervall timer only, if ..
            // (a) Proper intervall is defined
            // (b) A limited environemnt is detected (whcih doesn't support events) (see http://www.mono-project.com/archived/howtosystemioports/ )
            if (Properties.Settings.Default.ReadIntervall > 0 && _limitedEnvironment)
            {
                _log.Trace(m => m("Read intervall timer started, each {0}[s]", Properties.Settings.Default.PingIntervall));
                _timerPing.Interval = (Properties.Settings.Default.PingIntervall < 2 ? 2 : Properties.Settings.Default.PingIntervall) * 1000;
                _timerPing.Elapsed += new ElapsedEventHandler(_timerReadIntervall_Elapsed);
                _timerPing.Start();
                _readIntervalTimerRunning = true;
            }
            else
            {
                _log.Warn(m => m("Read intervall timer is disabled !!! ({0}[s])", Properties.Settings.Default.PingIntervall));
                _readIntervalTimerRunning = false;
            }

        }


        #region ISerialPort Members

        public event SerialPortEventHandler onDataReceived;

        public void Open(SerialPortConnectInformation serialPortConnectInformation)
        {
            _serialPortConnectInformation = serialPortConnectInformation;
            OpenPort();
        }

        public void Close()
        {
            ClosePort();
        }

        public bool IsOpen
        {
            get 
            {
                if (_comPort == null)
                    return false;
                else
                    return _comPort.IsOpen; 
            }
        }

        public void Write(string text)
        {
            if (IsOpen == false)
            {
                _log.Warn(m => m("Port is not open yet, open it before sending data!"));
                OpenPort();
            }
            if ( (_comPort != null) && (_comPort.IsOpen==true))
                _comPort.Write(text);
            else
                _log.Error(m => m("Port is not open, cannot send data {0} to serial port.", text));

            if (!_readIntervalTimerRunning)
            {
                // Read data "synchronous" (only in case read interval timmer is NOT started), because event notifications are not supported
                // See http://www.mono-project.com/archived/howtosystemioports/
                readData();
            }
        }

        #endregion

        #region comPort_DataReceived
        /// <summary>
        /// method that will be called when theres data waiting in the buffer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void comPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            readData();
        }

        private string readData()
        {
            string msg = "";
            try
            {
                //read data waiting in the buffer
                msg = _comPort.ReadExisting();
            }
            catch (System.TimeoutException exc)
            {
                // ignore timeout, finish read-out
            }

            //raise the event, and pass data to next layer
            if (onDataReceived != null)
            {
                onDataReceived(this,
                  new SerialPortEventArgs(msg));
            }
            return msg;
        }

        private string ReadByteData()
        {
            byte tmpByte;
            string rxString = "";

            try
            {
                tmpByte = (byte)_comPort.ReadByte();
                //Console.WriteLine("Start ...." + tmpByte);

                while (tmpByte != 255)
                {
                    rxString += ((char)tmpByte);
                    tmpByte = (byte)_comPort.ReadByte();
                    //Console.WriteLine("Get data ...." + tmpByte);
                }
            }
            catch (System.TimeoutException exc)
            {
                // ignore timeout, finish read-out
            }

            Console.WriteLine("Message received:" + rxString);
            return rxString;
        }
        
        /// <summary>
        /// Timer event method, to check for data on the serial port.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _timerReadIntervall_Elapsed(object sender, ElapsedEventArgs e)
        {
            readData();
        }        
        #endregion

        #region Open-/ClosePort

        private void ClosePort()
        {
            if(_comPort != null) 
            {
                if (_comPort.IsOpen == true)
                {
                    _comPort.Close();
                }
                _comPort = null;
            }
        }

        private bool OpenPort()
        {
            if (_serialPortConnectInformation == null)
            {
                _log.Error(m => m("No connection information available. Cannot open connection!"));
                return false;
            }

            try
            {
                //first check if the port is already open
                //if its open then close it
                ClosePort();

                _comPort = new System.IO.Ports.SerialPort();

                //set the properties of our SerialPort Object
                _comPort.PortName = _serialPortConnectInformation.PortName;    //PortName
                _comPort.BaudRate = _serialPortConnectInformation.BaudRate;    //BaudRate
                _comPort.Parity = _serialPortConnectInformation.Parity;        //Parity
                _comPort.DataBits = _serialPortConnectInformation.DataBits;    //DataBits
                _comPort.StopBits = _serialPortConnectInformation.StopBits;    //StopBits

                _comPort.Handshake = Handshake.None;

                // Set the read/write timeouts
                _comPort.ReadTimeout = 500;
                _comPort.WriteTimeout = 500;

                // now open the port
                _log.Trace(m=>m("Open serial port {0} (Limited={1})", _serialPortConnectInformation.PortName, _limitedEnvironment ));
                _log.Trace(m => m("--> {0}", this.ToString()));
                _comPort.Open();

                // add event handler
                // NOTE: event handlers are NOT supported in limited environments (see http://www.mono-project.com/archived/howtosystemioports/)
                _comPort.DataReceived += new SerialDataReceivedEventHandler(comPort_DataReceived);
                _comPort.RtsEnable = true;

                return true;
            }
            catch (Exception ex)
            {
                _log.Fatal(m=>m("Exception occured opening the serial port {0}! SerialPortInformation='{1}' / Exception='{2}'",
                    _serialPortConnectInformation.PortName, _serialPortConnectInformation, ex.ToString()));
                return false;
            }
        }

        #endregion

        /// <summary>
        /// Returns string representative of this class.
        /// </summary>
        /// <returns>String representative of this class.</returns>
        public override string ToString()
        {
            string strSerialPort = "";

            try
            {
                //strSerialPort += String.Format("BaseStream: {0} /", _comPort.BaseStream);
                strSerialPort += String.Format("BaudRate: {0} /", _comPort.BaudRate);
                //strSerialPort += String.Format("BreakState: {0} /", _comPort.BreakState);
                //strSerialPort += String.Format("BytesToRead: {0} /", _comPort.BytesToRead);
                //strSerialPort += String.Format("BytesToWrite: {0} /", _comPort.BytesToWrite);
                //strSerialPort += String.Format("CDHolding: {0} /", _comPort.CDHolding);
                //strSerialPort += String.Format("CtsHolding: {0} /", _comPort.CtsHolding);
                strSerialPort += String.Format("DataBits: {0} /", _comPort.DataBits);
                //strSerialPort += String.Format("DsrHolding: {0} /", _comPort.DsrHolding);
                strSerialPort += String.Format("DtrEnable: {0} /", _comPort.DtrEnable);
                strSerialPort += String.Format("Encoding: {0} /", _comPort.Encoding);
                strSerialPort += String.Format("Handshake: {0} /", _comPort.Handshake);
                strSerialPort += String.Format("IsOpen: {0} /", _comPort.IsOpen);
                strSerialPort += String.Format("NewLine: {0} /", _comPort.NewLine);
                strSerialPort += String.Format("Parity: {0} /", _comPort.Parity);
                strSerialPort += String.Format("PortName: {0} /", _comPort.PortName);
                strSerialPort += String.Format("ReadBufferSize: {0} /", _comPort.ReadBufferSize);
                strSerialPort += String.Format("ReadTimeout: {0} /", _comPort.ReadTimeout);
                strSerialPort += String.Format("RtsEnable: {0} /", _comPort.RtsEnable);
                strSerialPort += String.Format("StopBits: {0} /", _comPort.StopBits);
                strSerialPort += String.Format("WriteTimeout: {0} /", _comPort.WriteTimeout);
                strSerialPort += String.Format("WriteBufferSize: {0} /", _comPort.WriteBufferSize);

                strSerialPort += String.Format("_serialPortConnectInformation: {0} /", _serialPortConnectInformation);

                // NOTE: The following properties are not implemented in the "mono" framework
                // See "Limitations" in http://www.mono-project.com/archived/howtosystemioports/
                // Not implemented under Mono (Raspberry Pi)
                if (!_limitedEnvironment)
                {
                    strSerialPort += String.Format("ReceivedBytesThreshold: {0} /", _comPort.ReceivedBytesThreshold);
                    strSerialPort += String.Format("ParityReplace: {0} /", _comPort.ParityReplace);
                    strSerialPort += String.Format("DiscardNull: {0}", _comPort.DiscardNull);
                }   
            }
            catch (System.NotImplementedException exc)
            {
                // ignore exception, return so far available string to caller.
                strSerialPort += String.Format(" <Limited framework detected!>");
            }

            return strSerialPort;
        }
    
    }
}
