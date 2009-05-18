using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuvoControl.Server.ProtocolDriver.Interface;
using NuvoControl.Server.ProtocolDriver;
using Common.Logging;

namespace NuvoControl.Server.ProtocolDriver
{
    class NuvoTelegram : INuvoTelegram
    {
        private ILog _log = LogManager.GetCurrentClassLogger(); 

        private ISerialPort _serialPort;
        private string _currentTelegramBuffer = "";

        public NuvoTelegram( ISerialPort serialPort )
        {
            if (serialPort != null)
            {
                _log.Debug(m => m("Start of NuvoTelegram .... use passed-in serial port object!"));
                _serialPort = serialPort;
            }
            else
            {
                _log.Debug(m => m("Start of NuvoTelegram .... create own serial port object!"));
                _serialPort = new SerialPort();
            }

            _serialPort.onDataReceived += new SerialPortEventHandler(serialPort_DataReceived);
        }


        #region INuvoTelegram Members

        public event NuvoTelegramEventHandler onTelegramReceived;

        public void Open(SerialPortConnectInformation serialPortConnectInformation)
        {
            _serialPort.Open(serialPortConnectInformation);
        }

        public void Close()
        {
            _serialPort.Close();
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
            string text = telegram;
            if( text[0] != '*' )
            {
                text.Insert(0, "*");
            }
            if (text[text.Length - 1] != '\r')
            {
                text += '\r'; // Add 0x0D at the end, to match Nuvo Essentia needs
            }
            _serialPort.Write(text);
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
        }


        private void cutLeadingCharacters()
        {
            int startSignPosition = _currentTelegramBuffer.IndexOf('#');
            if (startSignPosition == -1)
            {
                // Start sign not received ...
                // Discarge the content and wait for start sign
                _log.Debug(m => m("Delete content received on serial port, start sign is missing. {0}", _currentTelegramBuffer));
                _currentTelegramBuffer = "";
            }
            else if (startSignPosition > 0)
            {
                // Start sign received, but not at the start ...
                // Discarge starting characters, till the start sign
                string delChars = _currentTelegramBuffer.Substring(0, startSignPosition);
                _currentTelegramBuffer = _currentTelegramBuffer.Remove(0, startSignPosition);
                _log.Debug(m => m("Delete content received on serial port, up to the start sign. {0}", delChars));
            }
        }

        private void cutAndSendTelegram()
        {
            cutLeadingCharacters();

            // Do further processing, if start sign is available ...
            if (_currentTelegramBuffer.IndexOf('#') == 0)
            {
                // Analyze the telegram end
                int endSignPosition = _currentTelegramBuffer.IndexOf('\r');
                if (endSignPosition > 0)
                {
                    string telegramFound = _currentTelegramBuffer.Substring(0, endSignPosition);
                    _currentTelegramBuffer = _currentTelegramBuffer.Remove(0, endSignPosition);
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
