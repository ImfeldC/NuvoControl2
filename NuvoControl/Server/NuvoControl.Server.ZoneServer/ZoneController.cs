/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.ZoneServer
 *   Author:         Bernhard Limacher
 *   Creation Date:  21.05.2009
 *   File Name:      ZoneController.cs
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
using System.Threading;

using Common.Logging;

using NuvoControl.Common;
using NuvoControl.Common.Configuration;
using NuvoControl.Server.ProtocolDriver.Interface;
using System.Diagnostics;

namespace NuvoControl.Server.ZoneServer
{
    /// <summary>
    /// This is the controller class for a zone. 
    /// 
    /// It is responsible to command the 'real' zone via the protocol driver.
    /// Upon receiving new values/states via protocol driver, it notifies subscribed clients.
    /// Thus, it manages subscribed clients.
    /// </summary>
    public class ZoneController: IZoneController
    {
        #region Fields

        private static ILog _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The zone id of this controller.
        /// </summary>
        private Address _zoneId = null;

        /// <summary>
        /// The current zone state.
        /// </summary>
        private ZoneState _zoneState = new ZoneState();

        /// <summary>
        /// The associated protocol driver.
        /// </summary>
        private IProtocol _protocolDriver = null;

        /// <summary>
        /// Event, on which clients can subscribe for zone state changes.
        /// </summary>
        private event ZoneNotification _zoneNotification;

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
            this._protocolDriver.onDeviceStatusUpdate += new ProtocolDeviceUpdatedEventHandler(_protocolDriver_onDeviceStatusUpdate);
        }

        #endregion

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
                Debug.WriteLine(String.Format("ZC.SetZoneState: Address={0}, Command={1}", _zoneId.ToString(), _zoneState.ToString()));
                _protocolDriver.SetZoneState(_zoneId, zoneState);
                _zoneState = new ZoneState(zoneState);
                _zoneState.CommandUnacknowledged = true;
            }
            NotifySubscribedClients();
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
            {
                Debug.WriteLine(String.Format("ZC.NotifySubscribers: Address={0}, Command={1}",_zoneId.ToString(), _zoneState.ToString()));
                _zoneNotification(this, new ZoneStateEventArgs(_zoneState));
            }
        }

        #endregion

        #region Protocol Driver Notifications

        /// <summary>
        /// Private event handler method, to handle any zone state change.
        /// </summary>
        /// <param name="sender">Pointer, to the sender of the event.</param>
        /// <param name="e">Event Argument, contains the zone state.</param>
        void _protocolDriver_onZoneStatusUpdate(object sender, ProtocolZoneUpdatedEventArgs e)
        {
            lock (this)
            {
                if (e.ZoneAddress.Equals(_zoneId))
                {
                    Debug.WriteLine(String.Format("ZC.onZoneStatusUpdate: Address={0}, Command={1}", _zoneId.ToString(), e.ZoneState.ToString()));
                    UpdateZoneStateFromDriver(e.ZoneState);
                }
            }
            NotifySubscribedClients();
        }


        /// <summary>
        /// Private event handler method, to handle device state updates.
        /// This event indicates changes of teh device state, from on-line to off-line
        /// or vise versa.
        /// </summary>
        /// <param name="sender">Pointer, to the sender of the event.</param>
        /// <param name="e">Event Argument, contains the device (=zone) quality.</param>
        private void _protocolDriver_onDeviceStatusUpdate(object sender, ProtocolDeviceUpdatedEventArgs e)
        {
            _log.Trace(m => m("Zone {0}: Device (with id {1}) state change received: {2}", _zoneId.ToString(), e.DeviceId, e.DeviceQuality.ToString()));
            lock (this)
            {
                if (e.DeviceId == _zoneId.DeviceId)
                {
                    Debug.WriteLine(String.Format("ZC.onDeviceStatusUpdate: Address={0}, Command={1}", _zoneId.ToString(), _zoneState.ToString()));
                    // update the device quality. Which in this case means, update the zone quality
                    _zoneState.ZoneQuality = e.DeviceQuality;
                }
            }
            NotifySubscribedClients();
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

