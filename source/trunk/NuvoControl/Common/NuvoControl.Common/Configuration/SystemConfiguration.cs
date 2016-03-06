/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Common
 *   Author:         Bernhard Limacher
 *   Creation Date:  12.05.2009
 *   File Name:      SystemConfiguration.cs
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
    /// It is the root of the whole NuvoControl system configuration.
    /// </summary>
    public class SystemConfiguration
    {
        #region Constants

        /// <summary>
        /// Public constant defining the system configuration version.
        /// </summary>
        public const string VERSION = "3.0";

        /// <summary>
        /// Public constant defining 'ID Undefined'.
        /// </summary>
        public const int ID_UNDEFINED = -1;

        /// <summary>
        /// Public constant defining 'ID Separator'.
        /// </summary>
        public const char ID_SEPARATOR = '.';


        #endregion

        #region Private Members

        /// <summary>
        /// The version of the configuration (read from xml file)
        /// </summary>
        private string _configurationVersion = "";

        /// <summary>
        /// The connected hardware (devices)
        /// </summary>
        private Hardware _hardware = null;

        /// <summary>
        /// Specifies graphical attributes of the system.
        /// </summary>
        private Graphic _graphic = null;

        /// <summary>
        /// Specifies all functions (alarm and sleep) of the system.
        /// </summary>
        private List<Function> _functions = new List<Function>();

        #endregion

        #region SystemConfiguration


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configurationVersion">The version of the configuration file.</param>
        /// <param name="hardware">The connected hardware (devices)</param>
        /// <param name="graphic">Specifies graphical attributes of the system.</param>
        /// <param name="functions">Specifies all functions (alarm and sleep) of the system.</param>
        public SystemConfiguration(string configurationVersion, Hardware hardware, Graphic graphic, List<Function> functions)
        {
            this._configurationVersion = configurationVersion;
            this._hardware = hardware;
            this._graphic = graphic;
            this._functions = functions;
        }

        #endregion

        #region Public Interface

        /// <summary>
        /// Returns the version of the configuration file.
        /// </summary>
        public string ConfigurationVersion
        {
            get { return _configurationVersion; }
        }

        /// <summary>
        /// The connected hardware (devices)
        /// </summary>
        public Hardware Hardware
        {
            get { return _hardware; }
        }

        /// <summary>
        /// Specifies graphical attributes of the system.
        /// </summary>
        public Graphic Graphic
        {
            get { return _graphic; }
        }

        /// <summary>
        /// Specifies all functions (alarm and sleep) of the system.
        /// </summary>
        public List<Function> Functions
        {
            get { return _functions; }
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
**************************************************************************************************/
