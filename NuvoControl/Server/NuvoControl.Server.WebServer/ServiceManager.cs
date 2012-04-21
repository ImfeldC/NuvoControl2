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
    /// <summary>
    /// This class is used a general manager to handle the two services:
    /// - Configuration Service
    /// - Monitor & Control Service
    /// </summary>
    public class ServiceManager
    {
        /// <summary>
        /// Private member for the logging object.
        /// </summary>
        private static ILog _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Default constructor. Initializes the privat members.
        /// </summary>
        public ServiceManager()
        {
            _log.Trace(m => m("ServiceManager created."));
            zones = new List<Zone>();
            sources = new List<Source>();
        }


        #region Zones & Sources
        //
        // Zones & Sources
        //

        /// <summary>
        /// Private member to store the availables zone configuration.
        /// </summary>
        private List<Zone> zones = null;

        /// <summary>
        /// Private member to store the available source configuration.
        /// </summary>
        private List<Source> sources = null;

        /// <summary>
        /// Public access method to retrieve configured zones.
        /// </summary>
        public List<Zone> Zones
        {
            get { return zones; }
        }

        /// <summary>
        /// Public access method to retrieve configured sources.
        /// </summary>
        public List<Source> Sources
        {
            get { return sources; }
        }
        #endregion

        #region Configuration Service
        //
        // Configuration Service
        //

        /// <summary>
        /// Private member to store discovered configuration service hosts.
        /// </summary>
        private FindResponse _DiscoveredConfigurationServiceHosts = null;

        /// <summary>
        /// Private member to store the adress (URL) of the configuration service host.
        /// e.g. http://imfi-laptopdell:8080/ConfigurationService
        /// </summary>
        private EndpointAddress _ConfigurationServiceHostAddress = null;

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
                _DiscoveredConfigurationServiceHosts = discoveryClient.Find(criteria);
                discoveryClient.Close();

                _log.Trace(m => m("{0} <Configuration Service> found.", _DiscoveredConfigurationServiceHosts.Endpoints.Count));
                LogHelper.LogEndPoint(_log, _DiscoveredConfigurationServiceHosts.Endpoints);

                if (_DiscoveredConfigurationServiceHosts.Endpoints.Count > 0)
                {
                    // store service host address
                    _ConfigurationServiceHostAddress = _DiscoveredConfigurationServiceHosts.Endpoints[0].Address;
                }
            }
            catch (Exception exc)
            {
                _log.Trace(m => m("Exception: during discovering <Configuration Service>: {0}", exc));
                _DiscoveredConfigurationServiceHosts = null;
            }

        }

        /// <summary>
        /// Retruns list of discovered configuration service hosts.
        /// </summary>
        public FindResponse DiscoveredConfigurationServiceHosts
        {
            get { return _DiscoveredConfigurationServiceHosts; }
        }

        /// <summary>
        /// Returns the number of discovered configuration service hosts. Returns -1 in case the search was not performed or an error occured.
        /// </summary>
        public int NumOfDiscoveredConfigurationServiceHosts
        {
            get { return (((DiscoveredConfigurationServiceHosts != null) && (DiscoveredConfigurationServiceHosts.Endpoints.Count >= 0)) ? DiscoveredConfigurationServiceHosts.Endpoints.Count : -1); }
        }

        /// <summary>
        /// Retruns the address (URL) of the configuration service host.
        /// This can either be a discovered address using the DiscoverConfigurationServices method or an address set by the application using the set method.
        /// </summary>
        public EndpointAddress ConfigurationServiceHostAdress
        {
            get { return _ConfigurationServiceHostAddress; }
            set 
            {
                _log.Trace(m => m("Set ConfigurationServiceHostAdress to {0}.", value));
                _ConfigurationServiceHostAddress = value; 
            }
        }

        /// <summary>
        /// Private member to store graphic configuration.
        /// </summary>
        private Graphic _graphic = null;

        /// <summary>
        /// Retruns the graphic configuration for the whole system.
        /// </summary>
        public Graphic Graphic
        {
            get { return _graphic; }
        }

        /// <summary>
        /// Reads (loads) the configuration of the system from the configuration service.
        /// </summary>
        public void LoadConfiguration()
        {
            _log.Trace(m => m("LoadConfiguration called for {0}.", (_ConfigurationServiceHostAddress == null) ? "null" : _ConfigurationServiceHostAddress.ToString()));

            if (_ConfigurationServiceHostAddress != null)
            {
                ConfigureClient cfgIfc = null;
                cfgIfc = new ConfigureClient();
                // Connect to the discovered service endpoint
                cfgIfc.Endpoint.Address = _ConfigurationServiceHostAddress;


                _graphic = cfgIfc.GetGraphicConfiguration();
                _log.Trace(m => m("All graphic details: {0}", _graphic.ToString()));

                // read available zones (via graphic configuration)
                // Root -> Graphic -> Building -> Floors -> Zone(s)
                zones.Clear();
                foreach (Floor floor in _graphic.Building.Floors)
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
                foreach (Source source in _graphic.Sources)
                {
                    _log.Trace(m => m("SOURCE found with id {0}, with the name {1}.", source.Id.ToString(), source.Name));
                    sources.Add(source);
                }
                _log.Trace(m => m("Totally {0} sources found!", sources.Count));
            }
        }

        #endregion

        #region Monitor & Control Service
        //
        // Monitor & Control Service
        //

        /// <summary>
        /// Private member to store the discovered monitor and control service hosts.
        /// </summary>
        private FindResponse discoveredMonitorControlServiceHosts = null;

        /// <summary>
        /// Private member to store the adress (URL) of the monitor and control service host.
        /// e.g. http://imfi-laptopdell:8080/MonitorAndControlService
        /// </summary>
        private EndpointAddress monitorControlServiceHostAddress = null;

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
                discoveredMonitorControlServiceHosts = discoveryClient.Find(criteria);
                discoveryClient.Close();

                _log.Trace(m => m("{0} <MonitorControl Service> found.", discoveredMonitorControlServiceHosts.Endpoints.Count));
                LogHelper.LogEndPoint(_log, discoveredMonitorControlServiceHosts.Endpoints);

                if (discoveredMonitorControlServiceHosts.Endpoints.Count > 0)
                {
                    // store service host address
                    monitorControlServiceHostAddress = discoveredMonitorControlServiceHosts.Endpoints[0].Address;
                }
            }
            catch (Exception exc)
            {
                _log.Trace(m => m("Exception: during discovering <MonitorControl Service>: {0}", exc));
                discoveredMonitorControlServiceHosts = null;
            }

        }

        /// <summary>
        /// Retruns the list of discovered monitor and control service hosts.
        /// </summary>
        public FindResponse DiscoveredMonitorControlServiceHosts
        {
            get { return discoveredMonitorControlServiceHosts; }
        }

        /// <summary>
        /// Retruns the number of discovered monitor and control service hosts. Returns -1 in case the search was not performed or an error occured.
        /// </summary>
        public int NumOfDiscoveredMonitorControlServiceHosts
        {
            get { return (((discoveredMonitorControlServiceHosts != null) && (discoveredMonitorControlServiceHosts.Endpoints.Count >= 0)) ? discoveredMonitorControlServiceHosts.Endpoints.Count : -1); ; }
        }

        /// <summary>
        /// Retruns the address (URL) of the monitor and control service host.
        /// This can either be a discovered address using the DiscoverMonitorControlServices method or an address set by the application using the set method.
        /// </summary>
        public EndpointAddress MonitorControlServiceHostAdress
        {
            get { return monitorControlServiceHostAddress; }
            set { monitorControlServiceHostAddress = value; }
        }
        #endregion

        #region Monitor & Control Service: Command Section
        /// <summary>
        /// Retruns a proxy object to the monitor and control service.
        /// </summary>
        /// <returns>Proxy Object to Monitor & Control service.</returns>
        private MonitorAndControlClient getMCProxy()
        {
            MonitorAndControlClient mcProxy = null;
            IMonitorAndControlCallback serverCallback = new ServerCallback();
            mcProxy = new MonitorAndControlClient(new InstanceContext(serverCallback));
            // Connect to the discovered service endpoint
            mcProxy.Endpoint.Address = monitorControlServiceHostAddress;
            return mcProxy;
        }

        /// <summary>
        /// Retruns Source configuration object for a specific Source Id.
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

        /// <summary>
        /// Retruns Source configuration object for a spefific source name.
        /// </summary>
        /// <param name="sourceName">Source Name</param>
        /// <returns>Source object. Returns an empty object in case the Source Name was not found.</returns>
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

        /// <summary>
        /// Returns Zone configuration Object for a specific Zone name.
        /// </summary>
        /// <param name="zoneId">Zone Name</param>
        /// <returns>Zone object. Returns an empty object in case the Zone Name was not found.</returns>
        public Zone GetZone(string zoneName)
        {
            Zone foundZone = new Zone();
            foreach (Zone zone in zones)
            {
                if (zone.Name == zoneName)
                {
                    foundZone = zone;
                    break;
                }
            }
            _log.Trace(m => m("Zone found!", foundZone.ToString()));
            return foundZone;
        }

        /// <summary>
        /// Returns the current zone state for a zone, specified by Zone Id.
        /// </summary>
        /// <param name="zoneId">Zone Id</param>
        /// <returns>Zone State.</returns>
        public ZoneState GetZoneState(Address zoneId)
        {
            MonitorAndControlClient mcProxy = getMCProxy();
            mcProxy.Connect();

            ZoneState zoneState = mcProxy.GetZoneState(zoneId);
            _log.Trace(m => m("Read zone configuration for zone with id {0}. Zone State = [{1}]", zoneId.ToString(), zoneState.ToString()));

            mcProxy.Disconnect();
            return zoneState;
        }

        /// <summary>
        /// Switches a zone, specified by its zone id, either on or off (depending on its current state).
        /// </summary>
        /// <param name="zoneId">Zone Id</param>
        /// <returns>Zone state, after the switch command has been performed.</returns>
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

        /// <summary>
        /// Changes the source of a specified zone.
        /// </summary>
        /// <param name="zoneId">Zone id, to specify zone</param>
        /// <param name="sourceId">Source id, tp specify source</param>
        /// <returns>Zone state, after the command has been performed.</returns>
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

        /// <summary>
        /// Increases the volume of a specified zone.
        /// </summary>
        /// <param name="zoneId">Zone id</param>
        /// <returns>Zone state, after the command has been performed.</returns>
        public ZoneState VolumeUp(Address zoneId)
        {
            return AdjustVolume(zoneId, +3);
        }

        /// <summary>
        /// Decrease the volume of a specified zone.
        /// </summary>
        /// <param name="zoneId">Zone id</param>
        /// <returns>Zone state, after the command has been performed.</returns>
        public ZoneState VolumeDown(Address zoneId)
        {
            return AdjustVolume(zoneId, -3);
        }

        /// <summary>
        /// Change the volume of a specified zone.
        /// </summary>
        /// <param name="zoneId">Zone id</param>
        /// <param name="adjVolume">Delta Volume</param>
        /// <returns>Zone state, after the command has been performed.</returns>
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
        #endregion


    }
}