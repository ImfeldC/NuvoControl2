﻿/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
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
        private List<ZoneGraphic> _zones = new List<ZoneGraphic>();

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
        public Floor(Address id, string name, string floorPlanPath, string floorPlanType, List<ZoneGraphic> zones)
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
        public List<ZoneGraphic> Zones
        {
            get { return _zones; }
        }

        /// <summary>
        /// Returns a string that represents the floor object.
        /// </summary>
        /// <returns>String representation of this floor.</returns>
        public override string ToString()
        {
            return String.Format("Name={0}, Id={1}, FloorPlanPath={2}, FloorPlanType={3}, Zones=[{4}]", _name, _id, _floorPlanPath, _floorPlanType, _zones.ToString<ZoneGraphic>(" / "));
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
**************************************************************************************************/
