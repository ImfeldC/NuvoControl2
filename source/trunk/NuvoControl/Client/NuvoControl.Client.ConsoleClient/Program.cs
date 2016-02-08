/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Client.ConsoleClient
 *   Author:         Christian Imfeld
 *   Creation Date:  15.03.2015
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 15.03.2015, Christian Imfeld: Initial implementation.
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Collections.ObjectModel;

using CommandLine;
using CommandLine.Text; // if you want text formatting helpers (recommended)

using Common.Logging;
using NuvoControl.Common;

using NuvoControl.Server.FunctionService;
using NuvoControl.Server.MonitorAndControlService;
using NuvoControl.Server.ConfigurationService;
//using NuvoControl.Client.ServiceAccess;

namespace NuvoControl.Client.ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            ILog _log = LogManager.GetCurrentClassLogger();

            // Load command line argumnets
            var options = new Options();
            CommandLine.Parser.Default.ParseArguments(args, options);
            // Set global verbose mode
            LogHelper.SetOptions(options);
            LogHelper.LogAppStart("**** Console client started. *******");
            if (options.Help)
            {
                Console.WriteLine(options.GetUsage());
            }


            // Discover Monitor&Control Service
            //FindResponse fr = Discover("IMonitorAndControl", typeof(IMonitorAndControl), 5);


            LogHelper.Log(LogLevel.All, ">>> Press <Enter> to stop the services.");
            Console.ReadLine();

        }

        /// <summary>
        /// Discover specific servcie type.
        /// </summary>
        /// <param name="identifier">String identifier. Only used in console/log output.</param>
        /// <param name="type">Service type.</param>
        /// <param name="timespan">Timeout to discover a service.</param>
        /// <returns></returns>
        private static FindResponse Discover(string identifier, Type type, int timespan)
        {
            FindResponse discovered = null;
            ILog _log = LogManager.GetCurrentClassLogger();
            try
            {
                // ------- DISCOVERY ----------
                LogHelper.Log(LogLevel.Info, String.Format("Start discovering {0} ...", identifier));

                DiscoveryClient discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint());
                FindCriteria criteria = new FindCriteria(type);
                criteria.Duration = TimeSpan.FromSeconds(timespan);
                discovered = discoveryClient.Find(criteria);

                LogHelper.Log(LogLevel.Info, String.Format("{0} Discovery: {1} services found.", identifier, discovered.Endpoints.Count));
                LogHelper.PrintEndPoints(discovered.Endpoints);
                discoveryClient.Close();
            }
            catch (FaultException<ArgumentException> exc)
            {
                _log.Fatal(m => m("FaultException: {0}", exc));
            }
            catch (Exception exc)
            {
                _log.Fatal(m => m("Exception: {0}", exc));
            }
            return discovered;
        }

    
    }
}
