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

namespace NuvoControl.Common.Configuration
{
    [Serializable]
    public class Communication
    {
        #region Private Members

        private string _port = "COM1";
        private int _baudRate = 9600;
        private int _dataBits = 8;
        private int _parityBit = 1;
        private string _parityMode = "No";

        #endregion

        #region Constructors

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
