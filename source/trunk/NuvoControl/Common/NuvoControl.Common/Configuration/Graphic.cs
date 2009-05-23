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
 *   File Name:      GraphicKonfiguration.cs
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
    public class Graphic
    {
        #region Private Members

        private Building _building = null;
        private List<Source> _sources = new List<Source>();

        #endregion

        #region Constructors

        public Graphic(Building building, List<Source> sources)
        {
            this._building = building;
            this._sources = sources;
        }

        #endregion

        #region Private Members

        public Building Building
        {
            get { return _building; }
        }

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
