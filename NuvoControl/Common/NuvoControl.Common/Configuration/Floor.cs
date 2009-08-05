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
 *   File Name:      Floor.cs
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
    /// It defines attributes of a floor.
    /// </summary>
    [DataContract]
    public class Floor
    {
        #region Private Members

        /// <summary>
        /// The name of the floor.
        /// </summary>
        [DataMember]
        private string _name = String.Empty;

        /// <summary>
        /// Address of the floor.
        /// </summary>
        [DataMember]
        private Address _id = new Address(SystemConfiguration.ID_UNDEFINED, SystemConfiguration.ID_UNDEFINED);

        /// <summary>
        /// The file name of the floor plan.
        /// </summary>
        [DataMember]
        private string _floorPlanPath = String.Empty;

        /// <summary>
        /// The file type of the floor plan.
        /// </summary>
        [DataMember]
        private string _floorPlanType = String.Empty;

        /// <summary>
        /// All zones of the floor
        /// </summary>
        [DataMember]
        private List<Zone> _zones = new List<Zone>();

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public Floor()
        {
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">Address of the building.</param>
        /// <param name="name">The name of the floor.</param>
        /// <param name="floorPlanPath">The file name of the floor plan.</param>
        /// <param name="floorPlanType">The file type of the floor plan.</param>
        /// <param name="zones">All zones of the floor.</param>
        public Floor(Address id, string name, string floorPlanPath, string floorPlanType, List<Zone> zones)
        {
            this._id = id;
            this._name = name;
            this._floorPlanPath = floorPlanPath;
            this._floorPlanType = floorPlanType;
            this._zones = zones;
        }

        #endregion

        #region Public Interface

        /// <summary>
        /// The name of the floor.
        /// </summary>
        public Address Id
        {
            get { return _id; }
        }

        /// <summary>
        /// The name of the floor.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// The file name of the floor plan.
        /// </summary>
        public string FloorPlanPath
        {
            get { return _floorPlanPath; }
        }

        /// <summary>
        /// The file type of the floor plan.
        /// </summary>
        public string FloorPlanType
        {
            get { return _floorPlanType; }
        }

        /// <summary>
        /// All zones of the floor.
        /// </summary>
        public List<Zone> Zones
        {
            get { return _zones; }
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
