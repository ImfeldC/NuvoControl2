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

using Common.Logging;

using NuvoControl.Common;
using NuvoControl.Common.Configuration;
using NuvoControl.Server.ProtocolDriver.Interface;
using NuvoControl.Common.Interfaces;

namespace NuvoControl.Server.Service.MandC
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// <see cref="IZoneController"/>
        /// </summary>
        /// <param name="subscriber"></param>
        /// <returns></returns>
        public bool Monitor(ZoneNotification subscriber)
        {
            try
            {
                _zoneNotification += subscriber;
                return true;
            }
            catch (ArgumentException exc)
            {
                _log.Error("Failed to subscribe for the zone.", exc);
                return false;
            }
        }

        /// <summary>
        /// <see cref="IZoneController"/>
        /// </summary>
        /// <param name="subscriber"></param>
        /// <returns></returns>
        public bool RemoveMonitor(ZoneNotification subscriber)
        {
            try
            {
                _zoneNotification -= subscriber;
                return true;
            }
            catch (Exception exc)
            {
                _log.Error("Failed to unsubscribe for the zone.", exc);
                return false;
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
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/

