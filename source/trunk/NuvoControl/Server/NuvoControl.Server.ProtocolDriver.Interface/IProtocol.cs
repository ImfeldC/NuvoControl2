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
    public class ProtocolCommandReceivedEventArgs : EventArgs
    {
        private Address _zoneAddress;

        NuvoEssentiaProtocolEventArgs _innerEventArgs;

        public ProtocolCommandReceivedEventArgs(Address zoneAddress, NuvoEssentiaProtocolEventArgs innerEventArgs)
        {
            _zoneAddress = zoneAddress;
            _innerEventArgs = innerEventArgs;
        }

        public int DeviceId
        {
            get { return _zoneAddress.DeviceId; }
        }

        public Address ZoneAddress
        {
            get { return _zoneAddress; }
        }

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
    public class ProtocolZoneUpdatedEventArgs : EventArgs
    {
        private Address _zoneAddress;
        private ZoneState _zoneState;

        NuvoEssentiaProtocolEventArgs _innerEventArgs;

        public ProtocolZoneUpdatedEventArgs(Address zoneAddress, ZoneState zoneState, NuvoEssentiaProtocolEventArgs innerEventArgs)
        {
            _zoneAddress = zoneAddress;
            _zoneState = zoneState;
            _innerEventArgs = innerEventArgs;
        }

        public int DeviceId
        {
            get { return _zoneAddress.DeviceId; }
        }

        public int ObjectId
        {
            get { return _zoneAddress.ObjectId; }
        }

        public Address ZoneAddress
        {
            get { return _zoneAddress; }
        }

        public ZoneState ZoneState
        {
            get { return _zoneState; }
        }

    }



    /// <summary>
    /// This is the main interface used by the NuvoControl Monitor and Control Service.
    /// All systems - which would like to be monitored and controlled - by NuvoControl
    /// are required to implement this interface.
    /// <br/>The following class diagram shows the protocol stack: <a href="../ClassDiagrams/ProtocolDriverOverview.jpg">ProtocolDriverOverview</a>
    /// </summary>
    public interface IProtocol
    {
        event ProtocolCommandReceivedEventHandler onCommandReceived;
        event ProtocolZoneUpdatedEventHandler onZoneStatusUpdate;

        void Open(ENuvoSystem system, int deviceId, Communication communicationConfiguration);
        void Close(int deviceId);

        void ReadZoneState(Address zoneAddress);
        void SetZoneState(Address zoneAddress, ZoneState zoneState);
        void CommandSwitchZoneON(Address zoneAddress);
        void CommandSwitchZoneOFF(Address zoneAddress);
        void CommandSetSource(Address zoneAddress, Address sourceAddress);
        void CommandSetVolume(Address zoneAddress, int volumeLevel);

    }
}
