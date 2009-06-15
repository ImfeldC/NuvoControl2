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
 *   File Name:      MonitorAndControl.cs
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 21.05.2009, Bernhard Limacher: Initial implementation.
 * 2) 22.05.2009, Bernhard Limacher: StartSession / EndSession / Subscription / Unsubscription.
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
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class MonitorAndControlService: IMonitorAndControl, IDisposable
    {
        #region Fields

        private static ILog _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Stores the client callback.
        /// </summary>
        private IMonitorAndControlNofification _subscriber = null;

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
        public MonitorAndControlService(IMonitorAndControlNofification callback, IZoneServer zoneServer)
        {
            this._subscriber = callback;
            this._zoneServer = zoneServer;
        }

        #endregion

        #region IMonitorAndControl Members

        /// <summary>
        /// <see cref=">IMonitorAndControl"/>
        /// </summary>
        public void Connect()
        {
            IMonitorAndControlNofification callback = OperationContext.Current.GetCallbackChannel<IMonitorAndControlNofification>();
            this._subscriber = callback;
            ServiceHostMc host = OperationContext.Current.Host as ServiceHostMc;
            Debug.Assert(host != null);
            _zoneServer = host.ZoneServer;
        }


        /// <summary>
        /// <see cref=">IMonitorAndControl"/>
        /// </summary>
        public void Disconnect()
        {
            this._subscriber = null;
            Cleanup();
        }


        /// <summary>
        /// <see cref=">IMonitorAndControl"/>
        /// </summary>
        /// <param name="zoneId"></param>
        /// <param name="stateCommand"></param>
        public void SetZoneState(Address zoneId, ZoneState stateCommand)
        {
            _zoneServer.SetZoneState(zoneId, stateCommand);
        }


        /// <summary>
        /// <see cref=">IMonitorAndControl"/>
        /// </summary>
        /// <param name="zoneId"></param>
        /// <returns></returns>
        public ZoneState GetZoneState(Address zoneId)
        {
            return _zoneServer.GetZoneState(zoneId);
        }


        /// <summary>
        /// <see cref=">IMonitorAndControl"/>
        /// </summary>
        /// <param name="zoneId"></param>
        public void Monitor(Address zoneId)
        {
            _zoneServer.Monitor(zoneId, OnZoneNotification);
            StoreSubscribedZoneId(zoneId);
        }


        /// <summary>
        /// <see cref=">IMonitorAndControl"/>
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
        /// <see cref=">IMonitorAndControl"/>
        /// </summary>
        /// <param name="zoneId"></param>
        public void RemoveMonitor(Address zoneId)
        {
            _zoneServer.RemoveMonitor(zoneId, OnZoneNotification);
            RemoveSubscribedZoneId(zoneId);
        }


        /// <summary>
        /// <see cref=">IMonitorAndControl"/>
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


        private void StoreSubscribedZoneId(Address zoneId)
        {
            if (_zoneSubscriptions.Contains(zoneId) == false)
                _zoneSubscriptions.Add(zoneId);
        }


        private void RemoveSubscribedZoneId(Address zoneId)
        {
            _zoneSubscriptions.Remove(zoneId);
        }


        private void OnZoneNotification(object sender, ZoneStateEventArgs e)
        {
            ZoneController zoneController = sender as ZoneController;
            if (zoneController != null)
                NotifySubscribers(zoneController.UniqueZoneId, e.ZoneState);
        }


        private void NotifySubscribers(Address zoneId, ZoneState zoneState)
        {
            if (_zoneSubscriptions.Contains(zoneId) && _subscriber!= null)
                _subscriber.OnZoneStateChanged(zoneId);
        }


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

