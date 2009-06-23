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
    /// Source Configuration class. Inherits and implements the interface IComparable.
    /// </summary>
    [DataContract]
    public class Source: IComparable<Source>
    {
        #region Private Members

        [DataMember]
        private Address _id = new Address(SystemConfiguration.ID_UNDEFINED, SystemConfiguration.ID_UNDEFINED);
        [DataMember]
        private string _name = String.Empty;
        [DataMember]
        private string _picturePath = String.Empty;
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

        public Source(Address id)
        {
            this._id = id;
        }

        public Source(Address id, string name, string picturePath, string pictureType)
        {
            this._id = id;
            this._name = name;
            this._picturePath = picturePath;
            this._pictureType = pictureType;
        }

        #endregion

        #region Public Interface

        public Address Id
        {
            get { return _id; }
        }

        public string Name
        {
            get { return _name; }
        }

        public string PicturePath
        {
            get { return _picturePath; }
        }

        public string PictureType
        {
            get { return _pictureType; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            Source source = obj as Source;
            if ((object)source == null)
                return false;

            return (_id == source._id);
        }


        public bool Equals(Source source)
        {
            if ((object)source == null)
                return false;

            return (_id == source._id);
        }


        public static bool operator ==(Source source1, Source source2)
        {
            if ((object)source1 == null)
                return (object)source2 == null;

            return source1.Equals(source2);
        }

        public static bool operator !=(Source source1, Source source2)
        {
            return !(source1 == source2);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        public override string ToString()
        {
            return _name;
        }

        #endregion


        #region IComparable<Source> Members

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
