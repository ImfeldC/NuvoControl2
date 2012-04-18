using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ServiceModel;
using System.ServiceModel.Discovery;
using Common.Logging;

using NuvoControl.Server.WebServer.ConfigurationServiceReference;
using NuvoControl.Server.WebServer.MonitorAndControlServiceReference;

using NuvoControl.Common.Configuration;
using NuvoControl.Common;



namespace NuvoControl.Server.WebServer
{
    public class ServiceManager
    {
        private static ILog _log = LogManager.GetCurrentClassLogger();

        private List<Zone> zones = null;
        private List<Source> sources = null;

        //
        // Configuration Service
        //

        private FindResponse discoveredConfigurationClients = null;
        private int numOfDiscoveredConfigurationHosts = -99;
        private string configurationHostAdress = "";

        public FindResponse DiscoveredConfigurationClients
        {
            get { return discoveredConfigurationClients; }
        }
        public int NumOfDiscoveredConfigurationHosts
        {
            get { return numOfDiscoveredConfigurationHosts; }
        }
        public string ConfigurationHostAdress
        {
            get { return configurationHostAdress; }
        }


        //
        // Monitor & Control Service
        //

        private FindResponse discoveredMonitorControlClients = null;
        private int numOfDiscoveredMonitorControlHosts = -99;
        private string monitorControlHostAdress = "";

        public FindResponse DiscoveredMonitorControlClients
        {
            get { return discoveredMonitorControlClients; }
        }
        public int NumOfDiscoveredMonitorControlHosts
        {
            get { return numOfDiscoveredMonitorControlHosts; }
        }
        public string MonitorControlHostAdress
        {
            get { return monitorControlHostAdress; }
        }


        public ServiceManager()
        {
            _log.Trace(m => m("ServiceManager created."));
            zones = new List<Zone>();
            sources = new List<Source>();
        }

        /// <summary>
        /// Searches for Configuration Services available on the network.
        /// </summary>
        public void DiscoverConfigurationServices()
        {
            try
            {
                // ------- DISCOVERY ----------

                _log.Trace(m => m("Start discovering <Configuration Service> ..."));

                DiscoveryClient discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint());
                FindCriteria criteria = new FindCriteria(typeof(IConfigure));
                discoveredConfigurationClients = discoveryClient.Find(criteria);
                discoveryClient.Close();

                _log.Trace(m => m("{0} <Configuration Service> found.", discoveredConfigurationClients.Endpoints.Count));
                LogHelper.LogEndPoint(_log,discoveredConfigurationClients.Endpoints);
                numOfDiscoveredConfigurationHosts = discoveredConfigurationClients.Endpoints.Count;
                if (numOfDiscoveredConfigurationHosts > 0)
                    configurationHostAdress = discoveredConfigurationClients.Endpoints[0].Address.ToString();
                else
                    configurationHostAdress = "";
            }
            catch (Exception exc)
            {
                _log.Trace(m => m("Exception: during discovering <Configuration Service>: {0}", exc));
                numOfDiscoveredConfigurationHosts = -1;
                configurationHostAdress = exc.ToString();
            }

        }

        /// <summary>
        /// Searches for Monitor&Control Services available on the network.
        /// </summary>
        public void DiscoverMonitorControlServices()
        {
            try
            {
                // ------- DISCOVERY ----------

                _log.Trace(m => m("Start discovering <MonitorControl Service> ..."));

                DiscoveryClient discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint());
                FindCriteria criteria = new FindCriteria(typeof(IMonitorAndControl));
                discoveredMonitorControlClients = discoveryClient.Find(criteria);
                discoveryClient.Close();

                _log.Trace(m => m("{0} <MonitorControl Service> found.", discoveredMonitorControlClients.Endpoints.Count));
                LogHelper.LogEndPoint(_log,discoveredMonitorControlClients.Endpoints);
                numOfDiscoveredMonitorControlHosts = discoveredMonitorControlClients.Endpoints.Count;
                if (numOfDiscoveredMonitorControlHosts > 0)
                    monitorControlHostAdress = discoveredMonitorControlClients.Endpoints[0].Address.ToString();
                else
                    monitorControlHostAdress = "";
            }
            catch (Exception exc)
            {
                _log.Trace(m => m("Exception: during discovering <MonitorControl Service>: {0}", exc));
                numOfDiscoveredMonitorControlHosts = -1;
                monitorControlHostAdress = exc.ToString();
            }

        }


        public void LoadConfiguration()
        {
            _log.Trace(m => m("LoadConfiguration called."));

            ConfigureClient cfgIfc = null;
            cfgIfc = new ConfigureClient();
            // Connect to the discovered service endpoint
            cfgIfc.Endpoint.Address = DiscoveredConfigurationClients.Endpoints[0].Address;


            Graphic graphic = cfgIfc.GetGraphicConfiguration();
            _log.Trace(m => m("All graphic details: {0}", graphic.ToString()));

            // read available zones (via graphic configuration)
            // Root -> Graphic -> Building -> Floors -> Zone(s)
            zones.Clear();
            for (int iFloorId = 0; iFloorId < graphic.Building.Floors.Count; iFloorId++)
            {
                _log.Trace(m => m("Read FLOOR {0}.", iFloorId));
                for (int iZoneId = 1; iZoneId < graphic.Building.Floors[iFloorId].Zones.Count; iZoneId++)
                {
                    Zone zone = graphic.Building.Floors[iFloorId].Zones[iZoneId];
                    _log.Trace(m => m("Zone found with id {0} on FLOOR {1}.", zone.Id.ToString(), iFloorId));
                    zones.Add(zone);
                }
            }
            _log.Trace(m => m("Totally {0} zones found!", zones.Count));


            // read available sources (via graphic configuration)
            // Root -> Graphic -> Source(s)
            sources.Clear();
            for (int iSourceId = 0; iSourceId < graphic.Sources.Count; iSourceId++)
            {
                Source source = graphic.Sources[iSourceId];
                _log.Trace(m => m("SOURCE found with id {0}.", source.Id.ToString()));
                sources.Add(source);
            }
            _log.Trace(m => m("Totally {0} sources found!", sources.Count));
        }

    }
}