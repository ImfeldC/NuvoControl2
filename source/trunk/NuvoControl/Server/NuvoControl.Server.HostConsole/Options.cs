using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommandLine;
using CommandLine.Text; // if you want text formatting helpers (recommended)

using NuvoControl.Common;       // used for CommonOptions


namespace NuvoControl.Server.HostConsole
{
    class Options : CommonOptions
    {
      [Option('p', "portname", Required = true, HelpText = "Portname to connect. E.g. COM1 or /dev/ttyAMA0")]
      public string portName { get; set; }

      [Option('b', "baudrate", HelpText = "Baudrate used to connect. E.g. 9600")]
      public int baudRate { get; set; }

      [Option('r', "readtimeout", HelpText = "Read timeout [ms]. E.g. 4000")]
      public int readTimeout { get; set; }

      [Option("senddata", HelpText = "Data to be send. E.g. *VER or *Z01OFF")]
      public string sendData { get; set; }

      [Option("command", HelpText = "Nuvo Essentia command to be send. E.g. ReadVersion or TurnALLZoneOFF (see ENuvoEssentiaCommands)")]
      public string strCommand { get; set; }

      [Option("zone", HelpText = "Nuvo Essentia zone identifier. E.g. Zone1 (see ENuvoEssentiaZones)")]
      public string strZone { get; set; }

      [Option("source", HelpText = "Nuvo Essentia source identifier. E.g. Source1 (see ENuvoEssentiaSources)")]
      public string strSource { get; set; }

      [Option("powerstatus", HelpText = "Nuvo Essentia zone power status. Either ZoneStatusOFF or ZoneStatusON (see EZonePowerStatus)")]
      public string strPowerStatus { get; set; }

      [Option("volume", HelpText = "Volume level.")]
      public int volume { get; set; }

    }
}
