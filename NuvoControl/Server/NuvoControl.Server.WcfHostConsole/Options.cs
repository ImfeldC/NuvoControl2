using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommandLine;
using CommandLine.Text; // if you want text formatting helpers (recommended)

using NuvoControl.Common;       // used for CommonOptions


namespace NuvoControl.Server.WcfHostConsole
{
    class Options : CommonOptions
    {
      [Option('p', "portname", Required = true, HelpText = "Portname to connect. E.g. COM1 or /dev/ttyAMA0")]
      public string portName { get; set; }

      [Option('b', "baudrate", HelpText = "Baudrate used to connect. E.g. 9600")]
      public int baudRate { get; set; }

      [Option('r', "readtimeout", HelpText = "Read timeout [ms]. E.g. 4000")]
      public int readTimeout { get; set; }

    }
}
