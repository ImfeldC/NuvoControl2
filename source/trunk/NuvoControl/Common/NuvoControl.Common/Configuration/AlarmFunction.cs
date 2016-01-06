/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Common
 *   Author:         Bernhard Limacher
 *   Creation Date:  14.05.2009
 *   File Name:      AlarmFunction.cs
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 14.05.2009, Bernhard Limacher: First implementation.
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
    /// It defines attributes, specifying aa alarm function.
    /// </summary>
    [DataContract]
    public class AlarmFunction : Function
    {
        #region Private Members

        /// <summary>
        /// Alarm time.
        /// </summary>
        [DataMember]
        private TimeSpan _alarmTime = new TimeSpan(7, 0, 0);

        /// <summary>
        /// Duration of the alarm.
        /// </summary>
        [DataMember]
        private TimeSpan _alarmDuration = new TimeSpan(0, 10, 0);

        /// <summary>
        /// Address of the source to play during the alarm.
        /// </summary>
        [DataMember]
        private Address _sourceId = new Address(SystemConfiguration.ID_UNDEFINED, SystemConfiguration.ID_UNDEFINED);

        /// <summary>
        /// Volume to play during the alarm.
        /// </summary>
        [DataMember]
        private int _volume = -1;

        /// <summary>
        /// The days, on which this alarm is valid.
        /// </summary>
        [DataMember]
        private List<DayOfWeek> _validOnDays = new List<DayOfWeek>();

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public AlarmFunction()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">The function id.</param>
        /// <param name="zoneId">The address of the zone, which this function is applied for.</param>
        /// <param name="sourceId">Address of the source to play during the alarm.</param>
        /// <param name="volume">Volume to play the alarm.</param>
        /// <param name="alarmTime">Alarm time.</param>
        /// <param name="alarmDuration">Duration of the alarm.</param>
        /// <param name="validOnDays">The days, on which this alarm is valid.</param>
        public AlarmFunction(Guid id, Address zoneId, Address sourceId, int volume, TimeSpan alarmTime, TimeSpan alarmDuration, List<DayOfWeek> validOnDays)
            : base(id, zoneId)
        {
            this._alarmTime = alarmTime;
            this._alarmDuration = alarmDuration;
            this._sourceId = sourceId;
            this._volume = volume;
            this._validOnDays = validOnDays;
        }

        #endregion

        #region Public Interface

        /// <summary>
        /// Alarm time.
        /// </summary>
        public TimeSpan AlarmTime
        {
            get { return _alarmTime; }
        }

        /// <summary>
        /// Duration of the alarm.
        /// </summary>
        public TimeSpan AlarmDuration
        {
            get { return _alarmDuration; }
        }

        /// <summary>
        /// Address of the source to play during the alarm.
        /// </summary>
        public Address SourceId
        {
            get { return _sourceId; }
        }

        /// <summary>
        /// Volume to play the alarm.
        /// </summary>
        public int Volume
        {
            get { return _volume; }
        }

        /// <summary>
        /// The days, on which this alarm is valid.
        /// </summary>
        public List<DayOfWeek> ValidOnDays
        {
            get { return _validOnDays; }
        }

        /// <summary>
        /// Public override of ToString() method.
        /// </summary>
        /// <returns>Returns string representative.</returns>
        public override string ToString()
        {
            return String.Format("AlarmFunction: Time={0}, Duration={1}, Source={2}, Volume={3}, {4}", AlarmTime, AlarmDuration, SourceId, Volume, base.ToString());
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
**************************************************************************************************/
