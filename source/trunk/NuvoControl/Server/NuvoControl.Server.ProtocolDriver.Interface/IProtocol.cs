using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuvoControl.Common.Configuration;
using NuvoControl.Common;

namespace NuvoControl.Server.ProtocolDriver.Interface
{
    public delegate void ProtocolEventHandler(
              object sender, ProtocolEventArgs e);

    public class ProtocolEventArgs : EventArgs
    {
        private Address _zoneAddress;
        private ZoneState _zoneState;

        NuvoEssentiaProtocolEventArgs _innerEventArgs;

        public ProtocolEventArgs(Address zoneAddress, NuvoEssentiaProtocolEventArgs innerEventArgs)
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
    /// 
    /// </summary>
    public interface IProtocol
    {
        event ProtocolEventHandler onCommandReceived;
        event ProtocolEventHandler onZoneStatusChanged; // TODO: Use ZoneState instead of Nuvo Essentia

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
