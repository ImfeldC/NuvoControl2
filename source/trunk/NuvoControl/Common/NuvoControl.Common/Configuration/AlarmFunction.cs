/**************************************************************************************************
 * 
 *   Copyright (C) Siemens AG 2006 All Rights Reserved. Confidential
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

namespace NuvoControl.Common.Configuration
{
    [Serializable]
    public class AlarmFunction: Function
    {
        #region Private Members

        private TimeSpan _alarmTime = new TimeSpan(7, 0, 0);
        private TimeSpan _alarmDuration = new TimeSpan(0, 10, 0);
        private UniqueSourceId _uniqueSourceId = new UniqueSourceId(SystemConfiguration.ID_UNDEFINED, SystemConfiguration.ID_UNDEFINED);
        private List<DayOfWeek> _validOnDays = new List<DayOfWeek>();

        #endregion

        #region Constructors

        public AlarmFunction(Guid id, UniqueZoneId uniqueZoneId, UniqueSourceId uniqueSourceId, TimeSpan alarmTime, TimeSpan alarmDuration, List<DayOfWeek> validOnDays)
            : base(id, uniqueZoneId)
        {
            this._alarmTime = alarmTime;
            this._alarmDuration = alarmDuration;
            this._uniqueSourceId = uniqueSourceId;
            this._validOnDays = validOnDays;
        }

        #endregion

        #region Public Interface

        public TimeSpan AlarmTime
        {
            get { return _alarmTime; }
            set { _alarmTime = value; }
        }

        public TimeSpan AlarmDuration
        {
            get { return _alarmDuration; }
            set { _alarmDuration = value; }
        }

        public UniqueSourceId UniqueSourceId
        {
            get { return _uniqueSourceId; }
            set { _uniqueSourceId = value; }
        }

        public List<DayOfWeek> ValidOnDays
        {
            get { return _validOnDays; }
            set { _validOnDays = value; }
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
