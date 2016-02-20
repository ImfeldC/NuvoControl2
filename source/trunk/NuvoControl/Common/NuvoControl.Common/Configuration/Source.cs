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
    public class Source : IComparable<Source>
    {
        /// <summary>
        /// Maximum number of sources for a NuVo (Essentia) system.
        /// </summary>
        public const int MAX_SOURCES = 6;

        #region Private Members

        /// <summary>
        /// The address of the source.
        /// </summary>
        [DataMember]
        private Address _id = new Address(SystemConfiguration.ID_UNDEFINED, SystemConfiguration.ID_UNDEFINED);

        /// <summary>
        /// The name of the source.
        /// </summary>
        [DataMember]
        private string _name = String.Empty;

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public Source()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">The address of the source.</param>
        public Source(Address id)
        {
            this._id = id;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">The address of the source.</param>
        /// <param name="name">The name of the source.</param>
        public Source(Address id, string name)
        {
            this._id = id;
            this._name = name;
        }

        #endregion

        #region Public Interface

        /// <summary>
        /// The address of the source.
        /// </summary>
        public Address Id
        {
            get { return _id; }
        }

        /// <summary>
        /// The name of the source.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Return true if the Source object is empty (not initialized).
        /// </summary>
        /// <returns>True if the source object is empty.</returns>
        public bool isEmpty()
        {
            if (_id.ObjectId == SystemConfiguration.ID_UNDEFINED && _id.DeviceId == SystemConfiguration.ID_UNDEFINED)
            {
                return true;
            }
            return false;
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

            Source source = obj as Source;
            if ((object)source == null)
                return false;

            return (_id == source._id);
        }


        /// <summary>
        /// Determines wether the specified source equals the current source.
        /// </summary>
        /// <param name="source">The source to compare with.</param>
        /// <returns>True if the specified source is equal to the current source; otherwise, false.</returns>
        public bool Equals(Source source)
        {
            if ((object)source == null)
                return false;

            return (_id == source._id);
        }


        /// <summary>
        /// Equality operator
        /// </summary>
        /// <param name="source1">Left hand side parameter.</param>
        /// <param name="source2">Reight hand side parameter</param>
        /// <returns>True, if the specified sources are equal; otherwise false.</returns>
        public static bool operator ==(Source source1, Source source2)
        {
            if ((object)source1 == null)
                return (object)source2 == null;

            return source1.Equals(source2);
        }


        /// <summary>
        /// Unequality operator
        /// </summary>
        /// <param name="source1">Left hand side parameter.</param>
        /// <param name="source2">Reight hand side parameter</param>
        /// <returns>True, if the specified sources are unequal; otherwise false.</returns>
        public static bool operator !=(Source source1, Source source2)
        {
            return !(source1 == source2);
        }


        /// <summary>
        /// Hash function for the address type.
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }


        /// <summary>
        /// Returns a string that represents the source object.
        /// </summary>
        /// <returns>String representation of this source.</returns>
        public override string ToString()
        {
            return String.Format("[id={0}, name={1}]", _id, _name);
        }

        #endregion

        #region IComparable<Source> Members

        /// <summary>
        /// Compares the specified source with the current source
        /// </summary>
        /// <param name="other">The source to compare with.</param>
        /// <returns>Less than zero, if the current instance is less than the parameter;
        /// zero, if the sources are equal; Greater than zero, if the current instance is greater than the parameter</returns>
        public int CompareTo(Source other)
        {
            if (_id == other._id)
                return 0;

            if (_id.DeviceId < other._id.DeviceId)
                return -1;
            else if (_id.DeviceId > other._id.DeviceId)
                return 1;
            else
            {
                if (_id.ObjectId < other._id.ObjectId)
                    return -1;
                else
                    return 1;
            }

        }

        #endregion
    }
}
