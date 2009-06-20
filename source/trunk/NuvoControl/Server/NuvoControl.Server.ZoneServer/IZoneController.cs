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
    /// Delegate declaration for the notification to the clients.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ZoneNotification(object sender, ZoneStateEventArgs e);


    [Serializable]
    public class ZoneStateEventArgs : EventArgs
    {
        private ZoneState _zoneState;

        public ZoneState ZoneState
        {
            get { return _zoneState; }
        }

        public ZoneStateEventArgs(ZoneState zoneState)
        {
            _zoneState = zoneState;
        }
    }


    /// <summary>
    /// Defines the interface of a zone controller.
    /// </summary>
    public interface IZoneController
    {
        /// <summary>
        /// The uniqueue Id of the zone.
        /// </summary>
        Address UniqueZoneId { get; }

        /// <summary>
        /// Returns the value/state of this zone.
        /// </summary>
        ZoneState ZoneState { get; }

        /// <summary>
        /// Sets the zone to a new value/state. This triggers the protocol driver to command the NuvoEssentia accordingly.
        /// </summary>
        /// <param name="zoneState"></param>
        void SetZoneState(ZoneState zoneState);

        /// <summary>
        /// Triggers reading the zone state from the devices.
        /// </summary>
        /// <returns></returns>
        void ReadZoneState();

        /// <summary>
        /// Subscirbes a client for value/state changes of this zone.
        /// </summary>
        /// <param name="subscriber"></param>
        void Monitor(ZoneNotification subscriber);

        /// <summary>
        /// Unsubscribes a client for value/state changes of this zone.
        /// </summary>
        /// <param name="subscriber"></param>
        void RemoveMonitor(ZoneNotification subscriber);

        /// <summary>
        /// Triggers the notification of all subscribed clients with the actual zone value/state.
        /// </summary>
        void NotifySubscribedClients();
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/

