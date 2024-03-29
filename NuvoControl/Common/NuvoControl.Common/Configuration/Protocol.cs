﻿/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Common
 *   Author:         Bernhard Limacher
 *   Creation Date:  19.05.2009
 *   File Name:      Protocol.cs
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
    /// <summary>
    /// This is a system configuration class. 
    /// 
    /// It is a data structurer.
    /// It defines attributes used to instantiate the appropriate protocol driver.
    /// </summary>
    public class Protocol
    {
        #region Private Members

        /// <summary>
        /// The name of the protocol driver.
        /// </summary>
        private string _name = String.Empty;

        /// <summary>
        /// The assembly name containing the protocol driver.
        /// </summary>
        private string _assemblyName = String.Empty;

        /// <summary>
        /// The fully qualified class name of the protocol driver.
        /// </summary>
        private string _className = String.Empty;

        #endregion

        #region Constructors


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the protocol driver.</param>
        /// <param name="assemblyName">The assembly name containing the protocol driver.</param>
        /// <param name="className">The fully qualified class name of the protocol driver.</param>
        public Protocol(string name, string assemblyName, string className)
        {
            this._name = name;
            this._assemblyName = assemblyName;
            this._className = className;
        }

        #endregion

        #region Public Interface

        /// <summary>
        /// The name of the protocol driver.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// The assembly name containing the protocol driver.
        /// </summary>
        public string AssemblyName
        {
            get { return _assemblyName; }
        }

        /// <summary>
        /// The fully qualified class name of the protocol driver.
        /// </summary>
        public string ClassName
        {
            get { return _className; }
        }

        #endregion

        /// <summary>
        /// Returns string representative of this class.
        /// </summary>
        /// <returns>String representative of this class.</returns>
        public override string ToString()
        {
            string strProtocol = "";

            strProtocol += String.Format("Name: {0} /", _name);
            strProtocol += String.Format("Assembly Name: {0} /", _assemblyName);
            strProtocol += String.Format("Class Name: {0} /", _className);

            return strProtocol;
        }
    
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
**************************************************************************************************/