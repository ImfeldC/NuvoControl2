using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NuvoControl.Common.Configuration;

namespace NuvoControl.Server.ProtocolDriver.Interface
{
    public delegate void ProtocolEventHandler(
              object sender, ProtocolEventArgs e);

    public class ProtocolEventArgs : EventArgs
    {
        int _deviceId;
        Address _zoneAddress;
        NuvoEssentiaProtocolEventArgs _innerEventArgs;

        public ProtocolEventArgs(int deviceId, Address zoneAddress, NuvoEssentiaProtocolEventArgs innerEventArgs)
        {
            _deviceId = deviceId;
            _zoneAddress = zoneAddress;
            _innerEventArgs = innerEventArgs;
        }

        public int DeviceId
        {
            get { return _deviceId; }
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

    public interface IProtocol
    {
        event ProtocolEventHandler onCommandReceived;
        event ProtocolEventHandler onZoneStatusChanged;

        void Open(ENuvoSystem system, int deviceId, Communication communicationConfiguration);
        void Open(ENuvoSystem system, int deviceId, Communication communicationConfiguration, INuvoEssentiaProtocol essentiaProtocol);
        void Close(int deviceId);

        void ReadZoneStatus(Address zoneAddress);
        void CommandSwitchZoneON(Address zoneAddress);
        void CommandSwitchZoneOFF(Address zoneAddress);
        void CommandSetSource(Address zoneAddress, Address sourceAddress);
        void CommandSetVolume(Address zoneAddress, int volumeLevel);

        void SendCommand(Address zoneAddress, INuvoEssentiaSingleCommand command);
        void SendCommand(Address zoneAddress, INuvoEssentiaCommand command);
    }
}
