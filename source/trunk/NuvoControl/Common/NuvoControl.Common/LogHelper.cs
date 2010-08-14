using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ServiceModel.Discovery;
using Common.Logging;

namespace NuvoControl.Common
{
    /// <summary>
    /// Helper class to collect logger and console output methods for complex
    /// structures, like endpoint collection as example.
    /// </summary>
    public class LogHelper
    {

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
