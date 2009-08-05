/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Common
 *   Author:         Bernhard Limacher
 *   Creation Date:  14.05.2009
 *   File Name:      SleepFunction.cs
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
    /// It defines attributes, specifying a sleep function.
    /// </summary>
    [DataContract]
    public class SleepFunction : Function
    {
        #region Private Members

        /// <summary>
        /// The duration of the sleep function.
        /// </summary>
        [DataMember]
        private TimeSpan _sleepDuration = new TimeSpan(1, 0, 0);

        /// <summary>
        /// Start time in which a sleep function can be triggered.
        /// </summary>
        [DataMember]
        private TimeSpan _validFrom = new TimeSpan(11, 0, 0);

        /// <summary>
        /// End time in which a sleep function can be triggered.
        /// </summary>
        [DataMember]
        private TimeSpan _validTo = new TimeSpan(04, 0, 0);

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public SleepFunction()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">The function id.</param>
        /// <param name="zoneId">The address of the zone, which this function is applied for.</param>
        /// <param name="sleepDuration">The duration of the sleep function.</param>
        /// <param name="validFrom">Start time in which a sleep function can be triggered.</param>
        /// <param name="validTo">End time in which a sleep function can be triggered.</param>
        public SleepFunction(Guid id, Address zoneId, TimeSpan sleepDuration, TimeSpan validFrom, TimeSpan validTo)
            : base(id, zoneId)
        {
            this._sleepDuration = sleepDuration;
            this._validFrom = validFrom;
            this._validTo = validTo;
        }

        #endregion

        #region Public Interface

        /// <summary>
        /// The duration of the sleep function.
        /// </summary>
        public TimeSpan SleepDuration
        {
            get { return _sleepDuration; }
        }

        /// <summary>
        /// Start time in which a sleep function can be triggered.
        /// </summary>
        public TimeSpan ValidFrom
        {
            get { return _validFrom; }
        }

        /// <summary>
        /// End time in which a sleep function can be triggered.
        /// </summary>
        public TimeSpan ValidTo
        {
            get { return _validTo; }
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
