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


namespace NuvoControl.Server.ProtocolDriver
{
    /// <summary>
    /// This is the main class which combines all protocol layers.
    /// An application should use this class to instantiate a protocol stack.
    /// </summary>
    public class NuvoEssentiaProtocolDriver : IProtocol
    {
        private ILog _log = LogManager.GetCurrentClassLogger();

        Dictionary<int,INuvoEssentiaProtocol> _protocolList = new Dictionary<int,INuvoEssentiaProtocol>();


        public NuvoEssentiaProtocolDriver()
        {
            _log.Debug(m => m("Protocol Driver instantiated!"));
        }


        void _essentiaProtocol_onCommandReceived(object sender, NuvoEssentiaProtocolEventArgs e)
        {
            if (_protocolList.ContainsKey(e.DeviceId))
            {
                //raise the command received event, and pass data to next layer
                if (onCommandReceived != null)
                {
                     onCommandReceived(this, new ProtocolCommandReceivedEventArgs(new Address(e.DeviceId,(int)e.Command.ZoneId),e));
                }

                //raise the zone status changed event, and pass data to next layer
                if (e.Command.ZoneId != ENuvoEssentiaZones.NoZone &&
                    e.Command.PowerStatus != EZonePowerStatus.ZoneStatusUnknown &&
                    e.Command.VolumeLevel != -999 &&
                    e.Command.SourceId != ENuvoEssentiaSources.NoSource)
                {
                    if (onZoneStatusUpdate != null)
                    {
                        ZoneState zoneState = new ZoneState(new Address(e.DeviceId,(int)e.Command.SourceId),(e.Command.PowerStatus==EZonePowerStatus.ZoneStatusON?true:false),e.Command.VolumeLevel);
                        onZoneStatusUpdate(this,new ProtocolZoneUpdatedEventArgs(new Address(e.DeviceId,(int)e.Command.ZoneId), zoneState,e));
                    }
                }
            }
            else
            {
                _log.Warn(m => m("Cannot find corresponding protocol layer associated to this device id {0}", e.DeviceId));
            }
        }


        #region IProtocol Members

        public event ProtocolCommandReceivedEventHandler onCommandReceived;

        public event ProtocolZoneUpdatedEventHandler onZoneStatusUpdate;


        public void Open(ENuvoSystem system, int deviceId, Communication communicationConfiguration)
        {
            Open(system, deviceId, communicationConfiguration, null);
        }

        public void Open(ENuvoSystem system, int deviceId, Communication communicationConfiguration, INuvoEssentiaProtocol essentiaProtocol)
        {
            if (system != ENuvoSystem.NuVoEssentia)
            {
                throw new ProtocolDriverException(string.Format("This system type is not supported! Cannot connect! '{0}'", system));
            }
            if (_protocolList.ContainsKey(deviceId))
            {
                throw new ProtocolDriverException(string.Format("A device with the id {0} is already registered. Cannot add a device with the same id!", deviceId));
            }

            // if not null, use the protocol object passed by the caller (e.g. as mock object for unit test)
            _protocolList.Add(deviceId, ((essentiaProtocol == null) ? new NuvoEssentiaProtocol(deviceId,null) : essentiaProtocol));

            // register for events from protocol layer
            _protocolList[deviceId].onCommandReceived += new NuvoEssentiaProtocolEventHandler(_essentiaProtocol_onCommandReceived);

            // open connection to the protocol layer
            _protocolList[deviceId].Open(new SerialPortConnectInformation(
                communicationConfiguration.Port,                                            // e.g. "COM1"
                communicationConfiguration.BaudRate,                                        // e.g. 9600
                (Parity)Enum.Parse(typeof(Parity), communicationConfiguration.ParityMode),  // e.g. None
                communicationConfiguration.DataBits,                                        // e.g. 8
                (StopBits)communicationConfiguration.ParityBit));                           // e.g. 1 = StopBits
        }

        public void Close(int deviceId)
        {
            checkZoneDeviceId(deviceId);
            _protocolList[deviceId].Close();
            _protocolList.Remove(deviceId);
        }

        public void ReadZoneStatus(Address zoneAddress)
        {
            checkZoneDeviceId(zoneAddress.DeviceId);
            INuvoEssentiaCommand command = new NuvoEssentiaCommand(
                ENuvoEssentiaCommands.GetZoneStatus,
                convertAddressZone2EssentiaZone(zoneAddress));
            _protocolList[zoneAddress.DeviceId].SendCommand(command);
        }

        public void CommandSwitchZoneON(Address zoneAddress)
        {
            checkZoneDeviceId(zoneAddress.DeviceId);
            INuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(
                ENuvoEssentiaCommands.TurnZoneON,
                convertAddressZone2EssentiaZone(zoneAddress));
            _protocolList[zoneAddress.DeviceId].SendCommand(command);
        }

        public void CommandSwitchZoneOFF(Address zoneAddress)
        {
            checkZoneDeviceId(zoneAddress.DeviceId);
            INuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(
                ENuvoEssentiaCommands.TurnZoneOFF,
                convertAddressZone2EssentiaZone(zoneAddress));
            _protocolList[zoneAddress.DeviceId].SendCommand(command);
        }

        public void CommandSetSource(Address zoneAddress, Address sourceAddress)
        {
            checkZoneDeviceId(zoneAddress.DeviceId);
            INuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(
                ENuvoEssentiaCommands.SetSource,
                convertAddressZone2EssentiaZone(zoneAddress),
                convertAddressSource2EssentiaSource(sourceAddress));
            _protocolList[zoneAddress.DeviceId].SendCommand(command);
        }

        public void CommandSetVolume(Address zoneAddress, int volumeLevel)
        {
            checkZoneDeviceId(zoneAddress.DeviceId);
            INuvoEssentiaSingleCommand command = new NuvoEssentiaSingleCommand(
                ENuvoEssentiaCommands.SetVolume,
                convertAddressZone2EssentiaZone(zoneAddress),
                volumeLevel);
            _protocolList[zoneAddress.DeviceId].SendCommand(command);
        }

        public void SendCommand(Address zoneAddress, INuvoEssentiaSingleCommand command)
        {
            checkZoneDeviceId(zoneAddress.DeviceId);

            if ((command.ZoneId != ENuvoEssentiaZones.NoZone) &&
                (convertAddressZone2EssentiaZone(zoneAddress) != command.ZoneId))
            {
                // The zoneAddress doesn't belong to the same zone as the command
                new ProtocolDriverException(string.Format("The Zone Address doesn't fit the zone used in the command. Cannot send this command!"));
            }

            _protocolList[zoneAddress.DeviceId].SendCommand(command);
        }

        public void SendCommand(Address zoneAddress, INuvoEssentiaCommand command)
        {
            checkZoneDeviceId(zoneAddress.DeviceId);
            _protocolList[zoneAddress.DeviceId].SendCommand(command);
        }

        #endregion

        /// <summary>
        /// Method checks the zone address. If the device id is not available 
        /// an error message is written to the console and an exception is thrown.
        /// </summary>
        /// <param name="zoneAddress">Address to check.</param>
        private void checkZoneDeviceId( int deviceId )
        {
            if (!_protocolList.ContainsKey(deviceId))
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
        /// <param name="address">Zone address with the object id to convert.</param>
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
    }
}
