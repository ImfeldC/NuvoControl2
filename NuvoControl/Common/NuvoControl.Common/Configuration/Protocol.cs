/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
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
using System.Runtime.Serialization;

namespace NuvoControl.Common.Configuration
{
    [DataContract]
    public class Protocol
    {
        #region Private Members

        [DataMember]
        private string _name = String.Empty;
        [DataMember]
        private string _assemblyName = String.Empty;
        [DataMember]
        private string _className = String.Empty;

        #endregion

        #region Constructors

        public Protocol()
        {
        }

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
        }

        public string AssemblyName
        {
            get { return _assemblyName; }
        }

        public string ClassName
        {
            get { return _className; }
        }


        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/