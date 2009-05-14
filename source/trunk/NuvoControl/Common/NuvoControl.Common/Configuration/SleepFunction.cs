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

namespace NuvoControl.Common.Configuration
{
    [Serializable]
    public class SleepFunction: Function
    {
        #region Private Members

        private TimeSpan _sleepDuration = new TimeSpan(1, 0, 0);
        private TimeSpan _validFrom = new TimeSpan(11, 0, 0);
        private TimeSpan _validTo = new TimeSpan(04, 0, 0);

        #endregion

        #region Constructors

        public SleepFunction(int id, string name, TimeSpan sleepDuration, TimeSpan validFrom, TimeSpan validTo)
            : base(id, name)
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
            set { _sleepDuration = value; }
        }

        public TimeSpan ValidFrom
        {
            get { return _validFrom; }
            set { _validFrom = value; }
        }

        public TimeSpan ValidTo
        {
            get { return _validTo; }
            set { _validTo = value; }
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
