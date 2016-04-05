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
        private List<Zone> _zones = new List<Zone>();

        /// <summary>
        /// All sources of the device.
        /// </summary>
        private List<Source> _sources = new List<Source>();

        /// <summary>
        /// The current zone state.
        /// </summary>
        private ZoneState _zoneState = new ZoneState();

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="oscDeviceId">The Id of this osc device.</param>
        /// <param name="oscDevice">The osc device configuration of this osc device.</param>
        /// <param name="protocolDriver">The associated protocol driver to command the device (NuvoEssentia) of the zone</param>
        /// <param name="oscDriver">The associated osc driver to command the osc device.</param>
        public OscDeviceController(Address oscDeviceId, OSCDevice oscDevice, IProtocol protocolDriver, IOscDriver oscDriver, List<Zone> zones, List<Source> sources)
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

                if (e.ZoneState.ZoneQuality == ZoneQuality.Online)
                {
                    string zoneBaseAdress = String.Format("/NuvoControl/Zone{0}", e.ZoneAddress.ObjectId);
                    // Zone is "online" ...
                    _oscDriver.SendMessage(String.Format("{0}/Status", zoneBaseAdress), (e.ZoneState.PowerStatus ? 1.0 : 0.0));
                    _oscDriver.SendMessage(String.Format("{0}/Volume", zoneBaseAdress), e.ZoneState.Volume);
                    _oscDriver.SendMessage(String.Format("{0}/SourceSelection/1/{1}", zoneBaseAdress, e.ZoneState.Source.ObjectId), 1);
                    _oscDriver.SendMessage(String.Format("{0}/message", zoneBaseAdress), String.Format("Zone {0} Update at {1}", e.ZoneAddress, DateTime.Now));
                }

                // Init "static" text (configuration data)
                updateConfigurationDataOnOscClient();
            }
            UpdateZoneStateFromDriver(e.ZoneState);
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
                _zoneState.ZoneQuality = e.DeviceQuality;
                // Init "static" text (configuration data)
                updateConfigurationDataOnOscClient();
            }
        }

        /// <summary>
        /// Private event handler method, to handle an osc nuvo event message.
        /// </summary>
        /// <param name="sender">Pointer, to the sender of the event.</param>
        /// <param name="e">Event Argument, contains the osc nuvo event message.</param>
        private void _oscDriver_onOscNuvoEventReceived(object sender, OscEventReceivedEventArgs e)
        {
            _log.Trace(m => m("OSCC.onOscNuvoEventReceived: Osc Device (with id {0}) osc event received: {1}", e.OscDevice, e.OscEvent.ToString()));

            if( e.OscEvent.OscCommand == eOscEvent.NuvoControl )
            {

                if( e.OscEvent.OscLabel.Contains("/Status") )
                {
                    if (e.OscEvent.getOscData == 0)
                    {
                        _protocolDriver.CommandSwitchZoneOFF(new Address(e.OscDeviceId, e.OscEvent.getZoneId));
                    }
                    else if (e.OscEvent.getOscData == 1)
                    {
                        _protocolDriver.CommandSwitchZoneON(new Address(e.OscDeviceId, e.OscEvent.getZoneId));
                    }
                    else
                    {
                        // Trace error, received an unkown status
                    }
                }
                else if (e.OscEvent.OscLabel.Contains("/SourceSelection"))
                {
                    if (e.OscEvent.getOscData == 0)
                    {
                        // ignore message with value 0, as this is only the "disable" message for the previous selected source
                    }
                    else if (e.OscEvent.getOscData == 1)
                    {
                        _protocolDriver.CommandSetSource(new Address(e.OscDeviceId, e.OscEvent.getZoneId), new Address(e.OscDeviceId, e.OscEvent.getSourceId));
                    }
                    else
                    {
                        // Trace error, received an unkown source command
                    }
                }
                else if (e.OscEvent.OscLabel.Contains("/VolumeUp"))
                {
                    if (e.OscEvent.getOscData == 0)
                    {
                        _protocolDriver.CommandStopRampVolume(new Address(e.OscDeviceId, e.OscEvent.getZoneId));
                    }
                    else if (e.OscEvent.getOscData == 1)
                    {
                        _protocolDriver.CommandRampVolumeUP(new Address(e.OscDeviceId, e.OscEvent.getZoneId));
                    }
                    else
                    {
                        // Trace error, received an unkown source command
                    }
                }
                else if (e.OscEvent.OscLabel.Contains("/VolumeDown"))
                {
                    if (e.OscEvent.getOscData == 0)
                    {
                        _protocolDriver.CommandStopRampVolume(new Address(e.OscDeviceId, e.OscEvent.getZoneId));
                    }
                    else if (e.OscEvent.getOscData == 1)
                    {
                        _protocolDriver.CommandRampVolumeDOWN(new Address(e.OscDeviceId, e.OscEvent.getZoneId));
                    }
                    else
                    {
                        // Trace error, received an unkown source command
                    }
                }
                else if (e.OscEvent.OscLabel.Contains("/Volume"))
                {
                    _protocolDriver.CommandSetVolume(new Address(e.OscDeviceId, e.OscEvent.getZoneId), e.OscEvent.getOscData);
                }
                else if (e.OscEvent.OscLabel.Contains("/Mute"))
                {
                    if (e.OscEvent.getOscData == 0)
                    {
                        _protocolDriver.CommandMuteOFF(new Address(e.OscDeviceId, e.OscEvent.getZoneId));
                    }
                    else if (e.OscEvent.getOscData == 1)
                    {
                        _protocolDriver.CommandMuteON(new Address(e.OscDeviceId, e.OscEvent.getZoneId));
                    }
                    else
                    {
                        // Trace error, received an unkown status
                    }
                }
                else if (e.OscEvent.OscLabel.Contains("/KeyPadLock"))
                {
                    if (e.OscEvent.getOscData == 0)
                    {
                        _protocolDriver.CommandSetKeypadLockOFF(new Address(e.OscDeviceId, e.OscEvent.getZoneId));
                    }
                    else if (e.OscEvent.getOscData == 1)
                    {
                        _protocolDriver.CommandSetKeypadLockON(new Address(e.OscDeviceId, e.OscEvent.getZoneId));
                    }
                    else
                    {
                        // Trace error, received an unkown status
                    }
                }
            }
            
            // Init "static" text (configuration data)
            updateConfigurationDataOnOscClient();
        }

        #endregion



        /// <summary>
        /// Private method to update "static" configuration data on osc clients
        /// </summary>
        private void updateConfigurationDataOnOscClient()
        {
            if( _oscDriver != null )
            {
                foreach( Zone zone in _zones)
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
    }
}
