using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// see https://commandline.codeplex.com/
using CommandLine;
using CommandLine.Text; // if you want text formatting helpers (recommended)


namespace NuvoControl.Common
{
    public class CommonOptions
    {
        [Option('v', "verbose", HelpText = "Enable verbose mode. Print-out more messages.")]
        public int volume { get; set; }

        [Option('?', "help", HelpText = "Print detailed help instructions.")]
        public bool Help { get; set; }

        [HelpOption(HelpText = "Dispaly this help screen.")]
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
