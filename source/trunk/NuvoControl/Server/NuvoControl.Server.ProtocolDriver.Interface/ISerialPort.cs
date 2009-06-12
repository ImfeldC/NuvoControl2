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
using System.IO.Ports;

namespace NuvoControl.Server.ProtocolDriver.Interface
{

    public delegate void SerialPortEventHandler(
              object sender, SerialPortEventArgs e);

    /// <summary>
    /// This class is used as parameter for the serial port event.
    /// It contains the received data, as string.
    /// </summary>
    public class SerialPortEventArgs : EventArgs
    {
        string _msg;

        public string Message
        {
            get { return _msg; }
        }

        public SerialPortEventArgs(string msg)
        {
            _msg = msg;
        }

    }

    /// <summary>
    /// This class contains all required information, to connet to a serial port.
    /// </summary>
    public class SerialPortConnectInformation
    {
        private string _portName;
        public string PortName
        {
            get { return _portName; }
        }

        private int _baudRate;
        public int BaudRate
        {
            get { return _baudRate; }
        }

        private Parity _parity;
        public Parity Parity
        {
            get { return _parity; }
        }

        private int _dataBits;
        public int DataBits
        {
            get { return _dataBits; }
        }

        StopBits _stopBits;
        public StopBits StopBits
        {
            get { return _stopBits; }
        }

        //
        // Summary:
        //     Initializes a new instance of the System.IO.Ports.SerialPort class using
        //     the specified port name.
        //
        // Parameters:
        //   portName:
        //     The port to use (for example, COM1).
        //
        // Exceptions:
        //   System.IO.IOException:
        //     The specified port could not be found or opened.
        public SerialPortConnectInformation(string portName)
        {
            _portName = portName;
        }

        //
        // Summary:
        //     Initializes a new instance of the System.IO.Ports.SerialPort class using
        //     the specified port name and baud rate.
        //
        // Parameters:
        //   portName:
        //     The port to use (for example, COM1).
        //
        //   baudRate:
        //     The baud rate.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     The specified port could not be found or opened.
        public SerialPortConnectInformation(string portName, int baudRate)
        {
            _portName = portName;
            _baudRate = baudRate;
        }

        //
        // Summary:
        //     Initializes a new instance of the System.IO.Ports.SerialPort class using
        //     the specified port name, baud rate, and parity bit.
        //
        // Parameters:
        //   portName:
        //     The port to use (for example, COM1).
        //
        //   baudRate:
        //     The baud rate.
        //
        //   parity:
        //     One of the System.IO.Ports.SerialPort.Parity values.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     The specified port could not be found or opened.
        public SerialPortConnectInformation(string portName, int baudRate, Parity parity)
        {
            _portName = portName;
            _baudRate = baudRate;
            _parity = parity;
        }

        //
        // Summary:
        //     Initializes a new instance of the System.IO.Ports.SerialPort class using
        //     the specified port name, baud rate, parity bit, and data bits.
        //
        // Parameters:
        //   portName:
        //     The port to use (for example, COM1).
        //
        //   baudRate:
        //     The baud rate.
        //
        //   parity:
        //     One of the System.IO.Ports.SerialPort.Parity values.
        //
        //   dataBits:
        //     The data bits value.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     The specified port could not be found or opened.
        public SerialPortConnectInformation(string portName, int baudRate, Parity parity, int dataBits)
        {
            _portName = portName;
            _baudRate = baudRate;
            _parity = parity;
            _dataBits = dataBits;
        }

        //
        // Summary:
        //     Initializes a new instance of the System.IO.Ports.SerialPort class using
        //     the specified port name, baud rate, parity bit, data bits, and stop bit.
        //
        // Parameters:
        //   portName:
        //     The port to use (for example, COM1).
        //
        //   baudRate:
        //     The baud rate.
        //
        //   parity:
        //     One of the System.IO.Ports.SerialPort.Parity values.
        //
        //   dataBits:
        //     The data bits value.
        //
        //   stopBits:
        //     One of the System.IO.Ports.SerialPort.StopBits values.
        //
        // Exceptions:
        //   System.IO.IOException:
        //     The specified port could not be found or opened.
        public SerialPortConnectInformation(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            _portName = portName;
            _baudRate = baudRate;
            _parity = parity;
            _dataBits = dataBits;
            _stopBits = stopBits;
        }

    }

    public interface ISerialPort
    {

        event SerialPortEventHandler onDataReceived;

        //
        // Summary:
        //     Opens a new serial port connection.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The specified port is open.
        //
        //   System.ArgumentOutOfRangeException:
        //     One or more of the properties for this instance are invalid. For example,
        //     the System.IO.Ports.SerialPort.Parity, System.IO.Ports.SerialPort.DataBits,
        //     or System.IO.Ports.SerialPort.Handshake properties are not valid values;
        //     the System.IO.Ports.SerialPort.BaudRate is less than or equal to zero; the
        //     System.IO.Ports.SerialPort.ReadTimeout or System.IO.Ports.SerialPort.WriteTimeout
        //     property is less than zero and is not System.IO.Ports.SerialPort.InfiniteTimeout.
        //
        //   System.ArgumentException:
        //     The port name does not begin with "COM". - or - The file type of the port
        //     is not supported.
        //
        //   System.IO.IOException:
        //     The port is in an invalid state. - or - An attempt to set the state of the
        //     underlying port failed. For example, the parameters passed from this System.IO.Ports.SerialPort
        //     object were invalid.
        //
        //   System.UnauthorizedAccessException:
        //     Access is denied to the port.
        void Open( SerialPortConnectInformation serialPortConnectInformation );

        // Summary:
        //     Closes the port connection, sets the System.IO.Ports.SerialPort.IsOpen property
        //     to false, and disposes of the internal System.IO.Stream object.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The specified port is not open.
        void Close();

        //
        // Summary:
        //     Gets a value indicating the open or closed status of the System.IO.Ports.SerialPort
        //     object.
        //
        // Returns:
        //     true if the serial port is open; otherwise, false. The default is false.
        //
        // Exceptions:
        //   System.ArgumentNullException:
        //     The System.IO.Ports.SerialPort.IsOpen value passed is null.
        //
        //   System.ArgumentException:
        //     The System.IO.Ports.SerialPort.IsOpen value passed is an empty string ("").
        bool IsOpen { get; }

        //
        // Summary:
        //     Writes the specified string to the serial port.
        //
        // Parameters:
        //   text:
        //     The string for output.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The specified port is not open.
        //
        //   System.ArgumentNullException:
        //     str is null.
        //
        //   System.ServiceProcess.TimeoutException:
        //     The operation did not complete before the time-out period ended.
        void Write(string text);
    }
}
