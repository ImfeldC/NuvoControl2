using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Collections.ObjectModel;

using Common.Logging;

using NuvoControl.Common.Configuration;
using NuvoControl.Common;

using NuvoControl.Server.WcfHostConsole;    // WCF Ping Test
using NuvoControl.Server.FunctionService;
using NuvoControl.Server.MonitorAndControlService;
using NuvoControl.Server.ConfigurationService;
using NuvoControl.Client.ServiceAccess;

namespace NuvoControl.Client.WcfTestConsole
{
    public static class WsDualProxyHelper
    {
        public static void SetClientBaseAddress<T>(this DuplexClientBase<T> proxy, int port) where T : class
        {
            WSDualHttpBinding binding = proxy.Endpoint.Binding as WSDualHttpBinding;
            Debug.Assert(binding != null);
            binding.ClientBaseAddress = new Uri("http://imfeldc.dyndns.org:" + port + "/");
        }
        

        public static void SetClientBaseAddress<T>(this DuplexClientBase<T> proxy) where T : class
        {
            lock (typeof(WsDualProxyHelper))
            {
                int portNumber = FindPort();
                SetClientBaseAddress(proxy, portNumber);
                proxy.Open();
            }
        }

        internal static int FindPort()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Bind(endPoint);
                IPEndPoint local = (IPEndPoint)socket.LocalEndPoint;
                return local.Port;
            }
        }
    }


    /// <summary>
    /// Monitor and Control Callback, to receive zone state changes.
    /// </summary>
    public class ServerCallback : IMonitorAndControlNotification
    {
        public int _id = 1;
        #region IMonitorAndControlCallback Members

        public void OnZoneStateChanged(Address zoneId, ZoneState zoneState)
        {
            Console.WriteLine("+++ Notification from server zone: zoneId={0} / zoneState={1}", zoneId.ToString(), zoneState.ToString());
        }

        #endregion
    }

    
    /// <summary>
    /// Main Program of WcfTestConsole
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            ILog _log = LogManager.GetCurrentClassLogger();
            LogHelper.Log("**** Console Test client started. *******", _log);

            Console.WriteLine(">>> Starting WCF Test Client  --- Assembly Version={0} / Deployment Version={1} (using .NET 4.0) ... ",
                AppInfoHelper.getAssemblyVersion(), AppInfoHelper.getDeploymentVersion());
            Console.WriteLine();

            CheckFile("NuvoControlKonfigurationRemote.xml");

            // WCF Ping Test
            Console.WriteLine(">>> Start ping test ...");
            WcfTestClient_SetupChannel();
            WcfTestClient_Ping();
            Console.WriteLine(">>> End ping test ...");

            //Discover();


            FindResponse fr = Discover("IMonitorAndControl", typeof(IMonitorAndControl), 5);

            // ------- IMonitorAndControl: GetZoneState ----------
            {/*
                Console.WriteLine(">>> Start GetZoneState test ...");
                if (fr.Endpoints.Count > 0)
                {
                    foreach (EndpointDiscoveryMetadata edm in fr.Endpoints)
                    {
                        GetZoneState(edm.Address, new Address(100, 2));
                    }
                }
                Console.WriteLine(">>> End GetZoneState test ...");
            */}

            // ------- IMonitorAndControl: SetZoneState ----------
            {/*
                Address adr = new Address(100, 2);
                ZoneState stateCommand = new ZoneState(adr, false, 0, ZoneQuality.Offline);
                Console.WriteLine(">>> Start SetZoneState test ...");
                if (fr.Endpoints.Count > 0)
                {
                    foreach (EndpointDiscoveryMetadata edm in fr.Endpoints)
                    {
                        SetZoneState(edm.Address, adr, stateCommand);
                    }
                }
                Console.WriteLine(">>> End SetZoneState test ...");
            */}

            // ------- IMonitorAndControl: ControlWithCallback ----------
            {/*
                Address adr = new Address(100, 2);
                Console.WriteLine(">>> Start ControlWithCallback test ...");
                if (fr.Endpoints.Count > 0)
                {
                    foreach (EndpointDiscoveryMetadata edm in fr.Endpoints)
                    {
                        ControlWithCallback(edm.Address, adr);
                    }
                }
                Console.WriteLine(">>> End ControlWithCallback test ...");
            */}

            Console.WriteLine(">>> Press <Enter> to stop the services.");
            Console.ReadLine();

        }

        /// <summary>
        /// Check if remote file (http) is available. If yes, it loads them down to local disk.
        /// </summary>
        /// <param name="filename">Filename as URI.</param>
        private static void CheckFile( string filename )
        {
            System.Net.HttpWebRequest request = null;
            System.Net.HttpWebResponse response = null;
            string fullname = "http://www.imfeld.net/publish/configuration/" + filename;
            request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(fullname);
            request.Timeout = 30000;
            int flag = 0;
            try
            {
                response = (System.Net.HttpWebResponse)request.GetResponse();
                flag = 1;
            }
            catch 
            {
                flag = -1;
            }

            if (flag==1)
            {
                Console.WriteLine("File Found at {0}", fullname);

                WebClient webClient = new WebClient();
                webClient.DownloadFile(fullname, @"c:\" + filename);

            }
            else
            {
                Console.WriteLine("File Not Found!!!");
            }
        }

        /// <summary>
        /// Find available port for client callback.
        /// </summary>
        /// <returns>Available port number.</returns>
        internal static int FindPort()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Bind(endPoint);
                IPEndPoint local = (IPEndPoint)socket.LocalEndPoint;
                return local.Port;
            }
        }

        /// <summary>
        /// Get zone status; connects to the end point address and reads the zone status.
        /// </summary>
        /// <param name="endPointAddress">Server end point address.</param>
        /// <param name="adr">Zone Address to get the status.</param>
        private static void GetZoneState( EndpointAddress endPointAddress, Address adr )
        {
            Console.WriteLine(">>> Setup M&C server ...");

            ILog _log = LogManager.GetCurrentClassLogger();
            IMonitorAndControl pipeProxy = null;
            IMonitorAndControlNotification serverCallback = new ServerCallback();
            try
            {
                //var binding = new WSDualHttpBinding(WSDualHttpSecurityMode.None);
                var binding = new WSDualHttpBinding("WSDualHttpBinding_IMonitorAndControl");

                int port = FindPort();
                //binding.ClientBaseAddress = new Uri("http://" + machineIpOrName + ":" + port + "/");
                //binding.ClientBaseAddress = new Uri("http://" + "192.168.1.115" + ":" + port + "/");
                binding.ClientBaseAddress = new Uri("http://" + NetworkHelper.getHostName() + ":" + port + "/");

                /*note the "DuplexChannelFactory".  This is necessary for Callbacks.
                 A regular "ChannelFactory" won't work with callbacks.*/
                DuplexChannelFactory<IMonitorAndControl> pipeFactory =
                      new DuplexChannelFactory<IMonitorAndControl>(
                          new InstanceContext(serverCallback),
                          binding /*new NetTcpBinding()*/,
                          endPointAddress /*new EndpointAddress("net.tcp://localhost:8000/ISubscribe")*/);
                try
                {
                    Console.WriteLine(">>>   creating channel to {0} with callback address {1}", endPointAddress.Uri.ToString(), binding.ClientBaseAddress.ToString());

                    //Open the channel to the server
                    pipeProxy = pipeFactory.CreateChannel();
                    pipeProxy.Connect();

                    // Get zone status
                    ZoneState state = pipeProxy.GetZoneState(adr);
                    Console.WriteLine(">>>   zone state: {0}", state.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("GetZoneState - Exception: {0}", e.Message);
                    _log.Fatal(m => m("GetZoneState - Exception: {0}", e.Message));
                }

            }
            catch (FaultException<ArgumentException> exc)
            {
                Console.WriteLine("GetZoneState - FaultException: {0}", exc);
                _log.Fatal(m => m("GetZoneState - FaultException: {0}", exc));
            }
            catch (Exception exc)
            {
                Console.WriteLine("GetZoneState - Exception: {0}", exc);
                _log.Fatal(m => m("GetZoneState - Exception: {0}", exc));
            }
        }

        /// <summary>
        /// Set zone status, connects to the end point address and sets a zone status.
        /// </summary>
        /// <param name="endPointAddress">Server end point address.</param>
        /// <param name="adr">Zone Address to set the status.</param>
        /// <param name="stateCommand">Command to send to zone.</param>
        private static void SetZoneState(EndpointAddress endPointAddress, Address adr, ZoneState stateCommand)
        {
            Console.WriteLine(">>> Setup M&C server ...");

            ILog _log = LogManager.GetCurrentClassLogger();
            IMonitorAndControl pipeProxy = null;
            IMonitorAndControlNotification serverCallback = new ServerCallback();
            try
            {
                int port = FindPort();
                var binding = new WSDualHttpBinding("WSDualHttpBinding_IMonitorAndControl");
                binding.ClientBaseAddress = new Uri("http://" + NetworkHelper.getHostName() + ":" + port + "/");

                /*note the "DuplexChannelFactory".  This is necessary for Callbacks.
                 A regular "ChannelFactory" won't work with callbacks.*/
                DuplexChannelFactory<IMonitorAndControl> pipeFactory =
                      new DuplexChannelFactory<IMonitorAndControl>(
                          new InstanceContext(serverCallback), binding, endPointAddress );
                try
                {
                    Console.WriteLine(">>>   creating channel to {0} with callback address {1}", endPointAddress.Uri.ToString(), binding.ClientBaseAddress.ToString());

                    //Open the channel to the server
                    pipeProxy = pipeFactory.CreateChannel();
                    pipeProxy.Connect();

                    // Get zone status
                    ZoneState state = pipeProxy.GetZoneState(adr);
                    Console.WriteLine(">>>   zone state: {0}", state.ToString());

                    // Get zone status
                    pipeProxy.SetZoneState(adr, stateCommand);
                }
                catch (Exception e)
                {
                    Console.WriteLine("SetZoneState - Exception: {0}", e.Message);
                    _log.Fatal(m => m("SetZoneState - Exception: {0}", e.Message));
                }

            }
            catch (FaultException<ArgumentException> exc)
            {
                Console.WriteLine("SetZoneState - FaultException: {0}", exc);
                _log.Fatal(m => m("SetZoneState - FaultException: {0}", exc));
            }
            catch (Exception exc)
            {
                Console.WriteLine("SetZoneState - Exception: {0}", exc);
                _log.Fatal(m => m("SetZoneState - Exception: {0}", exc));
            }
        }
        
        /// <summary>
        /// Control with Callback, connects to the end point address and waits for notifications.
        /// See also http://www.codeproject.com/Articles/39143/C-WCF-Client-Server-without-HTTP-with-Callbacks-Ma
        /// </summary>
        /// <param name="endPointAddress">Server end point address.</param>
        /// <param name="adr">Zone Address to monitor.</param>
        private static void ControlWithCallback(EndpointAddress endPointAddress, Address adr)
        {
            Console.WriteLine(">>> Setup M&C server ...");

            ILog _log = LogManager.GetCurrentClassLogger();
            IMonitorAndControl pipeProxy = null;
            IMonitorAndControlNotification serverCallback = new ServerCallback();
            try
            {
                int port = FindPort();
                var binding = new WSDualHttpBinding("WSDualHttpBinding_IMonitorAndControl");
                binding.ClientBaseAddress = new Uri("http://" + NetworkHelper.getHostName() + ":" + port + "/");

                /*note the "DuplexChannelFactory".  This is necessary for Callbacks.
                 A regular "ChannelFactory" won't work with callbacks.*/
                DuplexChannelFactory<IMonitorAndControl> pipeFactory =
                      new DuplexChannelFactory<IMonitorAndControl>(
                          new InstanceContext(serverCallback), binding, endPointAddress);
                try
                {
                    Console.WriteLine(">>>   creating channel to {0} with callback address {1}", endPointAddress.Uri.ToString(), binding.ClientBaseAddress.ToString());

                    //Open the channel to the server
                    pipeProxy = pipeFactory.CreateChannel();
                    pipeProxy.Connect();

                    // Get zone status
                    pipeProxy.Monitor(adr);

                    // Listen to changes ...
                    int iSecTimeout = 60;
                    Console.WriteLine(">>>   Wait {0} seconds, and listen to notifications!", iSecTimeout);
                    System.Threading.Thread.Sleep(iSecTimeout * 1000);
                    Console.WriteLine(">>>   Stop listening ....");
                    pipeProxy.RemoveMonitor(adr);

                }
                catch (Exception e)
                {
                    Console.WriteLine("ControlWithCallback - Exception: {0}", e.Message);
                    _log.Fatal(m => m("ControlWithCallback - Exception: {0}", e.Message));
                }

            }
            catch (FaultException<ArgumentException> exc)
            {
                Console.WriteLine("ControlWithCallback - FaultException: {0}", exc);
                _log.Fatal(m => m("ControlWithCallback - FaultException: {0}", exc));
            }
            catch (Exception exc)
            {
                Console.WriteLine("ControlWithCallback - Exception: {0}", exc);
                _log.Fatal(m => m("ControlWithCallback - Exception: {0}", exc));
            }
        }

        /// <summary>
        /// Discover all available services.
        /// </summary>
        private static void Discover()
        {
            ILog _log = LogManager.GetCurrentClassLogger();
            try
            {

                // ------- DISCOVERY: IFunction ----------
                Discover("IFunction", typeof(IFunction), 5);

                // ------- DISCOVERY: IMonitorAndControl ----------
                Discover("IMonitorAndControl", typeof(IMonitorAndControl), 5);

                // ------- DISCOVERY: IConfigure ----------
                Discover("IConfigure", typeof(IConfigure), 5);

            }
            catch (FaultException<ArgumentException> exc)
            {
                _log.Fatal(m => m("FaultException: {0}", exc));
            }
            catch (Exception exc)
            {
                _log.Fatal(m => m("Exception: {0}", exc));
            }

        }

        /// <summary>
        /// Discover specific servcie type.
        /// </summary>
        /// <param name="identifier">String identifier. Only used in console/log output.</param>
        /// <param name="type">Service type.</param>
        /// <param name="timespan">Timeout to discover a service.</param>
        /// <returns></returns>
        private static FindResponse Discover(string identifier, Type type, int timespan)
        {
            FindResponse discovered = null;
            ILog _log = LogManager.GetCurrentClassLogger();
            try
            {
                // ------- DISCOVERY ----------
                Console.WriteLine("Start discovering {0} ...", identifier);

                DiscoveryClient discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint());
                FindCriteria criteria = new FindCriteria(type);
                criteria.Duration = TimeSpan.FromSeconds(timespan);
                discovered = discoveryClient.Find(criteria);

                Console.WriteLine("{0} Discovery: {1} services found.", identifier, discovered.Endpoints.Count);
                LogHelper.PrintEndPoints(discovered.Endpoints);
                discoveryClient.Close();
            }
            catch (FaultException<ArgumentException> exc)
            {
                _log.Fatal(m => m("FaultException: {0}", exc));
            }
            catch (Exception exc)
            {
                _log.Fatal(m => m("Exception: {0}", exc));
            }
            return discovered;
        }


        private static void GetImage(IConfigure cfgIfc, string imageName, string imageSaveToName)
        {
            ILog _log = LogManager.GetCurrentClassLogger();
            try
            {
                _log.Trace(m => m("Start getting image: {0}", imageName));
                NuvoImage img = cfgIfc.GetImage(imageName);
                if (img == null)
                {
                    _log.Error(m => m("Cannot load Image: {0}", imageName));
                }
                else
                {
                    Console.WriteLine("Image details: {0}", img.ToString());
                    _log.Trace(m => m("Image details: {0}", img.ToString()));
                    img.Picture.Save(imageSaveToName);
                    _log.Trace(m => m("Image saved to: {0}", imageSaveToName));
                }
            }
            catch (Exception exc)
            {
                _log.Fatal(m => m("Exception getting image '{0}': {1}", imageName, exc));
            }

        }


        #region PingTest

        /// <summary>
        /// List of channels, for the ping test
        /// </summary>
        private static List<IWcfPingTest> channelList = new List<IWcfPingTest>();

        /// <summary>
        /// Discovers ping test servers.
        /// </summary>
        /// <returns>Discovered endpoint(s).</returns>
        private static FindResponse WcfTestClient_DiscoverChannel()
        {
            Console.WriteLine(">>> Discover ping test server(s) ...");
            var dc = new DiscoveryClient(new UdpDiscoveryEndpoint());
            FindCriteria fc = new FindCriteria(typeof(IWcfPingTest));
            fc.Duration = TimeSpan.FromSeconds(5);
            FindResponse fr = dc.Find(fc);
            foreach (EndpointDiscoveryMetadata edm in fr.Endpoints)
            {
                Console.WriteLine(">>>   uri found = " + edm.Address.Uri.ToString());
            }
            return fr;
        }

        /// <summary>
        /// Setup channels to ping test servers.
        /// </summary>
        private static void WcfTestClient_SetupChannel()
        {
            Console.WriteLine(">>> Setup channel to ping test server(s) ...");
            var binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            var factory = new ChannelFactory<IWcfPingTest>(binding);
            FindResponse fr = WcfTestClient_DiscoverChannel();
            foreach (EndpointDiscoveryMetadata edm in fr.Endpoints)
            {
                var uri = edm.Address.Uri;
                Console.WriteLine(">>>   creating channel to " + uri.ToString());
                EndpointAddress ea = new EndpointAddress(uri);
                IWcfPingTest channel = factory.CreateChannel(ea);
                channelList.Add(channel);
                Console.WriteLine(">>>   channel created");
                //string result = channel.Ping();
                //Console.WriteLine(">>>   ping result = " + result);
            }
        }

        /// <summary>
        /// Execute ping on the ping test servers.
        /// </summary>
        private static void WcfTestClient_Ping()
        {
            Console.WriteLine(">>> Ping test server(s) ...");
            foreach (IWcfPingTest channel in channelList)
            {
                Console.WriteLine(">>>   pinging host");
                string result = channel.Ping();
                Console.WriteLine(">>>   ping result = " + result);
            }
        }

        #endregion

    }
}
