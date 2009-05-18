/**************************************************************************************************
 * 
 *   Copyright (C) Siemens AG 2006 All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Common
 *   Author:         Bernhard Limacher
 *   Creation Date:  12.05.2009
 *   File Name:      FunctionConfiguration.cs
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 12.05.2009, Bernhard Limacher: First implementation.
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuvoControl.Common.Configuration
{
    [Serializable]
    public abstract class Function
    {
        #region Private Members

        private Guid _id = Guid.NewGuid();
        private UniqueZoneId _uniqueZoneId = new UniqueZoneId(SystemConfiguration.ID_UNDEFINED, SystemConfiguration.ID_UNDEFINED);

        #endregion

        #region Constructors

        public Function(Guid id, UniqueZoneId uniqueZoneId)
        {
            this._id = id;
            this._uniqueZoneId = uniqueZoneId;
        }

        #endregion

        #region Public Interface

        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public UniqueZoneId ZoneId
        {
            get { return _uniqueZoneId; }
            set { _uniqueZoneId = value; }
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
