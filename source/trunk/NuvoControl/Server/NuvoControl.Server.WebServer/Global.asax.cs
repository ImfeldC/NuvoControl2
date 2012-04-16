using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

using System.ServiceModel;
using System.ServiceModel.Discovery;

using NuvoControl.Common.Configuration;
using NuvoControl.Common;

using NuvoControl.Server.WebServer.ConfigurationServiceReference;
using NuvoControl.Server.WebServer.MonitorAndControlServiceReference;


namespace NuvoControl.Server.WebServer
{
    public class Global : System.Web.HttpApplication
    {
        //
        // Configuration Service
        //

        private static FindResponse discoveredConfigurationClients = null;
        private static int numOfDiscoveredConfigurationHosts = -99;
        private static string configurationHostAdress = "";

        public static FindResponse DiscoveredConfigurationClients
        {
            get { return discoveredConfigurationClients; }
        }
        public static int NumOfDiscoveredConfigurationHosts
        {
            get { return numOfDiscoveredConfigurationHosts; }
        }
        public static string ConfigurationHostAdress
        {
            get { return configurationHostAdress; }
        }


        //
        // Monitor & Control Service
        //

        private static FindResponse discoveredMonitorControlClients = null;
        private static int numOfDiscoveredMonitorControlHosts = -99;
        private static string monitorControlHostAdress = "";

        public static FindResponse DiscoveredMonitorControlClients
        {
            get { return Global.discoveredMonitorControlClients; }
        }
        public static int NumOfDiscoveredMonitorControlHosts
        {
            get { return Global.numOfDiscoveredMonitorControlHosts; }
        }
        public static string MonitorControlHostAdress
        {
            get { return Global.monitorControlHostAdress; }
        }


        /// <summary>
        /// Searches for Configuration Services available on the network.
        /// </summary>
        public void DiscoverConfigurationServices()
        {
            try
            {
                // ------- DISCOVERY ----------

                Console.WriteLine("Start discovering <Configuration Service> ...");

                DiscoveryClient discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint());
                FindCriteria criteria = new FindCriteria(typeof(IConfigure));
                discoveredConfigurationClients = discoveryClient.Find(criteria);
                discoveryClient.Close();

                Console.WriteLine("{0} <Configuration Service> found.", discoveredConfigurationClients.Endpoints.Count);
                LogHelper.PrintEndPoints(discoveredConfigurationClients.Endpoints);
                numOfDiscoveredConfigurationHosts = discoveredConfigurationClients.Endpoints.Count;
                if (numOfDiscoveredConfigurationHosts > 0)
                    configurationHostAdress = discoveredConfigurationClients.Endpoints[0].Address.ToString();
                else
                    configurationHostAdress = "";
            }
            catch (Exception exc)
            {
                Console.WriteLine("Exception: during discovering <Configuration Service>: {0}", exc);
                numOfDiscoveredConfigurationHosts = -1;
                configurationHostAdress = exc.ToString();
            }

        }

        /// <summary>
        /// Searches for Monitor&Control Servcies available on the network.
        /// </summary>
        public void DiscoverMonitorControlServices()
        {
            try
            {
                // ------- DISCOVERY ----------

                Console.WriteLine("Start discovering <MonitorControl Service> ...");

                DiscoveryClient discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint());
                FindCriteria criteria = new FindCriteria(typeof(IMonitorAndControl));
                discoveredMonitorControlClients = discoveryClient.Find(criteria);
                discoveryClient.Close();

                Console.WriteLine("{0} <MonitorControl Service> found.", discoveredMonitorControlClients.Endpoints.Count);
                LogHelper.PrintEndPoints(discoveredMonitorControlClients.Endpoints);
                numOfDiscoveredMonitorControlHosts = discoveredMonitorControlClients.Endpoints.Count;
                if (numOfDiscoveredMonitorControlHosts > 0)
                    monitorControlHostAdress = discoveredMonitorControlClients.Endpoints[0].Address.ToString();
                else
                    monitorControlHostAdress = "";
            }
            catch (Exception exc)
            {
                Console.WriteLine("Exception: during discovering <MonitorControl Service>: {0}", exc);
                numOfDiscoveredMonitorControlHosts = -1;
                monitorControlHostAdress = exc.ToString();
            }

        }


        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup

            DiscoverConfigurationServices();
            DiscoverMonitorControlServices();
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown

        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started

        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.

        }

    }
}
