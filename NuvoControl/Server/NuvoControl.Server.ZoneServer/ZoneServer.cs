/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.ZoneServer
 *   Author:         Bernhard Limacher
 *   Creation Date:  14.06.2009
 *   File Name:      ZoneServer.cs
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 21.05.2009, Bernhard Limacher: Initial implementation.
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common.Logging;

using NuvoControl.Common.Configuration;
using NuvoControl.Common;


namespace NuvoControl.Server.ZoneServer
{
    /// <summary>
    /// Implements the interface of the zone server.
    /// 
    /// This server contains an image of state of the connected devices.
    /// It allows to command the devices and to retrieve state of devices.
    /// Typically, the zone server is instantiated once per Nuvo Control server.
    /// </summary>
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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="zoneControllers">All zone controllers.</param>
        public ZoneServer(List<IZoneController> zoneControllers)
        {
            foreach (IZoneController zoneController in zoneControllers)
            {
                _zoneControllers[zoneController.UniqueZoneId] = zoneController;
            }
        }

        #endregion

        #region IZoneServer Members

        /// <summary>
        /// <see cref="IZoneServer"/>
        /// </summary>
        public void StartUp()
        {
            lock (this)
            {
                foreach (IZoneController zoneController in _zoneControllers.Values)
                {
                    zoneController.ReadZoneState();
                }
            }
        }

        /// <summary>
        /// <see cref="IZoneServer"/>
        /// </summary>
        public void ShutDown()
        {
            lock (this)
            {
                foreach (IZoneController zoneController in _zoneControllers.Values)
                {
                    zoneController.RemoveAllMonitor();
                }
                _zoneControllers.Clear();
            }
        }


        /// <summary>
        /// <see cref="IZoneServer"/>
        /// </summary>
        /// <param name="zoneId"></param>
        /// <returns></returns>
        public ZoneState GetZoneState(Address zoneId)
        {
            lock (this)
            {
                ValidateZone(zoneId);
                return _zoneControllers[zoneId].ZoneState;
            }
        }


        /// <summary>
        /// <see cref="IZoneServer"/>
        /// </summary>
        /// <param name="zoneId"></param>
        /// <param name="zoneState"></param>
        public void SetZoneState(Address zoneId, ZoneState zoneState)
        {
            lock (this)
            {
                ValidateZone(zoneId);
                _zoneControllers[zoneId].SetZoneState(zoneState);
            }
        }


        /// <summary>
        /// <see cref="IZoneServer"/>
        /// </summary>
        /// <param name="zoneId"></param>
        /// <param name="subscriber"></param>
        public void Monitor(Address zoneId, ZoneNotification subscriber)
        {
            lock (this)
            {
                ValidateZone(zoneId);
                _zoneControllers[zoneId].Monitor(subscriber);
            }
        }


        /// <summary>
        /// <see cref="IZoneServer"/>
        /// </summary>
        /// <param name="zoneId"></param>
        /// <param name="subscriber"></param>
        public void RemoveMonitor(Address zoneId, ZoneNotification subscriber)
        {
            lock (this)
            {
                ValidateZone(zoneId);
                _zoneControllers[zoneId].RemoveMonitor(subscriber);
            }
        }

        #endregion

        #region Non Public Interface

        /// <summary>
        /// Validate the zone id.
        /// </summary>
        /// <param name="zoneId">The zone id.</param>
        /// <exception cref="ArgumentException">Throws exception, if the zone id is not valid.</exception>
        private void ValidateZone(Address zoneId)
        {
            if (_zoneControllers.ContainsKey(zoneId) == false)
                throw new ArgumentException(String.Format("Invalid zone Id: {0}", zoneId), "zoneId");
        }

        #endregion

    }
}
