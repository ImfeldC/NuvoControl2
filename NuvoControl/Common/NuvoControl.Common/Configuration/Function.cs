/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
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
using System.Runtime.Serialization;

namespace NuvoControl.Common.Configuration
{
    [DataContract]
    public abstract class Function
    {
        #region Private Members

        [DataMember]
        private Guid _id = Guid.NewGuid();
        [DataMember]
        private Address _zoneId = new Address(SystemConfiguration.ID_UNDEFINED, SystemConfiguration.ID_UNDEFINED);

        #endregion

        #region Constructors

        public Function()
        {
        }

        public Function(Guid id, Address zoneId)
        {
            this._id = id;
            this._zoneId = zoneId;
        }

        #endregion

        #region Public Interface

        public Guid Id
        {
            get { return _id; }
        }

        public Address ZoneId
        {
            get { return _zoneId; }
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
