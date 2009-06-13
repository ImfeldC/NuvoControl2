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
    /// 
    /// </summary>
    public interface IProtocol
    {
        event ProtocolCommandReceivedEventHandler onCommandReceived;
        event ProtocolZoneUpdatedEventHandler onZoneStatusUpdate;

        //TODO: Discuss how to pass communication confugurations correct (IPorotocol should not be aware of Baudrate)
        void Open(ENuvoSystem system, int deviceId, Communication communicationConfiguration);
        void Open(ENuvoSystem system, int deviceId, Communication communicationConfiguration, INuvoEssentiaProtocol essentiaProtocol);
        void Close(int deviceId);

        void ReadZoneStatus(Address zoneAddress);
        void CommandSwitchZoneON(Address zoneAddress);
        void CommandSwitchZoneOFF(Address zoneAddress);
        void CommandSetSource(Address zoneAddress, Address sourceAddress);
        void CommandSetVolume(Address zoneAddress, int volumeLevel);

        //TODO: Is this allowed, this method is only available for a specific device
        void SendCommand(Address zoneAddress, INuvoEssentiaSingleCommand command);
        void SendCommand(Address zoneAddress, INuvoEssentiaCommand command);
    }
}
