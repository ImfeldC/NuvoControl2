/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.Service
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

using NuvoControl.Server.Service.Zones;

namespace NuvoControl.Server.Service.MandC
{
    public class ServiceHostMc: ServiceHost
    {
        private IZoneServer _zoneServer = null;

        public ServiceHostMc(Type serviceType, IZoneServer zoneServer, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
            this._zoneServer = zoneServer;
        }

        public IZoneServer ZoneServer
        {
            get { return _zoneServer; }
        }
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/