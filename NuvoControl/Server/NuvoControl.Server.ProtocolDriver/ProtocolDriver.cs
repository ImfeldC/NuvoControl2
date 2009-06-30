/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.ProtocolDriver
 *   Author:         Ch.Imfeld
 *   Creation Date:  6/12/2009 11:02:29 PM
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 6/12/2009 11:02:29 PM, Ch.Imfeld: Initial implementation.
 * 
 **************************************************************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using Common.Logging;
using NuvoControl.Server.ProtocolDriver.Interface;
using NuvoControl.Common.Configuration;
using NuvoControl.Common;
using System.Timers;

namespace NuvoControl.Server.ProtocolDriver
{
    /// <summary>
    /// This class implements the protocol stack to acces the Nuvo Essentia.
    /// This is the main class which combines the access to the protocol stack.
    /// It implements the extended <c>INuvoProtocol</c> interface, which inherits from 
    /// <c>IProtocol</c> interface.
    /// An application should use this class to instantiate a protocol stack to one or more
    /// devices.
    /// 
    /// <br/>The following class diagram shows the interface hirarchy: 
    /// <a href="../ClassDiagrams/ProtocolDriverInterfaceOverview.jpg">ProtocolDriverInterfaceOverview</a>
    /// \image html ProtocolDriverInterfaceOverview.jpg
    /// <br/>The following class diagram shows the protocol stack: 
    /// <a href="../ClassDiagrams/ProtocolDriverOverview.jpg">ProtocolDriverOverview</a>
    /// \image html ProtocolDriverOverview.jpg
    /// </summary>
    public class NuvoEssentiaProtocolDriver : INuvoProtocol, IDisposable
    {
        #region Common Logger
        /// <summary>
        /// Common logger object. Requires the using directive <c>Common.Logging</c>. See 
        /// <see cref="LogManager"/> for more information.
        /// </summary>
        private ILog _log = LogManager.GetCurrentClassLogger();
        #endregion

        /// <summary>
        /// Dictionary entry class, used as entry in the list of devices.
        /// </summary>
        private class DictEntry
        {
            private int _deviceId = -1;
            private INuvoEssentiaProtocol _protocolStack = null;
            public Boolean DeviceMarkedAsOffline = false;
            public DateTime LastTimeCommandReceived = new DateTime(1970, 1, 1);

            /// <summary>
            /// Public constructor to create a dictionary entry.
            /// It is required to set the protocol stack at creation time.
            /// </summary>
            /// <param name="deviceId">Device Id.</param>
            /// <param name="protocolStack">Protocol Stack</param>
            public DictEntry(int deviceId, INuvoEssentiaProtocol protocolStack)
            {
                _deviceId = deviceId;
                _protocolStack = protocolStack;
            }

            /// <summary>
            /// Get the protocol stack.
            /// </summary>
            public INuvoEssentiaProtocol ProtocolStack
            {
                get { return _protocolStack; }
            }

            /// <summary>
            /// Gets the device id, which belongs to the protocol stack also assigned with 
            /// this object.
            /// </summary>
            public int DeviceId
            {
                get { return _deviceId; }
            }
        }

        /// <summary>
        /// Private list holding all protocol driver stacks (for each device).
        /// </summary>
        private Dictionary<int, DictEntry> _deviceList = new Dictionary<int, DictEntry>();

        /// <summary>
        /// Private member to hold the timer used to send a 'ping' to the device.
        /// </summary>
        private Timer _timerPing = new Timer();


        /// <summary>
        /// Constructor for <c>NuvoEssentiaProtocolDriver</c>.
        /// It uses the application setting <c>PingIntervall</c>. This intervall specifies in [s] the
        /// intervall which is used to send the ping command. The minimum is 2 [s].
        /// </summary>
        public NuvoEssentiaProtocolDriver()
        {
            _log.Trace(m=>m("Protocol Driver instantiated!"));
            _timerPing.Interval = (Properties.Settings.Default.PingIntervall<2?2:Properties.Settings.Default.PingIntervall)*1000;
            _timerPing.Elapsed += new ElapsedEventHandler(_timerPing_Elapsed);
            _timerPing.Start();
        }

        /// <summary>
        /// Timer event method, to check the connection to the devices.
        /// It checks for all attached devcies the time the last update was received.
        /// If the <c>SendPingTimeSpan</c> is exceeded, a ping command is send to the device.
        /// If the <c>MarkAsOfflineTimeSpan</c> is exceeded, the device is marked as offline.
        /// If the device is sending again commands, the device quality is set back to online.
        /// It executes the event <c>onDeviceStatusUpdate</c> to notify any subscriber about any
        /// device quality change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _timerPing_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_deviceList != null)
            {
                _log.Trace(m => m("Ping ... check {0} device(s)", _deviceList.Count));

                DateTime _newestUpdate = new DateTime(2000, 1, 1);
                DateTime _oldestUpdate = new DateTime(3000, 1, 1);
                foreach (DictEntry entry in _deviceList.Values)
                {
                    // Check, if we need to send a ping
                    if ((DateTime.Now - entry.LastTimeCommandReceived) > Properties.Settings.Default.SendPingTimeSpan)
                    {
                        // The last update is behind the allowed time span.
                        // Send a 'ping' command (read version) to the device
                        entry.ProtocolStack.SendCommand(new NuvoEssentiaSingleCommand(ENuvoEssentiaCommands.ReadVersion));
                        _log.Info(m => m("Update of device with id {0} is behind, send ping! Last Update was at {1}", entry.DeviceId, entry.LastTimeCommandReceived.ToString()));
                    }

                    // Check, if we need to set the device offline
                    if ((DateTime.Now - entry.LastTimeCommandReceived) > Properties.Settings.Default.MarkAsOfflineTimeSpan)
                    {
                        if (entry.DeviceMarkedAsOffline == false)
                        {
                            // The last update is behind the allowed time span and it wasn't marked
                            // as offline till now.
                            // Set the device offline, notify the subscribers
                            if (onDeviceStatusUpdate != null)
                            {
                                try
                                {
                                    onDeviceStatusUpdate(this, new ProtocolDeviceUpdatedEventArgs(entry.DeviceId, ZoneQuality.Offline, null));
                                }
                                catch (Exception ex)
                                {
                                    _log.Fatal(m => m("Exception occured at forwarding event 'onDeviceStatusUpdate' to Device {0}! Exception={1}", entry.DeviceId, ex.ToString())); 
                                }
                            }
                            entry.DeviceMarkedAsOffline = true;
                            _log.Warn(m => m("Off-line! Update of device with id {0} is outdated, set device offline! Last Update was at {1}", entry.DeviceId, entry.LastTimeCommandReceived.ToString()));
                        }
                    }
                    else
                    {
                        if (entry.DeviceMarkedAsOffline == true)
                        {
                            // The device is working again, set them back to online
                            if (onDeviceStatusUpdate != null)
                            {
                                try
                                {
                                    onDeviceStatusUpdate(this, new ProtocolDeviceUpdatedEventArgs(entry.DeviceId, ZoneQuality.Online, null));
                                }
                                catch (Exception ex)
                                {
                                    _log.Fatal(m => m("Exception occured at forwarding event 'onDeviceStatusUpdate' to Device {0}! Exception={1}", entry.DeviceId, ex.ToString()));
                                }
                            }
                            entry.DeviceMarkedAsOffline = false;
                            _log.Warn(m => m("Update of device with id {0} is back, set device online! Last Update was at {1}", entry.DeviceId, entry.LastTimeCommandReceived.ToString()));
                        }
                    }

                    // Determine the newest update
                    if (_newestUpdate < entry.LastTimeCommandReceived)
                    {
                        _newestUpdate = entry.LastTimeCommandReceived;
                    }
                    // Determine the oldest update 
                    if (_oldestUpdate > entry.LastTimeCommandReceived)
                    {
                        _oldestUpdate = entry.LastTimeCommandReceived;
                    }
                }
                _log.Trace(m => m("Update(s) are between {0} and {1}", _oldestUpdate.ToString(), _newestUpdate.ToString()));
            }
        }

        /// <summary>
        /// Event handler of the Essentia Protocol layer, to receive commands
        /// from the Nuvo Essentia device.
        /// </summary>
        /// <param name="sender">This pointer, to the sender of this event.</param>
        /// <param name="e">Event argument, containing the Nuvo Essentia command.</param>
        void _essentiaProtocol_onCommandReceived(object sender, NuvoEssentiaProtocolEventArgs e)
        {
            if (_deviceList.ContainsKey(e.DeviceId))
            {
                // Update 'LastUpdated' marker for this device
                _deviceList[e.DeviceId].LastTimeCommandReceived = DateTime.Now;

                //raise the command received event, and pass data to next layer
                if (onCommandReceived != null)
                {
                    try
                    {
                        onCommandReceived(this, new ProtocolCommandReceivedEventArgs(new Address(e.DeviceId, (int)e.Command.ZoneId), e));
                    }
                    catch (Exception ex)
                    {
                        _log.Fatal(m=>m("Exception occured at forwarding event 'onCommandReceived' to Device {0} and Zone {1}! Exception={2}", e.DeviceId, e.Command.ZoneId, ex.ToString())); 
                    }
                }

                //raise the zone status changed event, and pass data to next layer
                if (e.Command.ZoneId != ENuvoEssentiaZones.NoZone &&
                    e.Command.PowerStatus != EZonePowerStatus.ZoneStatusUnknown &&
                    e.Command.VolumeLevel != ZoneState.VALUE_UNDEFINED &&
                    e.Command.SourceId != ENuvoEssentiaSources.NoSource)
                {
                    if (onZoneStatusUpdate != null)
                    {
                        try
                        {
                            ZoneState zoneState = new ZoneState(new Address(e.DeviceId,(int)e.Command.SourceId),(e.Command.PowerStatus==EZonePowerStatus.ZoneStatusON?true:false),
                                NuvoEssentiaCommand.calcVolume2NuvoControl(e.Command.VolumeLevel));
                            onZoneStatusUpdate(this,new ProtocolZoneUpdatedEventArgs(new Address(e.DeviceId,(int)e.Command.ZoneId), zoneState,e));
                        }
                        catch (Exception ex)
                        {
                            _log.Fatal(m=>m("Exception occured at forwarding event 'onZoneStatusUpdate' to Device {0} and Zone {1} (Command='{2}')! Exception={3}", e.DeviceId, e.Command.ZoneId, e.Command.ToString(), ex.ToString())); 
                        }
                    }
                }
            }
            else
            {
                _log.Warn(m => m("Cannot find corresponding protocol layer associated to this device id {0}", e.DeviceId));
            }
        }


        #region IProtocol Members

        /// <summary>
        /// Public event in case a (single) command is received from the device (zone).
        /// </summary>
        public event ProtocolCommandReceivedEventHandler onCommandReceived;

        /// <summary>
        /// Public event in case a full zone state is received from the device (zone).
        /// </summary>
        public event ProtocolZoneUpdatedEventHandler onZoneStatusUpdate;

        /// <summary>
        /// Public event in case the state of a device has changed.
        /// </summary>
        public event ProtocolDeviceUpdatedEventHandler onDeviceStatusUpdate;


        /// <summary>
        /// Public method to open a connection to a device.
        /// </summary>
        /// <param name="system">System Type, to connect to.</param>
        /// <param name="deviceId">Device Id, to connect to</param>
        /// <param name="communicationConfiguration">Communication Configuration, used to connect to the device.</param>
        public void Open(ENuvoSystem system, int deviceId, Communication communicationConfiguration)
        {
            Open(system, deviceId, communicationConfiguration, null);
        }

        /// <summary>
        /// Public method to close a connection.
        /// </summary>
        /// <param name="deviceId">Device Id, to close the connection</param>
        public void Close(int deviceId)
        {
            checkZoneDeviceId(deviceId);
            _deviceList[deviceId].ProtocolStack.Close();
            _deviceList.Remove(deviceId);
        }

        /// <summary>
        /// Public method to read a zone state.
        /// </summary>
        /// <param name="zoneAddress">Zone Adress, containing device and zone id.</param>
        public void ReadZoneState(Address zoneAddress)
        {
            checkZoneDeviceId(zoneAddress.DeviceId);
            INuvoEssentiaCommand command = new NuvoEssentiaCommand(
                ENuvoEssentiaCommands.GetZoneStatus,
                convertAddressZone2EssentiaZone(zoneAddress));
            _deviceList[zoneAddress.DeviceId].ProtocolStack.SendCommand(command);
        }

        /// <summary>
        /// Public method to set a zone state, according to a zone state object passed in.
        /// </summary>
        /// <param name="zoneAddress">Zone Adress, containing device and zone id.</param>
        /// <param name="zoneState">Zone state, which shall be applied.</param>
        public void SetZoneState(Address zoneAddress, ZoneState zoneState)
        {
            if (zoneState.PowerStatus)
            {
                INuvoEssentiaCommand command = new NuvoEssentiaCommand(
                    ENuvoEssentiaCommands.SetZoneStatus,
                    convertAddressZone2EssentiaZone(zoneAddress),
                    convertAddressSource2EssentiaSource(zoneState.Source), 
                    NuvoEssentiaCommand.calcVolume2NuvoEssentia(zoneState.Volume) );
                _deviceList[zoneAddress.DeviceId].ProtocolStack.SendCommand(command);
            }
            else
            {
                INuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(
                    ENuvoEssentiaCommands.TurnZoneOFF,
                    convertAddressZone2EssentiaZone(zoneAddress));
                _deviceList[zoneAddress.DeviceId].ProtocolStack.SendCommand(command);
            }
        }

        /// <summary>
        /// Public method to turn a zone on.
        /// </summary>
        /// <param name="zoneAddress">Zone Adress, containing device and zone id.</param>
        public void CommandSwitchZoneON(Address zoneAddress)
        {
            checkZoneDeviceId(zoneAddress.DeviceId);
            INuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(
                ENuvoEssentiaCommands.TurnZoneON,
                convertAddressZone2EssentiaZone(zoneAddress));
            _deviceList[zoneAddress.DeviceId].ProtocolStack.SendCommand(command);
        }

        /// <summary>
        /// Public method to turn a zone off.
        /// </summary>
        /// <param name="zoneAddress">Zone Adress, containing device and zone id.</param>
        public void CommandSwitchZoneOFF(Address zoneAddress)
        {
            checkZoneDeviceId(zoneAddress.DeviceId);
            INuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(
                ENuvoEssentiaCommands.TurnZoneOFF,
                convertAddressZone2EssentiaZone(zoneAddress));
            _deviceList[zoneAddress.DeviceId].ProtocolStack.SendCommand(command);
        }

        /// <summary>
        /// Public method to set the Source of a zone.
        /// </summary>
        /// <param name="zoneAddress">Zone Adress, containing device and zone id.</param>
        /// <param name="sourceAddress">Source Adress, containing device and source id.</param>
        public void CommandSetSource(Address zoneAddress, Address sourceAddress)
        {
            checkZoneDeviceId(zoneAddress.DeviceId);
            INuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(
                ENuvoEssentiaCommands.SetSource,
                convertAddressZone2EssentiaZone(zoneAddress),
                convertAddressSource2EssentiaSource(sourceAddress));
            _deviceList[zoneAddress.DeviceId].ProtocolStack.SendCommand(command);
        }

        /// <summary>
        /// Public method to set the volume of a zone.
        /// </summary>
        /// <param name="zoneAddress">Zone Adress, containing device and zone id.</param>
        /// <param name="volumeLevel">Volume level.</param>
        public void CommandSetVolume(Address zoneAddress, int volumeLevel)
        {
            checkZoneDeviceId(zoneAddress.DeviceId);
            INuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(
                ENuvoEssentiaCommands.SetVolume,
                convertAddressZone2EssentiaZone(zoneAddress),
                NuvoEssentiaCommand.calcVolume2NuvoEssentia(volumeLevel));
            _deviceList[zoneAddress.DeviceId].ProtocolStack.SendCommand(command);
        }

        #endregion


        #region INuvoProtocol Members

        /// <summary>
        /// Public method - specific for Nuvo - to connect to.
        /// </summary>
        /// <param name="system">System Type, to connect to.</param>
        /// <param name="deviceId">Device Id, to connect to</param>
        /// <param name="communicationConfiguration">Communication Configuration, used to connect to the device.</param>
        /// <param name="essentiaProtocol">Optional protocol layer object, used in case of test environment. Pass <c>null</c> in case of productive system.</param>
        public void Open(ENuvoSystem system, int deviceId, Communication communicationConfiguration, INuvoEssentiaProtocol essentiaProtocol)
        {
            if (system != ENuvoSystem.NuVoEssentia)
            {
                throw new ProtocolDriverException(string.Format("This system type is not supported! Cannot connect! '{0}'", system));
            }
            if (_deviceList.ContainsKey(deviceId))
            {
                throw new ProtocolDriverException(string.Format("A device with the id {0} is already registered. Cannot add a device with the same id!", deviceId));
            }

            // if not null, use the protocol object passed by the caller (e.g. as mock object for unit test)
            _deviceList.Add(deviceId, new DictEntry(deviceId, ((essentiaProtocol == null) ? new NuvoEssentiaProtocol(deviceId, null) : essentiaProtocol)));

            // register for events from protocol layer
            _deviceList[deviceId].ProtocolStack.onCommandReceived += new NuvoEssentiaProtocolEventHandler(_essentiaProtocol_onCommandReceived);

            // open connection to the protocol layer
            _deviceList[deviceId].ProtocolStack.Open(new SerialPortConnectInformation(
                communicationConfiguration.Port,                                            // e.g. "COM1"
                communicationConfiguration.BaudRate,                                        // e.g. 9600
                (Parity)Enum.Parse(typeof(Parity), communicationConfiguration.ParityMode),  // e.g. None
                communicationConfiguration.DataBits,                                        // e.g. 8
                (StopBits)communicationConfiguration.ParityBit));                           // e.g. 1 = StopBits
        }

        /// <summary>
        /// Public method - specific for Nuvo - to send a single command.
        /// </summary>
        /// <param name="zoneAddress">Zone Adress, containing device and zone id.</param>
        /// <param name="command">Command, to send to the device.</param>
        public void SendCommand(Address zoneAddress, INuvoEssentiaSingleCommand command)
        {
            checkZoneDeviceId(zoneAddress.DeviceId);

            if ((command.ZoneId != ENuvoEssentiaZones.NoZone) &&
                (convertAddressZone2EssentiaZone(zoneAddress) != command.ZoneId))
            {
                // The zoneAddress doesn't belong to the same zone as the command
                new ProtocolDriverException(string.Format("The Zone Address doesn't fit the zone used in the command. Cannot send this command!"));
            }

            _deviceList[zoneAddress.DeviceId].ProtocolStack.SendCommand(command);
        }

        /// <summary>
        /// Public method - specific for Nuvo - to send a command, which contains one or more
        /// single command.
        /// </summary>
        /// <param name="zoneAddress">Zone Adress, containing device and zone id.</param>
        /// <param name="command">Command, to send to the device.</param>
        public void SendCommand(Address zoneAddress, INuvoEssentiaCommand command)
        {
            checkZoneDeviceId(zoneAddress.DeviceId);
            _deviceList[zoneAddress.DeviceId].ProtocolStack.SendCommand(command);
        }

        #endregion

        /// <summary>
        /// Method checks the zone address. If the device id is not available 
        /// an error message is written to the console and an exception is thrown.
        /// </summary>
        /// <param name="deviceId">Device id to check.</param>
        private void checkZoneDeviceId( int deviceId )
        {
            if (!_deviceList.ContainsKey(deviceId))
            {
                // Device id is not available
                string message = string.Format("A device with the id {0} is not available. Cannot close device with this id!", deviceId);
                _log.Warn(m => m(message));
                throw new ProtocolDriverException(message);
            }
        }

        /// <summary>
        /// Converts the zone id read from address into a Nuvo
        /// Essentia zone id.
        /// </summary>
        /// <param name="zoneAddress">Zone address with the object id to convert.</param>
        /// <returns>Converted zone id.</returns>
        private ENuvoEssentiaZones convertAddressZone2EssentiaZone(Address zoneAddress)
        {
            //TODO Check this conversion !!!
            return (ENuvoEssentiaZones)zoneAddress.ObjectId;
        }

        /// <summary>
        /// Converts the source id read from address into a Nuvo
        /// Essentia source id.
        /// </summary>
        /// <param name="sourceAddress">Source address with the object id to convert.</param>
        /// <returns>Converted source id.</returns>
        private ENuvoEssentiaSources convertAddressSource2EssentiaSource(Address sourceAddress)
        {
            //TODO Check this conversion !!!
            return (ENuvoEssentiaSources)sourceAddress.ObjectId;
        }


        #region IDisposable Members

        /// <summary>
        /// Public Dispose method.
        /// </summary>
        public void Dispose()
        {
            _log.Trace(m => m("Protocol Driver disposed!"));
            _timerPing.Stop();
        }

        #endregion
    }
}
