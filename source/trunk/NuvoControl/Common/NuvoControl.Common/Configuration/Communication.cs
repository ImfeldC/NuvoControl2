/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Common
 *   Author:         Bernhard Limacher
 *   Creation Date:  19.05.2009
 *   File Name:      Communication.cs
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 19.05.2009, Bernhard Limacher: First implementation.
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace NuvoControl.Common.Configuration
{
    /// <summary>
    /// This is a system configuration class. 
    /// 
    /// It is a data structurer.
    /// It defines communication parameters of a link to a device.
    /// </summary>
    public class Communication
    {
        #region Private Members

        /// <summary>
        /// The name of the port.
        /// </summary>
        private string _port = "COM1";

        /// <summary>
        /// Baud rate.
        /// </summary>
        private int _baudRate = 9600;

        /// <summary>
        /// Data bits
        /// </summary>
        private int _dataBits = 8;

        /// <summary>
        /// Parity bit
        /// </summary>
        private int _parityBit = 1;

        /// <summary>
        /// Parity mode
        /// </summary>
        private string _parityMode = "No";

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="port">The name of the port.</param>
        /// <param name="baudRate">Baud rate.</param>
        /// <param name="dataBits">Data bits</param>
        /// <param name="parityBit">Parity bit</param>
        /// <param name="parityMode">Parity mode</param>
        public Communication(string port, int baudRate, int dataBits, int parityBit, string parityMode)
        {
            this._port = port;
            this._baudRate = baudRate;
            this._dataBits = dataBits;
            this._parityBit = parityBit;
            this._parityMode = parityMode;
        }

        #endregion

        #region Public Interface

        /// <summary>
        /// Defines the communication port, e.g. "COM1"
        /// </summary>
        public string Port
        {
            get { return _port; }
            set { _port = value; }
        }

        /// <summary>
        /// Defines the communication baud rate, e.g. 9600
        /// </summary>
        public int BaudRate
        {
            get { return _baudRate; }
            set { _baudRate = value; }
        }

        /// <summary>
        /// Defines the number of data bits, either 7 or 8.
        /// </summary>
        public int DataBits
        {
            get { return _dataBits; }
            set { _dataBits = value; }
        }

        /// <summary>
        /// Defines the number of parity bit, either 0 or 1.
        /// </summary>
        public int ParityBit
        {
            get { return _parityBit; }
            set { _parityBit = value; }
        }

        /// <summary>
        /// Defines the parity mode, e.g. N (=Normal), E (=Even) or O (=Odd)
        /// </summary>
        public string ParityMode
        {
            get { return _parityMode; }
            set { _parityMode = value; }
        }

        #endregion

        /// <summary>
        /// Returns string representative of this class.
        /// </summary>
        /// <returns>String representative of this class.</returns>
        public override string ToString()
        {
            string strCommunication = "";

            strCommunication += String.Format("Serial Port: {0} /", _port);
            strCommunication += String.Format("BaudRate: {0} /", _baudRate);
            strCommunication += String.Format("Data Bits: {0} /", _dataBits);
            strCommunication += String.Format("Parity Bit: {0} /", _parityBit);
            strCommunication += String.Format("Parity Mode: {0}", _parityMode);

            return strCommunication;
        }
    

    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
**************************************************************************************************/
