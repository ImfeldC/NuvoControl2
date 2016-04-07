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
using NuvoControl.Server.ProtocolDriver;



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

        /// <summary>
        /// All zones of the device.
        /// </summary>
        private Dictionary<Address, Zone> _zones = new Dictionary<Address, Zone>();

        /// <summary>
        /// All sources of the device.
        /// </summary>
        private List<Source> _sources = new List<Source>();

        /// <summary>
        /// The list with current device quality (status)
        /// </summary>
        private Dictionary<int, ZoneQuality> _deviceQualityList = new Dictionary<int, ZoneQuality>();

        /// <summary>
        /// The list with current zone states.
        /// </summary>
        private Dictionary<Address, ZoneState> _zoneStateList = new Dictionary<Address, ZoneState>();

        /// <summary>
        /// Private member to store generic zone id
        /// </summary>
        private int _genericZoneId = -1;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="oscDeviceId">The Id of this osc device.</param>
        /// <param name="oscDevice">The osc device configuration of this osc device.</param>
        /// <param name="protocolDriver">The associated protocol driver to command the device (NuvoEssentia) of the zone</param>
        /// <param name="oscDriver">The associated osc driver to command the osc device.</param>
        public OscDeviceController(Address oscDeviceId, OSCDevice oscDevice, IProtocol protocolDriver, IOscDriver oscDriver, Dictionary<Address,Zone> zones, List<Source> sources)
        {
            this._oscDeviceId = oscDeviceId;
            this._oscDevice = oscDevice;
            this._protocolDriver = protocolDriver;
            this._protocolDriver.onZoneStatusUpdate += new ProtocolZoneUpdatedEventHandler(_protocolDriver_onZoneStatusUpdate);
            this._protocolDriver.onDeviceStatusUpdate += new ProtocolDeviceUpdatedEventHandler(_protocolDriver_onDeviceStatusUpdate);
            this._oscDriver = oscDriver;
            this._oscDriver.onOscNuvoEventReceived += new OscEventReceivedEventHandler(_oscDriver_onOscNuvoEventReceived);
            this._zones = zones;
            this._sources = sources;

            // Init "static" text (configuration data)
            updateConfigurationDataOnOscClient();
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="oscDeviceId">The Id of this osc device.</param>
        /// <param name="oscDevice">The osc device configuration of this osc device.</param>
        /// <param name="oscDeviceController">Controller to copy.</param>
        public OscDeviceController( Address oscDeviceId, OSCDevice oscDevice, OscDeviceController oscDeviceController )
        {
            this._oscDeviceId = oscDeviceId;
            this._oscDevice = oscDevice;
            this._protocolDriver = oscDeviceController._protocolDriver;
            this._protocolDriver.onZoneStatusUpdate += new ProtocolZoneUpdatedEventHandler(_protocolDriver_onZoneStatusUpdate);
            this._protocolDriver.onDeviceStatusUpdate += new ProtocolDeviceUpdatedEventHandler(_protocolDriver_onDeviceStatusUpdate);
            this._oscDriver = new TouchOscDriver(oscDevice);
            this._oscDriver.onOscNuvoEventReceived += new OscEventReceivedEventHandler(_oscDriver_onOscNuvoEventReceived);
            this._zones = oscDeviceController._zones;
            this._sources = oscDeviceController._sources;

            // Init "static" text (configuration data)
            updateConfigurationDataOnOscClient();
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

        /// <summary>
        /// Returns the osc device configuration.
        /// </summary>
        public OSCDevice OscDevice
        {
            get { return _oscDevice; }
        }

        public Address OscDeviceId
        {
            get { return _oscDeviceId; }
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
            if (_oscDriver != null)
            {
                _oscDriver.SendMessage("/NuvoControl/message", String.Format("Zone {0} Update at {1}", e.ZoneAddress, DateTime.Now));

                // Update "dynamic" text (dynamic data)
                UpdateZoneStateOnOscClient(e.ZoneAddress, e.ZoneState);

                // Init "static" text (configuration data)
                updateConfigurationDataOnOscClient();
            }
            StoreZoneStateFromDriver(e.ZoneAddress, e.ZoneState);
        }

        /// <summary>
        /// Update the "dynamic" content on the osc client, based on the zone state
        /// </summary>
        /// <param name="zoneAddress">Zone address, to be updated.</param>
        /// <param name="newState">Zone state, to send to the osc client.</param>
        private void UpdateZoneStateOnOscClient(Address zoneAddress, ZoneState newState)
        {
            if (newState.ZoneQuality == ZoneQuality.Online)
            {
                // Zone is "online" ...
                string zoneBaseAdress = String.Format("/NuvoControl/Zone{0}", zoneAddress.ObjectId);
                _oscDriver.SendMessage(String.Format("{0}/Status", zoneBaseAdress), (newState.PowerStatus ? 1.0 : 0.0));
                _oscDriver.SendMessage(String.Format("{0}/Volume", zoneBaseAdress), newState.Volume);
                _oscDriver.SendMessage(String.Format("{0}/SourceSelection/1/{1}", zoneBaseAdress, newState.Source.ObjectId), 1);
                _oscDriver.SendMessage(String.Format("{0}/message", zoneBaseAdress), String.Format("Zone {0} Update at {1}", zoneAddress, DateTime.Now));

                _oscDriver.SendMessage(String.Format("/NuvoControl/ZoneStatus/{0}/1", zoneAddress.ObjectId), (newState.PowerStatus ? 1.0 : 0.0));
            }
        }

        /// <summary>
        /// Private method to store the zone status received from nuvo control.
        /// </summary>
        /// <param name="zoneAddress">Zone address, of the zone status.</param>
        /// <param name="newState">Zone state, to be stored.</param>
        private void StoreZoneStateFromDriver(Address zoneAddress, ZoneState newState)
        {
            _zoneStateList[zoneAddress] = newState;
            _zoneStateList[zoneAddress].CommandUnacknowledged = false;
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
            if (_oscDriver != null)
            {
                _oscDriver.SendMessage(new OscEvent(eOscEvent.SetValue, "/NuvoControl/NuvoStatus", (e.DeviceQuality == ZoneQuality.Online ? 1.0 : 0.25)));

                // update the device quality. Which in this case means, update the zone quality
                _deviceQualityList[e.DeviceId] = e.DeviceQuality;
                // Init "static" text (configuration data)
                updateConfigurationDataOnOscClient();
            }
        }

        /// <summary>
        /// Private event handler method, to handle an osc nuvo event message.
        /// NOTE: Only osc servers with a defined <ListenPort> will received osc messages. 
        /// Simple clients with <SendPort> only, will not receive messages.
        /// </summary>
        /// <param name="sender">Pointer, to the sender of the event.</param>
        /// <param name="e">Event Argument, contains the osc nuvo event message.</param>
        private void _oscDriver_onOscNuvoEventReceived(object sender, OscEventReceivedEventArgs e)
        {
            _log.Trace(m => m("OSCC.onOscNuvoEventReceived: Osc Device (with id {0}) osc event received: {1}", e.OscDevice, e.OscEvent.ToString()));
            processOscNuvoEventAsServer(new Address(e.OscDeviceId, e.OscEvent.getZoneId), e.OscEvent);
        }


        /// <summary>
        /// Private method which handles the osc messages for the server.
        /// </summary>
        /// <param name="zoneAddress">Zone Address.</param>
        /// <param name="oscEvent">Osc event.</param>
        private void processOscNuvoEventAsServer(Address zoneAddress, OscEvent oscEvent)
        {
            if (oscEvent.OscCommand == eOscEvent.NuvoControl)
            {
                if (oscEvent.OscLabel.Contains("/Status"))
                {
                    if (oscEvent.getOscData == 0)
                    {
                        _protocolDriver.CommandSwitchZoneOFF(zoneAddress);
                    }
                    else if (oscEvent.getOscData == 1)
                    {
                        _protocolDriver.CommandSwitchZoneON(zoneAddress);
                    }
                    else
                    {
                        // Trace error, received an unkown status
                    }
                }
                else if (oscEvent.OscLabel.Contains("/SourceSelection"))
                {
                    if (oscEvent.getOscData == 0)
                    {
                        // ignore message with value 0, as this is only the "disable" message for the previous selected source
                    }
                    else if (oscEvent.getOscData == 1)
                    {
                        _protocolDriver.CommandSetSource(zoneAddress, new Address(zoneAddress.DeviceId, oscEvent.getSourceId));
                    }
                    else
                    {
                        // Trace error, received an unkown source command
                    }
                }
                else if (oscEvent.OscLabel.Contains("/VolumeUp"))
                {
                    if (oscEvent.getOscData == 0)
                    {
                        _protocolDriver.CommandStopRampVolume(zoneAddress);
                    }
                    else if (oscEvent.getOscData == 1)
                    {
                        _protocolDriver.CommandRampVolumeUP(zoneAddress);
                    }
                    else
                    {
                        // Trace error, received an unkown source command
                    }
                }
                else if (oscEvent.OscLabel.Contains("/VolumeDown"))
                {
                    if (oscEvent.getOscData == 0)
                    {
                        _protocolDriver.CommandStopRampVolume(zoneAddress);
                    }
                    else if (oscEvent.getOscData == 1)
                    {
                        _protocolDriver.CommandRampVolumeDOWN(zoneAddress);
                    }
                    else
                    {
                        // Trace error, received an unkown source command
                    }
                }
                else if (oscEvent.OscLabel.Contains("/Volume"))
                {
                    _protocolDriver.CommandSetVolume(zoneAddress, oscEvent.getOscData);
                }
                else if (oscEvent.OscLabel.Contains("/Mute"))
                {
                    if (oscEvent.getOscData == 0)
                    {
                        _protocolDriver.CommandMuteOFF(zoneAddress);
                    }
                    else if (oscEvent.getOscData == 1)
                    {
                        _protocolDriver.CommandMuteON(zoneAddress);
                    }
                    else
                    {
                        // Trace error, received an unkown status
                    }
                }
                else if (oscEvent.OscLabel.Contains("/KeyPadLock"))
                {
                    if (oscEvent.getOscData == 0)
                    {
                        _protocolDriver.CommandSetKeypadLockOFF(zoneAddress);
                    }
                    else if (oscEvent.getOscData == 1)
                    {
                        _protocolDriver.CommandSetKeypadLockON(zoneAddress);
                    }
                    else
                    {
                        // Trace error, received an unkown status
                    }
                }
            }
        }

        /// <summary>
        /// Public method called by the osc server, to process osc event messages.
        /// </summary>
        /// <param name="zoneAddress">Zone address</param>
        /// <param name="oscEvent">Osc event</param>
        public void processOscNuvoEventForClients(Address zoneAddress, OscEvent oscEvent)
        {
            // forward message to client
            _oscDriver.SendMessage(oscEvent);
            // forward (debug) message to client
            _oscDriver.SendMessage("/NuvoControl/message", String.Format("Notify {0}: {1}", _oscDeviceId, oscEvent.ToString()));

            bool bGeneric = false;
            if (oscEvent.OscLabel.Contains("/Generic"))
            {
                if (oscEvent.OscLabel.Contains("/ZoneSelection"))
                {
                    if (oscEvent.getOscData == 0)
                    {
                        // ignore message with value 0, as this is only the "disable" message for the previous selected source
                    }
                    else if (oscEvent.getOscData == 1)
                    {
                        _genericZoneId = oscEvent.getZoneId;
                        _oscDriver.SendMessage(String.Format("/NuvoControl/Generic/ZoneName"), _zones[zoneAddress].Name);
                        _oscDriver.SendMessage("/NuvoControl/message", String.Format("Zone '{0}' selected", _zones[zoneAddress].Name));
                    }
                }
                bGeneric = true;
            }
        }

        #endregion



        /// <summary>
        /// Private method to update "static" configuration data on osc clients
        /// </summary>
        private void updateConfigurationDataOnOscClient()
        {
            if( _oscDriver != null )
            {
                foreach( Zone zone in _zones.Values)
                {
                    string zoneBaseAdress = String.Format("/NuvoControl/Zone{0}", zone.Id.ObjectId);
                    _oscDriver.SendMessage(String.Format("{0}/Name", zoneBaseAdress), zone.Name);
                }
                foreach( Source source in _sources)
                {
                    string sourceBaseAdress = String.Format("/NuvoControl/Source{0}", source.Id.ObjectId);
                    _oscDriver.SendMessage(String.Format("{0}/Name", sourceBaseAdress), source.Name);
                }
            }
        }

        /// <summary>
        /// Private method to update the "dynamic" data on osc clients
        /// </summary>
        private void updateZoneStatusOnOscClient()
        {
            if (_oscDriver != null)
            {
                foreach( Address zoneAddress in _zoneStateList.Keys)
                {
                    UpdateZoneStateOnOscClient(zoneAddress, _zoneStateList[zoneAddress]);
                }
            }
        }
    }
}
