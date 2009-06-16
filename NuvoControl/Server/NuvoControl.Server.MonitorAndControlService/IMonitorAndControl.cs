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
using System.ServiceModel;

using NuvoControl.Common.Configuration;
using NuvoControl.Common;

namespace NuvoControl.Server.MonitorAndControlService
{
    public interface IMonitorAndControlNotification
    {
        [OperationContract(IsOneWay = true)]
        void OnZoneStateChanged(Address zoneId, ZoneState zoneState);
    }

    /// <summary>
    /// Defines the functions for monitoring and controlling NuvoControl
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IMonitorAndControlNotification))]
    public interface IMonitorAndControl
    {
        /// <summary>
        /// This method shall be called from the client after instanitating the proxy.
        /// Stores the callback object, used for client notifications.
        /// </summary>
        [OperationContract]
        void Connect();

        /// <summary>
        /// Disconnects the client, does remove all subscriptions of the client.
        /// </summary>
        [OperationContract(IsInitiating = false, IsTerminating = true)]
        void Disconnect();

        /// <summary>
        /// Commands the state of the specified zone.
        /// </summary>
        /// <param name="zoneId">The zone Id to be commanded.</param>
        /// <param name="stateCommand">The command value.</param>
        [OperationContract(IsInitiating = false)]
        void SetZoneState(Address zoneId, ZoneState stateCommand);

        /// <summary>
        /// Reads the actual zone state of the specified zone. 
        /// </summary>
        /// <param name="zoneId">The zone Id to be commanded.</param>
        /// <returns>The state of the zone.</returns>
        [OperationContract(IsInitiating = false)]
        ZoneState GetZoneState(Address zoneId);

        /// <summary>
        /// Subscribes for value changes of the specified zone.
        /// </summary>
        /// <param name="zoneId">The zone Id to subscribe for.</param>
        [OperationContract(IsInitiating = false)]
        void Monitor(Address zoneId);

        /// <summary>
        /// Subscribes for value changes of the specified zones.
        /// </summary>
        /// <param name="zoneIds">The zone Ids to subscribe for.</param>
        [OperationContract(IsInitiating = false)]
        void MonitorMultiple(Address[] zoneIds);

        /// <summary>
        /// Unsubscribes for value changes of the specified zones.
        /// </summary>
        /// <param name="zoneId">The zone Id to unsubscribe for.</param>
        [OperationContract(IsInitiating = false)]
        void RemoveMonitor(Address zoneId);

        /// <summary>
        /// Unsubscribes for value changes of the specified zones.
        /// </summary>
        /// <param name="zoneIds">The zone Ids to unsubscribe for.</param>
        [OperationContract(IsInitiating = false)]
        void RemoveMonitorMultiple(Address[] zoneIds);
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
