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

using Common.Logging;

using NuvoControl.Common;
using NuvoControl.Common.Configuration;
using NuvoControl.Common.Interfaces;

namespace NuvoControl.Server.Service.MandC
{
    /// <summary>
    /// Implements the functions for monitoring and controlling NuvoControl zones.
    /// </summary>
    public class MonitorAndControl: IMonitorAndControl
    {
        #region Fields

        private static ILog _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Holds the zone controller per zone Id
        /// </summary>
        private Dictionary<Address, IZoneController> _zoneControllers = new Dictionary<Address, IZoneController>();
        
        /// <summary>
        /// Holds the connected clients.
        /// </summary>
        private Dictionary<Guid, Guid> _clients = null;

        /// <summary>
        /// Holds all subscribers per client.
        /// </summary>
        private Dictionary<Guid, List<ZoneNotification>> _subscribersPerClient = new Dictionary<Guid, List<ZoneNotification>>();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="zoneControllers">All zone controllers of the system.</param>
        /// <param name="clients">The list of connected clients.</param>
        public MonitorAndControl(List<IZoneController> zoneControllers, Dictionary<Guid, Guid> clients)
        {
            this._clients = clients;
            foreach (IZoneController zoneController in zoneControllers)
            {
                _zoneControllers[zoneController.UniqueZoneId] = zoneController;
            }
        }

        #endregion

        #region IMonitorAndControl Members

        /// <summary>
        /// <see cref=">IMonitorAndControl"/>
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="zoneId"></param>
        /// <param name="stateCommand"></param>
        /// <returns></returns>
        public bool SetZoneState(Guid clientId, Address zoneId, ZoneState stateCommand)
        {
            lock (this)
            {
                if (ValidateClientAndZoneId(clientId, zoneId) == false)
                    return false;

                _zoneControllers[zoneId].SetZoneState(stateCommand);
                return true;
            }
        }

        /// <summary>
        /// <see cref=">IMonitorAndControl"/>
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="zoneId"></param>
        /// <param name="zoneState"></param>
        /// <returns></returns>
        public bool GetZoneState(Guid clientId, Address zoneId, out ZoneState zoneState)
        {
            lock (this)
            {
                zoneState = null;
                if (ValidateClientAndZoneId(clientId, zoneId) == false)
                    return false;

                zoneState = _zoneControllers[zoneId].ZoneState;
                return true;
            }
        }

        /// <summary>
        /// <see cref=">IMonitorAndControl"/>
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="zoneId"></param>
        /// <param name="subscriber"></param>
        /// <returns></returns>
        public bool Monitor(Guid clientId, Address zoneId, ZoneNotification subscriber)
        {
            lock (this)
            {
                if (ValidateClientAndZoneId(clientId, zoneId) == false)
                    return false;

                _zoneControllers[zoneId].Monitor(subscriber);
                AddSubscriber(clientId, subscriber);
                return true;
            }
        }

        /// <summary>
        /// <see cref=">IMonitorAndControl"/>
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="zoneIds"></param>
        /// <param name="subscriber"></param>
        /// <returns></returns>
        public bool Monitor(Guid clientId, Address[] zoneIds, ZoneNotification subscriber)
        {
            lock (this)
            {
                if (ValidateClientId(clientId) == false)
                    return false;

                foreach (Address zoneId in zoneIds)
                {
                    if (ValidateZoneId(zoneId) == false)
                        continue;

                    _zoneControllers[zoneId].Monitor(subscriber);
                    AddSubscriber(clientId, subscriber);
                }

                return true;
            }
        }

        /// <summary>
        /// <see cref=">IMonitorAndControl"/>
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="zoneId"></param>
        /// <param name="subscriber"></param>
        /// <returns></returns>
        public bool RemoveMonitor(Guid clientId, Address zoneId, ZoneNotification subscriber)
        {
            lock (this)
            {
                if (ValidateClientAndZoneId(clientId, zoneId) == false)
                    return false;

                _zoneControllers[zoneId].RemoveMonitor(subscriber);
                RemoveSubscriber(clientId, subscriber);
                return true;
            }
        }

        /// <summary>
        /// <see cref=">IMonitorAndControl"/>
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="zoneIds"></param>
        /// <param name="subscriber"></param>
        /// <returns></returns>
        public bool RemoveMonitor(Guid clientId, Address[] zoneIds, ZoneNotification subscriber)
        {
            lock (this)
            {
                if (ValidateClientId(clientId) == false)
                    return false;

                foreach (Address zoneId in zoneIds)
                {
                    if (ValidateZoneId(zoneId) == false)
                        continue;

                    _zoneControllers[zoneId].RemoveMonitor(subscriber);
                    RemoveSubscriber(clientId, subscriber);
                }

                return true;
            }
        }

        /// <summary>
        /// <see cref=">IMonitorAndControl"/>
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public bool RemoveMonitor(Guid clientId)
        {
            lock (this)
            {
                if (ValidateClientId(clientId) == false)
                    return false;

                if (_subscribersPerClient.ContainsKey(clientId) == false)
                    return true;

                foreach (ZoneNotification subscriber in _subscribersPerClient[clientId])
                {
                    foreach (IZoneController zoneController in _zoneControllers.Values)
                    {
                        zoneController.RemoveMonitor(subscriber);
                    }
                    RemoveSubscriber(clientId, subscriber);
                }

                return true;
            }
        }

        #endregion

        #region Non-Public Interface

        /// <summary>
        /// Checks, if the client Id is connected to the serviceand thus valid.
        /// </summary>
        /// <param name="clientId">Id of the client.</param>
        /// <returns>True, if valid. Otherwise false.</returns>
        private bool ValidateClientId(Guid clientId)
        {
            if (_clients.ContainsKey(clientId) == false)
            {
                _log.WarnFormat("Client Id is unkown: {0}", clientId);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks, if the zone Id is valid.
        /// </summary>
        /// <param name="zoneId">Id of the zone.</param>
        /// <returns>True, if valid. Otherwise false.</returns>
        private bool ValidateZoneId(Address zoneId)
        {
            if (_zoneControllers.ContainsKey(zoneId) == false)
            {
                _log.WarnFormat("Zone Id is unkown: {0}", zoneId.ToString());
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks, if the client Id is connected to the service and if the zone Id is valid.
        /// </summary>
        /// <param name="clientId">Id of the client.</param>
        /// <param name="zoneId">Id of the zone.</param>
        /// <returns>True, if valid. Otherwise false.</returns>
        private bool ValidateClientAndZoneId(Guid clientId, Address zoneId)
        {
            if (ValidateClientId(clientId) == false)
                return false;

            if (ValidateZoneId(zoneId) == false)
                return false;

            return true;
        }

        /// <summary>
        /// Stores the association between subscriber and client.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="subscriber"></param>
        private void AddSubscriber(Guid clientId, ZoneNotification subscriber)
        {
            if (_subscribersPerClient.ContainsKey(clientId) == false)
                _subscribersPerClient[clientId] = new List<ZoneNotification>();

            _subscribersPerClient[clientId].Add(subscriber);
        }

        /// <summary>
        /// Removes the association between subscriber and client.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="subscriber"></param>
        private void RemoveSubscriber(Guid clientId, ZoneNotification subscriber)
        {
            if (_subscribersPerClient.ContainsKey(clientId) == false)
                return;

            List<ZoneNotification> toBeDeleted = new List<ZoneNotification>();
            foreach (ZoneNotification sub in _subscribersPerClient[clientId])
            {
                if (sub.Equals(subscriber))
                    toBeDeleted.Add(sub);
            }

            foreach (ZoneNotification toDelete in toBeDeleted)
            {
                _subscribersPerClient[clientId].Remove(toDelete);
            }
        }

        #endregion

    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/

