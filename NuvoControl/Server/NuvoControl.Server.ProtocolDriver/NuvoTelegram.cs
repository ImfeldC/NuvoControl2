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
using NuvoControl.Server.ProtocolDriver.Interface;
using NuvoControl.Server.ProtocolDriver;
using Common.Logging;
using NuvoControl.Server.ProtocolDriver.Simulator;

namespace NuvoControl.Server.ProtocolDriver
{
    /// <summary>
    /// Class to handle command telegram send to Nuvo Essentia
    /// </summary>
    public class NuvoCommandTelegram : ITelegram
    {
        private NuvoTelegram _nuvoTelegram = null;

        /// <summary>
        /// Event raised in case a valid telegram has been received.
        /// </summary>
        public event TelegramEventHandler onTelegramReceived;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="serialPort"></param>
        public NuvoCommandTelegram(ISerialPort serialPort)
        {
            // Command Telegram: Start Charater = '*' / Response Charater = '#'
            _nuvoTelegram = new NuvoTelegram(serialPort, "*", "#");
            _nuvoTelegram.onTelegramReceived += new TelegramEventHandler(_serialPort_onTelegramReceived); 
        }

        /// <summary>
        /// Event method called in case a telegram has been received.
        /// </summary>
        /// <param name="sender">This point to the sender of this event.</param>
        /// <param name="e">Event parameters, passed by the sender.</param>
        void _serialPort_onTelegramReceived(object sender, TelegramEventArgs e)
        {
            //raise the event, and pass data to next layer
            if (onTelegramReceived != null)
            {
                onTelegramReceived(this,e);
            }
        }

        /// <summary>
        /// Open a connection.
        /// </summary>
        /// <param name="serialPortConnectInformation">Contains the serial connection information.</param>
        public void Open(SerialPortConnectInformation serialPortConnectInformation)
        {
            _nuvoTelegram.Open(serialPortConnectInformation);
        }

        /// <summary>
        /// Close connection.
        /// </summary>
        public void Close()
        {
            _nuvoTelegram.Close();
        }

        /// <summary>
        /// Send a telegram (answer).
        /// </summary>
        /// <param name="telegram">Text string to send.</param>
        public void SendTelegram(string telegram)
        {
            _nuvoTelegram.SendTelegram(telegram);
        }
    }

    /// <summary>
    /// Class to handle response telegram send to Nuvo Essentia
    /// </summary>
    public class NuvoResponseTelegram : ITelegram
    {
        private NuvoTelegram _nuvoTelegram = null;

        /// <summary>
        /// Event raised in case a valid telegram has been received.
        /// </summary>
        public event TelegramEventHandler onTelegramReceived;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="serialPort"></param>
        public NuvoResponseTelegram(ISerialPort serialPort)
        {
            // Command Telegram: Start Charater = '*' / Response Charater = '#'
            _nuvoTelegram = new NuvoTelegram(serialPort, "#", "*");
            _nuvoTelegram.onTelegramReceived += new TelegramEventHandler(_serialPort_onTelegramReceived);
        }

        /// <summary>
        /// Event method called in case a telegram has been received.
        /// </summary>
        /// <param name="sender">This point to the sender of this event.</param>
        /// <param name="e">Event parameters, passed by the sender.</param>
        void _serialPort_onTelegramReceived(object sender, TelegramEventArgs e)
        {
            //raise the event, and pass data to next layer
            if (onTelegramReceived != null)
            {
                onTelegramReceived(this, e);
            }
        }

        /// <summary>
        /// Open a connection.
        /// </summary>
        /// <param name="serialPortConnectInformation">Contains the serial connection information.</param>
        public void Open(SerialPortConnectInformation serialPortConnectInformation)
        {
            _nuvoTelegram.Open(serialPortConnectInformation);
        }

        /// <summary>
        /// Close connection.
        /// </summary>
        public void Close()
        {
            _nuvoTelegram.Close();
        }

        /// <summary>
        /// Send a telegram (answer).
        /// </summary>
        /// <param name="telegram">Text string to send.</param>
        public void SendTelegram(string telegram)
        {
            throw new System.NotImplementedException();
            //_nuvoTelegram.SendTelegram(telegram);
        }
    }

    /// <summary>
    /// Native Nuvo telegram class.
    /// </summary>
    public class NuvoTelegram : ITelegram
    {
        #region Common Logger
        /// <summary>
        /// Common logger object. Requires the using directive <c>Common.Logging</c>. See 
        /// <see cref="LogManager"/> for more information.
        /// </summary>
        private ILog _log = LogManager.GetCurrentClassLogger();
        #endregion

        private string _startSendCharacter = "";
        private string _startRcvCharacter = "";

        /// <summary>
        /// Default port name for MSMQ.
        /// </summary>
        public const string defaultPortQueue = "QUEUE";
        /// <summary>
        /// Default port name for simulator
        /// </summary>
        public const string defaultPortSim = "SIM";

        private ISerialPort _serialPortPassedByCaller = null;
        private ISerialPort _serialPort = null;
        private SerialPortConnectInformation _serialPortConnectInformation = null;
        private string _currentTelegramBuffer = "";

        /// <summary>
        /// Default constructor. Similar to "NuvoCommandTelegram"
        /// </summary>
        /// <param name="serialPort">Serial port passed by called. Pass <null> if not passed by caller.</param>
        public NuvoTelegram(ISerialPort serialPort)
        {
            _serialPortPassedByCaller = serialPort;
            _startSendCharacter = "*";
            _startRcvCharacter = "#";
        }


        /// <summary>
        /// Constructor for basic Nuvo telegram class
        /// </summary>
        /// <param name="serialPort">Serial port passed by called. Pass <null> if not passed by caller.</null></param>
        /// <param name="startSendCharacter">Start character for telegrams to "send"</param>
        /// <param name="startRcvCharacter">Start character for telegrams to "receive"</param>
        public NuvoTelegram( ISerialPort serialPort, string startSendCharacter, string startRcvCharacter )
        {
            _serialPortPassedByCaller = serialPort;
            _startSendCharacter = startSendCharacter;
            _startRcvCharacter = startRcvCharacter;
        }


        #region ITelegram Members

        /// <summary>
        /// Event raised in case a valid telegram has been received.
        /// </summary>
        public event TelegramEventHandler onTelegramReceived;

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
                if (_serialPortConnectInformation.PortName.Equals(defaultPortSim))
                {
                    // Create serial port simulator
                    _serialPort = new ProtocolDriverSimulator();
                }
                else if (_serialPortConnectInformation.PortName.Equals(defaultPortQueue))
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

        /// <summary>
        /// Send telegram. Add leading '*' and 'carriage return' sign at end 
        /// </summary>
        /// <param name="telegram">The telegram without leading '*' and ending 'carriage return'</param>
        public void SendTelegram(string telegram)
        {
            if ((telegram != null) && (telegram.Length > 0))
            {
                string text = telegram;
                if (text[0] != _startSendCharacter[0])
                {
                    text = text.Insert(0, _startSendCharacter);
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
        /// 		It should start with a '#' sign and end with a 'carriage return'
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
                return ((_currentTelegramBuffer.IndexOf(_startRcvCharacter[0]) > -1) && (_currentTelegramBuffer.IndexOf('\r') > 0));
            }
        }

        private string cutLeadingCharacters( string telegram )
        {
            int startSignPosition = telegram.IndexOf(_startRcvCharacter[0]);
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
            while (telegram.Contains(_startRcvCharacter) == true)
            {
                int startSignPosition = telegram.IndexOf(_startRcvCharacter[0]);
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
            if (_currentTelegramBuffer.IndexOf(_startRcvCharacter[0]) == 0)
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
                          new TelegramEventArgs(telegramFound));
                    }
                }

            }
            else
            {
                _log.Error(m => m("Start sign missing. {0}", _currentTelegramBuffer));
            }
        }

        public SerialPort SerialPort
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public SerialPortQueue SerialPortQueue
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public NuvoControl.Server.ProtocolDriver.Simulator.ProtocolDriverSimulator ProtocolDriverSimulator
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

    }

}
