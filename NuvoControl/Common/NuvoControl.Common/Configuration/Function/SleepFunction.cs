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

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public SleepFunction()
        {
        }

        /// <summary>
        /// Standard constructor, called by configuration loader (w/o command section) V1.0
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

        /// <summary>
        /// Standard constructor, called by configuration loader (with command section) V2.0
        /// </summary>
        /// <param name="id">The function id.</param>
        /// <param name="zoneId">The address of the zone, which this function is applied for.</param>
        /// <param name="sleepDuration">The duration of the sleep function.</param>
        /// <param name="validFrom">Start time in which a sleep function can be triggered.</param>
        /// <param name="validTo">End time in which a sleep function can be triggered.</param>
        /// <param name="commands">List of commands, to be executed by this function.</param>
        public SleepFunction(Guid id, Address zoneId, TimeSpan sleepDuration, TimeSpan validFrom, TimeSpan validTo, List<Command> commands)
            : base(id, zoneId, commands)
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
        /// Public override of ToString() method.
        /// </summary>
        /// <returns>Returns string representative.</returns>
        public override string ToString()
        {
            return String.Format("SleepFunction: Duration={0}, Valid from={1} to={2}, {3}", SleepDuration, ValidFrom, ValidTo, base.ToString());
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
**************************************************************************************************/
