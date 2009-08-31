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
 *   File Name:      Function.cs
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
    /// <summary>
    /// This is a system configuration class. 
    /// 
    /// It is a data structurer.
    /// Is used as base class for all function classes.
    /// </summary>
    [DataContract]
    public abstract class Function
    {
        #region Private Members

        /// <summary>
        /// The id of the function.
        /// </summary>
        [DataMember]
        private Guid _id = Guid.NewGuid();

        /// <summary>
        /// The address of the zone, which this function is applied for.
        /// </summary>
        [DataMember]
        private Address _zoneId = new Address(SystemConfiguration.ID_UNDEFINED, SystemConfiguration.ID_UNDEFINED);

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public Function()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">The function id.</param>
        /// <param name="zoneId">The address of the zone, which this function is applied for.</param>
        public Function(Guid id, Address zoneId)
        {
            this._id = id;
            this._zoneId = zoneId;
        }

        #endregion

        #region Public Interface


        /// <summary>
        /// Accessor for the function id.
        /// </summary>
        public Guid Id
        {
            get { return _id; }
        }


        /// <summary>
        /// Accessor for the zone address which this function is applied for.
        /// </summary>
        public Address ZoneId
        {
            get { return _zoneId; }
        }

        /// <summary>
        /// Public override of ToString() method.
        /// </summary>
        /// <returns>Returns string representative.</returns>
        public override string ToString()
        {
            return String.Format("Zone={0}, Guid={1}", ZoneId, Id);
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
