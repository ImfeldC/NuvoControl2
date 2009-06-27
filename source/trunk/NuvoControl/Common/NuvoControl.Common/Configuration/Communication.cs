/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
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
    /// This is a system configuration class. It is a data structurer.
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

        public string Port
        {
            get { return _port; }
        }

        public int BaudRate
        {
            get { return _baudRate; }
        }

        public int DataBits
        {
            get { return _dataBits; }
        }

        public int ParityBit
        {
            get { return _parityBit; }
        }

        public string ParityMode
        {
            get { return _parityMode; }
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
