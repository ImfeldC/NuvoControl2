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
 *   File Name:      Source.cs
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
    /// Defines graphical attributes of a source.
    /// </summary>
    [DataContract]
    public class Source: IComparable<Source>
    {
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

        /// <summary>
        /// The file name of the source picture.
        /// </summary>
        [DataMember]
        private string _picturePath = String.Empty;

        /// <summary>
        /// The file type of the source picture.
        /// </summary>
        [DataMember]
        private string _pictureType = String.Empty;

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
        /// <param name="picturePath">The file name of the source picture.</param>
        /// <param name="pictureType">The file type of the source picture.</param>
        public Source(Address id, string name, string picturePath, string pictureType)
        {
            this._id = id;
            this._name = name;
            this._picturePath = picturePath;
            this._pictureType = pictureType;
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
        /// The file name of the source picture.
        /// </summary>
        public string PicturePath
        {
            get { return _picturePath; }
        }


        /// <summary>
        /// The file type of the source picture.
        /// </summary>
        public string PictureType
        {
            get { return _pictureType; }
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
        /// <param name="id">The source to compare with.</param>
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
        /// <param name="id1">Left hand side parameter.</param>
        /// <param name="id2">Reight hand side parameter</param>
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
        /// <param name="id1">Left hand side parameter.</param>
        /// <param name="id2">Reight hand side parameter</param>
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
        /// Returns a string that represents the current address.
        /// </summary>
        /// <returns>String representation of this address.</returns>
        public override string ToString()
        {
            return _name;
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

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
