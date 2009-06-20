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
 * 2) 18.06.2009, Christian Imfeld: Add description
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

    /// <summary>
    /// Public class to hold the state of a zone.
    /// It contains the main state of a zone, like selected source, the current power status
    /// (like on or off) and the current volume level.
    /// It holds also additional data about the zone state, like 'Lats Update' to indicate the
    /// date and time when the last comamnd (=update) arrived from hardware device. It holds also
    /// the zone quality, idicating the quality of the data for this zone. See <see cref="ZoneQuality"/>
    /// for more information.
    /// </summary>
    [DataContract]
    public class ZoneState
    {
        #region Fields

        private static ILog _log = LogManager.GetCurrentClassLogger();
        private Guid _guid;

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

        /// <summary>
        /// Default constructor, without any parameter.
        /// This is required to allow this object as command parameter via WCF framework.
        /// </summary>
        public ZoneState()
        {
            _guid = Guid.NewGuid();
            _lastUpdate = DateTime.Now;
            _zoneQuality = ZoneQuality.Online;  // Default Value
        }

        /// <summary>
        /// Constructor to set the memebers.
        /// </summary>
        /// <param name="source">Source, selected in this zone.</param>
        /// <param name="powerStatus">Power Status, of this zone.</param>
        /// <param name="volume">Volume, of this zone.</param>
        public ZoneState(Address source, bool powerStatus, int volume) : this()
        {
            _source = source;
            _powerStatus = powerStatus;
            _volume = volume;
        }

        /// <summary>
        /// Copy Constructor for a zone state.
        /// It copies all memebers (incl. the Guid) to the new object.
        /// </summary>
        /// <param name="sourceZoneState">Source zone state.</param>
        public ZoneState(ZoneState sourceZoneState)
        {
            if (sourceZoneState == null)
            {
                _guid = Guid.NewGuid();
            _lastUpdate = DateTime.Now;
            _zoneQuality = ZoneQuality.Online;  // Default Value
        }
            else
            {
                _guid = sourceZoneState._guid;
                _lastUpdate = sourceZoneState._lastUpdate;
                _zoneQuality = sourceZoneState._zoneQuality;
                _source = sourceZoneState._source;
                _powerStatus = sourceZoneState._powerStatus;
                _volume = sourceZoneState._volume;
            }
        }

        #endregion

        #region Field Accessors

        /// <summary>
        /// Gets and Sets the Zone Quality. This indicates the quality of the data 
        /// representing this zone.
        /// See <see cref="ZoneQuality"/> for more details.
        /// </summary>
        public ZoneQuality ZoneQuality
        {
            get { return _zoneQuality; }
            set { _zoneQuality = value; }
        }

        /// <summary>
        /// Gets and Sets the Volume.
        /// </summary>
        public int Volume
        {
            get { return _volume; }
            set { _volume = value; }
        }

        /// <summary>
        /// Gets and Sets the Power Status.
        /// </summary>
        public bool PowerStatus
        {
            get { return _powerStatus; }
            set { _powerStatus = value; }
        }

        /// <summary>
        /// Gets and Sets the Source
        /// </summary>
        public Address Source
        {
            get { return _source; }
            set { _source = value; }
        }

        /// <summary>
        /// Gets and Sets the Last Updated member.
        /// It indicates the date and time of the last update received from device.
        /// This can be used to determine the actuality of the zone state.
        /// </summary>
        public DateTime LastUpdate
        {
            get { return _lastUpdate; }
            set { _lastUpdate = value; }
        }

        /// <summary>
        /// Gets and Sets the Command Unacknowledged member.
        /// </summary>
        public bool CommandUnacknowledged
        {
            get { return _commandUnacknowledged; }
            set { _commandUnacknowledged = value; }
        }

        #endregion

        /// <summary>
        /// Public override for the <c>ToString</c> method.
        /// </summary>
        /// <returns>String representing the content of this object.</returns>
        public override string ToString() 
        {
            return string.Format("Power={0} Source={1} VolumeLevel={2} LastUpdated={3} Guid={4}", (_powerStatus ? "ON" : "OFF"), _source.ToString(), _volume, _lastUpdate.ToString(), _guid.ToString());
        }

        /// <summary>
        /// Public overlaod for the == operator.
        /// It compares all members exept the GUID, like the Volume, Source, Power state and 
        /// Zone Quality. If all these fields are equal in both
        /// objects, it returns true to indicate that these objects are equal.
        /// If one of the parameters is <c>null</c>, <c>flase</c> is returned, which indicates that this
        /// values are not equal.
        /// </summary>
        /// <param name="left">Left operand</param>
        /// <param name="right">Right operand</param>
        /// <returns>True, if the objects are equal. It compares all fields except the GUID.</returns>
        public static bool operator ==(ZoneState left, ZoneState right)
        {
            return ((object)left != null) && ((object)right != null) &&
                   (left._volume == right._volume) && (left._source == right._source) && 
                   (left._powerStatus == right._powerStatus) && (left._zoneQuality == right._zoneQuality) &&
                   (left._commandUnacknowledged == right._commandUnacknowledged) && (left._lastUpdate == right._lastUpdate);
    }

        /// <summary>
        /// Public overload for the != operator. Is required if operator == has been overwritten.
        /// See <see cref="operator =="/> for more information.
        /// If one of the parameters is <c>null</c>, <c>true</c> is returned, which indicates that this
        /// values are not equal.
        /// </summary>
        /// <param name="left">Left operand</param>
        /// <param name="right">Right operand</param>
        /// <returns>True, if the objects are NOT equal.</returns>
        public static bool operator !=(ZoneState left, ZoneState right)
        {
            if ((object)left != null && (object)right != null)
            {
                // both parameters are not null
                return (left._volume != right._volume) || (left._source != right._source) ||
                       (left._powerStatus != right._powerStatus) || (left._zoneQuality != right._zoneQuality) ||
                       (left._commandUnacknowledged != right._commandUnacknowledged) || (left._lastUpdate != right._lastUpdate);
}
            else
            {
                // one or both parameters are null
                return true;
            }
        }

    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/


