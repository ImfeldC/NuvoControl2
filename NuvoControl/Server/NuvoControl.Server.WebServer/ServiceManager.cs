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

        // Zones & Sources
        private List<Zone> zones = null;
        private List<Source> sources = null;
        public List<Zone> Zones
        {
            get { return zones; }
        }
        public List<Source> Sources
        {
            get { return sources; }
        }

        // Configuration Service
        private FindResponse discoveredConfigurationClients = null;
        public FindResponse DiscoveredConfigurationClients
        {
            get { return discoveredConfigurationClients; }
        }
        public int NumOfDiscoveredConfigurationHosts
        {
            get { return (((DiscoveredConfigurationClients != null) && (DiscoveredConfigurationClients.Endpoints.Count > 0)) ? DiscoveredConfigurationClients.Endpoints.Count : -1); }
        }
        public string ConfigurationHostAdress
        {
            get { return (((DiscoveredConfigurationClients != null) && (DiscoveredConfigurationClients.Endpoints.Count > 0)) ? DiscoveredConfigurationClients.Endpoints[0].Address.ToString() : ""); }
        }

        // Monitor & Control Service
        private FindResponse discoveredMonitorControlClients = null;
        public FindResponse DiscoveredMonitorControlClients
        {
            get { return discoveredMonitorControlClients; }
        }
        public int NumOfDiscoveredMonitorControlHosts
        {
            get { return (((discoveredMonitorControlClients != null) && (discoveredMonitorControlClients.Endpoints.Count > 0)) ? discoveredMonitorControlClients.Endpoints.Count : -1); ; }
        }
        public string MonitorControlHostAdress
        {
            get { return (((discoveredMonitorControlClients != null) && (discoveredMonitorControlClients.Endpoints.Count > 0)) ? discoveredMonitorControlClients.Endpoints[0].Address.ToString() : ""); ; }
        }


        public ServiceManager()
        {
            _log.Trace(m => m("ServiceManager created."));
            zones = new List<Zone>();
            sources = new List<Source>();
        }

        private MonitorAndControlClient getMCProxy()
        {
            MonitorAndControlClient mcProxy = null;
            IMonitorAndControlCallback serverCallback = new ServerCallback();
            mcProxy = new MonitorAndControlClient(new InstanceContext(serverCallback));
            // Connect to the discovered service endpoint
            mcProxy.Endpoint.Address = Global.ServiceManager.DiscoveredMonitorControlClients.Endpoints[0].Address;
            return mcProxy;
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
            }
            catch (Exception exc)
            {
                _log.Trace(m => m("Exception: during discovering <Configuration Service>: {0}", exc));
                discoveredConfigurationClients = null;
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
            }
            catch (Exception exc)
            {
                _log.Trace(m => m("Exception: during discovering <MonitorControl Service>: {0}", exc));
                discoveredMonitorControlClients = null;
            }

        }

        /// <summary>
        /// Reads (loads) the configuration of the system from the configuration service.
        /// </summary>
        public void LoadConfiguration()
        {
            _log.Trace(m => m("LoadConfiguration called."));

            if ( (DiscoveredConfigurationClients!=null) && (DiscoveredConfigurationClients.Endpoints.Count>0) )
            {
                ConfigureClient cfgIfc = null;
                cfgIfc = new ConfigureClient();
                // Connect to the discovered service endpoint
                cfgIfc.Endpoint.Address = DiscoveredConfigurationClients.Endpoints[0].Address;


                Graphic graphic = cfgIfc.GetGraphicConfiguration();
                _log.Trace(m => m("All graphic details: {0}", graphic.ToString()));

                // read available zones (via graphic configuration)
                // Root -> Graphic -> Building -> Floors -> Zone(s)
                zones.Clear();
                foreach (Floor floor in graphic.Building.Floors)
                {
                    _log.Trace(m => m("Read FLOOR with id={0}.", floor.Id));
                    foreach (Zone zone in floor.Zones)
                    {
                        _log.Trace(m => m("Zone found with id {0} with name {1}.", zone.Id.ToString(), zone.Name));
                        zones.Add(zone);
                    }
                }
                _log.Trace(m => m("Totally {0} zones found!", zones.Count));


                // read available sources (via graphic configuration)
                // Root -> Graphic -> Source(s)
                sources.Clear();
                foreach (Source source in graphic.Sources)
                {
                    _log.Trace(m => m("SOURCE found with id {0}, with the name {1}.", source.Id.ToString(), source.Name));
                    sources.Add(source);
                }
                _log.Trace(m => m("Totally {0} sources found!", sources.Count));
            }
        }

        /// <summary>
        /// Retruns Source configuration Object for a specific Source Id.
        /// </summary>
        /// <param name="sourceId">Source Id</param>
        /// <returns>Source object. Returns an empty object in case the Source Id was not found.</returns>
        public Source GetSource( Address sourceId )
        {
            Source foundSource = new Source();
            foreach (Source source in sources)
            {
                if (source.Id == sourceId)
                {
                    foundSource = source;
                    break;
                }
            }
            _log.Trace(m => m("Source found!", foundSource.ToString()));
            return foundSource;
        }

        public Source GetSource(string sourceName)
        {
            Source foundSource = new Source();
            foreach (Source source in sources)
            {
                if (source.Name == sourceName)
                {
                    foundSource = source;
                    break;
                }
            }
            _log.Trace(m => m("Source found!", foundSource.ToString()));
            return foundSource;
        }

        /// <summary>
        /// Returns Zone configuration Object for a specific Zone Id.
        /// </summary>
        /// <param name="zoneId">Zone Id</param>
        /// <returns>Zone object. Returns an empty object in case the Zone Id was not found.</returns>
        public Zone GetZone(Address zoneId)
        {
            Zone foundZone = new Zone();
            foreach (Zone zone in zones)
            {
                if (zone.Id == zoneId)
                {
                    foundZone = zone;
                    break;
                }
            }
            _log.Trace(m => m("Zone found!", foundZone.ToString()));
            return foundZone;
        }

        public ZoneState GetZoneState(Address zoneId)
        {
            MonitorAndControlClient mcProxy = null;
            IMonitorAndControlCallback serverCallback = new ServerCallback();
            mcProxy = new MonitorAndControlClient(new InstanceContext(serverCallback));
            // Connect to the discovered service endpoint
            mcProxy.Endpoint.Address = Global.ServiceManager.DiscoveredMonitorControlClients.Endpoints[0].Address;
            mcProxy.Connect();

            ZoneState zoneState = mcProxy.GetZoneState(zoneId);
            _log.Trace(m => m("Read zone configuration for zone with id {0}. Zone State = [{1}]", zoneId.ToString(), zoneState.ToString()));

            mcProxy.Disconnect();
            return zoneState;
        }

        public ZoneState SwitchZone(Address zoneId)
        {
            MonitorAndControlClient mcProxy = getMCProxy();
            mcProxy.Connect();

            ZoneState zoneState = mcProxy.GetZoneState(zoneId);
            zoneState.PowerStatus = !zoneState.PowerStatus;
            mcProxy.SetZoneState(zoneId, zoneState);
            zoneState = mcProxy.GetZoneState(zoneId);

            mcProxy.Disconnect();
            return zoneState;
        }

        public ZoneState SwitchSource(Address zoneId, Address sourceId)
        {
            MonitorAndControlClient mcProxy = getMCProxy();
            mcProxy.Connect();

            ZoneState zoneState = mcProxy.GetZoneState(zoneId);
            zoneState.Source = sourceId;
            mcProxy.SetZoneState(zoneId, zoneState);
            zoneState = mcProxy.GetZoneState(zoneId);

            mcProxy.Disconnect();
            return zoneState;
        }

        public ZoneState VolumeUp(Address zoneId)
        {
            return AdjustVolume(zoneId, +3);
        }

        public ZoneState VolumeDown(Address zoneId)
        {
            return AdjustVolume(zoneId, -3);
        }

        private ZoneState AdjustVolume(Address zoneId, int adjVolume)
        {
            MonitorAndControlClient mcProxy = getMCProxy();
            mcProxy.Connect();

            ZoneState zoneState = mcProxy.GetZoneState(zoneId);
            zoneState.Volume = zoneState.Volume + adjVolume;
            mcProxy.SetZoneState(zoneId, zoneState);
            zoneState = mcProxy.GetZoneState(zoneId);

            mcProxy.Disconnect();
            return zoneState;
        }

    }
}