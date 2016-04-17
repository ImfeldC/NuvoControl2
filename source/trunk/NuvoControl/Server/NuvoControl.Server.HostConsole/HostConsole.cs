/**************************************************************************************************
 * 
 *   Copyright (C) 2016 by Ch. Imfeld. All Rights Reserved.
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.HostConsole
 * 
 **************************************************************************************************/

using Common.Logging;
using NuvoControl.Common;
using NuvoControl.Common.Configuration;
using NuvoControl.Server.OscServer;
using NuvoControl.Server.ProtocolDriver;
using NuvoControl.Server.ProtocolDriver.Interface;
using NuvoControl.Server.ZoneServer;
using System;
using System.Collections.Generic;


namespace NuvoControl.Server.HostConsole
{
    class HostConsole
    {
        static void Main(string[] args)
        {
            // Load command line argumnets
            _options = new Options();
            CommandLine.Parser.Default.ParseArguments(args, _options);
            // Set global verbose mode
            LogHelper.SetOptions(_options );
            LogHelper.LogAppStart("Server Console"); 
            LogHelper.LogArgs(args);
            if (_options.Help)
            {
                Console.WriteLine(_options.GetUsage());
            }

            LogHelper.Log(LogLevel.Debug,String.Format("Check configuration timer started, each {0}[s]", Properties.Settings.Default.ConfigurationCheckIntervall));
            _timerCheckConfiguration.Interval = (Properties.Settings.Default.ConfigurationCheckIntervall < 30 ? 30 : Properties.Settings.Default.ConfigurationCheckIntervall) * 1000;
            _timerCheckConfiguration.Elapsed += new System.Timers.ElapsedEventHandler(_timerCheckConfiguration_Elapsed);
            _timerCheckConfiguration.Start();

            LoadAllServices();

            LogHelper.Log(LogLevel.All, ">>> ");
            LogHelper.Log(LogLevel.All, ">>> ");
            LogHelper.Log(LogLevel.All, ">>> Press <Enter> to stop the console application.");
            Console.ReadLine();

            UnloadAllServices();

        }

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
        /// Private list holding all play sound drivers (for each audio device)
        /// </summary>
        private static Dictionary<int, IAudioDriver> _audioDrivers = new Dictionary<int, IAudioDriver>();

        /// <summary>
        /// Private list holding all OSC server drivers
        /// </summary>
        private static Dictionary<int, IOscDriver> _oscDrivers = new Dictionary<int, IOscDriver>();

        /// <summary>
        /// Holds a reference to the zone server.
        /// </summary>
        private static IZoneServer _zoneServer = null;

        /// <summary>
        /// Holds the reference to the osc server.
        /// </summary>
        private static NuvoControl.Server.OscServer.OscServer _oscServer = null;

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
            if (_configurationService != null)
            {
                bool bChanged = _configurationService.CheckConfiguration();
                if (bChanged)
                {
                    UnloadAllServices();
                    LoadAllServices();
                }
            }
        }

        #endregion

        #region Services

        private static void LoadAllServices()
        {
            try
            {
                //LoadConfigurationService(Properties.Settings.Default.NuvoControlKonfigurationFile);
                // Remote File: http://www.imfeld.net/publish/configuration/NuvoControlKonfigurationRemote.xml
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
                LoadAudioDrivers();
            }
            catch (Exception exc)
            {
                LogHelper.LogException("Failed to load the audio drivers.", exc);
            }

            try
            {
                LoadOSCDrivers();
            }
            catch (Exception exc)
            {
                LogHelper.LogException("Failed to load the OSC drivers.", exc);
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
                InstantiateOscServer();
            }
            catch (Exception exc)
            {
                LogHelper.LogException("Failed to create the osc server.", exc);
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
                UnloadOscServer();
            }
            catch (Exception exc)
            {
                LogHelper.LogException("Failed to unload the osc server.", exc);
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
                UnloadOSCDrivers();
            }
            catch (Exception exc)
            {
                LogHelper.LogException("Failed to unload the OSC drivers.", exc);
            }

            try
            {
                UnloadAudioDrivers();
            }
            catch (Exception exc)
            {
                LogHelper.LogException("Failed to unload the audio drivers.", exc);
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


        #region Load Configuration
        /// <summary>
        /// Reads the XML configuration file and instantiates accordingly the system configuration objects of NuvoControl.
        /// </summary>
        /// <param name="configurationFile">Filename of configuration file</param>
        /// <param name="remoteConfigurationFile">Filename of configuration file to append</param>
        private static void LoadConfigurationService(string configurationFile, string remoteConfigurationFile)
        {
            LogHelper.Log(LogLevel.Info, String.Format("Loading the nuvo control configuration from '{0}' and '{1}' ...", configurationFile, remoteConfigurationFile));

            _configurationService = new NuvoControl.Server.ConfigurationService.ConfigurationService(configurationFile, remoteConfigurationFile);
        }

        /// <summary>
        /// Unloads the system configuration objects of NuvoControl.
        /// </summary>
        /// <param name="configurationFile">Filename of configuration file</param>
        private static void UnloadConfigurationService()
        {
            LogHelper.Log(LogLevel.Info, String.Format(">>> Unload the nuvo control configuration ..."));

            _configurationService.Dispose();
            _configurationService = null;
        }
        #endregion

        #region Protocol Driver
        /// <summary>
        /// Loads the protocol drivers according to the NuvoControl system configuration.
        /// </summary>
        private static void LoadProtocolDrivers()
        {
            LogHelper.Log(LogLevel.Info, String.Format(">>> Loading protocol drivers..."));

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

            foreach (int deviceId in _protocolDrivers.Keys)
            {
                _protocolDrivers[deviceId].Close(deviceId);
                //_protocolDrivers[deviceId] = null;
                LogHelper.Log(LogLevel.Info, String.Format(">>>   driver {0} unloaded ...", deviceId));
            }
            _protocolDrivers.Clear();
        }
        #endregion

        #region Audio Driver
        /// <summary>
        /// Loads the audio drivers according to the NuvoControl system configuration.
        /// </summary>
        private static void LoadAudioDrivers()
        {
            LogHelper.Log(LogLevel.Info, String.Format(">>> Loading audio drivers..."));

            foreach (Device device in _configurationService.SystemConfiguration.Hardware.Devices)
            {
                foreach (AudioDevice audioDevice in device.AudioDevices)
                {
                    LogHelper.Log(LogLevel.Info, String.Format(">>>   audio device {0} loaded ...", audioDevice.Id.ToString()));
                    _audioDrivers[audioDevice.SourceId.ObjectId] = new AudioDriver(audioDevice.SourceId, audioDevice);
                }
            }
        }

        /// <summary>
        /// Unload the audio drivers.
        /// </summary>
        private static void UnloadAudioDrivers()
        {
            LogHelper.Log(LogLevel.Info, String.Format(">>> Unload audio drivers..."));

            foreach (int deviceId in _audioDrivers.Keys)
            {
                _audioDrivers[deviceId].Close();
                //_audioDrivers[deviceId] = null;
                LogHelper.Log(LogLevel.Info, String.Format(">>>   audio driver {0} unloaded ...", deviceId));
            }
            _audioDrivers.Clear();
        }
        #endregion

        #region OSC Driver
        /// <summary>
        /// Loads the OSC drivers according to the NuvoControl system configuration.
        /// </summary>
        private static void LoadOSCDrivers()
        {
            LogHelper.Log(LogLevel.Info, String.Format(">>> Loading OSC drivers..."));

            foreach (Device device in _configurationService.SystemConfiguration.Hardware.Devices)
            {
                foreach (OSCDevice oscDevice in device.OscDevices)
                {
                    LogHelper.Log(LogLevel.Info, String.Format(">>>   OSC device {0} loaded ...", oscDevice.DeviceId.ToString()));
                    _oscDrivers[oscDevice.DeviceId.ObjectId] = new TouchOscDriver(oscDevice);
                    _oscDrivers[oscDevice.DeviceId.ObjectId].onOscEventReceived += new OscEventReceivedEventHandler(HostConsole_onOscEventReceived);
                    _oscDrivers[oscDevice.DeviceId.ObjectId].Start();
                }
            }
        }

        /// <summary>
        /// Unload the OSC drivers.
        /// </summary>
        private static void UnloadOSCDrivers()
        {
            LogHelper.Log(LogLevel.Info, String.Format(">>> Unload OSC drivers..."));

            foreach (int deviceId in _oscDrivers.Keys)
            {
                _oscDrivers[deviceId].Stop();
                LogHelper.Log(LogLevel.Info, String.Format(">>>   OSC driver {0} unloaded ...", deviceId));
            }
            _oscDrivers.Clear();
        }
        #endregion

        #region Zone Server
        /// <summary>
        /// Instantiates the zone server. This object holds all zone controllers.
        /// </summary>
        private static void InstantiateZoneServer()
        {
            LogHelper.Log(LogLevel.Info, String.Format(">>> Instantiating the zone server..."));

            List<IZoneController> zoneControllers = new List<IZoneController>();
            foreach (Device device in _configurationService.SystemConfiguration.Hardware.Devices)
            {
                LogHelper.Log(LogLevel.Info, String.Format(">>>   device {0} loaded ...", device.Name));
                foreach (Zone zone in device.ZoneList)
                {
                    zoneControllers.Add(new ZoneController(new Address(device.Id, zone.Id.ObjectId), _protocolDrivers[device.Id]));
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
        #endregion

        #region OSC Server
        /// <summary>
        /// Instantiates the osc server. This object holds all osc device controllers.
        /// </summary>
        private static void InstantiateOscServer()
        {
            LogHelper.Log(LogLevel.Info, String.Format(">>> Instantiating the osc server..."));

            List<OscDeviceController> oscDeviceControllers = new List<OscDeviceController>();
            foreach (Device device in _configurationService.SystemConfiguration.Hardware.Devices)
            {
                LogHelper.Log(LogLevel.Info, String.Format(">>>   device '{0}' loaded ...", device.Name));
                foreach (OSCDevice oscDevice in device.OscDevices)
                {
                    oscDeviceControllers.Add(new OscDeviceController(new Address(device.Id, oscDevice.DeviceId.ObjectId), oscDevice, _protocolDrivers[device.Id], _oscDrivers[oscDevice.DeviceId.ObjectId], device.ZoneDict, device.Sources));
                }
            }
            _oscServer = new OscServer.OscServer(oscDeviceControllers);
            _oscServer.Start();
        }

        /// <summary>
        /// Unload the osc server. This object holds all osc device controllers.
        /// </summary>
        private static void UnloadOscServer()
        {
            LogHelper.Log(LogLevel.Info, String.Format(">>> Unload the osc server..."));

            _oscServer.Stop();
            _oscServer = null;
        }
        #endregion

        #region Function Server
        /// <summary>
        /// Instantiates the function server. This object holds all functions.
        /// </summary>
        private static void InstantiateFunctionServer()
        {
            LogHelper.Log(LogLevel.Info, String.Format(">>> Instantiating the function server..."));

            _functionServer = new NuvoControl.Server.FunctionServer.FunctionServer(_zoneServer, _configurationService.SystemConfiguration.Functions, _audioDrivers, _oscDrivers);
            LogHelper.Log(LogLevel.Debug, String.Format(">>>   Functions: {0}", _functionServer.ToString()));
        }

        /// <summary>
        /// Unload the function server. This object holds all functions.
        /// </summary>
        private static void UnloadFunctionServer()
        {
            LogHelper.Log(LogLevel.Info, String.Format(">>> Unload the function server..."));

            _functionServer.Dispose();
            _functionServer = null;
        }
        #endregion

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
            Zone zone = _configurationService.GetZoneHWConfiguration(e.ZoneAddress);
            Source source = _configurationService.GetSourceHWConfiguration(e.ZoneState.Source);
            if (e.ZoneState.PowerStatus == true)
            {
                // Zone is ON ...
                LogHelper.Log(LogLevel.Info, String.Format(">>>   [{0}]  Zone '{1}'({2}) is ON -> Source '{3}' with Volume '{4}'", DateTime.Now.ToShortTimeString(), zone.Name, e.ZoneAddress.ToString(), source.Name, e.ZoneState.Volume));
            }
            else
            {
                // Zone is OFF ...
                LogHelper.Log(LogLevel.Info, String.Format(">>>   [{0}]  Zone '{1}'({2}) is OFF", DateTime.Now.ToShortTimeString(), zone.Name, e.ZoneAddress.ToString() ));
            }
            LogHelper.Log(LogLevel.Debug, String.Format(">>>   [{0}]  Zone {1}  Status Update: {2}", DateTime.Now.ToShortTimeString(), e.ZoneAddress.ToString(), e.ZoneState.ToString()));
        }

        /// <summary>
        /// Device Status Update method
        /// </summary>
        /// <param name="sender">This pointer to the sender of the event.</param>
        /// <param name="e">Event arguments, returned by the sender.</param>
        static void _driver_onDeviceStatusUpdate(object sender, ProtocolDeviceUpdatedEventArgs e)
        {
            LogHelper.Log(LogLevel.Info, String.Format(">>>   [{0}]  Device={1}  Status Update: {2}", DateTime.Now.ToShortTimeString(), e.DeviceId.ToString(), e.DeviceQuality.ToString()));
        }

        /// <summary>
        /// Command Received method
        /// </summary>
        /// <param name="sender">This pointer to the sender of the event.</param>
        /// <param name="e">Event arguments, returned by the sender.</param>
        static void _driver_onCommandReceived(object sender, ProtocolCommandReceivedEventArgs e)
        {
            LogHelper.Log(LogLevel.Debug, String.Format(">>>   [{0}]  Zone={1}  Command Received: {2}", DateTime.Now.ToShortTimeString(), e.ZoneAddress.ToString(), e.Command.ToString()));
        }

        #endregion


        #region OSC Server Events

        static void HostConsole_onOscEventReceived(object sender, OscEventReceivedEventArgs e)
        {
            LogHelper.Log(LogLevel.Info, String.Format(">->   [{0}]  Device={1} OscEvent:{2}", DateTime.Now.ToShortTimeString(), e.OscDevice, (e.OscEvent==null?"<null>":e.OscEvent.ToString()) ));
        }

        #endregion

    }
}
