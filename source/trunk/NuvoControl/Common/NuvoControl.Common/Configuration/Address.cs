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
 *   File Name:      Address.cs
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
    /// This is a system configuration class. It is a data structurer.
    /// It defines a unique address of any object of NuvoEssentia or any other device.
    /// A zone or a source is uniquely addressed with such objects.
    /// </summary>
    [DataContract]
    public class Address
    {
        #region Private Members

        /// <summary>
        /// The id of the object within a NuvoEssentia device.
        /// </summary>
        [DataMember]
        private int _objectId = SystemConfiguration.ID_UNDEFINED;

        /// <summary>
        /// The id of the device (e.g. NuvoEssentia device).
        /// </summary>
        [DataMember]
        private int _deviceId = SystemConfiguration.ID_UNDEFINED;

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public Address()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="deviceId">Device id.</param>
        /// <param name="objectId">Object id</param>
        public Address(int deviceId, int objectId)
        {
            this._deviceId = deviceId;
            this._objectId = objectId;
        }

        #endregion

        #region Public Interface

        /// <summary>
        /// Accessor for the object id.
        /// </summary>
        public int ObjectId
        {
            get { return _objectId; }
        }


        /// <summary>
        /// Accessor for the device id
        /// </summary>
        public int DeviceId
        {
            get { return _deviceId; }
        }


        /// <summary>
        /// Determines wether the specified object equals the current object.
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        /// <returns>True if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            Address id = obj as Address;
            if ((object)id == null)
                return false;

            return (_deviceId == id._deviceId) && (_objectId == id._objectId);
        }

        /// <summary>
        /// Determines wether the specified address equals the current address.
        /// </summary>
        /// <param name="id">The address to compare with.</param>
        /// <returns>True if the specified address is equal to the current address; otherwise, false.</returns>
        public bool Equals(Address id)
        {
            if ((object)id == null)
                return false;

            return (_deviceId == id._deviceId) && (_objectId == id._objectId);
        }


        /// <summary>
        /// Equality operator
        /// </summary>
        /// <param name="id1">Left hand side parameter.</param>
        /// <param name="id2">Reight hand side parameter</param>
        /// <returns>True, if the specified addresses are equal; otherwise false.</returns>
        public static bool operator ==(Address id1, Address id2)
        {
            if ((object)id1 == null)
                return (object)id2 == null;

            return id1.Equals(id2);
        }


        /// <summary>
        /// Unequality operator
        /// </summary>
        /// <param name="id1">Left hand side parameter.</param>
        /// <param name="id2">Reight hand side parameter</param>
        /// <returns>True, if the specified addresses are unequal; otherwise false.</returns>
        public static bool operator !=(Address id1, Address id2)
        {
            return !(id1 == id2);
        }


        /// <summary>
        /// Hash function for the address type.
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            return _deviceId ^ _objectId;
        }


        /// <summary>
        /// Returns a string that represents the current address.
        /// </summary>
        /// <returns>String representation of this address.</returns>
        public override string ToString()
        {
            return String.Format("{0}.{1}", _deviceId, _objectId);
        }


        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/