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
    [DataContract]
    public class Floor
    {
        #region Private Members

        [DataMember]
        private string _name = String.Empty;
        [DataMember]
        private string _floorPlanPath = String.Empty;
        [DataMember]
        private string _floorPlanType = String.Empty;
        [DataMember]
        private List<Zone> _zones = new List<Zone>();

        #endregion

        #region Constructors

        public Floor()
        {
        }

        public Floor(string name, string floorPlanPath, string floorPlanType, List<Zone> zones)
        {
            this._name = name;
            this._floorPlanPath = floorPlanPath;
            this._floorPlanType = floorPlanType;
            this._zones = zones;
        }

        #endregion

        #region Public Interface

        public string Name
        {
            get { return _name; }
        }

        public string FloorPlanPath
        {
            get { return _floorPlanPath; }
        }

        public string FloorPlanType
        {
            get { return _floorPlanType; }
        }

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
