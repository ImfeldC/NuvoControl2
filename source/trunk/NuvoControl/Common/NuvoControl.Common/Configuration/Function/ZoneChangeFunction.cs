/**************************************************************************************************
 * 
 *   Copyright (C) 2016 by Ch. Imfeld. All Rights Reserved.
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Common.Configuration
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
    /// It defines attributes, specifying a zone change function.
    /// </summary>
    [DataContract]
    public class ZoneChangeFunction : Function
    {
        #region Private Members

        /// <summary>
        /// Address of the source related to this zone change function.
        /// </summary>
        [DataMember]
        private Address _sourceId = new Address(SystemConfiguration.ID_UNDEFINED, SystemConfiguration.ID_UNDEFINED);

        /// <summary>
        /// Volume threshold related to this zone change function.
        /// </summary>
        [DataMember]
        private int _volumeThreshold = -1;

        /// <summary>
        /// If true, this function is valid on a (power) status change
        /// </summary>
        [DataMember]
        private bool _onStatusChange = false;

        /// <summary>
        /// If true, this function is valid on a source change
        /// </summary>
        [DataMember]
        private bool _onSourceChange = false;

        /// <summary>
        /// If true, this function is valid on a volume change
        /// </summary>
        [DataMember]
        private bool _onVolumeChange = false;

        /// <summary>
        /// If true, this function is valid on a quality change (online vs. offline)
        /// </summary>
        [DataMember]
        private bool _onQualityChange = false;

        #endregion


        #region Constructors

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public ZoneChangeFunction()
        {
        }

        public ZoneChangeFunction(SimpleId id, Address zoneId, Address sourceId, int volumeThreshold, List<DayOfWeek> validOnDays, List<Command> commands)
            : base(id, zoneId, commands)
        {
            initMembers(id, zoneId, sourceId, volumeThreshold, false, false, false, false, validOnDays, new TimeSpan(), new TimeSpan(), commands);
        }

        public ZoneChangeFunction(SimpleId id, Address zoneId, Address sourceId, int volumeThreshold, List<DayOfWeek> validOnDays, TimeSpan validFrom, TimeSpan validTo, List<Command> commands)
            : base(id, zoneId, commands)
        {
            initMembers(id, zoneId, sourceId, volumeThreshold, false, false, false, false, validOnDays, validFrom, validTo, commands);
        }

        public ZoneChangeFunction(SimpleId id, Address zoneId, Address sourceId, int volumeThreshold, bool OnStatusChange, bool OnSourceChange, bool OnVolumeChange, bool OnQualityChange, List<DayOfWeek> validOnDays, TimeSpan validFrom, TimeSpan validTo, List<Command> commands)
            : base(id, zoneId, commands)
        {
            initMembers(id, zoneId, sourceId, volumeThreshold, OnStatusChange, OnSourceChange, OnVolumeChange, OnQualityChange, validOnDays, validFrom, validTo, commands);
        }

        private void initMembers(SimpleId id, Address zoneId, Address sourceId, int volumeThreshold, bool OnStatusChange, bool OnSourceChange, bool OnVolumeChange, bool OnQualityChange, List<DayOfWeek> validOnDays, TimeSpan validFrom, TimeSpan validTo, List<Command> commands)
        {
            this._sourceId = sourceId;
            this._volumeThreshold = volumeThreshold;
            this._onStatusChange = OnStatusChange;
            this._onSourceChange = OnSourceChange;
            this._onVolumeChange = OnVolumeChange;
            this._onQualityChange = OnQualityChange;
            if (validOnDays != null)
                this._validOnDays = validOnDays;
            if (validFrom != null && validFrom.Ticks > 0 )
                this._validFrom = validFrom;
            if (validTo != null && validTo.Ticks > 0)
                this._validTo = validTo;
        }

        #endregion

        #region Public Interface

        /// <summary>
        /// Address of the source related to this zone change function.
        /// </summary>
        public Address SourceId
        {
            get { return _sourceId; }
        }

        /// <summary>
        /// Volume threshold related to this zone change function.
        /// </summary>
        public int VolumeThreshold
        {
            get { return _volumeThreshold; }
        }

        /// <summary>
        /// If true, this function is valid on a (power) status change
        /// </summary>
        public bool OnStatusChange
        {
            get { return _onStatusChange; }
        }

        /// <summary>
        /// If true, this function is valid on a source change
        /// </summary>
        public bool OnSourceChange
        {
            get { return _onSourceChange; }
        }

        /// <summary>
        /// If true, this function is valid on a volume change
        /// </summary>
        public bool OnVolumeChange
        {
            get { return _onVolumeChange; }
        }

        /// <summary>
        /// If true, this function is valid on a quality change (online vs. offline)
        /// </summary>
        public bool OnQualityChange
        {
            get { return _onQualityChange; }
        }

        #endregion

        
        /// <summary>
        /// Public override of ToString() method.
        /// </summary>
        /// <returns>Returns string representative.</returns>
        public override string ToString()
        {
            return String.Format("ZoneChangeFunction: Zone={0}, Source={1}, VolumeThreshold={2}, OnStatusChange={3}, OnSourceChange={4}, OnVolumeChange={5}, OnQualityChange={6}, Valid from={7} to={8}, {9}", ZoneId, SourceId, VolumeThreshold, OnStatusChange, OnSourceChange, OnVolumeChange, OnQualityChange, ValidFrom, ValidTo, base.ToString());
        }

    }
}
