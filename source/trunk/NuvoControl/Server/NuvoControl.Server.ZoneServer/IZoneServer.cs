/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.ZoneServer
 *   Author:         Bernhard Limacher
 *   Creation Date:  21.05.2009
 *   File Name:      IZoneServer.cs
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

using NuvoControl.Common;
using NuvoControl.Common.Configuration;
using NuvoControl.Server.ProtocolDriver.Interface;

namespace NuvoControl.Server.ZoneServer
{
    /// <summary>
    /// Defines the interface of the zone server.
    /// 
    /// This server contains an image of state of the connected devices.
    /// It allows to command the devices and to retrieve state of devices.
    /// </summary>
    public interface IZoneServer
    {
        /// <summary>
        /// Initializes the server state by reading the values/state from the devices.
        /// </summary>
        void StartUp();

        /// <summary>
        /// Unloads the zone servers.
        /// </summary>
        void ShutDown();


        /// <summary>
        /// Returns the protocol driver of the specified device.
        /// </summary>
        /// <param name="DeviceId">The device id.</param>
        IProtocol GetProtocolDriver(Address zoneId);

        /// <summary>
        /// Returns the value/state of the specified zone.
        /// </summary>
        /// <param name="zoneId">The zone id.</param>
        /// <exception cref="ArgumentException">Throws exception, if the zone id is not valid.</exception>
        ZoneState GetZoneState(Address zoneId);

        /// <summary>
        /// Sets the zone to a new value/state. This triggers the protocol driver to command the device accordingly.
        /// </summary>
        /// <param name="zoneId">The zone id.</param>
        /// <param name="zoneState">The required state.</param>
        /// <exception cref="ArgumentException">Throws exception, if the zone id is not valid.</exception>
        void SetZoneState(Address zoneId, ZoneState zoneState);

        /// <summary>
        /// Subscirbes a client for value/state changes of the specified zone.
        /// </summary>
        /// <param name="zoneId">The zone id.</param>
        /// <param name="subscriber">The subscriber.</param>
        /// <exception cref="ArgumentException">Throws exception, if the zone id is not valid.</exception>
        void Monitor(Address zoneId, ZoneNotification subscriber);

        /// <summary>
        /// Unsubscribes a client for value/state changes of the specified zone.
        /// </summary>
        /// <param name="zoneId">The zone id.</param>
        /// <param name="subscriber">The subscriber.</param>
        /// <exception cref="ArgumentException">Throws exception, if the zone id is not valid.</exception>
        void RemoveMonitor(Address zoneId, ZoneNotification subscriber);

    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
**************************************************************************************************/

