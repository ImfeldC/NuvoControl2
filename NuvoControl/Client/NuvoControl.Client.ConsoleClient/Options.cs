using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommandLine;
using CommandLine.Text; // if you want text formatting helpers (recommended)

namespace NuvoControl.Client.ConsoleClient
{
    class Options
    {
      [Option('u', "uri", Required = false, HelpText = "Full URI of the server to connect.")]
      public string Uri { get; set; }

      [Option('h', "host", HelpText = "Hostname or IP address of the server to connect.")]
      public string Host { get; set; }

      [Option('p', "port", DefaultValue = 8080, HelpText = "Port number of the service.")]
      public int Port { get; set; }

      [Option('v', null, HelpText = "Print details during execution.")]
      public bool Verbose { get; set; }

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
