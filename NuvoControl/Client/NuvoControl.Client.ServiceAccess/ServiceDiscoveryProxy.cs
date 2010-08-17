using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Discovery;

using Common.Logging;
using NuvoControl.Common;
using System.ServiceModel;


namespace NuvoControl.Client.ServiceAccess
{
    /// <summary>
    /// Class to handle the discovery service for one or more types.
    /// </summary>
    public class ServiceDiscoveryProxy
    {
        /// <summary>
        /// Delegate for asynchronous call of DiscoverService method
        /// </summary>
        /// <param name="discoverType">Type to retrieve.</param>
        /// <returns>Endpoints found for the specified type.</returns>
        private delegate FindResponse DelegateDiscoverService(Type discoverType);

        /// <summary>
        /// Clas used as entyr in the list.
        /// </summary>
        private class ServiceDiscoveryEntry
        {
            /// <summary>
            /// True if the disovery service was already executed once.
            /// </summary>
            private bool _serviceDiscovered = false;

            /// <summary>
            /// Name of the service.
            /// </summary>
            private string _name;

            /// <summary>
            /// Type to discover.
            /// </summary>
            private Type _discoverType;

            /// <summary>
            /// Contains the discovered endpoints for the specified type.
            /// Is null if the discovery service wasn't executed so far.
            /// </summary>
            private FindResponse _discoveredServices = null;

            /// <summary>
            /// Result used in case of asynchronous calls.
            /// </summary>
            private IAsyncResult _tag = null;


            /// <summary>
            /// Constructor, for the specified type.
            /// </summary>
            /// <param name="name">Name of the service.</param>
            /// <param name="discoverType">Type to discover.</param>
            public ServiceDiscoveryEntry( string name, Type discoverType)
            {
                _name = name;
                _discoverType = discoverType;
            }

            /// <summary>
            /// True, if the discovery service was once executed for the specified type.
            /// </summary>
            public bool ServiceDiscovered
            {
                get { return _serviceDiscovered; }
            }

            /// <summary>
            /// Retruns the name of the service.
            /// </summary>
            public string Name
            {
                get { return _name; }
            }

            /// <summary>
            /// Returns the type to discover.
            /// </summary>
            public Type DiscoverType
            {
                get { return _discoverType; }
            }

            /// <summary>
            /// Gets and Sets the found endpoints for the specified type.
            /// </summary>
            public FindResponse DiscoveredServices
            {
                get { return _discoveredServices; }
                set 
                { 
                    _discoveredServices = value;
                    _serviceDiscovered = true;
                }
            }

            /// <summary>
            /// Gets and Sets the result set for asynchronous calls.
            /// </summary>
            public IAsyncResult AsynchResult
            {
                get { return _tag; }
                set { _tag = value; }
            }

            /// <summary>
            /// Returns string equivalnet of the object.
            /// </summary>
            /// <returns>String representing the object.</returns>
            public override string ToString()
            {
                string retString = "";
                if( _serviceDiscovered )
                {
                    retString = String.Format("Name={0}, Type={1} - ", _name, _discoverType.ToString());
                    foreach (EndpointDiscoveryMetadata endpoint in _discoveredServices.Endpoints)
                    {
                        retString += String.Format("Address={0}, Version={1} [", endpoint.Address.ToString(), endpoint.Version);
                        foreach (Uri uri in endpoint.ListenUris)
                        {
                            retString += String.Format("Uri.AbsolutePath={0},Uri.AbsoluteUri={1}, Uri.Host={2}", uri.AbsolutePath, uri.AbsoluteUri, uri.Host);
                        }
                        retString += "]";
                    }
                }
                else
                {
                    retString = String.Format("Type={0} - (discovery not executed yet)", _discoverType.ToString());
                }
                return retString;
            }
        }

        /// <summary>
        /// Trace logger
        /// </summary>
        private static ILog _log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// List with the types to discover and its discovered endpoints.
        /// </summary>
        private List<ServiceDiscoveryEntry> _serviceDiscoveryList = null;

        /// <summary>
        /// List with the discovered servers.
        /// </summary>
        private List<string> _discoveredServers = null;

        /// <summary>
        /// Constructor. Creates an empty list of types to discover.
        /// Add the types with the addService method.
        /// </summary>
        public ServiceDiscoveryProxy()
        {
            _serviceDiscoveryList = new List<ServiceDiscoveryEntry>();
            _discoveredServers = new List<string>();
        }

        /// <summary>
        /// Add a type to the list which will be discovered later.
        /// </summary>
        /// <param name="name">Name of the service.</param>
        /// <param name="discoverType">Type to discover.</param>
        public void addService(string name, Type discoverType)
        {
            _serviceDiscoveryList.Add(new ServiceDiscoveryEntry(name,discoverType));
        }


        /// <summary>
        /// Returns true if the specified type was already discovered.
        /// </summary>
        /// <param name="serviceName">Name of the service.</param>
        /// <returns>True if type has been discovered and at least one service was found.</returns>
        public bool isServiceDiscovered(string serviceName)
        {
            bool bRet = false;
            foreach (ServiceDiscoveryEntry serviceDiscoveryEntry in _serviceDiscoveryList)
            {
                if (serviceDiscoveryEntry.Name == serviceName)
                {
                    bRet = (serviceDiscoveryEntry.ServiceDiscovered && serviceDiscoveryEntry.DiscoveredServices.Endpoints.Count>0);
                    break;
                }
            }
            return bRet;
        }

        /// <summary>
        /// Return list of discovered servers.
        /// </summary>
        public List<string> DiscoveredServers
        {
            get { return _discoveredServers; }
        }

        /// <summary>
        /// Get endpoint address for the specified service name and server name
        /// </summary>
        /// <param name="serviceName">Service name.</param>
        /// <param name="serverName">Server name.</param>
        /// <returns>Endpoint address, for the specified service name and server name.</returns>
        public EndpointAddress EndpointAddress(string serviceName, string serverName)
        {
            foreach (ServiceDiscoveryEntry serviceDiscoveryEntry in _serviceDiscoveryList)
            {
                if (serviceDiscoveryEntry.Name.Equals(serviceName))
                {
                    foreach (EndpointDiscoveryMetadata endpoint in serviceDiscoveryEntry.DiscoveredServices.Endpoints)
                    {
                        foreach (Uri uri in endpoint.ListenUris)
                        {
                            if (uri.Host.Equals(serverName))
                            {
                                return endpoint.Address;
                            }
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Public discovery method, starts the discovery service for all types in the list.
        /// Add the types with the addService method.
        /// </summary>
        /// <param name="bEnforceDiscovery">If true, enforces a new discovery even if it was already executed.</param>
        public void DiscoverAllServices(bool bEnforceDiscovery)
        {
            _log.Trace(m => m("DiscoverAllServices: Start discovering [bEnforceDiscovery={0}] ...", bEnforceDiscovery.ToString()));

            // create the delegate
            DelegateDiscoverService delDiscoverService = new DelegateDiscoverService(DiscoverService);

            // Start asynchronous the discover service for each type
            foreach (ServiceDiscoveryEntry serviceDiscoveryEntry in _serviceDiscoveryList)
            {
                if (!serviceDiscoveryEntry.ServiceDiscovered || bEnforceDiscovery)
                {
                    //serviceDiscoveryEntry.DiscoveredServices = DiscoverService(serviceDiscoveryEntry.DiscoverType);
                    serviceDiscoveryEntry.AsynchResult = delDiscoverService.BeginInvoke(serviceDiscoveryEntry.DiscoverType, null, null);
                }
                else
                {
                    // delete a previous existing result
                    serviceDiscoveryEntry.AsynchResult = null;
                }
            }

            // Wait for each discover service
            foreach (ServiceDiscoveryEntry serviceDiscoveryEntry in _serviceDiscoveryList)
            {
                if (serviceDiscoveryEntry.AsynchResult != null)
                {
                    serviceDiscoveryEntry.DiscoveredServices = delDiscoverService.EndInvoke(serviceDiscoveryEntry.AsynchResult);
                }
                serviceDiscoveryEntry.AsynchResult = null;
            }

            _log.Trace(m => m("DiscoverAllServices: End discovering ..."));
            builListOfDiscoveredServers();
            _log.Trace(m => m("DiscoverAllServices: {0}", this.ToString()));
        }

        /// <summary>
        /// Discovery method for the service. This method is called with a delegate asynchronous.
        /// </summary>
        /// <param name="discoverType">Type to discover.</param>
        /// <returns>Returns the discovered endpoints.</returns>
        private FindResponse DiscoverService(Type discoverType)
        {
            // ------- DISCOVERY ----------

            _log.Trace(m => m("DiscoverService: Start discovering {0} ...", discoverType.ToString()));

            DiscoveryClient discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint());
            FindCriteria criteria = new FindCriteria(discoverType);
            FindResponse discovered = discoveryClient.Find(criteria);
            discoveryClient.Close();

            _log.Trace(m => m("DiscoverService: {0} services found.", discovered.Endpoints.Count));
            LogHelper.LogEndPoint(_log, discovered.Endpoints);

            // ----------------------------

            return discovered;
        }

        /// <summary>
        /// Build list of discovered servers.
        /// </summary>
        private void builListOfDiscoveredServers()
        {
            _discoveredServers.Clear();
            foreach (ServiceDiscoveryEntry serviceDiscoveryEntry in _serviceDiscoveryList)
            {
                foreach( EndpointDiscoveryMetadata endpoint in serviceDiscoveryEntry.DiscoveredServices.Endpoints )
                {
                    foreach (Uri uri in endpoint.ListenUris)
                    {
                        if( !_discoveredServers.Contains(uri.Host) )
                        {
                            _discoveredServers.Add(uri.Host);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns string equivalnet of the object.
        /// </summary>
        /// <returns>String representing the object.</returns>
        public override string ToString()
        {
            string retString = "";
            foreach (ServiceDiscoveryEntry serviceDiscoveryEntry in _serviceDiscoveryList)
            {
                retString += serviceDiscoveryEntry.ToString();
            }
            return retString;
        }
    }
}
