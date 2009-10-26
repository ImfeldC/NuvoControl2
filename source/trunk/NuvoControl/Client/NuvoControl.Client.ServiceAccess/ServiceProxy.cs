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
        /// M&C proxy
        /// </summary>
        public static MonitorAndControlProxy _monitorAndControlProxy = null;

        /// <summary>
        /// Configuration proxy
        /// </summary>
        public static ConfigurationProxy _configurationProxy = null;

        /// <summary>
        /// The ip or name of the client.
        /// </summary>
        private static string _clientIpOrName = Properties.Settings.Default.ClientIPOrName;

        /// <summary>
        /// The ip or name of the server.
        /// </summary>
        private static string _serverName = Properties.Settings.Default.ServerName;

        #endregion

        #region Public Interface


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
        /// Get/Set the ip or the name of the client.
        /// </summary>
        public static string ClientIpOrName
        {
            get { return _clientIpOrName; }
            set 
            {
                _log.Trace(m => m("Save new client name! New address = '{0}' / Old name = '{1}'", value, _clientIpOrName));
                _clientIpOrName = value;
                Properties.Settings.Default.ClientIPOrName = _clientIpOrName;
                Properties.Settings.Default.Save();
            }
        }

        /// <summary>
        /// Get/Set the ip or the name of the server.
        /// </summary>
        public static string ServerName
        {
            get { return _serverName; }
            set 
            {
                _log.Trace(m => m("Save new server name! New address = '{0}' / Old name = '{1}'", value, _serverName));
                _serverName = value;
                Properties.Settings.Default.ServerName = _serverName;
                Properties.Settings.Default.Save();
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