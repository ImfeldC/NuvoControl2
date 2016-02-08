using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common.Logging; 

// see https://commandline.codeplex.com/
using CommandLine;
using CommandLine.Text; // if you want text formatting helpers (recommended)


namespace NuvoControl.Common
{
    public class CommonOptions
    {
        [Option('v', "verbose", HelpText = "Enable verbose mode. Print-out more messages.")]
        public bool verbose { get; set; }

        /// <summary>
        /// The six logging levels used by Log are (in order):
        ///     0.all
        ///     1.trace (the least serious)
        ///     2.debug
        ///     3.info
        ///     4.warn
        ///     5.error
        ///     6.fatal (the most serious)
        ///     7.Off
        /// </summary>
        [Option('l', "min-verbose-level", HelpText = "Define minimum log level to print-out more messages.")]
        public LogLevel minVerboseLevel { get; set; }

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
