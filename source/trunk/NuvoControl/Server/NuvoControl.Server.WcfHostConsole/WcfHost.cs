/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.MonitorAndControlService
 *   Author:         Bernhard Limacher
 *   Creation Date:  21.05.2009
 *   File Name:      WcfHost.cs
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 21.05.2009, Bernhard Limacher: Initial implementation.
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.ServiceModel.Description;      // WCF PingTest

using Common.Logging;

using NuvoControl.Common;
using NuvoControl.Common.Configuration;
using NuvoControl.Server.MonitorAndControlService;
using NuvoControl.Server.ConfigurationService;
using NuvoControl.Server.ProtocolDriver.Interface;
using NuvoControl.Server.ZoneServer;
using NuvoControl.Server.FunctionService;
using NuvoControl.Server.FunctionServer;

namespace NuvoControl.Server.WcfHostConsole
{
    class WcfHost
    {
        #region Fields

        private static ILog _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Holds the loaded system configuration.
        /// </summary>
        private static NuvoControl.Server.ConfigurationService.ConfigurationService _configurationService = null;

        /// <summary>
        /// Holds the protocol drivers per NuvoEssentia.
        /// </summary>
        private static Dictionary<int, IProtocol> _protocolDrivers = new Dictionary<int, IProtocol>();

        /// <summary>
        /// Holds a reference to the zone server.
        /// </summary>
        private static IZoneServer _zoneServer = null;

        /// <summary>
        /// Holds a reference to the function server.
        /// </summary>
        private static FunctionServer.FunctionServer _functionServer = null;

        /// <summary>
        /// Private member to hold the timer used to periodically check for configuration changes
        /// </summary>
        private static System.Timers.Timer _timerCheckConfiguration = new System.Timers.Timer();


        private static ServiceHost _configurationServiceHost = null;

        private static ServiceHostMc _mCServiceHost = null;

        private static ServiceHostFunction _functionServiceHost = null;

        private static int pingcount = 0;

        #endregion

        #region Non-Public Interface

        /// <summary>
        /// Main function
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine(">>> Starting WCF services V2  --- Assembly Version={0} / Deployment Version={1} (using .NET 4.0) ... ",
                AppInfoHelper.getAssemblyVersion(), AppInfoHelper.getDeploymentVersion());
            Console.WriteLine();

            LoadAllServices();
            HostAllServices();

        }
        private static void LoadAllServices()
        {
            try
            {
                //LoadConfigurationService(Properties.Settings.Default.NuvoControlKonfigurationFile);
                LoadConfigurationService(Properties.Settings.Default.NuvoControlKonfigurationFile, Properties.Settings.Default.NuvoControlRemoteKonfigurationFile);
            }
            catch (Exception exc)
            {
                _log.ErrorFormat("Failed to load the system configuration.", exc);
                Console.WriteLine("Failed to load the system configuration. Exception message: {0}", exc.Message);
                return;
            }

            try
            {
                LoadProtocolDrivers();
            }
            catch (Exception exc)
            {
                _log.ErrorFormat("Failed to load the protocol drivers.", exc);
                Console.WriteLine("Failed to load the protocol drivers. Exception message: {0}", exc.Message);
            }

            try
            {
                InstantiateZoneServer();
            }
            catch (Exception exc)
            {
                _log.ErrorFormat("Failed to create the zone server.", exc);
                Console.WriteLine("Failed to create the zone server. Exception message: {0}", exc.Message);
            }

            try
            {
                InstantiateFunctionServer();
            }
            catch (Exception exc)
            {
                _log.ErrorFormat("Failed to create the function server.", exc);
                Console.WriteLine("Failed to create the function server. Exception message: {0}", exc.Message);
            }

            Console.WriteLine();
        }


        private static void UnloadAllServices()
        {
            try
            {
                UnloadFunctionServer();
            }
            catch (Exception exc)
            {
                _log.ErrorFormat("Failed to unload the function server.", exc);
                Console.WriteLine("Failed to unload the function server. Exception message: {0}", exc.Message);
            }

            try
            {
                UnloadZoneServer();
            }
            catch (Exception exc)
            {
                _log.ErrorFormat("Failed to unload the zone server.", exc);
                Console.WriteLine("Failed to unload the zone server. Exception message: {0}", exc.Message);
            }

            try
            {
                UnloadProtocolDrivers();
            }
            catch (Exception exc)
            {
                _log.ErrorFormat("Failed to unload the protocol drivers.", exc);
                Console.WriteLine("Failed to unload the protocol drivers. Exception message: {0}", exc.Message);
            }

            try
            {
                UnloadConfigurationService();
            }
            catch (Exception exc)
            {
                _log.ErrorFormat("Failed to load the system configuration.", exc);
                Console.WriteLine("Failed to load the system configuration. Exception message: {0}", exc.Message);
                return;
            }

            Console.WriteLine();
        }


        private static void HostAllServices()
        {
            /*
            // URI used to connect within local LAN networks
            ServiceHost configurationServiceHost = new ServiceHost(_configurationService,
                new Uri(NetworkHelper.buildEndpointAddress("http://localhost:8080/ConfigurationService")));
            ServiceHostMc mCServiceHost = new ServiceHostMc(
                typeof(NuvoControl.Server.MonitorAndControlService.MonitorAndControlService), _zoneServer, 
                new Uri(NetworkHelper.buildEndpointAddress("http://localhost:8080/MonitorAndControlService")));
            ServiceHostFunction functionServiceHost = new ServiceHostFunction(
                typeof(NuvoControl.Server.FunctionService.FunctionService), _zoneServer, _configurationService.SystemConfiguration.Functions, 
                new Uri(NetworkHelper.buildEndpointAddress("http://localhost:8080/FunctionService")));
            */

            // URI used to connect via internet
            // NOTE: This requires that dyndns service is running and up-to-date. The imfeldc.dyndns.org address will point to the access point, which itself is configured to forward
            // and request to the virtual machine imfihpavm.
            ServiceHost configurationServiceHost = new ServiceHost(_configurationService,
                new Uri(NetworkHelper.buildEndpointAddress("http://imfeldc.dyndns.org:8080/ConfigurationService")));
            ServiceHostMc mCServiceHost = new ServiceHostMc(
                typeof(NuvoControl.Server.MonitorAndControlService.MonitorAndControlService), _zoneServer,
                new Uri(NetworkHelper.buildEndpointAddress("http://imfeldc.dyndns.org:8080/MonitorAndControlService")));
            ServiceHostFunction functionServiceHost = new ServiceHostFunction(
                typeof(NuvoControl.Server.FunctionService.FunctionService), _zoneServer, _configurationService.SystemConfiguration.Functions,
                new Uri(NetworkHelper.buildEndpointAddress("http://imfeldc.dyndns.org:8080/FunctionService")));

            try
            {
                // make the service discoverable by adding the discovery behavior
                configurationServiceHost.Description.Behaviors.Add(new ServiceDiscoveryBehavior());
                // add the discovery endpoint that specifies where to publish the services
                configurationServiceHost.AddServiceEndpoint(new UdpDiscoveryEndpoint());
                Console.WriteLine(">>> Discovery for Configuration service started ....");
                configurationServiceHost.Open();
                Console.WriteLine(">>> Configuration service is running.");
                Console.WriteLine(">>> URI: {0}", configurationServiceHost.BaseAddresses[0].AbsoluteUri);
                Console.WriteLine();

                // make the service discoverable by adding the discovery behavior
                mCServiceHost.Description.Behaviors.Add(new ServiceDiscoveryBehavior());
                // add the discovery endpoint that specifies where to publish the services
                mCServiceHost.AddServiceEndpoint(new UdpDiscoveryEndpoint());
                Console.WriteLine(">>> Discovery for Monitor and control service started ....");
                mCServiceHost.Open();
                Console.WriteLine(">>> Monitor and control service is running.");
                Console.WriteLine(">>> URI: {0}", mCServiceHost.BaseAddresses[0].AbsoluteUri);
                Console.WriteLine();

                // make the service discoverable by adding the discovery behavior
                functionServiceHost.Description.Behaviors.Add(new ServiceDiscoveryBehavior());
                // add the discovery endpoint that specifies where to publish the services
                functionServiceHost.AddServiceEndpoint(new UdpDiscoveryEndpoint());
                Console.WriteLine(">>> Discovery for Function service started ....");
                functionServiceHost.Open();
                Console.WriteLine(">>> Function service is running.");
                Console.WriteLine(">>> URI: {0}", functionServiceHost.BaseAddresses[0].AbsoluteUri);
                Console.WriteLine();

                Console.WriteLine(">>> Press <Enter> to stop the services.");
                Console.ReadLine();
            }
            catch (Exception exc)
            {
                _log.ErrorFormat("Failed to start services.", exc);
                Console.WriteLine("Failed to start services. Exception message: {0}", exc.Message);
                Console.WriteLine(">>> Press <Enter> to close the console.");
                Console.ReadLine();
            }
            finally
            {
                configurationServiceHost.Close();
                mCServiceHost.Close();
            }   
        }


        private static void DisposeAllService()
        {
            if (_functionServiceHost != null)
            {
                _functionServiceHost.Close();
                Console.WriteLine(">>> Discovery for Function service stopped ....");
                _functionServiceHost = null;
            }

            if (_mCServiceHost != null)
            {
                _mCServiceHost.Close();
                Console.WriteLine(">>> Discovery for Monitor and control service stopped ....");
                _mCServiceHost = null;
            }

            if (_configurationServiceHost != null)
            {
                _configurationServiceHost.Close();
                Console.WriteLine(">>> Discovery for Configuration service stopped ....");
                _configurationServiceHost = null;
            }
        }

        /// <summary>
        /// Reads the XML configuration file and instantiates accordingly the system configuration objects of NuvoControl.
        /// </summary>
        /// <param name="configurationFile">Filename of configuration file</param>
        private static void LoadConfigurationService(string configurationFile)
        {
            _log.Info(m=>m("Loading the nuvo control configuration from '{0}' ...", configurationFile));
            Console.WriteLine(">>> Loading configuration...");
            Console.WriteLine(">>>   from {0}", configurationFile);

            _configurationService = new NuvoControl.Server.ConfigurationService.ConfigurationService(configurationFile);
        }

        /// <summary>
        /// Reads the XML configuration file and instantiates accordingly the system configuration objects of NuvoControl.
        /// </summary>
        /// <param name="configurationFile">Filename of configuration file</param>
        /// <param name="remoteConfigurationFile">Filename of configuration file to append</param>
        private static void LoadConfigurationService(string configurationFile, string remoteConfigurationFile)
        {
            _log.Info(m => m("Loading the nuvo control configuration from '{0}' and '{1}' ...", configurationFile, remoteConfigurationFile));
            Console.WriteLine(">>> Loading configuration...");
            Console.WriteLine(">>>   from {0}", configurationFile);
            Console.WriteLine(">>>   and append {0}", remoteConfigurationFile);

            _configurationService = new NuvoControl.Server.ConfigurationService.ConfigurationService(configurationFile, remoteConfigurationFile);
        }

        /// <summary>
        /// Unloads the system configuration objects of NuvoControl.
        /// </summary>
        /// <param name="configurationFile">Filename of configuration file</param>
        private static void UnloadConfigurationService()
        {
            _log.Info(m => m("Unload the nuvo control configuration ..."));
            Console.WriteLine(">>> Unload the nuvo control configuration ...");

            _configurationService.Dispose();
            _configurationService = null;
        }


        /// <summary>
        /// Loads the protocol drivers according to the NuvoControl system configuration.
        /// </summary>
        private static void LoadProtocolDrivers()
        {
            _log.Info("Loading protocol drivers...");
            Console.WriteLine(">>> Loading protocol drivers...");

            foreach (Device device in _configurationService.SystemConfiguration.Hardware.Devices)
            {
                IProtocol driver = ProtocolDriverFactory.LoadDriver(device.ProtocolDriver.AssemblyName, device.ProtocolDriver.ClassName);
                if (driver != null)
                {
                    driver.Open(ENuvoSystem.NuVoEssentia, device.Id, device.Communication);
                    _protocolDrivers[device.Id] = driver;
                    Console.WriteLine(">>>   driver {0} loaded ...", device.Id);
                }
            }
        }

        /// <summary>
        /// Unload the protocol drivers.
        /// </summary>
        private static void UnloadProtocolDrivers()
        {
            _log.Info("Unload protocol drivers...");
            Console.WriteLine(">>> Unload protocol drivers...");

            foreach ( int deviceId in _protocolDrivers.Keys )
            {
                _protocolDrivers[deviceId].Close(deviceId);
                //_protocolDrivers[deviceId] = null;
                Console.WriteLine(">>>   driver {0} unloaded ...", deviceId);
            }
            _protocolDrivers.Clear();
        }

        
        /// <summary>
        /// Instantiates the zone server. This object holds all zone controllers.
        /// </summary>
        private static void InstantiateZoneServer()
        {
            _log.Info("Instantiating the zone server...");
            Console.WriteLine(">>> Instantiating the zone server...");

            List<IZoneController> zoneControllers = new List<IZoneController>();
            foreach (Device device in _configurationService.SystemConfiguration.Hardware.Devices)
            {
                foreach (int zoneId in device.Zones)
                {
                    zoneControllers.Add(new ZoneController(new Address(device.Id, zoneId), _protocolDrivers[device.Id]));
                }
            }
            _zoneServer = new NuvoControl.Server.ZoneServer.ZoneServer(zoneControllers);
            _zoneServer.StartUp();
        }

        /// <summary>
        /// Unload the zone server. This object holds all zone controllers.
        /// </summary>
        private static void UnloadZoneServer()
        {
            _log.Info("Unload the zone server...");
            Console.WriteLine(">>> Unload the zone server...");

            _zoneServer.ShutDown();
            _zoneServer = null;
        }


        /// <summary>
        /// Instantiates the function server. This object holds all functions.
        /// </summary>
        private static void InstantiateFunctionServer()
        {
            _log.Info("Instantiating the function server...");
            Console.WriteLine(">>> Instantiating the function server...");

            _functionServer = new NuvoControl.Server.FunctionServer.FunctionServer(_zoneServer, _configurationService.SystemConfiguration.Functions);
            //_functionServer.StartUp();
        }

        /// <summary>
        /// Unload the function server. This object holds all functions.
        /// </summary>
        private static void UnloadFunctionServer()
        {
            _log.Info("Unload the function server...");
            Console.WriteLine(">>> Unload the function server...");

            _functionServer = null;
        }

        #endregion

        #region WCF Ping Test

        /// <summary>
        /// Ping Test method, called by client to "ping"
        /// </summary>
        private class WcfPingTest : IWcfPingTest
        {
            public string Ping() 
            {
                pingcount++;
                string message = String.Format("PingTest called at {1} for {2} times", DateTime.Today.ToString(), pingcount.ToString());
                Console.WriteLine("\n### Ping received! {0}", message);
                return message; 
            }
        }

        /// <summary>
        /// Open ping service.
        /// </summary>
        private static void WcfTestHost_Open()
        {
            string hostname = System.Environment.MachineName;
            var baseAddress = new UriBuilder("http", hostname, 7400, "WcfPing");
            var h = new ServiceHost(typeof(WcfPingTest), baseAddress.Uri);

            // enable processing of discovery messages.  use UdpDiscoveryEndpoint to enable listening. use EndpointDiscoveryBehavior for fine control.
            h.Description.Behaviors.Add(new ServiceDiscoveryBehavior());
            h.AddServiceEndpoint(new UdpDiscoveryEndpoint());

            // enable wsdl, so you can use the service from WcfStorm, or other tools.
            var smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
            h.Description.Behaviors.Add(smb);

            // create endpoint
            var binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            h.AddServiceEndpoint(typeof(IWcfPingTest), binding, "");
            Console.WriteLine(">>> Discovery for Ping service started ....");
            h.Open();
            Console.WriteLine(">>> Ping service is running.");
            Console.WriteLine(">>> URI: {0}", h.BaseAddresses[0].AbsoluteUri);
            Console.WriteLine();
        }

        #endregion

    }
}
