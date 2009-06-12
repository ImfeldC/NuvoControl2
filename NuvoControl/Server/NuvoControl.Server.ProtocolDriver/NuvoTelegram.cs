﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuvoControl.Server.ProtocolDriver.Interface;
using NuvoControl.Server.ProtocolDriver;
using Common.Logging;
using NuvoControl.Server.ProtocolDriver.Simulator;

namespace NuvoControl.Server.ProtocolDriver
{
    class NuvoTelegram : INuvoTelegram
    {
        private ILog _log = LogManager.GetCurrentClassLogger();

        private ISerialPort _serialPortPassedByCaller = null;
        private ISerialPort _serialPort = null;
        private SerialPortConnectInformation _serialPortConnectInformation = null;
        private string _currentTelegramBuffer = "";

        public NuvoTelegram( ISerialPort serialPort )
        {
            _serialPortPassedByCaller = serialPort;
        }


        #region INuvoTelegram Members

        public event NuvoTelegramEventHandler onTelegramReceived;

        public void Open(SerialPortConnectInformation serialPortConnectInformation)
        {
            _serialPortConnectInformation = serialPortConnectInformation;
            CreateSerialPort();
        }

        public void Close()
        {
            _serialPort.Close();
            _serialPort = null;
        }

        private void CreateSerialPort()
        {
            if (_serialPort != null)
            {
                // Close already open port
                Close();
            }

            if (_serialPortPassedByCaller != null)
            {
                _log.Debug(m => m("Start of NuvoTelegram .... use passed-in serial port object!"));
                _serialPort = _serialPortPassedByCaller;
            }
            else
            {
                _log.Debug(m => m("Start of NuvoTelegram .... create own serial port object!"));
                if (_serialPortConnectInformation.PortName.Equals("SIM"))
                {
                    // Create serial port simulator
                    _serialPort = new ProtocolDriverSimulator();
                }
                else if (_serialPortConnectInformation.PortName.Equals("QUEUE"))
                {
                    // Create serial port queue
                    _serialPort = new SerialPortQueue();
                }
                else
                {
                    _serialPort = new SerialPort();
                }
            }

            // Register for events and open serial port
            _serialPort.onDataReceived += new SerialPortEventHandler(serialPort_DataReceived);
            _serialPort.Open(_serialPortConnectInformation);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn	public void SendTelegram(string telegram)
        ///
        /// \brief	Send telegram. Add leading '*' and '<CR>' sign 
        ///
        /// \author	Administrator
        /// \date	18.05.2009
        ///
        /// \param	telegram - The telegram without leading '*' and ending '<CR>'. 
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void SendTelegram(string telegram)
        {
            if ((telegram != null) && (telegram.Length > 0))
            {
                string text = telegram;
                if (text[0] != '*')
                {
                    text = text.Insert(0, "*");
                }
                if (text[text.Length - 1] != '\r')
                {
                    text += '\r'; // Add 0x0D at the end, to match Nuvo Essentia needs
                }
                _serialPort.Write(text);
            }
            else
            {
                _log.Warn(m => m("Empty telegram received, not passed to the serial port!"));
            }
        }

        #endregion


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// \fn	void serialPort_DataReceived(object sender, SerialPortEventArgs e)
        ///
        /// \brief	
        /// 		method that will be called when theres data waiting in the buffer. 
        /// 		The received text is parsed, that it macthes the expected Nuvo telegram.
        /// 		It should start with a '#' sign and end with a '<CR>'
        ///
        /// \author	Ch. Imfeld
        /// \date	18.05.2009
        ///
        /// \param	sender	 - .                        
        /// \param	e		 - .                   
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        void serialPort_DataReceived(object sender, SerialPortEventArgs e)
        {
            // Add received message to the telegram
            _currentTelegramBuffer += e.Message;
            cutAndSendTelegram();
            while (IsResponseTelegramComplete)
            {
                cutAndSendTelegram();
            }
        }

        private bool IsResponseTelegramComplete
        {
            get{
                return ((_currentTelegramBuffer.IndexOf('#') > -1) && (_currentTelegramBuffer.IndexOf('\r') > 0));
            }
        }

        private string cutLeadingCharacters( string telegram )
        {
            int startSignPosition = telegram.IndexOf('#');
            if (startSignPosition == -1)
            {
                // Start sign not received ...
                // Discarge the content and wait for start sign
                _log.Debug(m => m("Delete content received on serial port, start sign is missing. {0}", telegram));
                telegram = "";
            }
            else if (startSignPosition > 0)
            {
                // Start sign received, but not at the start ...
                // Discarge starting characters, till the start sign
                string delChars = telegram.Substring(0, startSignPosition);
                telegram = telegram.Remove(0, startSignPosition);
                _log.Debug(m => m("Delete content received on serial port, up to the start sign. {0}", delChars));
            }
            return telegram;
        }

        private string cutLeadingStartSigns(string telegram)
        {
            while (telegram.Contains("#") == true)
            {
                int startSignPosition = telegram.IndexOf('#');
                // Start sign received, but not at the start ...
                // Discarge starting characters, till the start sign
                string delChars = telegram.Substring(0, startSignPosition);
                telegram = telegram.Remove(0, startSignPosition+1);
                _log.Debug(m => m("Delete content received on serial port, up to the start sign. {0}", delChars));
            }
            return telegram;
        }

        private void cutAndSendTelegram()
        {
            _currentTelegramBuffer = cutLeadingCharacters(_currentTelegramBuffer);

            // Do further processing, if start sign is available ...
            if (_currentTelegramBuffer.IndexOf('#') == 0)
            {
                // Analyze the telegram end
                int endSignPosition = _currentTelegramBuffer.IndexOf('\r');
                if (endSignPosition > 0)
                {
                    string telegramFound = _currentTelegramBuffer.Substring(1, endSignPosition - 1);
                    _currentTelegramBuffer = _currentTelegramBuffer.Remove(0, endSignPosition + 1);

                    telegramFound = cutLeadingStartSigns(telegramFound);
                    //raise the event, and pass data to next layer
                    if (onTelegramReceived != null)
                    {
                        onTelegramReceived(this,
                          new NuvoTelegramEventArgs(telegramFound));
                    }
                }

            }
            else
            {
                _log.Error(m => m("Start sign missing. {0}", _currentTelegramBuffer));
            }
        }
    }
}