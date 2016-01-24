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
        /// Logs a message in a "standard" way to console and Logger.
        /// </summary>
        /// <param name="strMessage">Message to log.</param>
        public static void Log(string strMessage)
        {
            Log(strMessage, LogManager.GetCurrentClassLogger());
        }

        /// <summary>
        /// Logs an application startup message in a "standard" way to console and Logger.
        /// </summary>
        /// <param name="strStartMessage"></param>
        public static void LogAppStart(string strStartMessage)
        {
            Console.WriteLine(String.Format("**** {0} started. *******", strStartMessage));
            Log(String.Format(">>> Starting {0}  --- Assembly Version={1} / Deployment Version={2} / Product Version={3} (using .NET 4.0) ... ",
                strStartMessage, "n/a", "n/a", Application.ProductVersion));
            //Console.WriteLine(">>> Starting Server Console  --- Assembly Version={0} / Deployment Version={1} / Product Version={2} (using .NET 4.0) ... ",
            //    AppInfoHelper.getAssemblyVersion(), AppInfoHelper.getDeploymentVersion(), Application.ProductVersion);
            Log(String.Format("    Linux={0} / Detected environment: {1}", EnvironmentHelper.isRunningOnLinux(), EnvironmentHelper.getOperatingSystem()));
        }

        /// <summary>
        /// Logs an exception in a "standard" way to console and Logger.
        /// </summary>
        /// <param name="strMessage">Message to log.</param>
        /// <param name="exc">Exception to log.</param>
        public static void LogException(string strMessage, Exception exc)
        {
            Log(String.Format("----------------\nException! {0} [{1}]\n----------------\n", strMessage, exc.ToString()));
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
            Log(String.Format("    Command line arguments: {0}\n", strargs));
        }

        /// <summary>
        /// Logs a message in a "standard" way to console and Logger.
        /// </summary>
        /// <param name="strMessage">Message to log.</param>
        /// <param name="logger">Logger to log the message.</param>
        public static void Log(string strMessage, ILog logger)
        {
            if (Verbose)
            {
                Console.WriteLine(strMessage);
            }
            logger.Debug(m => m(strMessage));
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
                logger.Info(m => m("Address={0}", ep.Address.ToString()));
                foreach (Uri uri in ep.ListenUris)
                {
                    logger.Info(m => m("  Uri.AbsolutePath={0}", uri.AbsolutePath));
                    logger.Info(m => m("  Uri.AbsoluteUri={0}", uri.AbsoluteUri));
                    logger.Info(m => m("  Uri.Host={0}", uri.Host));
                }
                logger.Info(m => m("Version={0}", ep.Version));
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
