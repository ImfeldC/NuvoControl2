/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.MonitorAndControlService
 *   Author:         Bernhard Limacher
 *   Creation Date:  21.05.2009
 *   File Name:      MonitorAndControl.cs
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
using System.Diagnostics;
using System.ServiceModel;

using Common.Logging;

using NuvoControl.Common;
using NuvoControl.Common.Configuration;
using NuvoControl.Server.ZoneServer;

namespace NuvoControl.Server.MonitorAndControlService
{
    /// <summary>
    /// Implements the functions for monitoring and controlling NuvoControl zones.
    /// 
    /// A monitor and control service is WCF service, hosted as sessionful service. Thus an object of this class is
    /// instantiated per client proxy. This allows to keep state per client proxy.
    /// The monitor and control service keeps following state, related to a client: Callback interface and all subscribed zones.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Single)]
    public class MonitorAndControlService: IMonitorAndControl, IDisposable
    {
        #region Fields

        private static ILog _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Stores the client callback.
        /// </summary>
        private IMonitorAndControlNotification _subscriber = null;

        /// <summary>
        /// Stores all zones which are subscribed to.
        /// </summary>
        private List<Address> _zoneSubscriptions = new List<Address>();

        /// <summary>
        /// The zone server.
        /// </summary>
        private IZoneServer _zoneServer = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public MonitorAndControlService()
        {
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="callback">Callback to notify the client.</param>
        /// <param name="zoneServer">The zone server.</param>
        public MonitorAndControlService(IMonitorAndControlNotification callback, IZoneServer zoneServer)
        {
            this._subscriber = callback;
            this._zoneServer = zoneServer;
        }

        #endregion

        #region IMonitorAndControl Members

        /// <summary>
        /// <see cref="IMonitorAndControl"/>
        /// </summary>
        public void Connect()
        {
            Debug.WriteLine("M&C; Connect.");

            IMonitorAndControlNotification callback = OperationContext.Current.GetCallbackChannel<IMonitorAndControlNotification>();
            this._subscriber = callback;
            ServiceHostMc host = OperationContext.Current.Host as ServiceHostMc;
            Debug.Assert(host != null);
            _zoneServer = host.ZoneServer;
        }


        /// <summary>
        /// <see cref="IMonitorAndControl"/>
        /// </summary>
        public void Disconnect()
        {
            Debug.WriteLine("M&C; Disconnect.");

            this._subscriber = null;
            Cleanup();
        }


        /// <summary>
        /// <see cref="IMonitorAndControl"/>
        /// </summary>
        public void RenewLease()
        {
            Debug.WriteLine("M&C; RenewLease.");
        }


        /// <summary>
        /// <see cref=">IMonitorAndControl"/>
        /// </summary>
        /// <param name="zoneId"></param>
        /// <param name="stateCommand"></param>
        public void SetZoneState(Address zoneId, ZoneState stateCommand)
        {
            Debug.WriteLine(String.Format("M&C: SetZoneState: Address={0}, Command={1}", zoneId.ToString(), stateCommand.ToString()));
            _zoneServer.SetZoneState(zoneId, stateCommand);
        }


        /// <summary>
        /// <see cref="IMonitorAndControl"/>
        /// </summary>
        /// <param name="zoneId"></param>
        /// <returns></returns>
        public ZoneState GetZoneState(Address zoneId)
        {
            ZoneState currentZoneState = _zoneServer.GetZoneState(zoneId);
            Debug.WriteLine(String.Format("M&C: GetZoneState: Address={0}, State={1}", zoneId.ToString(), currentZoneState.ToString()));
            return currentZoneState;
        }


        /// <summary>
        /// <see cref="IMonitorAndControl"/>
        /// </summary>
        /// <param name="zoneId"></param>
        public void Monitor(Address zoneId)
        {
            Debug.WriteLine(String.Format("M&C; Monitor; Address: {0}", zoneId));

            _zoneServer.Monitor(zoneId, OnZoneNotification);
            StoreSubscribedZoneId(zoneId);
        }


        /// <summary>
        /// <see cref="IMonitorAndControl"/>
        /// </summary>
        /// <param name="zoneIds"></param>
        public void MonitorMultiple(Address[] zoneIds)
        {
            foreach (Address zoneId in zoneIds)
            {
                Monitor(zoneId);
            }
        }


        /// <summary>
        /// <see cref="IMonitorAndControl"/>
        /// </summary>
        /// <param name="zoneId"></param>
        public void RemoveMonitor(Address zoneId)
        {
            Debug.WriteLine(String.Format("M&C; RemoveMonitor; Address: {0}", zoneId));

            _zoneServer.RemoveMonitor(zoneId, OnZoneNotification);
            RemoveSubscribedZoneId(zoneId);
        }


        /// <summary>
        /// <see cref="IMonitorAndControl"/>
        /// </summary>
        /// <param name="zoneIds"></param>
        public void RemoveMonitorMultiple(Address[] zoneIds)
        {
            foreach (Address zoneId in zoneIds)
            {
                RemoveMonitor(zoneId);
            }

        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Cleanup();
        }

        #endregion

        #region Non-Public Interface

        /// <summary>
        /// Stores the address of the specified zone.
        /// </summary>
        /// <param name="zoneId"></param>
        private void StoreSubscribedZoneId(Address zoneId)
        {
            if (_zoneSubscriptions.Contains(zoneId) == false)
                _zoneSubscriptions.Add(zoneId);
        }


        /// <summary>
        /// Deletes the address of the specified zone.
        /// </summary>
        /// <param name="zoneId"></param>
        private void RemoveSubscribedZoneId(Address zoneId)
        {
            _zoneSubscriptions.Remove(zoneId);
        }


        /// <summary>
        /// Notifcation handler, called from the zone server, which delivers zone state changes
        /// </summary>
        /// <param name="sender">The zone controller, for which the state change appened.</param>
        /// <param name="e">State change event arguments.</param>
        private void OnZoneNotification(object sender, ZoneStateEventArgs e)
        {
            ZoneController zoneController = sender as ZoneController;
            if (zoneController != null)
                NotifySubscribers(zoneController.UniqueZoneId, e.ZoneState);
        }


        /// <summary>
        /// Notifies all subscribed clients for zone state changes.
        /// </summary>
        /// <param name="zoneId">The associated zone id.</param>
        /// <param name="zoneState">The zone state.</param>
        private void NotifySubscribers(Address zoneId, ZoneState zoneState)
        {
            Debug.WriteLine(String.Format("M&C: TryNotifySubscribers, Address={0}, Command={1}", zoneId.ToString(), zoneState.ToString()));

            if (_zoneSubscriptions.Contains(zoneId) && _subscriber != null)
            {
                Debug.WriteLine(String.Format("M&C: NotifySubscribers, Address={0}, Command={1}", zoneId.ToString(), zoneState.ToString()));
                _subscriber.OnZoneStateChanged(zoneId, zoneState);
            }
        }


        /// <summary>
        /// Removes all subscriptions done on the zone server.
        /// </summary>
        private void Cleanup()
        {
            RemoveMonitorMultiple(_zoneSubscriptions.ToArray());
        }

        #endregion

    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/

