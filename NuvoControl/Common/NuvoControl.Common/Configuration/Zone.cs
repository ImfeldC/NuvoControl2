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
using System.ServiceModel;

namespace NuvoControl.Common.Configuration
{
    [DataContract]
    public class Zone
    {
        #region Private Members

        private Address _id = new Address(SystemConfiguration.ID_UNDEFINED, SystemConfiguration.ID_UNDEFINED);
        private string _name = String.Empty;
        string _picturePath = String.Empty;
        private string _pictureType = String.Empty;
        private List<Point> _floorPlanCoordinates = new List<Point>();

        #endregion

        #region Constructors

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

        [DataMember]
        public Address Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [DataMember]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [DataMember]
        public string PicturePath
        {
            get { return _picturePath; }
            set { _picturePath = value; }
        }

        [DataMember]
        public string PictureType
        {
            get { return _pictureType; }
            set { _pictureType = value; }
        }

        [DataMember]
        public List<Point> FloorPlanCoordinates
        {
            get { return _floorPlanCoordinates; }
            set { _floorPlanCoordinates = value; }
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
