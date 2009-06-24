/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.ProtocolDriver.Interface
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
using NuvoControl.Common.Configuration;
using NuvoControl.Common;

namespace NuvoControl.Server.ProtocolDriver.Interface
{
    /// <summary>
    /// Public delegate used in case a command is received.
    /// </summary>
    /// <param name="sender">Sender of the event</param>
    /// <param name="e">Additional information passed by the Sender</param>
    public delegate void ProtocolCommandReceivedEventHandler(
              object sender, ProtocolCommandReceivedEventArgs e);

    /// <summary>
    /// Argument class, used in the delegate <c>ProtocolCommandReceivedEventHandler</c>.
    /// Inherits from base class <c>EventArgs</c>.
    /// </summary>
    public class ProtocolCommandReceivedEventArgs : EventArgs
    {
        private Address _zoneAddress;
        private NuvoEssentiaProtocolEventArgs _innerEventArgs;

        /// <summary>
        /// Constructor for the argument class.
        /// </summary>
        /// <param name="zoneAddress">Zone address, where the associated event belongs to.</param>
        /// <param name="innerEventArgs">Inner argument class, provided by the underlying protocol layer.</param>
        public ProtocolCommandReceivedEventArgs(Address zoneAddress, NuvoEssentiaProtocolEventArgs innerEventArgs)
        {
            _zoneAddress = zoneAddress;
            _innerEventArgs = innerEventArgs;
        }

        /// <summary>
        /// Returns the Device Id, as part of the Zone Address.
        /// </summary>
        public int DeviceId
        {
            get { return _zoneAddress.DeviceId; }
        }

        /// <summary>
        /// Returns the Zone Address.
        /// </summary>
        public Address ZoneAddress
        {
            get { return _zoneAddress; }
        }

        /// <summary>
        /// Returns the Command, which was the initiator of this event.
        /// This is part of the inner argument class.
        /// </summary>
        public INuvoEssentiaSingleCommand Command
        {
            get { return _innerEventArgs.Command; }
        }
    }

    /// <summary>
    /// Public delegate used in case a zone is updated.
    /// </summary>
    /// <param name="sender">Sender of the event</param>
    /// <param name="e">Additional information passed by the Sender</param>
    public delegate void ProtocolZoneUpdatedEventHandler(
              object sender, ProtocolZoneUpdatedEventArgs e);

    /// <summary>
    /// Argument class, used in the delegate <c>ProtocolZoneUpdatedEventHandler</c>.
    /// Inherits from base class <c>EventArgs</c>.
    /// </summary>
    public class ProtocolZoneUpdatedEventArgs : EventArgs
    {
        private Address _zoneAddress;
        private ZoneState _zoneState;
        private NuvoEssentiaProtocolEventArgs _innerEventArgs;

        /// <summary>
        /// Public constructor for the protocol zone update event argument.
        /// </summary>
        /// <param name="zoneAddress">Zone Address, containing Device Id and Zone Id</param>
        /// <param name="zoneState">Current Zone State.</param>
        /// <param name="innerEventArgs">Inner Event Argument.</param>
        public ProtocolZoneUpdatedEventArgs(Address zoneAddress, ZoneState zoneState, NuvoEssentiaProtocolEventArgs innerEventArgs)
        {
            _zoneAddress = zoneAddress;
            _zoneState = zoneState;
            _innerEventArgs = innerEventArgs;
        }

        /// <summary>
        /// Get Device Id, as part of the Zone address.
        /// </summary>
        public int DeviceId
        {
            get { return _zoneAddress.DeviceId; }
        }

        /// <summary>
        /// Get Object Id (=Zone Id), as part of the Zone address.
        /// </summary>
        public int ObjectId
        {
            get { return _zoneAddress.ObjectId; }
        }

        /// <summary>
        /// Get the full Zone Address, containing Device Id and Zone Id (=Object Id)
        /// </summary>
        public Address ZoneAddress
        {
            get { return _zoneAddress; }
        }

        /// <summary>
        /// Get Zone State for this zone
        /// </summary>
        public ZoneState ZoneState
        {
            get { return _zoneState; }
        }

    }



    /// <summary>
    /// This is the main interface used by the NuvoControl Monitor and Control Service.
    /// All systems - which would like to be monitored and controlled - by NuvoControl
    /// are required to implement this interface.
    /// <br/>The following class diagram shows the interface hirarchy: <a href="../ClassDiagrams/ProtocolDriverInterfaceOverview.jpg">ProtocolDriverInterfaceOverview</a>
    /// <br/>The following class diagram shows the protocol stack: <a href="../ClassDiagrams/ProtocolDriverOverview.jpg">ProtocolDriverOverview</a>
    /// </summary>
    public interface IProtocol
    {
        /// <summary>
        /// This event is raised in case a command has been recieved from the underlying device.
        /// </summary>
        event ProtocolCommandReceivedEventHandler onCommandReceived;

        /// <summary>
        /// This event is raised in case any zone state has changed.
        /// The receiver needs to check, which zone has changed.
        /// </summary>
        event ProtocolZoneUpdatedEventHandler onZoneStatusUpdate;

        /// <summary>
        /// Opens a connection to the device.
        /// The <c>communicationConfiguration</c> contains the required information to establish a connection.
        /// </summary>
        /// <param name="system">System Type. See <see cref="ENuvoSystem"/> for more information.</param>
        /// <param name="deviceId">Device Id, to connect to.</param>
        /// <param name="communicationConfiguration">Communication Configuration, rquired to establish a connection.</param>
        void Open(ENuvoSystem system, int deviceId, Communication communicationConfiguration);

        /// <summary>
        /// Close an open connection.
        /// </summary>
        /// <param name="deviceId">Device id, to close the connection.</param>
        void Close(int deviceId);

        /// <summary>
        /// Reads the zone state for a specific zone.
        /// </summary>
        /// <param name="zoneAddress">Address of the zone.</param>
        void ReadZoneState(Address zoneAddress);

        /// <summary>
        /// Sets a zone state for a specific zone.
        /// </summary>
        /// <param name="zoneAddress">Zone address, to set the state.</param>
        /// <param name="zoneState">Zoen state, which shall be applied to the specified zone.</param>
        void SetZoneState(Address zoneAddress, ZoneState zoneState);

        /// <summary>
        /// Switch specified zone ON.
        /// </summary>
        /// <param name="zoneAddress">Zone address, to switch on.</param>
        void CommandSwitchZoneON(Address zoneAddress);

        /// <summary>
        /// Switch specified zone OFF.
        /// </summary>
        /// <param name="zoneAddress">Zone address, to switch off.</param>
        void CommandSwitchZoneOFF(Address zoneAddress);

        /// <summary>
        /// Set specified source in the specified zone.
        /// </summary>
        /// <param name="zoneAddress">Zone address, where to change the source.</param>
        /// <param name="sourceAddress">Source Address, which will be selected in the selected zone.</param>
        void CommandSetSource(Address zoneAddress, Address sourceAddress);

        /// <summary>
        /// Set volume level in the specified zone.
        /// </summary>
        /// <param name="zoneAddress">Zone address, where to set the volume level.</param>
        /// <param name="volumeLevel">Volume Level, which shall be set in the selected zone.</param>
        void CommandSetVolume(Address zoneAddress, int volumeLevel);

    }
}
