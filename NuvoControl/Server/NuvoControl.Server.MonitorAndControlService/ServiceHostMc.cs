/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.MonitorAndControlService
 *   Author:         Bernhard Limacher
 *   Creation Date:  14.06.2009
 *   File Name:      ServiceHostMc.cs
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

using NuvoControl.Server.ZoneServer;

namespace NuvoControl.Server.MonitorAndControlService
{
    /// <summary>
    /// Derivation of the <see cref="ServiceHost"/> class needed to add access to the zone server.
    /// </summary>
    public class ServiceHostMc: ServiceHost
    {
        /// <summary>
        /// The zone server.
        /// </summary>
        private IZoneServer _zoneServer = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="zoneServer">The zone server.</param>
        /// <param name="baseAddresses">Options base addresses of the service.</param>
        public ServiceHostMc(Type serviceType, IZoneServer zoneServer, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
            this._zoneServer = zoneServer;
        }

        /// <summary>
        /// The zone server.
        /// </summary>
        public IZoneServer ZoneServer
        {
            get { return _zoneServer; }
        }
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
**************************************************************************************************/