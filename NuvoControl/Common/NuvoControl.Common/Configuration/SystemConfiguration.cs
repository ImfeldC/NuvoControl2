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
        #region Constants

        public const string VERSION = "1.0";
        public const int ID_UNDEFINED = -1;
        public const char ID_SEPARATOR = '.';


        #endregion

        #region Private Members

        private Hardware _hardware = null;
        private Graphic _graphic = null;
        private List<Function> _functions = new List<Function>();

        #endregion

        #region SystemConfiguration

        public SystemConfiguration(Hardware hardware, Graphic graphic, List<Function> functions)
        {
            this._hardware = hardware;
            this._graphic = graphic;
            this._functions = functions;
        }

        #endregion

        #region Public Interface

        public Hardware Hardware
        {
            get { return _hardware; }
        }

        public Graphic Graphic
        {
            get { return _graphic; }
        }

        public List<Function> Functions
        {
            get { return _functions; }
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
