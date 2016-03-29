/**************************************************************************************************
 * 
 *   Copyright (C) 2016 by Ch. Imfeld. All Rights Reserved.
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Common.Logging;

using NuvoControl.Common;
using NuvoControl.Common.Configuration;
using NuvoControl.Server.ProtocolDriver.Interface;



namespace NuvoControl.Server.OscServer
{
    /// <summary>
    /// This is the controller class for a osc device. 
    /// 
    /// It is responsible to command the 'real' osc device via the osc driver.
    /// Upon receiving new values/states via osc driver, it notifies subscribed clients.
    /// Thus, it manages subscribed clients.
    /// </summary>
    public class OscDeviceController
    {
        #region Fields

        private static ILog _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The osc device id of this controller.
        /// </summary>
        private Address _oscDeviceId = null;

        /// <summary>
        /// The osc device configuration of this controller.
        /// </summary>
        private OSCDevice _oscDevice = null;

        /// <summary>
        /// The associated protocol driver.
        /// </summary>
        private IProtocol _protocolDriver = null;

        /// <summary>
        /// The associated osc protocol driver.
        /// </summary>
        private IOscDriver _oscDriver = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="oscDeviceId">The Id of this osc device.</param>
        /// <param name="oscDevice">The osc device configuration of this osc device.</param>
        /// <param name="protocolDriver">The associated protocol driver to command the device (NuvoEssentia) of the zone</param>
        /// <param name="oscDriver">The associated osc driver to command the osc device.</param>
        public OscDeviceController(Address oscDeviceId, OSCDevice oscDevice, IProtocol protocolDriver, IOscDriver oscDriver)
        {
            this._oscDeviceId = oscDeviceId;
            this._oscDevice = oscDevice;
            this._protocolDriver = protocolDriver;
            this._protocolDriver.onZoneStatusUpdate += new ProtocolZoneUpdatedEventHandler(_protocolDriver_onZoneStatusUpdate);
            this._protocolDriver.onDeviceStatusUpdate += new ProtocolDeviceUpdatedEventHandler(_protocolDriver_onDeviceStatusUpdate);
            this._oscDriver = oscDriver;
            this._oscDriver.onOscNuvoEventReceived += new OscEventReceivedEventHandler(_oscDriver_onOscNuvoEventReceived);
        }

        #endregion


        /// <summary>
        /// Returns the protocol driver.
        /// </summary>
        public IProtocol GetProtocolDriver()
        {
            return _protocolDriver;
        }

        /// <summary>
        /// Returns the osc driver.
        /// </summary>
        public IOscDriver GetOscDriver()
        {
            return _oscDriver;
        }

        public OSCDevice OscDevice
        {
            get { return _oscDevice; }
        }



        #region Protocol Driver Notifications

        /// <summary>
        /// Private event handler method, to handle any zone state change.
        /// </summary>
        /// <param name="sender">Pointer, to the sender of the event.</param>
        /// <param name="e">Event Argument, contains the zone state.</param>
        void _protocolDriver_onZoneStatusUpdate(object sender, ProtocolZoneUpdatedEventArgs e)
        {
            _log.Trace(m => m(String.Format("OSCC.onZoneStatusUpdate: Address={0}, Command={1}", e.ZoneAddress.ToString(), e.ZoneState.ToString())));
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
            _log.Trace(m => m("OSCC.onDeviceStatusUpdate: Device (with id {0}) state change received: {1}", e.DeviceId.ToString(), e.DeviceQuality.ToString()));
        }


        private void _oscDriver_onOscNuvoEventReceived(object sender, OscEventReceivedEventArgs e)
        {
            _log.Trace(m => m("OSCC.onOscNuvoEventReceived: Osc Device (with id {0}) osc event received: {1}", e.OscDevice, e.OscEvent.ToString()));
        }

        #endregion

        public Address OscDeviceId
        {
            get { return _oscDeviceId; }
        }


    }
}
