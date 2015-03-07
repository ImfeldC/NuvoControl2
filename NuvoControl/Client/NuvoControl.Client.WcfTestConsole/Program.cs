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

using NuvoControl.Common.Configuration;
using NuvoControl.Common;

using NuvoControl.Client.WcfTestConsole.ConfigurationServiceReference;
using NuvoControl.Client.WcfTestConsole.MonitorAndControlServiceReference;
using Common.Logging;

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

    public class ServerCallback : IMonitorAndControlCallback
    {
        public int _id = 1;
        #region IMonitorAndControlCallback Members

        public void OnZoneStateChanged(Address zoneId, ZoneState zoneState)
        {
            Console.WriteLine("Notification from server zone: {0}", zoneId.ToString());
        }

        #endregion
    }

    class Program
    {
        static void Main(string[] args)
        {
            ILog _log = LogManager.GetCurrentClassLogger();

            Console.WriteLine("**** Console client started. *******");
            _log.Debug(m => m("**** Console client started. *******"));

            Console.WriteLine(">>> Starting WCF Test Client  --- Assembly Version={0} / Deployment Version={1} (using .NET 4.0) ... ",
                AppInfoHelper.getAssemblyVersion(), AppInfoHelper.getDeploymentVersion());
            Console.WriteLine();

            CheckFile("NuvoControlKonfiguration.xml");

            //Discover();

            //Control();

            Console.WriteLine(">>> Press <Enter> to stop the services.");
            Console.ReadLine();

        }


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

        private static void Control()
        {
            ILog _log = LogManager.GetCurrentClassLogger();
            try
            {
                int iSecTimeout = 60;
                IMonitorAndControlCallback serverCallback = new ServerCallback();
                //IMonitorAndControlCallback serverCallback = null;
                MonitorAndControlClient mcProxy = new MonitorAndControlClient(new InstanceContext(serverCallback));
                Console.WriteLine("Created Service at {0}", mcProxy.Endpoint.Address);

                // Connect to the discovered service endpoint
                //mcProxy.SetClientBaseAddress();
                mcProxy.Endpoint.Address = new EndpointAddress("http://imfeldc.dyndns.org:" + 8080 + "/MonitorAndControlService");
                Console.WriteLine("Invoking Service at {0}", mcProxy.Endpoint.Address);
                                
                mcProxy.Connect();
                mcProxy.Monitor(new Address(100, 1));
                Console.WriteLine("Wait {0} seconds, and listen to notifications!", iSecTimeout);
                System.Threading.Thread.Sleep(iSecTimeout * 1000);
                Console.WriteLine("Stop listening ....");
                mcProxy.RemoveMonitor(new Address(100, 1));
            }
            catch (FaultException<ArgumentException> exc)
            {
                Console.WriteLine("FaultException: {0}", exc);
                _log.Fatal(m => m("FaultException: {0}", exc));
            }
            catch (Exception exc)
            {
                Console.WriteLine("Exception: {0}", exc);
                _log.Fatal(m => m("Exception: {0}", exc));
            }
        }

        private static void ControlWithCallback()
        {
            ILog _log = LogManager.GetCurrentClassLogger();
            try
            {
                int iSecTimeout = 60;
                IMonitorAndControlCallback serverCallback = new ServerCallback();
                MonitorAndControlClient mcProxy = new MonitorAndControlClient(new InstanceContext(serverCallback));
                mcProxy.SetClientBaseAddress();
                mcProxy.Connect();
                mcProxy.Monitor(new Address(100, 1));
                Console.WriteLine("Wait {0} seconds, and listen to notifications!", iSecTimeout);
                System.Threading.Thread.Sleep(iSecTimeout * 1000);
                Console.WriteLine("Stop listening ....");
                mcProxy.RemoveMonitor(new Address(100, 1));
            }
            catch (FaultException<ArgumentException> exc)
            {
                _log.Fatal(m => m("FaultException: {0}", exc));
            }
            catch (Exception exc)
            {
                Console.WriteLine("Exception: {0}", exc);
                _log.Fatal(m => m("Exception: {0}", exc));
            }
        }

        private static void Discover()
        {
            ConfigureClient cfgIfc = null;
            //IConfigure cfgIfc = null;

            ILog _log = LogManager.GetCurrentClassLogger();
            try
            {

                // ------- DISCOVERY ----------

                Console.WriteLine("Start discovering ...");

                DiscoveryClient discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint());
                FindCriteria criteria = new FindCriteria(typeof(IConfigure));
                FindResponse discovered = discoveryClient.Find(criteria);
                discoveryClient.Close();

                Console.WriteLine("Discovery: {0} services found.", discovered.Endpoints.Count);
                LogHelper.PrintEndPoints(discovered.Endpoints);

                // ----------------------------

                if (discovered.Endpoints.Count > 0)
                {
                    int iZoneId = 4;

                    cfgIfc = new ConfigureClient();
                    // Connect to the discovered service endpoint
                    cfgIfc.Endpoint.Address = discovered.Endpoints[0].Address;
                    Console.WriteLine("Invoking Service at {0}", discovered.Endpoints[0].Address);


                    Console.WriteLine("Server: ListenUri={0}", cfgIfc.Endpoint.ListenUri);

                    Console.WriteLine("Read zone configuration for zone with id {0}.", iZoneId);
                    Zone zone = cfgIfc.GetZoneKonfiguration(new Address(100, iZoneId));

                    Console.WriteLine("Zone name: {0}", zone.Name);
                    Console.WriteLine("Picture type: {0}", zone.PictureType);
                    Console.WriteLine("All zone details: {0}", zone.ToString());

                    //Graphic graphic = cfgIfc.GetGraphicConfiguration();
                    //Console.WriteLine("All graphic details: {0}", graphic.ToString());

                    //GetImage(cfgIfc, graphic.Building.PicturePath, "c:\\temp\\temp.jpg");
                    //GetImage(cfgIfc, ".\\Images\\Funk.jpg", "c:\\temp\\Funk.jpg");
                    //GetImage(cfgIfc, ".\\Images\\Building.png", "c:\\temp\\Building.png");
                    //GetImage(cfgIfc, ".\\Images\\Galerie.bmp", "c:\\temp\\Galerie.bmp");
                    //GetImage(cfgIfc, ".\\Images\\Galerie-Original.bmp", "c:\\temp\\Galerie-Original.bmp");
                    //GetImage(cfgIfc, ".\\Images\\Hasenzimmer.jpg", "c:\\temp\\Hasenzimmer.jpg");

                    //cfgIfc.Close();
                }
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

    }
}
