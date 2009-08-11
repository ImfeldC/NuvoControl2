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
 *   File Name:      ServiceConfigurator.cs
 * 
 ***************************************************************************************************
 * 
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NuvoControl.Client.ServiceMock;
using NuvoControl.Client.ServiceAccess;

namespace NuvoControl.Client.Viewer.ServiceAccess
{
    /// <summary>
    /// Configures the underlying service access.
    /// Either a service mock object is instantiated or the real service.
    /// </summary>
    class ServiceConfigurator
    {
        /// <summary>
        /// Configures the underlying service access.
        /// </summary>
        /// <param name="test">Set to true, if the mock service shall be instantiated.</param>
        public static void Configure(bool test)
        {
            if (test == true)
            {
                ServiceProxy.Inject(new ConfigurationProxyMock(Properties.Settings.Default.NuvoControlKonfigurationFile));
                MonitorAndControlProxyMock mcMock = new MonitorAndControlProxyMock();
                ServiceProxy.Inject(mcMock);
                mcMock.SetCallback(ServiceProxy.MonitorAndControlProxy);
            }                

        }
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
