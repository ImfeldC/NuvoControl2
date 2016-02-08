using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommandLine;
using CommandLine.Text; // if you want text formatting helpers (recommended)

using NuvoControl.Common;       // used for CommonOptions


namespace NuvoControl.Client.ConsoleClient
{
    class Options : CommonOptions
    {
      [Option('u', "uri", Required = false, HelpText = "Full URI of the server to connect.")]
      public string Uri { get; set; }

      [Option('h', "host", HelpText = "Hostname or IP address of the server to connect.")]
      public string Host { get; set; }

      [Option('p', "port", DefaultValue = 8080, HelpText = "Port number of the service.")]
      public int Port { get; set; }

    }
}
