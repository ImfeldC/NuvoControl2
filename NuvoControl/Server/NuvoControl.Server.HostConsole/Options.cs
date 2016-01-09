using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommandLine;
using CommandLine.Text; // if you want text formatting helpers (recommended)

namespace NuvoControl.Server.HostConsole
{
    class Options
    {
      [Option('p', "portname", Required = true, HelpText = "Portname to connect. E.g. COM1 or /dev/ttyAMA0")]
      public string portName { get; set; }

      [Option('b', "baudrate", HelpText = "Baudrate used to connect. E.g. 9600")]
      public int baudRate { get; set; }

      [Option('r', "readtimeout", HelpText = "Read timeout [ms]. E.g. 4000")]
      public int readTimeout { get; set; }

      [Option('s', "senddata", HelpText = "Data to be send. E.g. *VER or *Z01OFF")]
      public string sendData { get; set; }

      [Option('c', "command", HelpText = "Nuvo Essentia command to be send. E.g. ReadVersion or TurnALLZoneOFF (see ENuvoEssentiaCommands)")]
      public string strCommand { get; set; }

      [Option('z', "zone", HelpText = "Nuvo Essentia zone identifier. E.g. Zone1 (see ENuvoEssentiaZones)")]
      public string strZone { get; set; }

      [Option('o', "source", HelpText = "Nuvo Essentia source identifier. E.g. Source1 (see ENuvoEssentiaSources)")]
      public string strSource { get; set; }

      [Option('w', "powerstatus", HelpText = "Nuvo Essentia zone power status. Either ZoneStatusOFF or ZoneStatusON (see EZonePowerStatus)")]
      public string strPowerStatus { get; set; }

      [Option('v', "volume", HelpText = "Volume level.")]
      public int volume { get; set; }

      [Option('?', "help", HelpText = "Print detailed help instructions.")]
      public bool Help { get; set; }

      [HelpOption]
      public string GetUsage()
      {
        // this without using CommandLine.Text
        //var usage = new StringBuilder();
        //usage.AppendLine("Quickstart Application 1.0");
        //usage.AppendLine("Read user manual for usage instructions...");
        //return usage.ToString();

        return HelpText.AutoBuild(this,
            (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
      }
    }
}
