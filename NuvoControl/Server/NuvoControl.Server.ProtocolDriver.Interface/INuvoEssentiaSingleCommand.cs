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
    /// <summary>
    /// Public interface for a single Nuvo Essentai command.
    /// It has members for all possible values send and/or received from Nuvo Essentia.
    /// Depending on the specified command not all members will be used and set at runtime
    /// to the correct value received from Nuvo Essentia.
    /// 
    /// For the implementation details, look into the class NuvoEssentiaSingleCommand
    /// </summary>
    public interface INuvoEssentiaSingleCommand : IComparable
    {
        /// <summary>
        /// Returns a unique GUID for this command instance.
        /// This allows to identify a running command exactly.
        /// </summary>
        Guid Guid { get; }

        /// <summary>
        /// Returns the NuvoEssentia command.
        /// </summary>
        ENuvoEssentiaCommands Command { get; }

        /// <summary>
        /// Returns true in case, the command and its respone is valid.
        /// </summary>
        bool Valid { get; }

        /// <summary>
        /// Returns the date and time, if this command was created.
        /// </summary>
        DateTime CreatedDateTime { get; }

        /// <summary>
        /// Returns the date and time, if this command was send out.
        /// </summary>
        DateTime SendDateTime { get; set; }

        /// <summary>
        /// Returns the date and time, if this command was received.
        /// </summary>
        DateTime ReceiveDateTime { get; }

        /// <summary>
        /// Returns the command template for the outgoing command.
        /// </summary>
        string OutgoingCommandTemplate { get; }

        /// <summary>
        /// Returns the outgoing command string.
        /// </summary>
        string OutgoingCommand { get; }

        /// <summary>
        /// Returns the (expected) command template string for the incoming response.
        /// </summary>
        string IncomingCommandTemplate { get; }

        /// <summary>
        /// Returns the incoming command string.
        /// </summary>
        string IncomingCommand { get; set; }

        /// <summary>
        /// Returns the Zone Id.
        /// </summary>
        ENuvoEssentiaZones ZoneId { get; }

        /// <summary>
        /// Returns the Source Id
        /// </summary>
        ENuvoEssentiaSources SourceId { get; }

        /// <summary>
        /// Returns the Zone Power Status
        /// </summary>
        EZonePowerStatus PowerStatus { get; }

        /// <summary>
        /// Returns the IR Carries Frequency, for the specified source.
        /// </summary>
        /// <param name="source">Source id</param>
        /// <returns>IR Carrier Frequency, for the specified source.</returns>
        EIRCarrierFrequency IrCarrierFrequencySource(ENuvoEssentiaSources source);

        /// <summary>
        /// Retruns the Volume Level.
        /// </summary>
        int VolumeLevel { get; set; }

        /// <summary>
        /// Returns the Bass Level.
        /// </summary>
        int BassLevel { get; }

        /// <summary>
        /// Returns the Treble Level.
        /// </summary>
        int TrebleLevel { get; }

        /// <summary>
        /// Returns the Volume Resest Status.
        /// </summary>
        EVolumeResetStatus VolumeResetStatus { get; }

        /// <summary>
        /// Returns the DIP Switch Override Status.
        /// </summary>
        EDIPSwitchOverrideStatus DIPSwitchOverrideStatus { get; }

        /// <summary>
        /// Returns the Source Group Status.
        /// </summary>
        ESourceGroupStatus SourceGrupStatus { get; }

        /// <summary>
        /// Returns the firmware version string.
        /// </summary>
        string FirmwareVersion { get; }
    }

}
