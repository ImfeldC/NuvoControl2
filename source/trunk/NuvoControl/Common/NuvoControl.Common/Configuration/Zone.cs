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
 *   File Name:      Zone.cs
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
using System.Drawing;
using System.Runtime.Serialization;

namespace NuvoControl.Common.Configuration
{
    /// <summary>
    /// Zone Configuration class.
    /// </summary>
    [DataContract]
    public class Zone
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
        [DataMember]
        private List<Point> _floorPlanCoordinates = new List<Point>();

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public Zone()
        {
        }

        public Zone(Address id, string name, string picturePath, string pictureType, List<Point> floorPlanCoordinates)
        {
            this._id = id;
            this._name = name;
            this._picturePath = picturePath;
            this._pictureType = pictureType;
            this._floorPlanCoordinates = floorPlanCoordinates;
        }

        #endregion

        #region Public Interface Members

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

        public List<Point> FloorPlanCoordinates
        {
            get { return _floorPlanCoordinates; }
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
