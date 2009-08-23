/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Test.COMListener
 *   Author:         Ch.Imfeld
 *   Creation Date:  6/12/2009 11:02:29 PM
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 6/12/2009 11:02:29 PM, Ch.Imfeld: Initial implementation.
 * 
 **************************************************************************************************/
//
// Example used from http://www.dreamincode.net/forums/showtopic35775.htm
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;
using NuvoControl.Server.ProtocolDriver.Interface;

namespace NuvoControl.Test.COMListener
{
    class CommunicationManager
    {
        #region Manager Enums
        /// <summary>
        /// enumeration to hold our transmission types
        /// </summary>
        public enum TransmissionType { Text, Hex }

        /// <summary>
        /// enumeration to hold our message types
        /// </summary>
        public enum MessageType { Incoming, Outgoing, Normal, Warning, Error };
        #endregion

        #region Manager Variables
        //property variables
        private string _baudRate = string.Empty;
        private string _parity = string.Empty;
        private string _stopBits = string.Empty;
        private string _dataBits = string.Empty;
        private string _portName = string.Empty;
        private TransmissionType _transType;
        private RichTextBox _displayWindow;

        SerialPortConnectInformation _serialPortConnectInformation;

        //global manager variables
        private Color[] MessageColor = { Color.Blue, Color.Green, Color.Black, Color.Orange, Color.Red };
        
        // Private member for the serial port
        private SerialPort comNativePort = new SerialPort();
        private ISerialPort comSerialPort = new NuvoControl.Server.ProtocolDriver.SerialPort();

        #endregion


        #region Manager Properties
        /// <summary>
        /// Property to hold the BaudRate
        /// of our manager class
        /// </summary>
        public string BaudRate
        {
            get { return _baudRate; }
            set { _baudRate = value; }
        }

        /// <summary>
        /// property to hold the Parity
        /// of our manager class
        /// </summary>
        public string Parity
        {
            get { return _parity; }
            set { _parity = value; }
        }

        /// <summary>
        /// property to hold the StopBits
        /// of our manager class
        /// </summary>
        public string StopBits
        {
            get { return _stopBits; }
            set { _stopBits = value; }
        }

        /// <summary>
        /// property to hold the DataBits
        /// of our manager class
        /// </summary>
        public string DataBits
        {
            get { return _dataBits; }
            set { _dataBits = value; }
        }

        /// <summary>
        /// property to hold the PortName
        /// of our manager class
        /// </summary>
        public string PortName
        {
            get { return _portName; }
            set { _portName = value; }
        }

        /// <summary>
        /// property to hold our TransmissionType
        /// of our manager class
        /// </summary>
        public TransmissionType CurrentTransmissionType
        {
            get { return _transType; }
            set { _transType = value; }
        }

        /// <summary>
        /// property to hold our display window
        /// value
        /// </summary>
        public RichTextBox DisplayWindow
        {
            get { return _displayWindow; }
            set { _displayWindow = value; }
        }
        #endregion


        #region Manager Constructors
        /// <summary>
        /// Constructor to set the properties of our Manager Class
        /// </summary>
        /// <param name="baud">Desired BaudRate</param>
        /// <param name="par">Desired Parity</param>
        /// <param name="sBits">Desired StopBits</param>
        /// <param name="dBits">Desired DataBits</param>
        /// <param name="name">Desired PortName</param>
        /// <param name="rtb">Rich Textbox, to display messages.</param>
        public CommunicationManager(string baud, string par, string sBits, string dBits, string name, RichTextBox rtb)
        {
            _baudRate = baud;
            _parity = par;
            _stopBits = sBits;
            _dataBits = dBits;
            _portName = name;
            _displayWindow = rtb;

            _serialPortConnectInformation = new SerialPortConnectInformation(
                name,
                int.Parse(_baudRate), 
                (Parity)Enum.Parse(typeof(Parity), _parity),
                int.Parse(_dataBits),
                (StopBits)Enum.Parse(typeof(StopBits), _stopBits)  );

            //now add an event handler
            comNativePort.DataReceived += new SerialDataReceivedEventHandler(comNativePort_DataReceived);
            comSerialPort.onDataReceived += new SerialPortEventHandler(comSerialPort_DataReceived);
        }

        /// <summary>
        /// Comstructor to set the properties of our
        /// serial port communicator to nothing
        /// </summary>
        public CommunicationManager()
        {
            _baudRate = string.Empty;
            _parity = string.Empty;
            _stopBits = string.Empty;
            _dataBits = string.Empty;
            _portName = "COM1";
            _displayWindow = null;
            _serialPortConnectInformation = null;
        }
        #endregion


        #region WriteData

        public void WriteData(string msg)
        {
            WriteSerialData(msg);
        }

        #region WriteNativeData
        private void WriteNativeData(string msg)
        {
            switch (CurrentTransmissionType)
            {
                case TransmissionType.Text:
                    //first make sure the port is open
                    //if its not open then open it
                    if (!(comNativePort.IsOpen == true)) comNativePort.Open();
                    //send the message to the port
                    comNativePort.Write(msg + "\r");  // Add 0x0D at the end, to match Nuvo Essentia needs
                    //display the message
                    DisplayData(MessageType.Outgoing, msg + "\n");
                    break;
                case TransmissionType.Hex:
                    try
                    {
                        //convert the message to byte array
                        byte[] newMsg = HexToByte(msg);
                        //send the message to the port
                        comNativePort.Write(newMsg, 0, newMsg.Length);
                        //convert back to hex and display
                        DisplayData(MessageType.Outgoing, ByteToHex(newMsg) + "\n");
                    }
                    catch (FormatException ex)
                    {
                        //display error message
                        DisplayData(MessageType.Error, ex.Message);
                    }
                    finally
                    {
                        _displayWindow.SelectAll();
                    }
                    break;
                default:
                    //first make sure the port is open
                    //if its not open then open it
                    if (!(comNativePort.IsOpen == true)) comNativePort.Open();
                    //send the message to the port
                    comNativePort.Write(msg);
                    //display the message
                    DisplayData(MessageType.Outgoing, msg + "\n");
                    break;
            }
        }
        #endregion

        #region WriteSerialData
        private void WriteSerialData(string msg)
        {
            switch (CurrentTransmissionType)
            {
                case TransmissionType.Text:
                    //first make sure the port is open
                    //if its not open then open it
                    if (!(comSerialPort.IsOpen == true)) comSerialPort.Open(_serialPortConnectInformation);
                    //send the message to the port
                    comSerialPort.Write(msg + "\r");  // Add 0x0D at the end, to match Nuvo Essentia needs
                    //display the message
                    DisplayData(MessageType.Outgoing, msg + "\n");
                    break;
                case TransmissionType.Hex:
                    //display error message
                    DisplayData(MessageType.Error, "Transmission type 'Hex' is not supported for this serial port");
                    break;
                default:
                    //first make sure the port is open
                    //if its not open then open it
                    if (!(comSerialPort.IsOpen == true)) comSerialPort.Open(_serialPortConnectInformation);
                    //send the message to the port
                    comSerialPort.Write(msg);
                    //display the message
                    DisplayData(MessageType.Outgoing, msg + "\n");
                    break;
            }
        }
        #endregion

        #endregion


        #region HexToByte
        /// <summary>
        /// method to convert hex string into a byte array
        /// </summary>
        /// <param name="msg">string to convert</param>
        /// <returns>a byte array</returns>
        private byte[] HexToByte(string msg)
        {
            //remove any spaces from the string
            msg = msg.Replace(" ", "");
            //create a byte array the length of the
            //string divided by 2
            byte[] comBuffer = new byte[msg.Length / 2];
            //loop through the length of the provided string
            for (int i = 0; i < msg.Length; i += 2)
                //convert each set of 2 characters to a byte
                //and add to the array
                comBuffer[i / 2] = (byte)Convert.ToByte(msg.Substring(i, 2), 16);
            //return the array
            return comBuffer;
        }
        #endregion

        #region ByteToHex
        /// <summary>
        /// method to convert a byte array into a hex string
        /// </summary>
        /// <param name="comByte">byte array to convert</param>
        /// <returns>a hex string</returns>
        private string ByteToHex(byte[] comByte)
        {
            //create a new StringBuilder object
            StringBuilder builder = new StringBuilder(comByte.Length * 3);
            //loop through each byte in the array
            foreach (byte data in comByte)
                //convert the byte to a string and add to the stringbuilder
                builder.Append(Convert.ToString(data, 16).PadLeft(2, '0').PadRight(3, ' '));
            //return the converted value
            return builder.ToString().ToUpper();
        }
        #endregion

        #region DisplayData
        /// <summary>
        /// method to display the data to and from the port
        /// on the screen
        /// </summary>
        /// <param name="type">MessageType of the message</param>
        /// <param name="msg">Message to display</param>
        [STAThread]
        private void DisplayData(MessageType type, string msg)
        {
            _displayWindow.Invoke(new EventHandler(delegate
            {
                _displayWindow.SelectedText = string.Empty;
                _displayWindow.SelectionFont = new Font(_displayWindow.SelectionFont, FontStyle.Bold);
                _displayWindow.SelectionColor = MessageColor[(int)type];
                _displayWindow.AppendText(msg);
                _displayWindow.ScrollToCaret();
            }));
        }
        #endregion


        # region OpenPort
        public bool OpenPort()
        {
            return OpenSerialPort();
        }

        #region OpenNativePort
        private bool OpenNativePort()
        {
            try
            {
                //first check if the port is already open
                //if its open then close it
                if (comNativePort.IsOpen == true) comNativePort.Close();

                //set the properties of our SerialPort Object
                comNativePort.BaudRate = int.Parse(_baudRate);    //BaudRate
                comNativePort.DataBits = int.Parse(_dataBits);    //DataBits
                comNativePort.StopBits = (StopBits)Enum.Parse(typeof(StopBits), _stopBits);    //StopBits
                comNativePort.Parity = (Parity)Enum.Parse(typeof(Parity), _parity);    //Parity
                comNativePort.PortName = _portName;   //PortName
                //comPort.ReadTimeout = 100;

                //now open the port
                comNativePort.Open();
                //display message
                DisplayData(MessageType.Normal, "Native Port opened at " + DateTime.Now + "\n");
                //return true

                //add event handler
                //comNativePort.DataReceived += new SerialDataReceivedEventHandler(comNativePort_DataReceived);
                comNativePort.RtsEnable = true;
                
                return true;
            }
            catch (Exception ex)
            {
                DisplayData(MessageType.Error, ex.Message);
                return false;
            }
        }
        #endregion

        #region OpenSerialPort
        private bool OpenSerialPort()
        {
            try
            {
                //first check if the port is already open
                //if its open then close it
                if (comSerialPort.IsOpen == true) comSerialPort.Close();

                //now open the port
                comSerialPort.Open(_serialPortConnectInformation);
                //display message
                DisplayData(MessageType.Normal, "Serial Port opened at " + DateTime.Now + "\n");
                //return true

                //add event handler
                //comSerialPort.onDataReceived += new SerialPortEventHandler(comSerialPort_DataReceived);

                return true;
            }
            catch (Exception ex)
            {
                DisplayData(MessageType.Error, ex.Message);
                return false;
            }
        }
        #endregion

        #endregion


        #region comNativePort_DataReceived
        /// <summary>
        /// method that will be called when theres data waiting in the buffer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void comNativePort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //determine the mode the user selected (binary/string)
            switch (CurrentTransmissionType)
            {
                //user chose string
                case TransmissionType.Text:
                    //read data waiting in the buffer
                    string msg = comNativePort.ReadExisting();
                    //display the data to the user
                    DisplayData(MessageType.Incoming, msg + "\n");
                    break;
                //user chose binary
                case TransmissionType.Hex:
                    //retrieve number of bytes in the buffer
                    int bytes = comNativePort.BytesToRead;
                    //create a byte array to hold the awaiting data
                    byte[] comBuffer = new byte[bytes];
                    //read the data and store it
                    comNativePort.Read(comBuffer, 0, bytes);
                    //display the data to the user
                    DisplayData(MessageType.Incoming, ByteToHex(comBuffer) + "\n");
                    break;
                default:
                    //read data waiting in the buffer
                    string str = comNativePort.ReadExisting();
                    //display the data to the user
                    DisplayData(MessageType.Incoming, str + "\n");
                    break;
            }
        }
        #endregion

        #region comSerialPort_DataReceived
        /// <summary>
        /// method that will be called when theres data waiting in the buffer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void comSerialPort_DataReceived(object sender, SerialPortEventArgs e)
        {
            //read data waiting in the buffer
            string str = e.Message;
            //display the data to the user
            DisplayData(MessageType.Incoming, str + "\n");
        }
        #endregion


        #region SetParityValues
        public void SetParityValues(object obj)
        {
            foreach (string str in Enum.GetNames(typeof(Parity)))
            {
                ((ComboBox)obj).Items.Add(str);
            }
        }
        #endregion

        #region SetStopBitValues
        public void SetStopBitValues(object obj)
        {
            foreach (string str in Enum.GetNames(typeof(StopBits)))
            {
                ((ComboBox)obj).Items.Add(str);
            }
        }
        #endregion

        #region SetPortNameValues
        public void SetPortNameValues(object obj)
        {

            foreach (string str in SerialPort.GetPortNames())
            {
                ((ComboBox)obj).Items.Add(str);
            }
        }
        #endregion

    }
}
