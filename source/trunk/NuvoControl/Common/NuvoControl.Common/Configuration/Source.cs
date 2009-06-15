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
    [DataContract]
    public class Source
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

        public Source()
        {
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

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
