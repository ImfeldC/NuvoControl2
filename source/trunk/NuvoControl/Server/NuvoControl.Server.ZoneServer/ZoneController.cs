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
 * 1) 21.05.2009, Bernhard Limacher: Initial implementation.
 * 2) 22.05.2009, Bernhard Limacher: Subscription / Unsubscription.
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Common.Logging;

using NuvoControl.Common;
using NuvoControl.Common.Configuration;
using NuvoControl.Server.ProtocolDriver.Interface;

namespace NuvoControl.Server.ZoneServer
{
    /// <summary>
    /// This is the controller class for a zone. It is responsible to command the 'real' zone via the protocol driver.
    /// Upon receiving new values/states via protocol driver, it notifies subscribed clients.
    /// Thus, it manages subscribed clients.
    /// </summary>
    public class ZoneController: IZoneController
    {
        #region Fields

        private static ILog _log = LogManager.GetCurrentClassLogger();

        private Address _zoneId = null;
        private ZoneState _zoneState = new ZoneState();
        private IProtocol _protocolDriver = null;
        private event ZoneNotification _zoneNotification;

        Timer timer = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="zoneId">The Id of this zone.</param>
        /// <param name="protocolDriver">The associated protocol driver to command the device (NuvoEssentia) of the zone</param>
        public ZoneController(Address zoneId, IProtocol protocolDriver)
        {
            this._zoneId = zoneId;
            this._protocolDriver = protocolDriver;
            this._protocolDriver.onZoneStatusUpdate += new ProtocolZoneUpdatedEventHandler(_protocolDriver_onZoneStatusUpdate);
            this._protocolDriver.onCommandReceived += new ProtocolCommandReceivedEventHandler(_protocolDriver_onCommandReceived);

            //timer = new Timer(OnTimerCallback, this, Timeout.Infinite, Timeout.Infinite);
            //timer.Change(2000, 2000);
        }

        #endregion

        private void OnTimerCallback(object state)
        {
            //((IZoneController)state).NotifySubscribedClients();
        }

        #region IZoneController Members

        /// <summary>
        /// <see cref="IZoneController"/>
        /// </summary>
        public Address UniqueZoneId
        {
            get { return this._zoneId; }
        }

        /// <summary>
        /// <see cref="IZoneController"/>
        /// </summary>
        public ZoneState ZoneState
        {
            get { return this._zoneState; }
        }

        /// <summary>
        /// <see cref="IZoneController"/>
        /// </summary>
        /// <param name="zoneState"></param>
        public void SetZoneState(ZoneState zoneState)
        {
            lock (this)
            {
                _protocolDriver.SetZoneState(_zoneId, zoneState);
                _zoneState.CommandUnacknowledged = true;
                NotifySubscribedClients();
            }
        }


        /// <summary>
        /// <see cref="IZoneController"/>
        /// </summary>
        public void ReadZoneState()
        {
            lock (this)
            {
                _protocolDriver.ReadZoneState(_zoneId);
            }
        }

        /// <summary>
        /// <see cref="IZoneController"/>
        /// </summary>
        /// <param name="subscriber"></param>
        public void Monitor(ZoneNotification subscriber)
        {
            lock (this)
            {
                try
                {
                    _zoneNotification += subscriber;
                }
                catch (ArgumentException exc)
                {
                    _log.Error("Failed to subscribe for the zone.", exc);
                    throw exc;
                }
            }
        }

        /// <summary>
        /// <see cref="IZoneController"/>
        /// </summary>
        /// <param name="subscriber"></param>
        public void RemoveMonitor(ZoneNotification subscriber)
        {
            lock (this)
            {
                try
                {
                    _zoneNotification -= subscriber;
                }
                catch (Exception exc)
                {
                    _log.Warn("Failed to unsubscribe for the zone.", exc);
                }
            }
        }

        /// <summary>
        /// <see cref="IZoneController"/>
        /// </summary>
        public void NotifySubscribedClients()
        {
            if (_zoneNotification != null)
                _zoneNotification(this, new ZoneStateEventArgs(_zoneState));
        }

        #endregion

        #region Protocol Driver Notifications

        void _protocolDriver_onZoneStatusUpdate(object sender, ProtocolZoneUpdatedEventArgs e)
        {
            lock (_zoneState)
            {
                if (e.ZoneAddress.Equals(_zoneId))
                {
                    UpdateZoneStateFromDriver(e.ZoneState);
                    NotifySubscribedClients();
                }
            }
        }

        void _protocolDriver_onCommandReceived(object sender, ProtocolCommandReceivedEventArgs e)
        {
            // TODO:
            //throw new NotImplementedException();
        }

        private void UpdateZoneStateFromDriver(ZoneState newState)
        {
            _zoneState.ZoneQuality = newState.ZoneQuality;
            if (newState.ZoneQuality == ZoneQuality.Online)
            {
                _zoneState.Volume = newState.Volume;
                _zoneState.Source = newState.Source;
                _zoneState.PowerStatus = newState.PowerStatus;
                _zoneState.LastUpdate = newState.LastUpdate;
            }
            _zoneState.CommandUnacknowledged = false;
        }


        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/

