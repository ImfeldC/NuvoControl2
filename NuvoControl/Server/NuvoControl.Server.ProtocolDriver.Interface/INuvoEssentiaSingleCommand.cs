using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuvoControl.Server.ProtocolDriver.Interface
{
    public interface INuvoEssentiaSingleCommand : IComparable
    {
        Guid Guid { get; }
        ENuvoEssentiaCommands Command { get; }
        bool Valid { get; }

        DateTime CreatedDateTime { get; }
        DateTime SendDateTime { get; set; }
        DateTime ReceiveDateTime { get; }

        string OutgoingCommandTemplate { get; }
        string OutgoingCommand { get; }
        string IncomingCommandTemplate { get; }
        string IncomingCommand { get; set; }

        ENuvoEssentiaZones ZoneId { get; }
        ENuvoEssentiaSources SourceId { get; }
        EZonePowerStatus PowerStatus { get; }
        EIRCarrierFrequency IrCarrierFrequencySource(ENuvoEssentiaSources source);
        int VolumeLevel { get; }
        int BassLevel { get; }
        int TrebleLevel { get; }
        EVolumeResetStatus VolumeResetStatus { get; }
        EDIPSwitchOverrideStatus DIPSwitchOverrideStatus { get; }
        ESourceGroupStatus SourceGrupStatus { get; }
        string FirmwareVersion { get; }
    }

}
