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
 *   File Name:      Building.cs
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
    /// It defines a building of a NuvoControl system.
    /// </summary>
    [DataContract]
    public class Building
    {
        #region Private Members

        /// <summary>
        /// All floors of the building.
        /// </summary>
        [DataMember]
        private List<Floor> _floors = new List<Floor>();

        /// <summary>
        /// Address of the building.
        /// </summary>
        [DataMember]
        private Address _id = new Address(SystemConfiguration.ID_UNDEFINED, SystemConfiguration.ID_UNDEFINED);

        /// <summary>
        /// The name of the building.
        /// </summary>
        [DataMember]
        private string _name = String.Empty;

        /// <summary>
        /// The file name of the building picture.
        /// </summary>
        [DataMember]
        private string _picturePath = String.Empty;

        /// <summary>
        /// The file type of the building picture.
        /// </summary>
        [DataMember]
        private string _pictureType = String.Empty;

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public Building()
        {
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">Address of the building.</param>
        /// <param name="name">Name of the building.</param>
        /// <param name="floors">All floors of the building.</param>
        /// <param name="picturePath">The file name of the building picture.</param>
        /// <param name="pictureType">The file type of the building picture.</param>
        public Building(Address id, string name, List<Floor> floors, string picturePath, string pictureType)
        {
            this._id = id;
            this._name = name;
            this._floors = floors;
            this._picturePath = picturePath;
            this._pictureType = pictureType;
        }

        #endregion

        #region Public Interface

        /// <summary>
        /// The address of the buidling.
        /// </summary>
        public Address Id
        {
            get { return _id; }
        }

        /// <summary>
        /// The name of the buidling.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// The file name of the buidling picture.
        /// </summary>
        public string PicturePath
        {
            get { return _picturePath; }
        }

        /// <summary>
        /// The file type of the buidling picture.
        /// </summary>
        public string PictureType
        {
            get { return _pictureType; }
        }

        /// <summary>
        /// Accessor for all floors of the building.
        /// </summary>
        public List<Floor> Floors
        {
            get { return _floors; }
        }

        /// <summary>
        /// Returns a string that represents the building object.
        /// </summary>
        /// <returns>String representation of this building.</returns>
        public override string ToString()
        {
            return String.Format("Id={0}, Name={1}, PicturePath={2}, PictureType={3}, Floors={4}", _id, _name, _picturePath, _pictureType, _floors);
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
