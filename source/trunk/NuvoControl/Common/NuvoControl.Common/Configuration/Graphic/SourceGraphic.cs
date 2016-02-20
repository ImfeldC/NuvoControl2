/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
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
    /// This is a system configuration class. 
    /// 
    /// It is a data structurer.
    /// Defines graphical attributes of a source.
    /// </summary>
    [DataContract]
    public class SourceGraphic: IComparable<SourceGraphic>
    {
        #region Private Members

        /// <summary>
        /// Source device (with HW settings)
        /// </summary>
        [DataMember]
        private Source _source;

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
        public SourceGraphic()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">The address of the source.</param>
        public SourceGraphic(Address id)
        {
            this._source = new Source(id);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">The address of the source.</param>
        /// <param name="name">The name of the source.</param>
        /// <param name="picturePath">The file name of the source picture.</param>
        /// <param name="pictureType">The file type of the source picture.</param>
        public SourceGraphic(Address id, string name, string picturePath, string pictureType)
        {
            this._source = new Source(id, name);
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
            get { return _source.Id; }
        }

        /// <summary>
        /// The name of the source.
        /// </summary>
        public string Name
        {
            get { return _source.Name; }
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

/*
 * No longer used (after moved base class to Source.cs)
 * 
        /// <summary>
        /// Return true if the Source object is empty (not initialized).
        /// </summary>
        /// <returns>True if the source object is empty.</returns>
        public bool isEmpty()
        {
            return _source.isEmpty();
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

            SourceGraphic source = obj as SourceGraphic;
            if ((object)source == null)
                return false;

            return _source.Equals(source._source);
        }


        /// <summary>
        /// Determines wether the specified source equals the current source.
        /// </summary>
        /// <param name="source">The source to compare with.</param>
        /// <returns>True if the specified source is equal to the current source; otherwise, false.</returns>
        public bool Equals(SourceGraphic source)
        {
            return _source.Equals(source._source);
        }


        /// <summary>
        /// Equality operator
        /// </summary>
        /// <param name="source1">Left hand side parameter.</param>
        /// <param name="source2">Reight hand side parameter</param>
        /// <returns>True, if the specified sources are equal; otherwise false.</returns>
        public static bool operator ==(SourceGraphic source1, SourceGraphic source2)
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
        public static bool operator !=(SourceGraphic source1, SourceGraphic source2)
        {
            return !(source1 == source2);
        }
*/

        /// <summary>
        /// Hash function for the address type.
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            return _source.Id.GetHashCode();
        }


        /// <summary>
        /// Returns a string that represents the source object.
        /// </summary>
        /// <returns>String representation of this source.</returns>
        public override string ToString()
        {
            return String.Format("[id={0}, name={1}] {2}/{3}", _source.Id, _source.Name, _picturePath, _pictureType);
        }

        #endregion

        #region IComparable<SourceGraphic> Members

        /// <summary>
        /// Compares the specified source with the current source
        /// </summary>
        /// <param name="other">The source to compare with.</param>
        /// <returns>Less than zero, if the current instance is less than the parameter;
        /// zero, if the sources are equal; Greater than zero, if the current instance is greater than the parameter</returns>
        public int CompareTo(SourceGraphic other)
        {
            return _source.CompareTo(other._source);
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
**************************************************************************************************/
