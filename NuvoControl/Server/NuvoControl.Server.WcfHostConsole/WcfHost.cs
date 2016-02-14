/**************************************************************************************************
 * 
 *   Copyright (C) 2009 by B. Limacher, Ch. Imfeld. All Rights Reserved.
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

        private static Options _options = null;

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
            // Load command line argumnets
            _options = new Options();
            if (args != null && args.Length > 0)
            {
                CommandLine.Parser.Default.ParseArguments(args, _options);
            }
            else
            {
                _options.verbose = Properties.Settings.Default.Verbose;
                _options.minVerboseLevel = (LogLevel)Properties.Settings.Default.MinVerboseLevel;
                _options.portName = Properties.Settings.Default.Portname;
                _options.baudRate = Properties.Settings.Default.Baudrate;
                _options.readTimeout = Properties.Settings.Default.ReadTimeout;
            }
            LogHelper.SetOptions(_options);
            LogHelper.LogAppStart("WCF services V2");
            LogHelper.LogArgs(args);
            if (_options.Help)
            {
                Console.WriteLine(_options.GetUsage());
            }

            WcfTestHost_Open();

            LogHelper.Log(LogLevel.Debug, String.Format("Check configuration timer started, each {0}[s]", Properties.Settings.Default.ConfigurationCheckIntervall));
            _timerCheckConfiguration.Interval = (Properties.Settings.Default.ConfigurationCheckIntervall < 30 ? 30 : Properties.Settings.Default.ConfigurationCheckIntervall) * 1000;
            _timerCheckConfiguration.Elapsed += new System.Timers.ElapsedEventHandler(_timerCheckConfiguration_Elapsed);
            _timerCheckConfiguration.Start();

            LoadAllServices();
            HostAllServices();

            LogHelper.Log(LogLevel.All, ">>> Press <Enter> to stop the services.");
            Console.ReadLine();

            DisposeAllService();
            UnloadAllServices();
        }

        /// <summary>
        /// Periodic timer routine to check if configuration file changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void _timerCheckConfiguration_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //Console.Write(".");
            bool bChanged = _configurationService.CheckConfiguration();
            if (bChanged)
            {
                DisposeAllService();
                UnloadAllServices();

                LoadAllServices();
                HostAllServices();
            }
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
                LogHelper.LogException("Failed to load the system configuration.", exc);
                return;
            }

            try
            {
                LoadProtocolDrivers();
            }
            catch (Exception exc)
            {
                LogHelper.LogException("Failed to load the protocol drivers.", exc);
            }

            try
            {
                InstantiateZoneServer();
            }
            catch (Exception exc)
            {
                LogHelper.LogException("Failed to create the zone server.", exc);
            }

            try
            {
                InstantiateFunctionServer();
            }
            catch (Exception exc)
            {
                LogHelper.LogException("Failed to create the function server.", exc);
            }

        }


        private static void UnloadAllServices()
        {
            try
            {
                UnloadFunctionServer();
            }
            catch (Exception exc)
            {
                LogHelper.LogException("Failed to unload the function server.", exc);
            }

            try
            {
                UnloadZoneServer();
            }
            catch (Exception exc)
            {
                LogHelper.LogException("Failed to unload the zone server.", exc);
            }

            try
            {
                UnloadProtocolDrivers();
            }
            catch (Exception exc)
            {
                LogHelper.LogException("Failed to unload the protocol drivers.", exc);
            }

            try
            {
                UnloadConfigurationService();
            }
            catch (Exception exc)
            {
                LogHelper.LogException("Failed to load the system configuration.", exc);
                return;
            }

        }

        /// <summary>
        /// To set permission, call with admin rights:
        ///    netsh http add urlacl url=http://+:7400/ConfigurationService user=WW002\imfeldc
        ///    netsh http add urlacl url=http://+:7400/FunctionService user=WW002\imfeldc
        ///    netsh http add urlacl url=http://+:7400/MonitorAndControlService user=WW002\imfeldc
        /// More details see https://msdn.microsoft.com/library/ms733768.aspx
        /// </summary>
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

            /*
            // URI used to connect via internet
            // NOTE: This requires that dyndns service is running and up-to-date. The imfeldc.dyndns.org address will point to the access point, which itself is configured to forward
            // and request to the virtual machine imfihpavm.
            _configurationServiceHost = new ServiceHost(_configurationService,
                new Uri(NetworkHelper.buildEndpointAddress("http://imfeldc.dyndns.org:8080/ConfigurationService")));
            _mCServiceHost = new ServiceHostMc(
                typeof(NuvoControl.Server.MonitorAndControlService.MonitorAndControlService), _zoneServer,
                new Uri(NetworkHelper.buildEndpointAddress("http://imfeldc.dyndns.org:8080/MonitorAndControlService")));
            _functionServiceHost = new ServiceHostFunction(
                typeof(NuvoControl.Server.FunctionService.FunctionService), _zoneServer, _configurationService.SystemConfiguration.Functions,
                new Uri(NetworkHelper.buildEndpointAddress("http://imfeldc.dyndns.org:8080/FunctionService")));
            */

            
            string hostname = System.Environment.MachineName;
            int portnumber = 7400;
            var baseAddress = new UriBuilder("http", hostname, portnumber, "ConfigurationService");
            _configurationServiceHost = new ServiceHost(
                /*typeof(NuvoControl.Server.ConfigurationService.ConfigurationService)*/ _configurationService, new UriBuilder("http", hostname, portnumber, "ConfigurationService").Uri);
            _mCServiceHost = new ServiceHostMc(
                typeof(NuvoControl.Server.MonitorAndControlService.MonitorAndControlService), _zoneServer,
                new UriBuilder("http", hostname, portnumber, "MonitorAndControlService").Uri);
            _functionServiceHost = new ServiceHostFunction(
                typeof(NuvoControl.Server.FunctionService.FunctionService), _zoneServer, _configurationService.SystemConfiguration.Functions,
                new UriBuilder("http", hostname, portnumber, "FunctionService").Uri);
            

            try
            {
                // make the service discoverable by adding the discovery behavior
                _configurationServiceHost.Description.Behaviors.Add(new ServiceDiscoveryBehavior());
                // add the discovery endpoint that specifies where to publish the services
                _configurationServiceHost.AddServiceEndpoint(new UdpDiscoveryEndpoint());
                LogHelper.Log(LogLevel.All, ">>> Discovery for Configuration service started ....");
                try
                {
                    _configurationServiceHost.Open();
                    LogHelper.Log(LogLevel.Info, ">>> Configuration service is running.");
                    LogHelper.Log(LogLevel.Debug, String.Format(">>> URI: {0}", _configurationServiceHost.BaseAddresses[0].AbsoluteUri));
                }
                catch (System.ServiceModel.AddressAccessDeniedException exc)
                {
                    // see comment above, to set correct required permissions
                    LogHelper.LogException(">>> Cannot open Discovery for Configuration service!", exc);
                }

                // make the service discoverable by adding the discovery behavior
                _mCServiceHost.Description.Behaviors.Add(new ServiceDiscoveryBehavior());
                // add the discovery endpoint that specifies where to publish the services
                _mCServiceHost.AddServiceEndpoint(new UdpDiscoveryEndpoint());
                LogHelper.Log(LogLevel.All, ">>> Discovery for Monitor and control service started ....");
                try
                {
                    _mCServiceHost.Open();
                    LogHelper.Log(LogLevel.Info, ">>> Monitor and control service is running.");
                    LogHelper.Log(LogLevel.Debug, String.Format(">>> URI: {0}", _mCServiceHost.BaseAddresses[0].AbsoluteUri));
                }
                catch (System.ServiceModel.AddressAccessDeniedException exc)
                {
                    // see comment above, to set correct required permissions
                    LogHelper.LogException(">>> Cannot open Discovery for Monitor and control service!", exc);
                }

                // make the service discoverable by adding the discovery behavior
                _functionServiceHost.Description.Behaviors.Add(new ServiceDiscoveryBehavior());
                // add the discovery endpoint that specifies where to publish the services
                _functionServiceHost.AddServiceEndpoint(new UdpDiscoveryEndpoint());
                LogHelper.Log(LogLevel.All, ">>> Discovery for Function service started ....");
                try
                {
                    _functionServiceHost.Open();
                    LogHelper.Log(LogLevel.Info, ">>> Function service is running.");
                    LogHelper.Log(LogLevel.Debug, String.Format(">>> URI: {0}", _functionServiceHost.BaseAddresses[0].AbsoluteUri));
                }
                catch (System.ServiceModel.AddressAccessDeniedException exc)
                {
                    // see comment above, to set correct required permissions
                    LogHelper.LogException(">>> Cannot open Discovery for Function service!", exc);
                }

            }
            catch (Exception exc)
            {
                LogHelper.LogException("Failed to start services.", exc);
                LogHelper.Log(LogLevel.All, ">>> Press <Enter> to close the console.");
                Console.ReadLine();
            }
        }


        private static void DisposeAllService()
        {
            if (_functionServiceHost != null)
            {
                try
                {
                    _functionServiceHost.Close();
                    LogHelper.Log(LogLevel.Info, ">>> Discovery for Function service stopped ....");
                    _functionServiceHost = null;
                }
                catch (System.ServiceModel.CommunicationObjectFaultedException exc)
                {
                    LogHelper.LogException(">>> Cannot proper close Discovery for Function service!", exc);
                }
            }

            if (_mCServiceHost != null)
            {
                try
                {
                    _mCServiceHost.Close();
                    LogHelper.Log(LogLevel.Info, ">>> Discovery for Monitor and control service stopped ....");
                    _mCServiceHost = null;
                }
                catch (System.ServiceModel.CommunicationObjectFaultedException exc)
                {
                    LogHelper.LogException(">>> Cannot proper close Discovery for Monitor and control service!", exc);
                }
            }

            if (_configurationServiceHost != null)
            {
                try
                {
                    _configurationServiceHost.Close();
                    LogHelper.Log(LogLevel.Info, ">>> Discovery for Configuration service stopped ....");
                    _configurationServiceHost = null;
                }
                catch (System.ServiceModel.CommunicationObjectFaultedException exc)
                {
                    LogHelper.LogException(">>> Cannot proper close Discovery for Configuration service!", exc);
                }
            }
        }

        /// <summary>
        /// Reads the XML configuration file and instantiates accordingly the system configuration objects of NuvoControl.
        /// </summary>
        /// <param name="configurationFile">Filename of configuration file</param>
        private static void LoadConfigurationService(string configurationFile)
        {
            LogHelper.Log(LogLevel.Info, String.Format(">>> Loading the nuvo control configuration from '{0}' ...", configurationFile));

            _configurationService = new NuvoControl.Server.ConfigurationService.ConfigurationService(configurationFile);
        }

        /// <summary>
        /// Reads the XML configuration file and instantiates accordingly the system configuration objects of NuvoControl.
        /// </summary>
        /// <param name="configurationFile">Filename of configuration file</param>
        /// <param name="remoteConfigurationFile">Filename of configuration file to append</param>
        private static void LoadConfigurationService(string configurationFile, string remoteConfigurationFile)
        {
            LogHelper.Log(LogLevel.Info, String.Format(">>> Loading the nuvo control configuration from '{0}' and '{1}' ...", configurationFile, remoteConfigurationFile));

            _configurationService = new NuvoControl.Server.ConfigurationService.ConfigurationService(configurationFile, remoteConfigurationFile);
        }

        /// <summary>
        /// Unloads the system configuration objects of NuvoControl.
        /// </summary>
        private static void UnloadConfigurationService()
        {
            LogHelper.Log(LogLevel.Info, String.Format(">>> Unload the nuvo control configuration ..."));

            _configurationService.Dispose();
            _configurationService = null;
        }


        /// <summary>
        /// Loads the protocol drivers according to the NuvoControl system configuration.
        /// </summary>
        private static void LoadProtocolDrivers()
        {
            LogHelper.Log(LogLevel.Info, String.Format("Loading protocol drivers..."));

            foreach (Device device in _configurationService.SystemConfiguration.Hardware.Devices)
            {
                IProtocol driver = ProtocolDriverFactory.LoadDriver(device.ProtocolDriver.AssemblyName, device.ProtocolDriver.ClassName);
                if (driver != null)
                {
                    AdjustDeviceSettings(device);
                    driver.Open(ENuvoSystem.NuVoEssentia, device.Id, device.Communication);
                    _protocolDrivers[device.Id] = driver;
                    LogHelper.Log(LogLevel.Info, String.Format(">>>   driver {0} loaded ... Communication Parameter=[{1}]", device.Id, device.Communication.ToString()));

                    // Subscribe for events
                    driver.onCommandReceived += new ProtocolCommandReceivedEventHandler(_driver_onCommandReceived);
                    driver.onDeviceStatusUpdate += new ProtocolDeviceUpdatedEventHandler(_driver_onDeviceStatusUpdate);
                    driver.onZoneStatusUpdate += new ProtocolZoneUpdatedEventHandler(_driver_onZoneStatusUpdate);
                }
            }
        }

        /// <summary>
        /// Unload the protocol drivers.
        /// </summary>
        private static void UnloadProtocolDrivers()
        {
            LogHelper.Log(LogLevel.Info, String.Format(">>> Unload protocol drivers..."));

            foreach ( int deviceId in _protocolDrivers.Keys )
            {
                _protocolDrivers[deviceId].Close(deviceId);
                //_protocolDrivers[deviceId] = null;
                LogHelper.Log(LogLevel.Info, String.Format(">>>   driver {0} unloaded ...", deviceId));
            }
            _protocolDrivers.Clear();
        }

        
        /// <summary>
        /// Instantiates the zone server. This object holds all zone controllers.
        /// </summary>
        private static void InstantiateZoneServer()
        {
            LogHelper.Log(LogLevel.Info, String.Format(">>> Instantiating the zone server..."));

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
            LogHelper.Log(LogLevel.Info, String.Format(">>> Unload the zone server..."));

            _zoneServer.ShutDown();
            _zoneServer = null;
        }


        /// <summary>
        /// Instantiates the function server. This object holds all functions.
        /// </summary>
        private static void InstantiateFunctionServer()
        {
            LogHelper.Log(LogLevel.Info, String.Format(">>> Instantiating the function server..."));

            _functionServer = new NuvoControl.Server.FunctionServer.FunctionServer(_zoneServer, _configurationService.SystemConfiguration.Functions);
            //_functionServer.StartUp();
        }

        /// <summary>
        /// Unload the function server. This object holds all functions.
        /// </summary>
        private static void UnloadFunctionServer()
        {
            LogHelper.Log(LogLevel.Info, String.Format(">>> Unload the function server..."));

            _functionServer = null;
        }

        /// <summary>
        /// This methods evalutes the command line arguments (passed in Options) and overrides the settings read from configuration file.
        /// </summary>
        /// <param name="device">Device configuartion, to be adapted.</param>
        private static void AdjustDeviceSettings(Device device)
        {
            if (_options != null)
            {
                if (_options.portName != null)
                {
                    LogHelper.Log(LogLevel.Info, String.Format(">>>   Override loaded configuration for 'Port Name', use {0} instead of {1}", _options.portName, device.Communication.Port));
                    device.Communication.Port = _options.portName;
                }
                if (_options.baudRate > 0)
                {
                    LogHelper.Log(LogLevel.Info, String.Format(">>>   Override loaded configuration for 'Baud Rate', use {0} instead of {1}", _options.baudRate, device.Communication.BaudRate));
                    device.Communication.BaudRate = _options.baudRate;
                }
            }
        }


        #endregion


        #region Protocol Driver Events

        /// <summary>
        /// Zone Status Event method
        /// </summary>
        /// <param name="sender">This pointer to the sender of the event.</param>
        /// <param name="e">Event arguments, returned by the sender.</param>
        static void _driver_onZoneStatusUpdate(object sender, ProtocolZoneUpdatedEventArgs e)
        {
            LogHelper.Log(LogLevel.Info, String.Format(">>>   [{0}]  Zone {1}  Status Update: {2}", DateTime.Now.ToShortTimeString(), e.ZoneAddress.ToString(), e.ZoneState.ToString()));
        }

        /// <summary>
        /// Device Status Update method
        /// </summary>
        /// <param name="sender">This pointer to the sender of the event.</param>
        /// <param name="e">Event arguments, returned by the sender.</param>
        static void _driver_onDeviceStatusUpdate(object sender, ProtocolDeviceUpdatedEventArgs e)
        {
            LogHelper.Log(LogLevel.Info, String.Format(">>>   [{0}]  Device {1}  Status Update: {2}", DateTime.Now.ToShortTimeString(), e.DeviceId.ToString(), e.DeviceQuality.ToString()));
        }

        /// <summary>
        /// Command Received method
        /// </summary>
        /// <param name="sender">This pointer to the sender of the event.</param>
        /// <param name="e">Event arguments, returned by the sender.</param>
        static void _driver_onCommandReceived(object sender, ProtocolCommandReceivedEventArgs e)
        {
            LogHelper.Log(LogLevel.Debug, String.Format(">>>   [{0}]  Zone {1}  Command Received: {2}", DateTime.Now.ToShortTimeString(), e.ZoneAddress.ToString(), e.Command.ToString()));
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
                string message = String.Format("PingTest called at {0} for {1} times", DateTime.Now.ToString(), pingcount.ToString());
                LogHelper.Log(LogLevel.Info, String.Format("\n### Ping received! {0}", message));
                return message; 
            }
        }

        /// <summary>
        /// Open ping service.
        /// To set permission, call with admin rights:
        ///    netsh http add urlacl url=http://+:7400/WcfPing user=WW002\imfeldc
        ///    This enables: http://md11p91c:7400/WcfPing
        /// More details see https://msdn.microsoft.com/library/ms733768.aspx
        /// </summary>
        private static void WcfTestHost_Open()
        {
            LogHelper.Log(LogLevel.All, ">>> Ping service (start) ....");

            string hostname = System.Environment.MachineName;
            var baseAddress = new UriBuilder("http", hostname, 7400, "WcfPing");
            var h = new ServiceHost(typeof(WcfPingTest), baseAddress.Uri);

            // enable processing of discovery messages.  use UdpDiscoveryEndpoint to enable listening. use EndpointDiscoveryBehavior for fine control.
            h.Description.Behaviors.Add(new ServiceDiscoveryBehavior());
            h.AddServiceEndpoint(new UdpDiscoveryEndpoint());
            LogHelper.Log(LogLevel.Debug, String.Format("Ping service: HostName={0}, Address={1}, ServiceHost={2}", hostname, baseAddress.ToString(), h.ToString()));

            // enable wsdl, so you can use the service from WcfStorm, or other tools.
            var smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            // This method is not implemented in mono/RaspberryPi: System.MissingMethodException exc is thrown
            // LogHelper.Log(LogLevel.Error, String.Format("Method 'smb.MetadataExporter.PolicyVersion' does not exists on Unix. [Exception={0}]", exc.Message));
            smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
            h.Description.Behaviors.Add(smb);

            // create endpoint
            var binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            h.AddServiceEndpoint(typeof(IWcfPingTest), binding, "");
            LogHelper.Log(LogLevel.All, ">>> Discovery for Ping service started ....");
            try
            {
                h.Open();
                LogHelper.Log(LogLevel.All, ">>> Ping service is running.");
                LogHelper.Log(LogLevel.Info, String.Format(">>> URI: {0}", h.BaseAddresses[0].AbsoluteUri));
            }
            catch (System.ServiceModel.AddressAccessDeniedException exc)
            {
                // see comment above, to set correct required permissions
                LogHelper.LogException(">>> Cannot open Discovery for Ping service!", exc);
            }
        }

        #endregion

    }
}
