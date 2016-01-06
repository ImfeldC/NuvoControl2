/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
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

using NuvoControl.Server.ZoneServer;
using NuvoControl.Common.Configuration;

namespace NuvoControl.Server.FunctionService
{
    public class ServiceHostFunction: ServiceHost
    {
        private IZoneServer _zoneServer = null;
        private List<Function> _functions = null;

        public ServiceHostFunction(Type serviceType, IZoneServer zoneServer, List<Function> _functions, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
            this._zoneServer = zoneServer;
            this._functions = Functions;
        }

        public IZoneServer ZoneServer
        {
            get { return _zoneServer; }
        }

        public List<Function> Functions
        {
            get { return _functions; }
        }
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
**************************************************************************************************/