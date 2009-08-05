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
 *   File Name:      Graphic.cs
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
    /// It defines attributes used to create a graphical representation of the whole NuvoControl system.
    /// </summary>
    [DataContract]
    public class Graphic
    {
        #region Private Members

        /// <summary>
        /// The building containing the NuvoControl system.
        /// </summary>
        [DataMember]
        private Building _building = null;

        /// <summary>
        /// All available sources of the NuvoControl system.
        /// </summary>
        [DataMember]
        private List<Source> _sources = new List<Source>();

        #endregion

        #region Constructors

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public Graphic()
        {
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="building">The building containing the NuvoControl system.</param>
        /// <param name="sources">All available sources of the NuvoControl system.</param>
        public Graphic(Building building, List<Source> sources)
        {
            this._building = building;
            this._sources = sources;
        }

        #endregion

        #region Private Members

        /// <summary>
        /// The building containing the NuvoControl system.
        /// </summary>
        public Building Building
        {
            get { return _building; }
        }


        /// <summary>
        /// All available sources of the NuvoControl system.
        /// </summary>
        public List<Source> Sources
        {
            get { return _sources; }
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
