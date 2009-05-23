/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Common.Interfaces
 *   Author:         Bernhard Limacher
 *   Creation Date:  19.05.2009
 *   File Name:      IMonitorControl.cs
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 19.05.2009, Bernhard Limacher: Definition of the interface.
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NuvoControl.Common.Configuration;

namespace NuvoControl.Common.Interfaces
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
    /// Defines the functions for monitoring and controlling NuvoControl
    /// </summary>
    public interface IMonitorAndControl
    {
        /// <summary>
        /// Commands the state of the specified zone.
        /// </summary>
        /// <param name="clientId">The Id of the commanding client.</param>
        /// <param name="zoneId">The zone Id to be commanded.</param>
        /// <param name="stateCommand">The command value.</param>
        /// <returns>True, in case of success. Otherwise false.</returns>
        bool SetZoneState(Guid clientId, Address zoneId, ZoneState stateCommand);

        /// <summary>
        /// Reads the actual zone state of the specified zone. 
        /// </summary>
        /// <param name="clientId">The Id of the reading client.</param>
        /// <param name="zoneId">The zone Id to be commanded.</param>
        /// <param name="zoneState">The returned zone state.</param>
        /// <returns>True, in case of success. Otherwise false.</returns>
        bool GetZoneState(Guid clientId, Address zoneId, out ZoneState zoneState);

        /// <summary>
        /// Subscribes for value changes of the specified zone.
        /// </summary>
        /// <param name="clientId">The Id of the subscribing client.</param>
        /// <param name="zoneId">The zone Id to subscribe for.</param>
        /// <param name="subscriber">The subscriber.</param>
        /// <returns>True, in case of success. Otherwise false.</returns>
        bool Monitor(Guid clientId, Address zoneId, ZoneNotification subscriber);

        /// <summary>
        /// Subscribes for value changes of the specified zones.
        /// </summary>
        /// <param name="clientId">The Id of the subscribing client.</param>
        /// <param name="zoneIds">The zone Ids to subscribe for.</param>
        /// <param name="subscriber">The subscriber.</param>
        /// <returns>True, in case of success. Otherwise false.</returns>
        bool Monitor(Guid clientId, Address[] zoneIds, ZoneNotification subscriber);

        /// <summary>
        /// Unsubscribes for value changes of the specified zones.
        /// </summary>
        /// <param name="clientId">The Id of the unsubscribing client.</param>
        /// <param name="zoneId">The zone Id to unsubscribe for.</param>
        /// <param name="subscriber">The subscriber.</param>
        /// <returns>True, in case of success. Otherwise false.</returns>
        bool RemoveMonitor(Guid clientId, Address zoneId, ZoneNotification subscriber);

        /// <summary>
        /// Unsubscribes for value changes of the specified zones.
        /// </summary>
        /// <param name="clientId">The Id of the unsubscribing client.</param>
        /// <param name="zoneIds">The zone Ids to unsubscribe for.</param>
        /// <param name="subscriber">The subscriber.</param>
        /// <returns>True, in case of success. Otherwise false.</returns>
        bool RemoveMonitor(Guid clientId, Address[] zoneIds, ZoneNotification subscriber);

        /// <summary>
        /// Unsubscribes for value changes of the specified client.
        /// </summary>
        /// <param name="clientId">The Id of the unsubscribing client.</param>
        /// <returns>True, in case of success. Otherwise false.</returns>
        bool RemoveMonitor(Guid clientId);
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
