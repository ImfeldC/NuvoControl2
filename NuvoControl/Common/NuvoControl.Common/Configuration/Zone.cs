/**************************************************************************************************
 * 
 *   Copyright (C) 2016 by Ch. Imfeld. All Rights Reserved.
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Common
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace NuvoControl.Common.Configuration
{
    class Zone
    {

        #region Private Members

        /// <summary>
        /// The address of the zone.
        /// </summary>
        [DataMember]
        private Address _id = new Address(SystemConfiguration.ID_UNDEFINED, SystemConfiguration.ID_UNDEFINED);

        /// <summary>
        /// The name of the zone.
        /// </summary>
        [DataMember]
        private string _name = String.Empty;

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public Zone()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">The address of the zone.</param>
        /// <param name="name">The name of the zone.</param>
        public Zone(Address id, string name )
        {
            this._id = id;
            this._name = name;
        }

        #endregion

        #region Public Interface Members

        /// <summary>
        /// The address of the zone.
        /// </summary>
        public Address Id
        {
            get { return _id; }
        }

        /// <summary>
        /// The name of the zone.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Returns a string that represents the zone object.
        /// </summary>
        /// <returns>String representation of this zone.</returns>
        public override string ToString()
        {
            return String.Format("Address={0}, Name={1} ", _id, _name );
        }

        #endregion

    }
}
