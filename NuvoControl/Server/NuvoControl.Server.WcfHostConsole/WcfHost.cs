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

using Common.Logging;

using System.ServiceModel.Discovery;

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

        #endregion

        #region Non-Public Interface

        /// <summary>
        /// Main function
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine(">>> Starting WCF services V2 Version={0} (using .NET 4.0) ... ", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
            Console.WriteLine();

            try
            {
                LoadConfigurationService(Properties.Settings.Default.NuvoControlKonfigurationFile);
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

            HostAllServices();
        }


        private static void HostAllServices()
        {
            ServiceHost configurationServiceHost = new ServiceHost(_configurationService);
            ServiceHostMc mCServiceHost = new ServiceHostMc(
                typeof(NuvoControl.Server.MonitorAndControlService.MonitorAndControlService), _zoneServer);
            ServiceHostFunction functionServiceHost = new ServiceHostFunction(
                typeof(NuvoControl.Server.FunctionService.FunctionService), _zoneServer, _configurationService.SystemConfiguration.Functions);

            try
            {
                // ** DISCOVERY ** //
                // make the service discoverable by adding the discovery behavior
                configurationServiceHost.Description.Behaviors.Add(new ServiceDiscoveryBehavior());
                // add the discovery endpoint that specifies where to publish the services
                configurationServiceHost.AddServiceEndpoint(new UdpDiscoveryEndpoint());
                Console.WriteLine(">>> Discovery Service started ....");
    
                configurationServiceHost.Open();
                Console.WriteLine(">>> Configuration service is running.");
                Console.WriteLine(">>> URI: {0}", configurationServiceHost.BaseAddresses[0].AbsoluteUri);
                Console.WriteLine();

                mCServiceHost.Open();
                Console.WriteLine(">>> Monitor and control service is running.");
                Console.WriteLine(">>> URI: {0}", mCServiceHost.BaseAddresses[0].AbsoluteUri);
                Console.WriteLine();

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

        /// <summary>
        /// Reads the XML configuration file and instantiates accordingly the system configuration objects of NuvoControl.
        /// </summary>
        /// <param name="configurationFile"></param>
        private static void LoadConfigurationService(string configurationFile)
        {
            _log.Info(m=>m("Loading the nuvo control configuration from '{0}' ...", configurationFile));
            Console.WriteLine(">>> Loading configuration...");

            _configurationService = new NuvoControl.Server.ConfigurationService.ConfigurationService(configurationFile);
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
                }
            }
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
        /// Instantiates the function server. This object holds all functions.
        /// </summary>
        private static void InstantiateFunctionServer()
        {
            _log.Info("Instantiating the function server...");
            Console.WriteLine(">>> Instantiating the function server...");

            _functionServer = new NuvoControl.Server.FunctionServer.FunctionServer(_zoneServer, _configurationService.SystemConfiguration.Functions);
            //_functionServer.StartUp();
        }

        #endregion
    }
}
