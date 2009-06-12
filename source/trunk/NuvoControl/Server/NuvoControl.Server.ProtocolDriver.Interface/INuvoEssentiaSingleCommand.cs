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
        int VolumeLevel { get; set; }
        int BassLevel { get; }
        int TrebleLevel { get; }
        EVolumeResetStatus VolumeResetStatus { get; }
        EDIPSwitchOverrideStatus DIPSwitchOverrideStatus { get; }
        ESourceGroupStatus SourceGrupStatus { get; }
        string FirmwareVersion { get; }
    }

}
