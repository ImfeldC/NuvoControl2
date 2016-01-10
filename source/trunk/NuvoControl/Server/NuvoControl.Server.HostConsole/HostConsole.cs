using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;     // used for Application
using System.Text;

using Common.Logging;

using NuvoControl.Common;
using NuvoControl.Common.Configuration;

using NuvoControl.Server.ProtocolDriver;
using NuvoControl.Server.ProtocolDriver.Interface;

using NuvoControl.Server.ConfigurationService;
using NuvoControl.Server.FunctionServer;
using NuvoControl.Server.ZoneServer;



namespace NuvoControl.Server.HostConsole
{
    class HostConsole
    {
        static void Main(string[] args)
        {
            ILog log = LogManager.GetCurrentClassLogger();
            log.Debug(m => m("Starting Server Console! (Version={0})", Application.ProductVersion));
            LogHelper.Log("**** Server Console started. *******", log);

            Console.WriteLine(">>> Starting Server Console  --- Assembly Version={0} / Deployment Version={1} / Product Version={2} (using .NET 4.0) ... ",
                "n/a", "n/a", Application.ProductVersion);
            //Console.WriteLine(">>> Starting Server Console  --- Assembly Version={0} / Deployment Version={1} / Product Version={2} (using .NET 4.0) ... ",
            //    AppInfoHelper.getAssemblyVersion(), AppInfoHelper.getDeploymentVersion(), Application.ProductVersion);
            Console.WriteLine("    Linux={0} / Detected environment: {1}", EnvironmentHelper.isRunningOnLinux(), EnvironmentHelper.getOperatingSystem());
            Console.WriteLine();

            // Load command line argumnets
            _options = new Options();
            CommandLine.Parser.Default.ParseArguments(args, _options);
            if (_options.Help)
            {
                Console.WriteLine(_options.GetUsage());
            }

            _log.Trace(m => m("Check configuration timer started, each {0}[s]", Properties.Settings.Default.ConfigurationCheckIntervall));
            _timerCheckConfiguration.Interval = (Properties.Settings.Default.ConfigurationCheckIntervall < 30 ? 30 : Properties.Settings.Default.ConfigurationCheckIntervall) * 1000;
            _timerCheckConfiguration.Elapsed += new System.Timers.ElapsedEventHandler(_timerCheckConfiguration_Elapsed);
            _timerCheckConfiguration.Start();

            LoadAllServices();

            Console.WriteLine(">>> ");
            Console.WriteLine(">>> ");
            Console.WriteLine(">>> Press <Enter> to stop the console application.");
            Console.ReadLine();

            UnloadAllServices();

        }

        #region Fields

        private static ILog _log = LogManager.GetCurrentClassLogger();

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

        #endregion

        #region Configuration Check Timer

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
                UnloadAllServices();
                LoadAllServices();
            }
        }

        #endregion

        #region Services

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
                    AdjustDeviceSettings(device);
                    driver.Open(ENuvoSystem.NuVoEssentia, device.Id, device.Communication);
                    _protocolDrivers[device.Id] = driver;
                    if (_options.verbose)
                    {
                        Console.WriteLine(">>>   driver {0} loaded ... Communication Parameter=[{1}]", device.Id, device.Communication.ToString());
                    }
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

            foreach (int deviceId in _protocolDrivers.Keys)
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
                Console.WriteLine(">>>   device {0} loaded ...", device.ToString());
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

            if (_options.verbose)
            {
                Console.WriteLine(">>>   Functions: {0}", _functionServer.ToString());
            }
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
                    if (_options.verbose)
                    {
                        Console.WriteLine(">>>   Override loaded configuration for 'Port Name', use {0} instead of {1}", _options.portName, device.Communication.Port);
                    }
                    device.Communication.Port = _options.portName;
                }
                if (_options.baudRate > 0)
                {
                    Console.WriteLine(">>>   Override loaded configuration for 'Baud Rate', use {0} instead of {1}", _options.baudRate, device.Communication.BaudRate);
                    device.Communication.BaudRate = _options.baudRate;
                }
            }
        }

        #endregion
    }
}
