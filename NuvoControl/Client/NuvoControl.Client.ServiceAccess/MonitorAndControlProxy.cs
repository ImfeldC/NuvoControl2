using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Diagnostics;

using Common.Logging;
using NuvoControl.Common;
using NuvoControl.Common.Configuration;
using NuvoControl.Client.ServiceAccess.MonitorAndControlService;

namespace NuvoControl.Client.ServiceAccess
{
    [CallbackBehavior(UseSynchronizationContext = false)]
    public class MonitorAndControlProxy: IMonitorAndControlCallback, IDisposable
    {
        private class ZoneProxy
        {
             /// <summary>
            /// Event, on which clients can subscribe for zone state changes.
            /// </summary>
            private event ZoneNotification _zoneNotification;
            /// <summary>
            /// The zone, for which a client wants to subscribe.
            /// </summary>
            Address _zoneId;

            Dictionary<ZoneNotification, ZoneNotification> subscribers = new Dictionary<ZoneNotification, ZoneNotification>();

            public ZoneProxy(Address zoneId)
            {
                this._zoneId = zoneId;
            }

            public Address ZoneId
            {
                get { return _zoneId; }
            }

            public void AddSubscriber(ZoneNotification zoneNotification)
            {
                if (subscribers.ContainsKey(zoneNotification))
                    return;

                subscribers.Add(zoneNotification, zoneNotification);
                _zoneNotification += zoneNotification;
                return;
            }

            public int RemoveSubscriber(ZoneNotification zoneNotification)
            {
                if (subscribers.ContainsKey(zoneNotification) == false)
                    return subscribers.Count;

                subscribers.Remove(zoneNotification);
                _zoneNotification -= zoneNotification;
                return subscribers.Count;
            }

            public void NotifyClients(ZoneState zoneState)
            {
                if (_zoneNotification != null)
                    _zoneNotification(this, new ZoneStateEventArgs(zoneState));
            }
        }




        IMonitorAndControl _mcServiceProxy;
        // Track whether Dispose has been called.
        private bool disposed = false;

        private static ILog _log = LogManager.GetCurrentClassLogger();

        private Dictionary<Address, ZoneProxy> _zoneProxies = new Dictionary<Address, ZoneProxy>();


        public MonitorAndControlProxy(IMonitorAndControl mcServiceProxy)
        {
            this._mcServiceProxy = mcServiceProxy;
        }

        public MonitorAndControlProxy()
        {
            Initialize();
        }

        public ZoneState GetZoneState(Address zoneId)
        {
            Debug.WriteLine(String.Format("M&C Proxy; GetZoneState(); Address: {0}", zoneId));

            return _mcServiceProxy.GetZoneState(zoneId);
        }

        public void SetZoneState(Address zoneId, ZoneState command)
        {
            Debug.WriteLine(String.Format("M&C Proxy; SetZoneState(); Address: {0}, Command: {1}", zoneId, command));

            _mcServiceProxy.SetZoneState(zoneId, command);
        }

        public void Monitor(Address zoneId, ZoneNotification subscriber)
        {
            Debug.WriteLine(String.Format("M&C Proxy; Monitor(); Address: {0}", zoneId));

            try
            {
                if (_zoneProxies.ContainsKey(zoneId) == false)
                {
                    _zoneProxies.Add(zoneId, new ZoneProxy(zoneId));
                    _mcServiceProxy.Monitor(zoneId);
                }
                _zoneProxies[zoneId].AddSubscriber(subscriber);
            }
            catch (ArgumentException exc)
            {
                _log.Error("Failed to subscribe for the zone.", exc);
                throw exc;
            }
        }

        public void RemoveMonitor(Address zoneId, ZoneNotification subscriber)
        {
            Debug.WriteLine(String.Format("M&C Proxy; RemoveMonitor(); Address: {0}", zoneId));

            try
            {
                if (_zoneProxies.ContainsKey(zoneId) == false)
                    return;

                if (_zoneProxies[zoneId].RemoveSubscriber(subscriber) == 0)
                {
                    _mcServiceProxy.RemoveMonitor(zoneId);
                    _zoneProxies.Remove(zoneId);
                }
            }
            catch (Exception exc)
            {
                _log.Warn("Failed to unsubscribe for the zone.", exc);
            }
        }


        private void Initialize()
        {

            try
            {
                Debug.WriteLine("M&C Proxy; Initialize()");

                IMonitorAndControlCallback serverCallback = this;
                _mcServiceProxy = new MonitorAndControlClient(new InstanceContext(serverCallback));
                (_mcServiceProxy as MonitorAndControlClient).SetClientBaseAddress();
                _mcServiceProxy.Connect();

                Debug.WriteLine("M&C Proxy; Initialize() done.");
            }
            catch (Exception exc)
            {
                (_mcServiceProxy as MonitorAndControlClient).Abort();
            }     
        }

        #region IMonitorAndControlCallback Members

        public void OnZoneStateChanged(Address zoneId, ZoneState zoneState)
        {
            Debug.WriteLine(String.Format("M&C Proxy; OnZoneStateChanged(); Address: {0}, State: {1}", zoneId, zoneState));

            if (_zoneProxies.ContainsKey(zoneId))
                _zoneProxies[zoneId].NotifyClients(zoneState);
        }

        #endregion

        #region IDisposable Members

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);

        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if (_mcServiceProxy != null)
                    {
                        _mcServiceProxy.Disconnect();
                    }
                }

                // Note disposing has been done.
                disposed = true;

            }
        }
        #endregion
    }

    /// <summary>
    /// Delegate declaration for the notification to the clients.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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
