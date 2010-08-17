/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Client.Viewer
 *   Author:         Bernhard Limacher
 *   Creation Date:  12.07.2009
 *   File Name:      ServiceProxy.cs
 * 
 ***************************************************************************************************
 * 
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using System.ServiceModel.Configuration;

using NuvoControl.Client.ServiceAccess.MonitorAndControlService;
using NuvoControl.Client.ServiceAccess.ConfigurationService;
using Common.Logging;

namespace NuvoControl.Client.ServiceAccess
{
    /// <summary>
    /// Static class, used to gain access from all over the viewer to the service proxies.
    /// </summary>
    public static class ServiceProxy
    {
        #region Fields

        /// <summary>
        /// Trace logger
        /// </summary>
        private static ILog _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Discovery service object.
        /// Register types to discovery with the addService method, before executing the discover method.
        /// </summary>
        private static ServiceDiscoveryProxy _serviceDiscovery = null;

        /// <summary>
        /// M&C proxy
        /// </summary>
        public static MonitorAndControlProxy _monitorAndControlProxy = null;

        /// <summary>
        /// Configuration proxy
        /// </summary>
        public static ConfigurationProxy _configurationProxy = null;

        /// <summary>
        /// The ip or name of the client.
        /// It is read from System.Environment.MachineName
        /// </summary>
        private static string _clientIpOrName = System.Environment.MachineName;

        /// <summary>
        /// The ip or name of the server.
        /// </summary>
        private static string _serverName = Properties.Settings.Default.ServerName;

        #endregion

        #region Public Interface

        /// <summary>
        /// Retruns Discovery Service object.
        /// </summary>
        public static ServiceDiscoveryProxy ServiceDiscovery
        {
            get 
            {
                if (_serviceDiscovery == null)
                {
                    _serviceDiscovery = new ServiceDiscoveryProxy();
                    _serviceDiscovery.addService(typeof(IConfigure));
                    _serviceDiscovery.addService(typeof(IMonitorAndControl));
                }
                return _serviceDiscovery; 
            }
        }

        /// <summary>
        /// Returns the M&C proxy.
        /// </summary>
        public static MonitorAndControlProxy MonitorAndControlProxy
        {
            get
            {
                if (_monitorAndControlProxy == null)
                {
                    _monitorAndControlProxy = new MonitorAndControlProxy(_clientIpOrName);
                }
                
                return _monitorAndControlProxy;
            }
        }


        /// <summary>
        /// Returns the configuration proxy.
        /// </summary>
        public static ConfigurationProxy ConfigurationProxy
        {
            get
            {
                if (_configurationProxy == null)
                    _configurationProxy = new ConfigurationProxy();

                return _configurationProxy;
            }
        }

        /// <summary>
        /// Public method to discover available services, like configuration service
        /// and monitor&control service.
        /// </summary>
        public static void DiscoverServices()
        {
            ServiceDiscovery.DiscoverAllServices(false);
        }

        /// <summary>
        /// Dispose the services. Using this method instead of Dispose() directly on the service,
        /// avoids that it is instantiated because of the singleton pattern.
        /// </summary>
        public static void Dispose()
        {
            if (_monitorAndControlProxy != null)
            {
                _monitorAndControlProxy.Dispose();
            }
            if (_configurationProxy != null)
            {
                _configurationProxy.Dispose();
            }
        }

        /// <summary>
        /// Injects a configuration service proxy
        /// </summary>
        /// <param name="configurationProxy"></param>
        public static void Inject(IConfigure configurationProxy)
        {
            _configurationProxy = new ConfigurationProxy(configurationProxy);
        }


        /// <summary>
        /// Injects a M&C service proxy
        /// </summary>
        public static void Inject(IMonitorAndControl mAndCProxy)
        {
            _monitorAndControlProxy = new MonitorAndControlProxy(mAndCProxy);
        }

        /// <summary>
        /// Get the ip or the name of the client.
        /// </summary>
        public static string ClientIpOrName
        {
            get { return _clientIpOrName; }
        }

        /// <summary>
        /// Get/Set the ip or the name of the server.
        /// </summary>
        public static string ServerName
        {
            get 
            {
                string serverName = "";
                serverName = ConfigurationProxy.endPointAdress.Uri.Host;
                return serverName; 
            }
            set 
            {
                if (_serverName != value)
                {
                    _log.Trace(m => m("Save new server name! New address = '{0}' / Old name = '{1}'", value, _serverName));
                    _serverName = value;
                    Properties.Settings.Default.ServerName = _serverName;
                    Properties.Settings.Default.Save();
                }
            }
        }

        /// <summary>
        /// 
        /// More information at http://social.msdn.microsoft.com/Forums/en-US/wcf/thread/843ef9fd-f48e-42ed-ab7c-2baf68d6f0d1
        /// </summary>
        /// <param name="sectionName">Name of the endpoint.</param>
        /// <returns>Address with replaced server name.</returns>
        public static string buildEndpointAddress( string sectionName )
        {
            string endpointAdress = "";
            ClientSection clientSection = (ClientSection)WebConfigurationManager.GetSection("system.serviceModel/client");
            //ChannelEndpointElement endpoint = clientSection.Endpoints[0];
            foreach( ChannelEndpointElement endpoint in clientSection.Endpoints )
            {
                if (endpoint.Name == sectionName)
                {
                    // correct section found ...
                    endpointAdress = endpoint.Address.AbsoluteUri;
                    endpointAdress = endpointAdress.Replace("localhost", _serverName);
                    break;  // exit foreach loop
                }
            }
            return endpointAdress;
        }


        #endregion
    }
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/