/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Client.Viewer
 *   Author:         Bernhard Limacher
 *   Creation Date:  12.07.2009
 *   File Name:      ServiceProxy.cs
 * 
 ***************************************************************************************************
 * 
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NuvoControl.Client.ServiceAccess.MonitorAndControlService;
using NuvoControl.Client.ServiceAccess.ConfigurationService;

namespace NuvoControl.Client.ServiceAccess
{
    /// <summary>
    /// Static class, used to gain access from all over the viewer to the service proxies.
    /// </summary>
    public static class ServiceProxy
    {
        #region Fields

        /// <summary>
        /// M&C proxy
        /// </summary>
        public static MonitorAndControlProxy _monitorAndControlProxy = null;

        /// <summary>
        /// Configuration proxy
        /// </summary>
        public static ConfigurationProxy _configurationProxy = null;

        #endregion

        #region Public Interface


        /// <summary>
        /// Returns the M&C proxy.
        /// </summary>
        public static MonitorAndControlProxy MonitorAndControlProxy
        {
            get
            {
                if (_monitorAndControlProxy == null)
                    _monitorAndControlProxy = new MonitorAndControlProxy();
                
                return _monitorAndControlProxy;
            }
        }


        /// <summary>
        /// Returns the configuration proxy.
        /// </summary>
        public static ConfigurationProxy ConfigurationProxy
        {
            get
            {
                if (_configurationProxy == null)
                    _configurationProxy = new ConfigurationProxy();

                return _configurationProxy;
            }
        }


        /// <summary>
        /// Injects a configuration service proxy
        /// </summary>
        /// <param name="configurationProxy"></param>
        public static void Inject(IConfigure configurationProxy)
        {
            _configurationProxy = new ConfigurationProxy(configurationProxy);
        }


        /// <summary>
        /// Injects a M&C service proxy
        /// </summary>
        public static void Inject(IMonitorAndControl mAndCProxy)
        {
            _monitorAndControlProxy = new MonitorAndControlProxy(mAndCProxy);
        }

        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/