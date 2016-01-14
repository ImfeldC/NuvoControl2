using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommandLine;
using CommandLine.Text; // if you want text formatting helpers (recommended)

using NuvoControl.Common;       // used for CommonOptions


namespace NuvoControl.Test.ConsoleClient
{
    class Options :CommonOptions
    {
      // Communication Settings

      [Option('p', "portname", Required = true, HelpText = "Portname to connect. E.g. COM1 or /dev/ttyAMA0")]
      public string portName { get; set; }

      [Option('b', "baudrate", HelpText = "Baudrate used to connect. E.g. 9600")]
      public int baudRate { get; set; }

      [Option('r', "readtimeout", HelpText = "Read timeout [ms]. E.g. 4000")]
      public int readTimeout { get; set; }


      // Command Send Settings

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


      // Play Sound Settings

      [Option("soundfile", HelpText = "Sound file to play. (e.g. Example.mp3)")]
      public string soundFile { get; set; }

      [Option("soundstream", HelpText = "Sound stream to play. (e.g. http://drs3.radio.net/)")]
      public string soundStream { get; set; }


      // Process Settings

      [Option("process_cmd", HelpText = "Command to be started as own process. (e.g. '/usr/bin/mpg321')")]
      public string processCmd { get; set; }

      [Option("process_arg", HelpText = "Argumnets to be passed with the command. (e.g. 'example.mp3')")]
      public string processArg { get; set; }


      // Mail Settings

      [Option("mail_recepient", HelpText = "Mail address of the recepient. (e.g. 'christian@imfeld.net')")]
      public string mailRecepient { get; set; }

      [Option("mail_subject", HelpText = "Mail subject.")]
      public string mailSubject { get; set; }

      [Option("mail_body", HelpText = "Mail body.")]
      public string mailBody { get; set; }

    
    }
}
