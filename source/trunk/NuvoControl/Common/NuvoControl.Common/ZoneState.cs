/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Common
 *   Author:         Bernhard Limacher
 *   Creation Date:  21.05.2009
 *   File Name:      ZoneState.cs
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 21.05.2009, Bernhard Limacher: Initial implementation.
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using Common.Logging;

using NuvoControl.Common.Configuration;


namespace NuvoControl.Common
{

    public enum ZoneQuality
    {
        Online = 0,
        Offline = 1
    }


    [DataContract]
    public class ZoneState
    {
        #region Fields

        private static ILog _log = LogManager.GetCurrentClassLogger();

        [DataMember]
        private ZoneQuality _zoneQuality = ZoneQuality.Offline;

        [DataMember]
        private bool _commandUnacknowledged = false;

        // members only relevant in case of 'online'
        [DataMember]
        private int _volume = 0;
        [DataMember]
        private bool _powerStatus = false;
        [DataMember]
        private Address _source = new Address();
        [DataMember]
        private DateTime _lastUpdate = DateTime.Now;   // is set in case a command is received from underlying transport layer

        #endregion

        #region Constructors

        public ZoneState()
        {
        }


        public ZoneState(Address source, bool powerStatus, int volume)
        {
            _source = source;
            _powerStatus = powerStatus;
            _volume = volume;
            _lastUpdate = DateTime.Now;
            _zoneQuality = ZoneQuality.Online;  // Default Value
        }
        #endregion

        #region Field Accessors
        public ZoneQuality ZoneQuality
        {
            get { return _zoneQuality; }
            set { _zoneQuality = value; }
        }

        public int Volume
        {
            get { return _volume; }
            set { _volume = value; }
        }

        public bool PowerStatus
        {
            get { return _powerStatus; }
            set { _powerStatus = value; }
        }

        public Address Source
        {
            get { return _source; }
            set { _source = value; }
        }

        public DateTime LastUpdate
        {
            get { return _lastUpdate; }
            set { _lastUpdate = value; }
        }

        public bool CommandUnacknowledged
        {
            get { return _commandUnacknowledged; }
            set { _commandUnacknowledged = value; }
        }

        #endregion

        public override string ToString() 
        {
            return string.Format("Power={0} Source={1} VolumeLevel={2}", (_powerStatus ? "ON" : "OFF"), _source.ToString(), _volume);
        }
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/


