/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
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

        private int _id = SystemConfiguration.ID_UNDEFINED;
        Communication _communication = null;
        private Protocol _protocolDriver = null;
        private List<int> _zones = new List<int>();
        private List<int> _sources = new List<int>();

        #endregion

        #region Constructors

        public NuvoEssentia(int id, Communication communication, Protocol protocolDriver, List<int> zones, List<int> sources)
        {
            this._id = id;
            this._communication = communication;
            this._protocolDriver = protocolDriver;
            this._zones = zones;
            this._sources = sources;
        }

        #endregion

        #region Public Interface

        public int Id
        {
            get { return _id; }
        }

        public Communication Communication
        {
            get { return _communication; }
        }

        public Protocol ProtocolDriver
        {
            get { return _protocolDriver; }
        }

        public List<int> Zones
        {
            get { return _zones; }
        }

        public List<int> Sources
        {
            get { return _sources; }
        }

        #endregion

    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
