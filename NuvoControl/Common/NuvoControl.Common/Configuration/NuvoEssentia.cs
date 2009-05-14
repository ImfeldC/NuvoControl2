/**************************************************************************************************
 * 
 *   Copyright (C) Siemens AG 2006 All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Common
 *   Author:         Bernhard Limacher
 *   Creation Date:  12.05.2009
 *   File Name:      NuvoEssentia.cs
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 12.05.2009, Bernhard Limacher: First implementation.
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuvoControl.Common.Configuration
{
    [Serializable]
    public class NuvoEssentia
    {
        #region Private Members

        private int _id = -1;
        private string _port = "COM1";
        private int _baudRate = 9600;
        private int _dataBits = 8;
        private int _parityBit = 1;
        private string _parityMode = "No";
        private List<int> _zones = new List<int>();
        private List<int> _sources = new List<int>();

        #endregion

        #region Constructors

        public NuvoEssentia(int id, string port, int baudRate, int dataBits, int parityBit, string parityMode, List<int> zones, List<int> sources)
        {
            this._id = id;
            this._port = port;
            this._baudRate = baudRate;
            this._dataBits = dataBits;
            this._parityBit = parityBit;
            this._parityMode = parityMode;
            this._zones = zones;
            this._sources = sources;
        }

        #endregion

        #region Public Interface

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string Port
        {
            get { return _port; }
            set { _port = value; }
        }

        public int BaudRate
        {
            get { return _baudRate; }
            set { _baudRate = value; }
        }

        public int DataBits
        {
            get { return _dataBits; }
            set { _dataBits = value; }
        }

        public int ParityBit
        {
            get { return _parityBit; }
            set { _parityBit = value; }
        }

        public string ParityMode
        {
            get { return _parityMode; }
            set { _parityMode = value; }
        }

        public List<int> Zones
        {
            get { return _zones; }
            set { _zones = value; }
        }

        public List<int> Sources
        {
            get { return _sources; }
            set { _sources = value; }
        }

        #endregion

    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
