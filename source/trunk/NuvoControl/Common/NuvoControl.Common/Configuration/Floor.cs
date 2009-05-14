/**************************************************************************************************
 * 
 *   Copyright (C) Siemens AG 2006 All Rights Reserved. Confidential
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

namespace NuvoControl.Common.Configuration
{
    [Serializable]
    public class Floor
    {
        #region Private Members

        private string _name = String.Empty;
        private string _floorPlanPath = String.Empty;
        private string _floorPlanType = String.Empty;
        private List<Zone> _zones = new List<Zone>();

        #endregion

        #region Constructors

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
            set { _name = value; }
        }

        public string FloorPlanPath
        {
            get { return _floorPlanPath; }
            set { _floorPlanPath = value; }
        }

        public string FloorPlanType
        {
            get { return _floorPlanType; }
            set { _floorPlanType = value; }
        }

        public List<Zone> Zones
        {
            get { return _zones; }
            set { _zones = value; }
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
