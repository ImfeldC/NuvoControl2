/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Server.Service
 *   Author:         Bernhard Limacher
 *   Creation Date:  19.05.2009
 *   File Name:      IControl.cs
 * 
 ***************************************************************************************************
 * 
 * Revisions:
 * 1) 19.05.2009, Bernhard Limacher: Starting up the service, instantiation of all service objects.
 * 2) 22.05.2009, Bernhard Limacher: StartSession / EndSession / Subscription / Unsubscription.
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common.Logging;

using NuvoControl.Common.Interfaces;
using NuvoControl.Common.Configuration;
using NuvoControl.Server.Service.Configuration;
using NuvoControl.Server.Service.MandC;
using NuvoControl.Server.ProtocolDriver.Interface;

namespace NuvoControl.Server.Service
{
    /// <summary>
    /// Implements the interface <see cref="INuvoControl"/>.
    /// It represents the Nuvo Control service. A host process is responsible for the lifetime of it.
    /// Its responsibility is to "bootstrap" and configure all other objects of the service.
    /// Furthermore it manages all the connected clients.
    /// </summary>
    public class NuvoControlService: INuvoControl
    {
        #region Fields

        private static ILog _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Holds the loaded system configuration.
        /// </summary>
        private NuvoControlConfiguration _configuration = null;

        /// <summary>
        /// Holds the protocol drivers per NuvoEssentia.
        /// </summary>
        private Dictionary<int, IProtocol> _protocolDrivers = new Dictionary<int, IProtocol>();

        /// <summary>
        /// Holds the communication parameters per NuvoEssentia.
        /// </summary>
        private Dictionary<int, Communication> _deviceCommunication = new Dictionary<int, Communication>();

        /// <summary>
        /// Holds the connected .
        /// </summary>
        private Dictionary<Guid, Guid> _clients = new Dictionary<Guid, Guid>();

        /// <summary>
        /// Hold the monitor and control object.
        /// </summary>
        private IMonitorAndControl _monitorAndControl = null;

        /// <summary>
        /// Flag specifies, if the service is started or not.
        /// </summary>
        private bool started = false;

        #endregion

        #region INuvoControl Members

        /// <summary>
        /// Starts up the service. To be called once. Instantates all service objects.
        /// </summary>
        /// <param name="configurationFile">The Nuvo Control configuration.</param>
        public void StartUp(string configurationFile)
        {
            lock (typeof(NuvoControlService))
            {
                if (started)
                {
                    _log.Warn("The service is already started. No need to start a second time.");
                    return;
                }

                _log.Info("Starting up Nuvo Control service...");

                LoadNuvoControlConfiguration(configurationFile);
                LoadProtocolDrivers();
                ReadCommuncationParameters();
                CreateProcessModel();
            }
        }

        /// <summary>
        /// Shuts down the service.
        /// </summary>
        public void ShutDown()
        {
            lock (typeof(NuvoControlService))
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Starts a session for a new client.
        /// </summary>
        /// <param name="clientId">The Id of the client.</param>
        /// <returns>True, in case of success. Otherwise false.</returns>
        public bool StartSession(Guid clientId)
        {
            lock (_clients)
            {
                if (_clients.ContainsKey(clientId))
                {
                    _log.ErrorFormat("Client with Id = {0} already connected.", clientId);
                    return false;
                }
                else
                {
                    _log.ErrorFormat("Client with Id = {0} now connected.", clientId);
                    _clients[clientId] = clientId;
                    return true;
                }
            }
        }

        /// <summary>
        /// Terminates a client session. All subscriptions of the client are deleted.
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns>True, in case of success. Otherwise false.</returns>
        public bool EndSession(Guid clientId)
        {
            lock (_clients)
            {
                if (_clients.ContainsKey(clientId))
                {
                    _log.ErrorFormat("Releasing client with Id = {0}.", clientId);
                    _monitorAndControl.RemoveMonitor(clientId);
                    return true;
                }
                else
                {
                    _log.ErrorFormat("Client with Id = {0} unknown.", clientId);
                    return false;
                }
            }
        }

        /// <summary>
        /// Returns the <see cref="IConfigure"/> interface.
        /// This provides access to functionality related to the system configuration of the service.
        /// </summary>
        public IConfigure IConfigure
        {
            get { return _configuration; }
        }

        /// <summary>
        /// Returns the <see cref="IMonitorAndControl"/> interface.
        /// This provides access to functionality related to controlling and monitoring the state of the service with its zones
        /// </summary>
        public IMonitorAndControl IMonitorAndControl
        {
            get { return _monitorAndControl; }
        }


        #endregion

        #region Non-Public Interface

        /// <summary>
        /// Reads the XML configuration file and instantiates accordingly the system configuration objects of NuvoControl.
        /// </summary>
        /// <param name="configurationFile"></param>
        private void LoadNuvoControlConfiguration(string configurationFile)
        {
            _log.Info("Loading the nuvo control configuration...");

            _configuration = new NuvoControlConfiguration(configurationFile);
        }

        /// <summary>
        /// Loads the protocol drivers according to the NuvoControl system configuration.
        /// </summary>
        private void LoadProtocolDrivers()
        {
            _log.Info("Loading protocol drivers...");

            foreach (NuvoEssentia device in _configuration.SystemConfiguration.Hardware.Devices)
            {
                IProtocol driver = ProtocolDriverFactory.LoadDriver(device.ProtocolDriver.AssemblyName, device.ProtocolDriver.ClassName);
                if (driver != null)
                    _protocolDrivers[device.Id] = driver;
            }
        }

        /// <summary>
        /// Reads the communications parameters from the NuvoControl system configuration.
        /// </summary>
        private void ReadCommuncationParameters()
        {
            _log.Info("Reading communication parameters...");

            foreach (NuvoEssentia device in _configuration.SystemConfiguration.Hardware.Devices)
            {
                _deviceCommunication[device.Id] = device.Communication;
            }
        }

        /// <summary>
        /// Instantiates the process model objects.
        /// </summary>
        private void CreateProcessModel()
        {
            _log.Info("Creating the process model...");

            List<IZoneController> zoneControllers = new List<IZoneController>();
            foreach (NuvoEssentia device in _configuration.SystemConfiguration.Hardware.Devices)
            {
                foreach (int zoneId in device.Zones)
                {
                    zoneControllers.Add(new ZoneController(new Address(device.Id, zoneId), _protocolDrivers[device.Id]));
                }
            }
            _monitorAndControl = new MonitorAndControl(zoneControllers, _clients);
        }


        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/
