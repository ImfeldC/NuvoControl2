/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Client.Viewer
 *   Author:         Bernhard Limacher
 *   Creation Date:  12.07.2009
 *   File Name:      MonitorAndControlProxy.cs
 * 
 ***************************************************************************************************
 * 
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Diagnostics;
using System.Threading;
using System.ServiceModel.Discovery;

using Common.Logging;
using NuvoControl.Common;
using NuvoControl.Common.Configuration;
using NuvoControl.Client.ServiceAccess.MonitorAndControlService;

namespace NuvoControl.Client.ServiceAccess
{
    /// <summary>
    /// Monitor and Control service proxy class.
    /// Handles renewing the lease time.
    /// Dispatches the notifications of the service to the appropriate zone contexts.
    /// </summary>
    [CallbackBehavior(UseSynchronizationContext = false)]
    public class MonitorAndControlProxy: IMonitorAndControlCallback, IDisposable
    {
        #region Nested Class

        /// <summary>
        /// Stores the client (zone context) subscriptions per zone
        /// </summary>
        private class ZoneSubscriptions
        {
            #region Fields

            /// <summary>
            /// Event, on which clients can subscribe for zone state changes.
            /// </summary>
            private event ZoneNotification _zoneNotification;

            /// <summary>
            /// The zone, for which a client wants to subscribe.
            /// </summary>
            Address _zoneId;

            /// <summary>
            /// The subscribers
            /// </summary>
            Dictionary<ZoneNotification, ZoneNotification> subscribers = new Dictionary<ZoneNotification, ZoneNotification>();

            #endregion

            #region Constructors

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="zoneId">Id of the zone.</param>
            public ZoneSubscriptions(Address zoneId)
            {
                this._zoneId = zoneId;
            }

            #endregion

            #region Public Interface

            /// <summary>
            /// Id of the zone.
            /// </summary>
            public Address ZoneId
            {
                get { return _zoneId; }
            }


            /// <summary>
            /// Add a new subriber to this zone.
            /// </summary>
            /// <param name="zoneNotification">Delegate of the subscriber.</param>
            public void AddSubscriber(ZoneNotification zoneNotification)
            {
                if (subscribers.ContainsKey(zoneNotification))
                    return;

                subscribers.Add(zoneNotification, zoneNotification);
                _zoneNotification += zoneNotification;
                return;
            }


            /// <summary>
            /// Remove the subscriber from this zone.
            /// </summary>
            /// <param name="zoneNotification">Delegate of the subscriber.</param>
            /// <returns>The number of subscribers.</returns>
            public int RemoveSubscriber(ZoneNotification zoneNotification)
            {
                if (subscribers.ContainsKey(zoneNotification) == false)
                    return subscribers.Count;

                subscribers.Remove(zoneNotification);
                _zoneNotification -= zoneNotification;
                return subscribers.Count;
            }


            /// <summary>
            /// Notifies all subscribers with the new zone state.
            /// </summary>
            /// <param name="zoneState">The new zone state.</param>
            public void NotifyClients(ZoneState zoneState)
            {
                if (_zoneNotification != null)
                    _zoneNotification(this, new ZoneStateEventArgs(zoneState));
            }

            #endregion
        }

        #endregion

        #region Fields

        /// <summary>
        /// Renew the lease after every 30 seconds
        /// </summary>
        private const int RENEW_LEASE_TIME = 30000;

        /// <summary>
        /// The WCF service proxy.
        /// </summary>
        private IMonitorAndControl _mcServiceProxy;

        /// <summary>
        /// WCF discovery service response.
        /// </summary>
        private FindResponse _discovered;

        /// <summary>
        /// Track, whether Dispose has been called.
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Trace logger
        /// </summary>
        private static ILog _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Keeps the subscriptions per zone.
        /// </summary>
        private Dictionary<Address, ZoneSubscriptions> _zoneSubscriptions = new Dictionary<Address, ZoneSubscriptions>();

        /// <summary>
        /// Timer, used to renew the lease periodically.
        /// </summary>
        private Timer _timerRenewLease;

        #endregion

        #region Constructors

        /// <summary>
        /// Contructor, injects the service proxy
        /// </summary>
        /// <param name="mcServiceProxy"></param>
        public MonitorAndControlProxy(IMonitorAndControl mcServiceProxy)
        {
            this._mcServiceProxy = mcServiceProxy;
        }


        /// <summary>
        /// Default constructor, instantiates the M&C service proxy.
        /// </summary>
        public MonitorAndControlProxy()
        {
            Initialize("localhost");
        }


        /// <summary>
        /// Constructor, instantiates the M&C service proxy.
        /// </summary>
        /// <param name="clientIpOrName">The IP address or the name of the machine.</param>
        public MonitorAndControlProxy(string clientIpOrName)
        {
            if ((clientIpOrName == null) || (clientIpOrName == String.Empty))
                Initialize("localhost");
            else
                Initialize(clientIpOrName);
        }

        #endregion

        #region Public Interface

        /// <summary>
        /// Reads the zone state from the service.
        /// </summary>
        /// <param name="zoneId">Address of the zone.</param>
        /// <returns>The read zone state.</returns>
        public ZoneState GetZoneState(Address zoneId)
        {
            _log.Trace(m=>m(String.Format("M&C Proxy; GetZoneState(); Address: {0}", zoneId)));

            return _mcServiceProxy.GetZoneState(zoneId);
        }


        /// <summary>
        /// Commands the zone state.
        /// </summary>
        /// <param name="zoneId">Address of the zone.</param>
        /// <param name="command">The new zone state.</param>
        public void SetZoneState(Address zoneId, ZoneState command)
        {
            _log.Trace(m=>m(String.Format("M&C Proxy; SetZoneState(); Address: {0}, Command: {1}", zoneId, command)));

            _mcServiceProxy.SetZoneState(zoneId, command);
        }


        /// <summary>
        /// Monitors a zone. (Subscribes for zone state changes.)
        /// </summary>
        /// <param name="zoneId">Address of the zone.</param>
        /// <param name="subscriber">The subscriber.</param>
        public void Monitor(Address zoneId, ZoneNotification subscriber)
        {
            _log.Trace(m=>m(String.Format("M&C Proxy; Monitor(); Address: {0}", zoneId)));

            try
            {
                if (_zoneSubscriptions.ContainsKey(zoneId) == false)
                {
                    _zoneSubscriptions.Add(zoneId, new ZoneSubscriptions(zoneId));
                    _mcServiceProxy.Monitor(zoneId);
                }
                _zoneSubscriptions[zoneId].AddSubscriber(subscriber);
            }
            catch (ArgumentException exc)
            {
                _log.Error("Failed to subscribe for the zone.", exc);
                throw exc;
            }
        }


        /// <summary>
        /// Removes a monitor for a zone. (Unubscribes for zone state changes.)
        /// </summary>
        /// <param name="zoneId">Address of the zone.</param>
        /// <param name="subscriber">The subscriber.</param>
        public void RemoveMonitor(Address zoneId, ZoneNotification subscriber)
        {
            _log.Trace(m=>m(String.Format("M&C Proxy; RemoveMonitor(); Address: {0}", zoneId)));

            try
            {
                if (_zoneSubscriptions.ContainsKey(zoneId) == false)
                    return;

                if (_zoneSubscriptions[zoneId].RemoveSubscriber(subscriber) == 0)
                {
                    _mcServiceProxy.RemoveMonitor(zoneId);
                    _zoneSubscriptions.Remove(zoneId);
                }
            }
            catch (Exception exc)
            {
                _log.Warn("Failed to unsubscribe for the zone.", exc);
            }
        }

        #endregion

        #region Non-Public Interface

        /// <summary>
        /// Initializes the connection to the service.
        /// Starts the timer to periodically renew the lease.
        /// </summary>
        /// <param name="clientIpOrName">The IP address or the name of the machine.</param>
        private void Initialize(string clientIpOrName)
        {
            try
            {
                _log.Trace(m=>m("M&C Proxy; Initialize()"));
                _discovered = DiscoverService();

                IMonitorAndControlCallback serverCallback = this;
                _mcServiceProxy = new MonitorAndControlClient(new InstanceContext(serverCallback), "WSDualHttpBinding_IMonitorAndControl", ServiceProxy.buildEndpointAddress("WSDualHttpBinding_IMonitorAndControl"));
                (_mcServiceProxy as MonitorAndControlClient).SetClientBaseAddress(clientIpOrName);
                _mcServiceProxy.Connect();

                _timerRenewLease = new Timer(OnRenewLeaseCallback);
                _timerRenewLease.Change(RENEW_LEASE_TIME, Timeout.Infinite);

                _log.Trace(m=>m("M&C Proxy; Initialize() done."));
            }
            catch (Exception exc)
            {
                _log.Error("Creating connection to the service failed.", exc);
                (_mcServiceProxy as MonitorAndControlClient).Abort();
            }     
        }


        /// <summary>
        /// Discovery method for the IMonitorAndControl service.
        /// </summary>
        /// <returns>Returns the discivered endpoints.</returns>
        private FindResponse DiscoverService()
        {
            // ------- DISCOVERY ----------

            _log.Trace(m => m("M&C Proxy; Start discovering ..."));

            DiscoveryClient discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint());
            FindCriteria criteria = new FindCriteria(typeof(IMonitorAndControl));
            FindResponse discovered = discoveryClient.Find(criteria);
            discoveryClient.Close();

            _log.Trace(m => m("M&C Proxy; Discovery: {0} services found.", discovered.Endpoints.Count));
            LogHelper.LogEndPoint(_log, discovered.Endpoints);

            // ----------------------------

            return discovered;
        }

        /// <summary>
        /// Timer callback to renew the lease time.
        /// </summary>
        /// <param name="obj"></param>
        private void OnRenewLeaseCallback(object obj)
        {
            try
            {
                _timerRenewLease.Change(RENEW_LEASE_TIME, Timeout.Infinite);
                _mcServiceProxy.RenewLease();
            }
            catch (Exception exc)
            {
                _log.Fatal(m=>m("Renew lease for M&C failed. Exception message: " + exc.Message));
            }
        }

        #endregion

        #region IMonitorAndControlCallback Members

        /// <summary>
        /// Notification from service on zone state changes.
        /// </summary>
        /// <param name="zoneId"></param>
        /// <param name="zoneState"></param>
        public void OnZoneStateChanged(Address zoneId, ZoneState zoneState)
        {
            _log.Trace(m=>m(String.Format("M&C Proxy; OnZoneStateChanged(); Address: {0}, State: {1}", zoneId, zoneState)));

            if (_zoneSubscriptions.ContainsKey(zoneId))
                _zoneSubscriptions[zoneId].NotifyClients(zoneState);
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes the M&C service proxy and the internal time
        /// </summary>
        public void Dispose()
        {
            lock (this)
            {
                if (_disposed == false)
                {
                    if (_mcServiceProxy != null)
                    {
                        if (_mcServiceProxy != null)
                            _mcServiceProxy.Disconnect();
                    }
                    if (_timerRenewLease != null)
                    {
                        _timerRenewLease.Dispose();
                        _timerRenewLease = null;
                    }

                    // Note disposing has been done.
                    _disposed = true;

                }
            }
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    /// <summary>
    /// Delegate declaration for the notification to the clients.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">The zone state.</param>
    public delegate void ZoneNotification(object sender, ZoneStateEventArgs e);

    /// <summary>
    /// Contains the event arguments for zone state change events.
    /// </summary>
    public class ZoneStateEventArgs : EventArgs
    {
        /// <summary>
        /// The state of the zone.
        /// </summary>
        private ZoneState _zoneState;

        /// <summary>
        /// The state of the zone.
        /// </summary>
        public ZoneState ZoneState
        {
            get { return _zoneState; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="zoneState">The state of the zone.</param>
        public ZoneStateEventArgs(ZoneState zoneState)
        {
            _zoneState = zoneState;
        }
    }

}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/