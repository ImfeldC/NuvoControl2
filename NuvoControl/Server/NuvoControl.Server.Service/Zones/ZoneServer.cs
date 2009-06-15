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
 *   File Name:      ZoneServer.cs
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

using Common.Logging;

using NuvoControl.Common.Configuration;
using NuvoControl.Common;
using NuvoControl.Server.Service.Zones;

namespace NuvoControl.Server.Service.Zones
{
    public class ZoneServer: IZoneServer
    {
        #region Fields

        private static ILog _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Holds the zone controller per zone Id
        /// </summary>
        private Dictionary<Address, IZoneController> _zoneControllers = new Dictionary<Address, IZoneController>();


        #endregion

        #region Constructors

        public ZoneServer(List<IZoneController> zoneControllers)
        {
            foreach (IZoneController zoneController in zoneControllers)
            {
                _zoneControllers[zoneController.UniqueZoneId] = zoneController;
            }
        }

        #endregion


        #region IZoneServer Members

        public ZoneState GetZoneState(Address zoneId)
        {
            ValidateZone(zoneId);
            return _zoneControllers[zoneId].ZoneState;
        }


        public void SetZoneState(Address zoneId, ZoneState zoneState)
        {
            ValidateZone(zoneId);
            _zoneControllers[zoneId].SetZoneState(zoneState);
        }

        public void Monitor(Address zoneId, ZoneNotification subscriber)
        {
            ValidateZone(zoneId);
            _zoneControllers[zoneId].Monitor(subscriber);
        }

        public void RemoveMonitor(Address zoneId, ZoneNotification subscriber)
        {
            ValidateZone(zoneId);
            _zoneControllers[zoneId].RemoveMonitor(subscriber);
        }


        #endregion

        #region Non Public Interface

        private void ValidateZone(Address zoneId)
        {
            if (_zoneControllers.ContainsKey(zoneId) == false)
                throw new ArgumentException(String.Format("Invalid zone Id: {0}", zoneId), "zoneId");
        }

        #endregion

    }
}
