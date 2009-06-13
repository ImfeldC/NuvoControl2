﻿/**************************************************************************************************
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
        private static ILog _log = LogManager.GetCurrentClassLogger();

        private System.IO.Ports.SerialPort _comPort; 
        private SerialPortConnectInformation _serialPortConnectInformation;

        public SerialPort()
        {
            _comPort = new System.IO.Ports.SerialPort();
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
            if (_comPort.IsOpen == true) 
                _comPort.Close();
            _comPort = null;
        }

        public bool IsOpen
        {
            get { return _comPort.IsOpen; }
        }

        public void Write(string text)
        {
            if (!(_comPort.IsOpen == true)) _comPort.Open();
            _comPort.Write(text);
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

        #region OpenPort
        private bool OpenPort()
        {
            if (_serialPortConnectInformation == null)
            {
                return false;
            }

            try
            {
                //first check if the port is already open
                //if its open then close it
                if (_comPort.IsOpen == true) _comPort.Close();

                //set the properties of our SerialPort Object
                _comPort.BaudRate = _serialPortConnectInformation.BaudRate;    //BaudRate
                _comPort.DataBits = _serialPortConnectInformation.DataBits;    //DataBits
                _comPort.StopBits = _serialPortConnectInformation.StopBits;    //StopBits
                _comPort.Parity = _serialPortConnectInformation.Parity;        //Parity
                _comPort.PortName = _serialPortConnectInformation.PortName;    //PortName
                //_comPort.ReadTimeout = 100;

                //now open the port
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

    
    }
}