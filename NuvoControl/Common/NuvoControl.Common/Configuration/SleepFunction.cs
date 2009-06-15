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
    [DataContract]
    public class SleepFunction : Function
    {
        #region Private Members

        [DataMember]
        private TimeSpan _sleepDuration = new TimeSpan(1, 0, 0);
        [DataMember]
        private TimeSpan _validFrom = new TimeSpan(11, 0, 0);
        [DataMember]
        private TimeSpan _validTo = new TimeSpan(04, 0, 0);

        #endregion

        #region Constructors

        public SleepFunction()
        {
        }

        public SleepFunction(Guid id, Address zoneId, TimeSpan sleepDuration, TimeSpan validFrom, TimeSpan validTo)
            : base(id, zoneId)
        {
            this._sleepDuration = sleepDuration;
            this._validFrom = validFrom;
            this._validTo = validTo;
        }

        #endregion

        #region Public Interface

        public TimeSpan SleepDuration
        {
            get { return _sleepDuration; }
        }

        public TimeSpan ValidFrom
        {
            get { return _validFrom; }
        }

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
