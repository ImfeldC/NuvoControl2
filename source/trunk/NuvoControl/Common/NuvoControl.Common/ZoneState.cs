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
 * 2) 18.06.2009, Christian Imfeld: Add description/copy constructor/equality operations
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
    /// <summary>
    /// Enumeration specifing the zone quality of the data.
    /// </summary>
    public enum ZoneQuality
    {
        /// <summary>
        /// Online, the data is up-to-date and the connection to the device is working.
        /// </summary>
        Online = 0,

        /// <summary>
        /// Offline, the data is may out-dated and the connection to the device is not proper working.
        /// </summary>
        Offline = 1
    }

    /// <summary>
    /// Public class to hold the state of a zone.
    /// 
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
        #region Constants

        /// <summary>
        /// Public constant defining the value is not defined.
        /// Used for the Volume Level, Bass Value and Treble Value.
        /// </summary>
        public const int VALUE_UNDEFINED = -999;

        /// <summary>
        /// Public constant defining the maximum value of the volume level.
        /// Each value matching the following ruls is ok:
        /// VOLUME_MINVALUE is less or equal to 'value' is less or equal to VOLUMEMAXLEVEL
        /// </summary>
        public const int VOLUME_MAXVALUE = 100;

        /// <summary>
        /// Public constant defining the minimum value of the volume level.
        /// Each value matching the following ruls is ok:
        /// VOLUME_MINVALUE is less or equal to 'value' is less or equal to VOLUMEMAXLEVEL
        /// </summary>
        public const int VOLUME_MINVALUE = 0;

        #endregion

        #region Fields

        #region Common Logger
        /// <summary>
        /// Common logger object. Requires the using directive <c>Common.Logging</c>. See 
        /// <see cref="LogManager"/> for more information.
        /// </summary>
        private ILog _log = LogManager.GetCurrentClassLogger();
        #endregion

        [DataMember]
        private Guid _guid;

        [DataMember]
        private ZoneQuality _zoneQuality = ZoneQuality.Offline;

        [DataMember]
        private bool _commandUnacknowledged = false;

        // members only relevant in case of 'online'
        [DataMember]
        private int _volume = VALUE_UNDEFINED;
        [DataMember]
        private bool _powerStatus = false;
        [DataMember]
        private Address _source = new Address();
        [DataMember]
        private DateTime _lastUpdate = new DateTime(1970, 1, 1);    // set to a default time

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
            _zoneQuality = ZoneQuality.Offline;  // Default Value
        }


        /// <summary>
        /// Constructor to set the memebers.
        /// </summary>
        /// <param name="source">Source, selected in this zone.</param>
        /// <param name="powerStatus">Power Status, of this zone.</param>
        /// <param name="volume">Volume, of this zone.</param>
        /// <param name="zoneQuality">Zone Quality, of this zone.</param>
        public ZoneState(Address source, bool powerStatus, int volume, ZoneQuality zoneQuality) : this()
        {
            _source = source;
            _powerStatus = powerStatus;
            Volume = volume;
            ZoneQuality = zoneQuality;
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
                _zoneQuality = ZoneQuality.Offline;  // Default Value
            }
            else
            {
                _guid = sourceZoneState._guid;
                _lastUpdate = sourceZoneState._lastUpdate;
                _zoneQuality = sourceZoneState._zoneQuality;
                _source = sourceZoneState._source;
                _powerStatus = sourceZoneState._powerStatus;
                Volume = sourceZoneState._volume;
                _commandUnacknowledged = sourceZoneState._commandUnacknowledged;
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
        /// If the value is outside of the boundaries VOLUME_MINVALUE and VOLUME_MAXVALUE, it is
        /// reduced/set to the boundary value.
        /// </summary>
        public int Volume
        {
            get { return _volume; }
            set { _volume = (value<VOLUME_MINVALUE?VOLUME_MINVALUE:(value>VOLUME_MAXVALUE?VOLUME_MAXVALUE:value)); }
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
            return string.Format("Power={0} Source={1} Volume={2} Quality={3} Unack={4} LastUpdated={5}",
                (_powerStatus ? "ON" : "OFF"), _source.ToString(), _volume, _zoneQuality, _commandUnacknowledged, _lastUpdate.ToString());
        }

        /// <summary>
        /// Public overlaod for the == operator.
        /// It compares all members exept the GUID, like the Volume, Source, Power state and 
        /// Zone Quality. If all these fields are equal in both
        /// objects, it returns true to indicate that these objects are equal.
        /// If one of the parameters is <c>null</c>, <c>false</c> is returned, which indicates that this
        /// values are not equal.
        /// Returns: True, if the objects are equal. False, if the obejcts are not equal. False, if one or both are null. It compares all fields except the GUID.
        /// </summary>
        /// <param name="left">Left operand</param>
        /// <param name="right">Right operand</param>
        /// <returns>True, if the objects are equal. False, if the obejcts are not equal. False, if one or both are null. It compares all fields except the GUID.</returns>
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
        /// If both parameters are <c>null</c>, <c>true</c> is returned, which indicates that this
        /// values are not equal.
        /// Returns: False, if the objects are equal. True, if the objects are NOT equal. Returns False, if both are null.
        /// </summary>
        /// <param name="left">Left operand</param>
        /// <param name="right">Right operand</param>
        /// <returns>False, if the objects are equal. True, if the objects are NOT equal. Returns False, if both are null.</returns>
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
                // return true, if only one is null.
                // return false, if both are null.
                return !((object)left == null && (object)right == null);
            }
        }

    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/


