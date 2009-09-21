/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, C. Imfeld. All Rights Reserved. Confidential
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

        private System.IO.Ports.SerialPort _comPort; 
        private SerialPortConnectInformation _serialPortConnectInformation;

        public SerialPort()
        {
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
            if (_comPort != null)
                _comPort.Write(text);
            else
                _log.Error(m => m("Port is not open, cannot send data {0} to serial port.", text));
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
            //read data waiting in the buffer
            string msg = _comPort.ReadExisting();
            //raise the event, and pass data to next layer
            if (onDataReceived != null)
            {
                onDataReceived(this,
                  new SerialPortEventArgs(msg));
            }
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

                //now open the port
                _log.Trace(m=>m("Open serial port {0}", _serialPortConnectInformation.PortName ));
                _log.Trace(m => m("--> {0}", this.ToString()));
                _comPort.Open();

                //add event handler
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

        public override string ToString()
        {
            string strSerialPort = "";

            //strSerialPort += String.Format("BaseStream: {0} /", _comPort.BaseStream);
            strSerialPort += String.Format("BaudRate: {0} /", _comPort.BaudRate);
            //strSerialPort += String.Format("BreakState: {0} /", _comPort.BreakState);
            //strSerialPort += String.Format("BytesToRead: {0} /", _comPort.BytesToRead);
            //strSerialPort += String.Format("BytesToWrite: {0} /", _comPort.BytesToWrite);
            //strSerialPort += String.Format("CDHolding: {0} /", _comPort.CDHolding);
            //strSerialPort += String.Format("CtsHolding: {0} /", _comPort.CtsHolding);
            strSerialPort += String.Format("DataBits: {0} /", _comPort.DataBits);
            strSerialPort += String.Format("DiscardNull: {0} /", _comPort.DiscardNull);
            //strSerialPort += String.Format("DsrHolding: {0} /", _comPort.DsrHolding);
            strSerialPort += String.Format("DtrEnable: {0} /", _comPort.DtrEnable);
            strSerialPort += String.Format("Encoding: {0} /", _comPort.Encoding);
            strSerialPort += String.Format("Handshake: {0} /", _comPort.Handshake);
            strSerialPort += String.Format("IsOpen: {0} /", _comPort.IsOpen);
            strSerialPort += String.Format("NewLine: {0} /", _comPort.NewLine);
            strSerialPort += String.Format("Parity: {0} /", _comPort.Parity);
            strSerialPort += String.Format("ParityReplace: {0} /", _comPort.ParityReplace);
            strSerialPort += String.Format("PortName: {0} /", _comPort.PortName);
            strSerialPort += String.Format("ReadBufferSize: {0} /", _comPort.ReadBufferSize);
            strSerialPort += String.Format("ReadTimeout: {0} /", _comPort.ReadTimeout);
            strSerialPort += String.Format("ReceivedBytesThreshold: {0} /", _comPort.ReceivedBytesThreshold);
            strSerialPort += String.Format("RtsEnable: {0} /", _comPort.RtsEnable);
            strSerialPort += String.Format("StopBits: {0} /", _comPort.StopBits);
            strSerialPort += String.Format("WriteBufferSize: {0} /", _comPort.WriteBufferSize);
            strSerialPort += String.Format("WriteTimeout: {0} /", _comPort.WriteTimeout);

            strSerialPort += String.Format("_serialPortConnectInformation: {0} /", _serialPortConnectInformation);

            return strSerialPort;
        }
    
    }
}
