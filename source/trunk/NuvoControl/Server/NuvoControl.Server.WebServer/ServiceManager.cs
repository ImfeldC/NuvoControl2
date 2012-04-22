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
    /// This class is used as server callback interface.
    /// </summary>
    public class ServcieManagerCallback : IMonitorAndControlCallback
    {
        private static ILog _log = LogManager.GetCurrentClassLogger();

        #region IMonitorAndControlCallback Members

        public void OnZoneStateChanged(Address zoneId, ZoneState zoneState)
        {
            _log.Trace(m => m("Notification from server zone with id={0} has changed. New state is {1}", zoneId.ToString(), zoneState.ToString()));
        }

        #endregion
    }


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
            _zones = new List<Zone>();
            _sources = new List<Source>();
            _functions = new List<Function>();
        }


        #region Zones & Sources
        //
        // Zones & Sources
        //

        /// <summary>
        /// Private member to store the availables zone configuration.
        /// </summary>
        private List<Zone> _zones = null;

        /// <summary>
        /// Private member to store the available source configuration.
        /// </summary>
        private List<Source> _sources = null;

        /// <summary>
        /// Public access method to retrieve configured zones.
        /// </summary>
        public List<Zone> Zones
        {
            get { return _zones; }
        }

        /// <summary>
        /// Public access method to retrieve configured sources.
        /// </summary>
        public List<Source> Sources
        {
            get { return _sources; }
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
        /// Private member to store all functions.
        /// </summary>
        private List<Function> _functions = null;

        /// <summary>
        /// Returns list of all functions.
        /// </summary>
        public List<Function> Functions
        {
            get { return _functions; }
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
                _zones.Clear();
                _functions.Clear();
                foreach (Floor floor in _graphic.Building.Floors)
                {
                    _log.Trace(m => m("Read FLOOR with id={0}.", floor.Id));
                    foreach (Zone zone in floor.Zones)
                    {
                        _log.Trace(m => m("Zone found with id {0} with name {1}.", zone.Id.ToString(), zone.Name));
                        _zones.Add(zone);

                        // ToDo: Not implemented yet! Implement GetFunctions(zone.Id) on server side!
                        //Function[] functions = cfgIfc.GetFunctions(zone.Id);
                        //_functions.AddRange(functions);
                    }
                }
                _log.Trace(m => m("Totally {0} zones found!", _zones.Count));
                _log.Trace(m => m("Totally {0} funtions found!", _functions.Count));


                // read available sources (via graphic configuration)
                // Root -> Graphic -> Source(s)
                _sources.Clear();
                foreach (Source source in _graphic.Sources)
                {
                    _log.Trace(m => m("SOURCE found with id {0}, with the name {1}.", source.Id.ToString(), source.Name));
                    _sources.Add(source);
                }
                _log.Trace(m => m("Totally {0} sources found!", _sources.Count));
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
            IMonitorAndControlCallback serverCallback = new ServcieManagerCallback();
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
            foreach (Source source in _sources)
            {
                if (source.Id == sourceId)
                {
                    return source;
                 }
            }
            _log.Warn(m => m("Source NOT found, with id {0}! _sources=[{1}]", sourceId.ToString(), _sources.ToString<Source>(" / ")));
            return new Source();
        }

        /// <summary>
        /// Retruns Source configuration object for a spefific source name.
        /// </summary>
        /// <param name="sourceName">Source Name</param>
        /// <returns>Source object. Returns an empty object in case the Source Name was not found.</returns>
        public Source GetSource(string sourceName)
        {
            foreach (Source source in _sources)
            {
                if (source.Name == sourceName)
                {
                    return source;
                }
            }
            _log.Warn(m => m("Source NOT found, with name {0}! _sources=[{1}]", sourceName, _sources.ToString<Source>(" / ")));
            return new Source();
        }

        /// <summary>
        /// Returns Zone configuration Object for a specific Zone Id.
        /// </summary>
        /// <param name="zoneId">Zone Id</param>
        /// <returns>Zone object. Returns an empty object in case the Zone Id was not found.</returns>
        public Zone GetZone(Address zoneId)
        {
            foreach (Zone zone in _zones)
            {
                if (zone.Id == zoneId)
                {
                    return zone;
                }
            }
            _log.Warn(m => m("Zone NOT found, with id {0}! _zones=[{1}]", zoneId.ToString(), _zones.ToString<Zone>(" / ")));
            return new Zone();
        }

        /// <summary>
        /// Returns Zone configuration Object for a specific Zone name.
        /// </summary>
        /// <param name="zoneId">Zone Name</param>
        /// <returns>Zone object. Returns an empty object in case the Zone Name was not found.</returns>
        public Zone GetZone(string zoneName)
        {
            foreach (Zone zone in _zones)
            {
                if (zone.Name == zoneName)
                {
                    return zone;
                }
            }
            _log.Warn(m => m("Zone NOT found, with name {0}! _zones=[{1}]", zoneName, _zones.ToString<Zone>(" / ")));
            return new Zone();
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
            _log.Trace(m => m("Read zone status for zone with id {0}. Zone State = [{1}]", zoneId.ToString(), zoneState.ToString()));

            mcProxy.Disconnect();

            // Tweak: If a source is not configured, it needs to be added programmatically to the list, because it is still possible to select this source on the keypad.
            // If such an unconfigured source has been selected an error occurs, becuase the source is unkown!
            // ToDo: Implement this tweak on server side
            Source source = GetSource(zoneState.Source);
            if( source.isEmpty() )
            {
                _sources.Add(new Source(zoneState.Source, String.Format("[{0}]",zoneState.Source), "n/a", "n/a"));
                _log.Trace(m => m("SOURCE added with id {0}.", zoneState.Source));
            }
            return zoneState;
        }

        /// <summary>
        /// Returns the zone state for all zones.
        /// </summary>
        /// <returns>List of Zone State for all zones.</returns>
        public List<ZoneState> GetAllZoneStates()
        {
            List<ZoneState> zoneStates = new List<ZoneState>();
            foreach (Zone zone in _zones)
            {
                zoneStates.Add(GetZoneState(zone.Id));
            }
            _log.Trace(m => m("Zone state for all {0} zones loaded.", zoneStates.Count ));
            return zoneStates;
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