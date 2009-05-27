using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuvoControl.Server.ProtocolDriver.Interface
{
    public delegate void ProtocolEventHandler(
              object sender, ProtocolEventArgs e);

    public class ProtocolEventArgs : EventArgs
    {
        ICommand _command;

        public ICommand Command
        {
            get { return _command; }
        }

        public ProtocolEventArgs(ICommand command)
        {
            _command = command;
        }

    }

    public interface ICommand : IComparable
    {
        Guid Guid { get; }

        DateTime CreatedDateTime { get; }
        DateTime SendDateTime { get; }
        DateTime ReceiveDateTime { get; }

        ENuvoEssentiaZones ZoneId { get; }
        ENuvoEssentiaSources SourceId { get; }
        EZonePowerStatus PowerStatus { get; }
        int VolumeLevel { get; }
    }


    public interface IProtocol
    {
        event ProtocolEventHandler onCommandReceived;

        void SendCommand(ICommand command);
    }
}
