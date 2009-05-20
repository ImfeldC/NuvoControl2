/**************************************************************************************************
 * 
 *   Copyright (C) Siemens AG 2006 All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Common
 *   Author:         Bernhard Limacher
 *   Creation Date:  19.05.2009
 *   File Name:      ProtocolDriver.cs
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 19.05.2009, Bernhard Limacher: First implementation.
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuvoControl.Common.Configuration
{
    [Serializable]
    public class Protocol
    {
        #region Private Members

        private string _name = String.Empty;
        private string _assemblyName = String.Empty;
        private string _className = String.Empty;

        #endregion

        #region Constructors

        public Protocol(string name, string assemblyName, string className)
        {
            this._name = name;
            this._assemblyName = assemblyName;
            this._className = className;
        }

        #endregion

        #region Public Interface

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string AssemblyName
        {
            get { return _assemblyName; }
            set { _assemblyName = value; }
        }

        public string ClassName
        {
            get { return _className; }
            set { _className = value; }
        }


        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/