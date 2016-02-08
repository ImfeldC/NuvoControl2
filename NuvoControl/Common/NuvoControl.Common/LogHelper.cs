using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ServiceModel.Discovery;
using Common.Logging;
using System.Windows.Forms;

namespace NuvoControl.Common
{
    /// <summary>
    /// Helper class to collect logger and console output methods for complex
    /// structures, like endpoint collection as example.
    /// </summary>
    public class LogHelper
    {

        #region Verbose Mode
        /*
        The six logging levels used by Log are (in order): 
            1.trace (the least serious)
            2.debug
            3.info
            4.warn
            5.error
            6.fatal (the most serious)
        */

        /// <summary>
        /// Global minimum level, for verbose mode.
        /// E.g. if minimum level is set to "info"; all messages of level "info" and higher ("warn", "error", "fatal") are shown
        /// </summary>
        private static LogLevel _minVerboseLogLevel = LogLevel.All;

        /// <summary>
        /// Public accessor for minimum level for verbose mode.
        /// </summary>
        public static LogLevel MinVerboseLogLevel
        {
            get { return LogHelper._minVerboseLogLevel; }
            set { LogHelper._minVerboseLogLevel = value; }
        }


        /// <summary>
        /// Global verbose mode, per default switched-off
        /// </summary>
        private static bool _verbose = false;

        /// <summary>
        /// Public accessor for verbose mode
        /// </summary>
        public static bool Verbose
        {
            get { return LogHelper._verbose; }
            set { LogHelper._verbose = value; }
        }

        /// <summary>
        /// Set all options, passed with command line options.
        /// </summary>
        /// <param name="options">Options passed with command line options.</param>
        public static void SetOptions(CommonOptions options)
        {
            Verbose = options.verbose;
            MinVerboseLogLevel = options.minVerboseLevel;
        }

        #endregion

        /// <summary>
        /// Logs a message in a "standard" way to console and Logger.
        /// </summary>
        /// <param name="strMessage">Message to log.</param>
        public static void Log(LogLevel logLevel, string strMessage)
        {
            Log(logLevel, strMessage, LogManager.GetCurrentClassLogger());
        }

        /// <summary>
        /// Logs a message in a "standard" way to console and Logger.
        /// </summary>
        /// <param name="logLevel">Log Level to log</param>
        /// <param name="strMessage">Message to log.</param>
        /// <param name="logger">Logger to log the message.</param>
        public static void Log(LogLevel logLevel, string strMessage, ILog logger)
        {
            if (Verbose && (logLevel >= _minVerboseLogLevel | _minVerboseLogLevel == LogLevel.All) )
            {
                Console.WriteLine(String.Format("[{0}] {1}", logLevel.ToString(), strMessage));
            }

            switch (logLevel)
            {
                // All Level
                case LogLevel.All:
                    // Log "All Level" as "Info"
                    logger.Info(m => m(strMessage));
                    break;
                // 6.fatal (the most serious)
                case LogLevel.Fatal:
                    logger.Fatal(m => m(strMessage));
                    break;
                // 5.error
                case LogLevel.Error:
                    logger.Error(m => m(strMessage));
                    break;
                // 4.warn
                case LogLevel.Warn:
                    logger.Warn(m => m(strMessage));
                    break;
                // 3.info
                case LogLevel.Info:
                    logger.Info(m => m(strMessage));
                    break;
                // 2.debug
                case LogLevel.Debug:
                    logger.Debug(m => m(strMessage));
                    break;
                // 1.trace (the least serious)
                case LogLevel.Trace:
                    logger.Trace(m => m(strMessage));
                    break;
                // No Level
                case LogLevel.Off:
                    break;
                // default
                default:
                    break;
            }
        }

        /// <summary>
        /// Logs an application startup message in a "standard" way to console and Logger.
        /// </summary>
        /// <param name="strStartMessage"></param>
        public static void LogAppStart(string strStartMessage)
        {
            Log(LogLevel.All, String.Format("**** {0} started. *******", strStartMessage));
            Log(LogLevel.Info, String.Format(">>> Starting {0}  --- Assembly Version={1} / Deployment Version={2} / Product Version={3} (using .NET 4.0) ... ",
                strStartMessage, "n/a", "n/a", Application.ProductVersion));
            //Console.WriteLine(">>> Starting Server Console  --- Assembly Version={0} / Deployment Version={1} / Product Version={2} (using .NET 4.0) ... ",
            //    AppInfoHelper.getAssemblyVersion(), AppInfoHelper.getDeploymentVersion(), Application.ProductVersion);
            Log(LogLevel.Info, String.Format("    Linux={0} / Detected environment: {1}", EnvironmentHelper.isRunningOnLinux(), EnvironmentHelper.getOperatingSystem()));
        }

        /// <summary>
        /// Logs an exception in a "standard" way to console and Logger.
        /// </summary>
        /// <param name="strMessage">Message to log.</param>
        /// <param name="exc">Exception to log.</param>
        public static void LogException(string strMessage, Exception exc)
        {
            Log(LogLevel.Fatal, String.Format("----------------\nException! {0} [{1}]\n----------------\n", strMessage, exc.ToString()));
        }

        /// <summary>
        /// Logs the command line arguments in a "standard" way to console and logger.
        /// </summary>
        /// <param name="args">Arguments passed by command line.</param>
        public static void LogArgs(string[] args)
        {
            string strargs = "";
            foreach (string arg in args)
            {
                strargs += arg;
                strargs += " ";
            }
            Log(LogLevel.Info, String.Format("    Command line arguments: {0}\n", strargs));
        }

        /// <summary>
        /// Static method to print the endpoint collection to the logger.
        /// </summary>
        /// <param name="logger">Logger object.</param>
        /// <param name="endpointCollection">Endpoint collection to print.</param>
        public static void LogEndPoint(ILog logger, Collection<EndpointDiscoveryMetadata> endpointCollection)
        {
            foreach (EndpointDiscoveryMetadata ep in endpointCollection)
            {
                Log(LogLevel.Info, String.Format("Address={0}", ep.Address.ToString()), logger);
                foreach (Uri uri in ep.ListenUris)
                {
                    Log(LogLevel.Info, String.Format("  Uri.AbsolutePath={0}", uri.AbsolutePath), logger);
                    Log(LogLevel.Info, String.Format("  Uri.AbsoluteUri={0}", uri.AbsoluteUri), logger);
                    Log(LogLevel.Info, String.Format("  Uri.Host={0}", uri.Host));
                }
                Log(LogLevel.Info, String.Format("Version={0}", ep.Version), logger);
            }
        }

        /// <summary>
        /// Static method to print the endpoint collection to the console.
        /// </summary>
        /// <param name="endpointCollection">Endpoint collection to print.</param>
        public static void PrintEndPoints(Collection<EndpointDiscoveryMetadata> endpointCollection)
        {
            foreach (EndpointDiscoveryMetadata ep in endpointCollection)
            {
                Console.WriteLine("Address={0}", ep.Address.ToString());
                foreach (Uri uri in ep.ListenUris)
                {
                    Console.WriteLine("  Uri.AbsolutePath={0}", uri.AbsolutePath);
                    Console.WriteLine("  Uri.AbsoluteUri={0}", uri.AbsoluteUri);
                    Console.WriteLine("  Uri.Host={0}", uri.Host);
                }
                Console.WriteLine("Version={0}", ep.Version);
            }
        }

    }
}
