/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.Service
 *   Author:         Bernhard Limacher
 *   Creation Date:  21.05.2009
 *   File Name:      ZoneController.cs
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 21.05.2009, Bernhard Limacher: Initial implementation (creation of the process model).
 * 2) 22.05.2009, Bernhard Limacher: Subscription / Unsubscription.
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
    /// Defines the interface of a zone server.
    /// </summary>
    public interface IZoneServer
    {
        /// <summary>
        /// Returns the value/state of the specified zone.
        /// </summary>
        /// <param name="zoneId"></param>
        ZoneState GetZoneState(Address zoneId);

        /// <summary>
        /// Sets the zone to a new value/state. This triggers the protocol driver to command the NuvoEssentia accordingly.
        /// </summary>
        /// <param name="zoneId"></param>
        /// <param name="zoneState"></param>
        void SetZoneState(Address zoneId, ZoneState zoneState);

        /// <summary>
        /// Subscirbes a client for value/state changes of this zone.
        /// </summary>
        /// <param name="zoneId"></param>
        /// <param name="subscriber"></param>
        void Monitor(Address zoneId, ZoneNotification subscriber);

        /// <summary>
        /// Unsubscribes a client for value/state changes of this zone.
        /// </summary>
        /// <param name="zoneId"></param>
        /// <param name="subscriber"></param>
        void RemoveMonitor(Address zoneId, ZoneNotification subscriber);

    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/

