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
 *   File Name:      SystemKonfiguration.cs
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
    public class SystemConfiguration
    {
        #region Private Members

        private Building _building = null;
        private Graphic _graphic = null;
        private List<Function> _functions = new List<Function>();

        #endregion

        #region SystemConfiguration

        public SystemConfiguration(Building building, Graphic graphic, List<Function> functions)
        {
            this._building = building;
            this._graphic = graphic;
            this._functions = functions;
        }

        #endregion

        #region Public Interface

        public Building Building
        {
            get { return _building; }
            set { _building = value; }
        }

        public Graphic Graphic
        {
            get { return _graphic; }
            set { _graphic = value; }
        }

        public List<Function> Functions
        {
            get { return _functions; }
            set { _functions = value; }
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
