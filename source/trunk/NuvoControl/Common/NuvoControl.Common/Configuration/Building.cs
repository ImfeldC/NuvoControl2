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
    /// This is a system configuration class. It is a data structurer.
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
        /// <param name="floors">All floors of the building.</param>
        public Building(List<Floor> floors)
        {
            this._floors = floors;
        }

        #endregion

        #region Private Members

        /// <summary>
        /// Accessor for all floors of the building.
        /// </summary>
        public List<Floor> Floors
        {
            get { return _floors; }
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
