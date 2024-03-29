/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
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
using System.Collections;
using System.Runtime.Serialization;
using System.Xml.Serialization;


namespace NuvoControl.Server.ProtocolDriver.Interface
{

	//===========================================================
	// Enumeration Defintions
    //===========================================================

    #region NuVo Systems
    /// <summary>
	/// NuVo Systems
	/// </summary>
	[Serializable]
	public enum ENuvoSystem
	{
		/// <summary>
		/// NuVo Essentia
		/// </summary>
		NuVoEssentia = 0,

		/// <summary>
		/// NuVo Tuner T2
		/// </summary>
		NuVoTunerT2 = 1,
    }
    #endregion

    #region NuVo Essentia Commands
    /// <summary>
    /// NuVo Essentia Commands 
	/// </summary>
    [Serializable]
    public enum ENuvoEssentiaCommands
    {
        ///
        /// NUVO ESSENTIA COMMANDS
        ///

        /// <summary>
        /// Command: *IRSETSR � Reads status of SOURCE IR carrier frequency settings
        /// RESPONSE:#IRSET:aa,bb,cc,dd,ee,ff where:aa = IR carrier frequency of SOURCE 1 ("38" or "56") bb = carrier frequency of SOURCE 2 ("38" or "56") cc = carrier frequency of SOURCE 3 ("38" or "56") dd = carrier frequency of SOURCE 4 ("38" or "56") ee = carrier frequency of SOURCE 5 ("38" or "56") ff = carrier frequency of SOURCE 6 ("38" or "56")
        /// NOTE � the Main Unit ships with the carrier frequency DEFAULT setting of38 KHz for all six sources
        /// </summary>
        ReadStatusSOURCEIR = 0,

        /// <summary>
        /// COMMAND: *IRSETDF � Restores DEFAULT SOURCE IR carrier frequency settings (38 KHz for all six sources).
        /// RESPONSE: Same response as for #IRSETSR
        /// </summary>
        RestoreDefaultSOURCEIR = 1,

        /// <summary>
        /// COMMAND: *SxIR56SET - sets SOURCE x to 56 KHz IR repeat carrier (x is 1 to 6).
        /// RESPONSE: Same response as for #IRSETSR
        /// </summary>
        SetSOURCEIR56 = 2,

        /// <summary>
        /// COMMAND: *SxIR38SET - sets SOURCE x to 38 KHz IR repeat carrier (x is 1 to 6).
        /// RESPONSE: Same response as for #IRSETSR
        /// </summary>
        SetSOURCEIR38 = 3,

        /// <summary>
        /// COMMAND: *ZxxCONSR - Connect STATUS REQUEST where xx is zone # from 1 to 12
        /// RESPONSE:#ZxxPWRppp,SRCs,GRPq,VOL-yy
        /// -ppp = "ON" (2 characters)or "OFF" (3 characters)
        /// -s = SOURCE NUMBER 1 to 6
        /// -q = 0 if SOURCE GROUP is ON / 1 if SOURCE GROUP is OFF
        /// -yy = level below max in dB: -00 to -79 dB (include lead 0 for all single-digit values)
        /// -yy = "MT" if in MUTE state
        /// -yy = "XM" if external MUTE is being held active 
        /// This response will also be issued in response to pressing the ON/OFF, VOLUME, or SOURCEkeys on a KEYPAD. NOTE � the response will be issued if a SOURCE key is press`ed on azone that is powered OFF even though the key press has no effect on the system. It will beoutput at every increment during a volume ramp initiated by HOLDING a VOLUME UP orVOLUME DOWN key on a keypad. It will also be issued at every increment of a volumeramp commanded by the *ZxxVOL+ and *ZxxVOL- commands (see below).
        /// The MUTE value will be asserted if a *ZxMTON command has been received, OR if thevolume is run all the way to the lowest possible point (volume off). An active EXTERNALMUTE input, however, will always override other volume response values with the "XM" response.
        /// </summary>
        ReadStatusCONNECT = 4,

        /// <summary>
        /// COMMAND:*ZxxSETSR � ZoneSet STATUS REQUEST where xx is zone # from 1 to 12
        /// RESPONSE:#ZxxORp,BASSyy,TREByy,GRPq,VRSTr
        /// -p = 1 if DIP switches are overridden*0 if DIP switches are in control
        /// -yy = EQ level, dB, �8 to +0 (flat) to +8 in 1 dB increments
        /// -q = 0 if SOURCE GROUP is ON / 1 if SOURCE GROUP is OFF (This follows DIP switch definition.)
        /// -r = 0 if VOLUME RESET is ON / 1 if VOLUME RESET is OFF (This follows DIP switch definition.)
        /// *override set to 1 FOR THIS ZONE only if one of commands *ZxxBASSyy, *ZxxTREByy, *ZxxGRPq, or *ZxxVRSTr are issued (seedescriptions below).
        /// Once it is SET by one of these commands:
        /// a. It will remain set until power is cycled on the unit.
        /// b. Non-address DIP switch changes on a connected KEYPAD connected to this zone will be ignored.
        /// If override state is "0", this response is also issued whenever non-address KEYPAD DIP switches are changed
        /// </summary>
        ReadStatusZONE = 5,

        /// <summary>
        /// COMMAND: *ZxxON � Turn zone xx ON
        /// RESPONSE:Same response as for *ZxxCONSR
        /// </summary>
        TurnZoneON = 6,

        /// <summary>
        /// COMMAND: *ZxxOFF � Turn zone xx OFF
        /// RESPONSE:Same response as for *ZxxCONSR
        /// </summary>
        TurnZoneOFF = 7,

        /// <summary>
        /// COMMAND: *ALLOFF � Turn ALL zones OFF
        /// RESPONSE:#ALLOFF
        /// This RESPONSE is also issued when ALL OFF is pressed on any KEYPAD
        /// </summary>
        TurnALLZoneOFF = 8,

        /// <summary>
        /// COMMAND: *ALLV+ � Ramp ALL zones UP at a 10 dB/second rate in 1 dB steps
        /// RESPONSE:#ALLV+
        /// The ramp action will be cancelled when all zones reach MAXIMUM volume, or when an *ALLHLD Command is received. Note that to stop theramp with this command before maximum volume, one reference zone must be periodically polled with a *ZxxCONSR Command to determinewhen the desired volume point has been reached. Note that ramps in different zones may start at different levels and will all ramp at the samerate.
        /// </summary>
        RampVolumeALLZoneUP = 9,

        /// <summary>
        /// COMMAND: *ALLV- � Ramp ALL zones DOWN at a 10 dB/second rate in 1 dB steps
        /// RESPONSE:#ALLV-
        /// The ramp action will be cancelled when all zones reach MINIMUM (OFF) volume, or when an *ALLHLD Command is received. Note that tostop the ramp with this command before the minimum volume, one reference zone must be periodically polled with a *ZxxCONSR Commandto determine when the desired volume point has been reached. Note that ramps in different zones may start at different levels and will all rampat the same rate
        /// </summary>
        RampVolumeALLZoneDOWN = 10,

        /// <summary>
        /// COMMAND: *ALLHLD � Stops ramp action initiated by *ALLV+ Or *ALLV-
        /// RESPONSE:#ALLHLD-
        /// This results in a HOLD of the level at time of command receipt
        /// </summary>
        StopRampVolumeALLZone = 11,

        /// <summary>
        /// COMMAND: *ALLMON � ALL MUTE ON
        /// RESPONSE:# ALLMON
        /// </summary>
        MuteALLZoneON = 12,

        /// <summary>
        /// COMMAND: *ALLMOFF � ALL MUTE OFF
        /// RESPONSE:#ALLMOFF
        /// </summary>
        MuteALLZoneOFF = 13,

        /// <summary>
        /// COMMAND: *ZxxSRCp � Switch zone xx to SOURCE p ( 1 to 6).
        /// RESPONSE:Same response as for *ZxxCONSR
        /// </summary>
        SetSource = 14,

        /// <summary>
        /// COMMAND: *ZxxVOLyy � Set volume of zone xx to level yy below max in dB from �0 to �78 dB (include lead 0 for all single-digit values).
        /// RESPONSE:Same response as for *ZxxCONSR
        /// </summary>
        SetVolume = 15,

        /// <summary>
        /// COMMAND:*ZxxVOL+ � STARTS zone xx volume ramp UP at the rate of +10 dB per second in +1 dB steps. (This is the same as holding VOLUME UP key on a KEYPAD for 1 second).
        /// RESPONSE: Same response as for *ZxxCONSR, updated 10 times per second.
        /// </summary>
        RampVolumeUP = 16,

        /// <summary>
        /// COMMAND:*ZxxVOL- � STARTS zone xx volume ramp DOWN at the rate of -10 dB per second in -1 dB steps. (This is the same as holding VOLUME UP key on a KEYPAD for 1 second).
        /// RESPONSE: Same response as for *ZxxCONSR, updated 10 times per second.
        /// </summary>
        RampVolumeDOWN = 17,

        /// <summary>
        /// COMMAND:*ZxxVHLD � STOPS zone xx volume ramp initiated by *ZxxVOL+ or *ZxxVOL- commands. This results in a HOLD of the level at time of command receipt.
        /// RESPONSE: #ZxxVHLD
        /// </summary>
        StopRampVolume = 18,

        /// <summary>
        /// COMMAND:*ZxxMTON � zone xx MUTE ON (mutes currently connected source)
        /// RESPONSE:Same response as for *ZxxCONSR
        /// </summary>
        MuteON = 19,

        /// <summary>
        /// COMMAND:*ZxxMTOFF � zone xx MUTE OFF (returns zone output to currently connected source at previous volume setting).
        /// RESPONSE:Same response as for *ZxxCONSR
        /// </summary>
        MuteOFF = 20,

        /// <summary>
        /// COMMAND:*ZxxBASSyyy � zone xx BASS EQ with yyy = EQ level, dB, �12 to +0 (flat) to +12 in 2 dB increments. USE LEAD "0" IN TENS PLACE FOR VALUE LESS THAN 10.
        /// RESPONSE:Same response as for *ZxxSETSR
        /// NOTE: sending this command to the E6D will set override (lock out KEYPAD non-address DIP switches) for this zone until power is cycled
        /// </summary>
        SetBassLevel = 21,

        /// <summary>
        /// COMMAND: *ZxxTREByyy � zone xx TREBLE EQ with yyy = EQ level, dB, �12 to +0 (flat) to +12 in 2 dB increments. USE LEAD "0" IN TENS PLACE FOR VALUE LESS THAN 10.
        /// RESPONSE:Same response as for *ZxxSETSR
        /// NOTE: sending this command to the E6D will set override (lock out KEYPAD non-address DIP switches) for this zone until power is cycled.
        /// </summary>
        SetTrebleLevel = 22,

        /// <summary>
        /// COMMAND:*ZxxGRPON � zone xx SOURCE GROUP ON.
        /// RESPONSE:Same response as for *ZxxSETSR
        /// NOTE: sending this command to the E6D will set override (lock out KEYPAD non-address DIP switches) for this zone until power is cycled.
        /// </summary>
        SetSourceGroupON = 23,

        /// <summary>
        /// COMMAND:*ZxxGRPOFF � zone xx SOURCE GROUP OFF.
        /// RESPONSE:Same response as for *ZxxSETSR
        /// NOTE: sending this command to the E6D will set override (lock out KEYPAD non-address DIP switches) for this zone until power is cycled.
        /// </summary>
        SetSourceGroupOFF = 24,

        /// <summary>
        /// COMMAND:*ZxxVRSTON � zone xx VOLUME RESET ON.
        /// RESPONSE:Same response as for *ZxxSETSR
        /// NOTE: sending this command to the E6D will set override (lock out KEYPAD non-address DIP switches) for this zone until power is cycled.
        /// </summary>
        SetVolumeResetON = 25,

        /// <summary>
        /// COMMAND:*ZxxVRSTOFF � zone xx VOLUME RESET OFF.
        /// RESPONSE:Same response as for *ZxxSETSR
        /// NOTE: sending this command to the E6D will set override (lock out KEYPAD non-address DIP switches) for this zone until power is cycled.
        /// </summary>
        SetVolumeResetOFF = 26,

        /// <summary>
        /// COMMAND:*ZxxLKON � zone xx KEYPAD LOCK ON � This will INHIBIT ANY keypad control input on the zone. This is the same as activating the Parental lock control at a keypad by holding down a SOURCE key for three seconds).
        /// RESPONSE:#ZxxLKON
        /// </summary>
        SetKeypadLockON = 27,

        /// <summary>
        /// COMMAND:*ZxxLKOFF � zone xx KEYPAD LOCK ON � This will RESTORE ALL keypad control input on the zone (useful as Parental lock control) . This is the same as de-activating the Parental lock control at a keypad byholding down a SOURCE key for three seconds).
        /// RESPONSE: #ZxxLKOFF
        /// </summary>
        SetKeypadLockOFF = 28,

        /// <summary>
        /// COMMAND:*VER � Firmware version query.
        /// RESPONSE: #MPU_E6Dvx.yy where x is the major version number and yyis the minor version number.
        /// </summary>
        ReadVersion = 29,

        /// <summary>
        /// RESPONSE: #EXTMON Issued whenever the External MUTE first activates (closure to ground) and 0 whenever the External MUTE de-activates (open connection to ground).
        /// NOTE � there is no COMMAND associated with this response; it is always initiated by a change at the EXT. MUTE input.
        /// </summary>
        ExternalMuteActivated = 30,

        /// <summary>
        /// RESPONSE: #EXTMOFF Issued whenever External MUTE de-activates (open connection to ground).
        /// NOTE � there is no COMMAND associated with this response; it is always initiated by a change at the EXT. MUTE input
        /// </summary>
        ExternalMuteDeactivated = 31,

        /// <summary>
        /// RESPONSE: #? If a command has an error in it (does not adhere to exact command syntax), the E6D will respond with a "#?" string.
        /// NOTE � there is no COMMAND associated with this response; it is always initiated if an error has been found in a command.
        /// </summary>
        ErrorInCommand = 32,


        ///
        /// COMMON NON-NUVO COMMANDS
        ///

        /// <summary>
        /// COMMAND: Three commands => TurnZoneON, SetVolume and SetSource.
        /// RESPONSE: see the single command
        /// </summary>
        SetZoneStatus = 60,

        /// <summary>
        /// COMMAND: Two commands => ReadStatusCONNECT and ReadStatusZONE.
        /// RESPONSE: see the single command
        /// </summary>
        GetZoneStatus = 61,


        /// <summary>
        /// NO Comamnd
        /// </summary>
        NoCommand = 100
    }
    #endregion

    #region NuVo Tuner T2 Commands
    /// <summary>
    /// NuVo Tuner T2 Commands 
	/// </summary>
	[Serializable]
	public enum ENuvoTunerT2Commands
	{
		///
		/// NUVO TUNER T2 COMMANDS
		///

		/// <summary>
		/// COMMAND:*T'r'VER � Get Version Information
		/// RESPONSE: #T'r'VER"NV-T2 FWv1.00 HWv00"
		/// </summary>
		T2ReadVersion = 110,

		/// <summary>
		/// COMMAND:*T'r'STATUS � Get Tuner Status
		/// RESPONSE: Status Indication ON or OFF response
		/// </summary>
		T2GetTunerStatus = 111,

		/// <summary>
		/// COMMAND:*T'r'ON � Turn Tuner On
		/// RESPONSE: Status Indication ON response. No effect, if tuner is already on.
		/// </summary>
		T2TurnTunerOn = 112,

		/// <summary>
		/// COMMAND:*T'r'OFF � Turn Tuner Off
		/// RESPONSE: Status Indication OFF response. No effect, if tuner is already off.
		/// </summary>
		T2TurnTunerOff = 113,

		/// <summary>
		/// COMMAND:*T'r'AMgggg � Tune to AM Frequency
		/// </summary>
		T2TuneAM = 114,

		/// <summary>
		/// COMMAND:*T'r'FMffff � Tune to FM Frequency
		/// EXAMPLES: 
		///   - FM102.5  => 102.5 MHz
		///   - FM102.55 => 102.55 MHz
		///   - FM98     => 98.0 MHz
		///   - FM980    => 98.0 MHz
		///   - FM9800   => 98.0 MHz
		/// </summary>
		T2TuneFM = 115,

		/// <summary>
		/// COMMAND:*T'r'WXi � Tune to Weather Band
		/// </summary>
		T2TuneWX = 116,

		/// <summary>
		/// COMMAND:*T'r'AUX � Select AUX Input
		/// </summary>
		T2SelectAUX = 117,

		/// <summary>
		/// COMMAND:*T'r'PRESETjj � Tune to a Preset
		/// First digit is preset bank (0-9)
		/// Second digit is preset number inside bank (0-9)
		/// </summary>
		T2TunePreset = 118,

		/// <summary>
		/// COMMAND:*T'r'TUNE+ - Tune Up One Step
		/// </summary>
		T2TuneStepUp = 119,

		/// <summary>
		/// COMMAND:*T'r'TUNE- - Tune Down One Step
		/// </summary>
		T2TuneStepDown = 120,

		/// <summary>
		/// COMMAND:*T'r'SEEK+ - Seek Up
		/// </summary>
		T2SeekStepUp = 121,

		/// <summary>
		/// COMMAND:*T'r'SEEK- - Seek Down
		/// </summary>
		T2SeekStepDown = 122,

		/// <summary>
		/// COMMAND:*T'r'SCAN - Scan
		/// </summary>
		T2Scan = 123,

		/// <summary>
		/// COMMAND:*T'r'SCRAM - Change Band to AM
		/// </summary>
		T2ChangeBandAM = 124,

		/// <summary>
		/// COMMAND:*T'r'SCRFM - Change Band to FM
		/// </summary>
		T2ChangeBandFM = 125,

		/// <summary>
		/// COMMAND:*T'r'SCRWX - Change Band to WX
		/// </summary>
		T2ChangeBandWX = 126,

		/// COMMAND:*T'r'SCRAUX - Select AUX Input
		/// </summary>
		T2SelectAUX2 = 127,		// = T2SelectAUX


		/// <summary>
		/// COMMAND:*T'r'ONE - '1' Button
		/// </summary>
		T2Button1 = 141,

		/// <summary>
		/// COMMAND:*T'r'TWO - '2' Button
		/// </summary>
		T2Button2 = 142,

		/// <summary>
		/// COMMAND:*T'r'THREE - '3' Button
		/// </summary>
		T2Button3 = 143,

		/// <summary>
		/// COMMAND:*T'r'FOUR - '4' Button
		/// </summary>
		T2Button4 = 144,

		/// <summary>
		/// COMMAND:*T'r'FIVE - '5' Button
		/// </summary>
		T2Button5 = 145,

		/// <summary>
		/// COMMAND:*T'r'SIX - '6' Button
		/// </summary>
		T2Button6 = 146,

		/// <summary>
		/// COMMAND:*T'r'SEVEN - '7' Button
		/// </summary>
		T2Button7 = 147,

		/// <summary>
		/// COMMAND:*T'r'EIGHT - '8' Button
		/// </summary>
		T2Button8 = 148,

		/// <summary>
		/// COMMAND:*T'r'NINE - '9' Button
		/// </summary>
		T2Button9 = 149,

		/// <summary>
		/// COMMAND:*T'r'ZERO - '0' Button
		/// </summary>
		T2Button0 = 140,

		/// <summary>
		/// COMMAND:*T'r'ENTER - Enter Button
		/// </summary>
		T2ButtonEnter = 150,

		/// <summary>
		/// COMMAND:*T'r'PREDIR - 'PRE/DIR' Button
		/// </summary>
		T2ButtonPreDir = 151,

		/// <summary>
		/// COMMAND:*T'r'PRE - 'PRE/DIR' Button (skip 'Direct Frequency Mode')
		/// </summary>
		T2ButtonPre = 152,

		/// <summary>
		/// COMMAND:*T'r'DIR - 'Direct Frequency Mode' Button
		/// </summary>
		T2ButtonDir = 153,

		/// <summary>
		/// COMMAND:*T'r'MONO - Mono Button
		/// </summary>
		T2ButtonMono = 154,

		/// <summary>
		/// COMMAND:*T'r'STEREO - Stereo Button
		/// </summary>
		T2ButtonStereo = 155,

		/// <summary>
		/// COMMAND:*T'r'DISPLAY - 'Display' Button
		/// </summary>
		T2ButtonDisplay = 156,

		/// <summary>
		/// COMMAND:*T'r'CAT+ - 'Category (up)' Button
		/// </summary>
		T2ButtonCategoryUp = 157,

		/// <summary>
		/// COMMAND:*T'r'CAT- - 'Category (down)' Button
		/// </summary>
		T2ButtonCategoryDown = 158,


		/// <summary>
		/// RESPONSE:#T'r'RDSRT"xyz" - RDS Radio text for Current Fm Station (V1.07 and higher)
		/// </summary>
		T2RDSRT = 170,

		/// <summary>
		/// RESPONSE:#T'r'RDSPSN"xyz" - RDS Program Service Name for Current FM Station (V1.07 and higher)
		/// </summary>
		T2RDSPSN = 171,

		/// <summary>
		/// RESPONSE:#T'r'FREQDESC"xyz" - Tuner is Tuned to a Frequency with a description (V1.07 and higher)
		/// </summary>
		T2FREQDESC = 172,

	}
    #endregion

    #region NuVo Essentia Sources
    /// <summary>
    /// NuVo Essentia Sources
	/// </summary>
	[Serializable]
	public enum ENuvoEssentiaSources
	{
		/// <summary>
		/// Source 1
		/// </summary>
		Source1 = 1,

		/// <summary>
		/// Source 2
		/// </summary>
		Source2 = 2,

		/// <summary>
		/// Source 3
		/// </summary>
		Source3 = 3,

		/// <summary>
		/// Source 4
		/// </summary>
		Source4 = 4,

		/// <summary>
		/// Source 5
		/// </summary>
		Source5 = 5,

		/// <summary>
		/// Source 6
		/// </summary>
		Source6 = 6,

		/// <summary>
		/// NO Source
		/// </summary>
		NoSource = 99,
    }
    #endregion

    #region NuVo Essentia Zones
    /// <summary>
    /// NuVo Essentia Zones
	/// </summary>
	[Serializable]
	public enum ENuvoEssentiaZones
	{
		/// <summary>
		/// Zone 1
		/// </summary>
		Zone1 = 1,

		/// <summary>
		/// Zone 2
		/// </summary>
		Zone2 = 2,

		/// <summary>
		/// Zone 3
		/// </summary>
		Zone3 = 3,

		/// <summary>
		/// Zone 4
		/// </summary>
		Zone4 = 4,

		/// <summary>
		/// Zone 5
		/// </summary>
		Zone5 = 5,

		/// <summary>
		/// Zone 6
		/// </summary>
		Zone6 = 6,

		/// <summary>
		/// Zone 7
		/// </summary>
		Zone7 = 7,

		/// <summary>
		/// Zone 8
		/// </summary>
		Zone8 = 8,

		/// <summary>
		/// Zone 9
		/// </summary>
		Zone9 = 9,

		/// <summary>
		/// Zone 10
		/// </summary>
		Zone10 = 10,

		/// <summary>
		/// Zone 11
		/// </summary>
		Zone11 = 11,

		/// <summary>
		/// Zone 12
		/// </summary>
		Zone12 = 12,

		/// <summary>
		/// No Zone
		/// </summary>
		NoZone = 99,
    }
    #endregion

    #region Zone Power Status
    /// <summary>
	/// Zone Power Status
	/// </summary>
	[Serializable]
	public enum EZonePowerStatus
	{
		/// <summary>
		/// Zone Status is OFF
		/// </summary>
		ZoneStatusOFF = 0,

		/// <summary>
		/// Zone Status is ON
		/// </summary>
		ZoneStatusON = 1,

		/// <summary>
		/// Zone Status is Unknown
		/// </summary>
		ZoneStatusUnknown = 2,
    }
    #endregion

    #region Source Group Status
    /// <summary>
    /// Source Group Status
    /// </summary>
    [Serializable]
    public enum ESourceGroupStatus
    {
        /// <summary>
        /// Source Group is OFF
        /// </summary>
        SourceGroupOFF = 0,

        /// <summary>
        /// Source Group is ON
        /// </summary>
        SourceGroupON = 1,

        /// <summary>
        /// Source Group is Unknown
        /// </summary>
        SourceGroupUnknown = 2,
    }
    #endregion

    #region Volume Reset Status
    /// <summary>
    /// Volume Reset Status
    /// </summary>
    [Serializable]
    public enum EVolumeResetStatus
    {
        /// <summary>
        /// Volume Reset is OFF
        /// </summary>
        VolumeResetOFF = 0,

        /// <summary>
        /// Volume Reset is ON
        /// </summary>
        VolumeResetON = 1,

        /// <summary>
        /// VolumeReset is Unknown
        /// </summary>
        VolumeResetUnknown = 2,
    }
    #endregion

    #region DIP Switch Override Status
    /// <summary>
    /// DIP Switch Override Status
    /// </summary>
    [Serializable]
    public enum EDIPSwitchOverrideStatus
    {
        /// <summary>
        /// DIP Switch Override is OFF
        /// </summary>
        DIPSwitchOverrideOFF = 0,

        /// <summary>
        /// DIP Switch Override is ON
        /// </summary>
        DIPSwitchOverrideON = 1,

        /// <summary>
        /// DIP Switch Override Status is Unknown
        /// </summary>
        DIPSwitchOverrideUnknown = 2,
    }
    #endregion

    #region IR Carrier Frequency
    /// <summary>
    /// IR carrier frequency
    /// </summary>
    [Serializable]
    public enum EIRCarrierFrequency
    {
        /// <summary>
        /// IR carrier frequency is 38kHz
        /// </summary>
        IR38kHz = 0,

        /// <summary>
        /// IR carrier frequency is 56kHz
        /// </summary>
        IR56kHz = 1,

        /// <summary>
        /// IR carrier frequency is unknown (invalid)
        /// </summary>
        IRUnknown = 2,
    }
    #endregion
}