using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;

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
            binding.ClientBaseAddress = new Uri("http://localhost:" + port + "/");
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

            //ConfigureClient cfgIfc = null;
            IConfigure cfgIfc = null;
            try
            {
                int iZoneId = 4;

                cfgIfc = new ConfigureClient();

                Console.WriteLine("Read zone configuration for zone with id {0}.", iZoneId);
                Zone zone = cfgIfc.GetZoneKonfiguration(new Address(100, iZoneId));

                Console.WriteLine("Zone name: {0}", zone.Name);
                Console.WriteLine("Picture type: {0}", zone.PictureType);
                Console.WriteLine("All zone details: {0}", zone.ToString());
                
                Graphic graphic = cfgIfc.GetGraphicConfiguration();
                Console.WriteLine("All graphic details: {0}", graphic.ToString());

                GetImage(cfgIfc, graphic.Building.PicturePath, "c:\\temp\\temp.jpg");
                GetImage(cfgIfc, ".\\Images\\Funk.jpg", "c:\\temp\\Funk.jpg");
                GetImage(cfgIfc, ".\\Images\\Building.png", "c:\\temp\\Building.png");
                GetImage(cfgIfc, ".\\Images\\Galerie.bmp", "c:\\temp\\Galerie.bmp");
                GetImage(cfgIfc, ".\\Images\\Galerie-Original.bmp", "c:\\temp\\Galerie-Original.bmp");

                //cfgIfc.Close();
            }
            catch (FaultException<ArgumentException> exc)
            {
                _log.Fatal(m => m("FaultException: {0}", exc));
            }
            catch (Exception exc)
            {
                _log.Fatal(m => m("Exception: {0}", exc));
            }

            try
            {
                int iSecTimeout = 60;
                IMonitorAndControlCallback serverCallback = new ServerCallback();
                MonitorAndControlClient mcProxy = new MonitorAndControlClient(new InstanceContext(serverCallback));
                mcProxy.SetClientBaseAddress();
                mcProxy.Connect();
                mcProxy.Monitor(new Address(100, 1));
                Console.WriteLine("Wait {0} seconds, and listen to notifications!", iSecTimeout);
                System.Threading.Thread.Sleep(iSecTimeout*1000);
                Console.WriteLine("Stop listening ....");
                mcProxy.RemoveMonitor(new Address(100, 1));
            }
            catch (FaultException<ArgumentException> exc)
            {
                _log.Fatal(m => m("FaultException: {0}", exc));
            }
            catch (Exception exc)
            {
                _log.Fatal(m => m("Exception: {0}", exc));
            }

            Console.WriteLine(">>> Press <Enter> to stop the services.");
            Console.ReadLine();

        }

        private static void GetImage(IConfigure cfgIfc, string imageName, string imageSaveToName)
        {
            ILog _log = LogManager.GetCurrentClassLogger();
            try
            {
                _log.Trace(m => m("Start getting image: {0}", imageName));
                NuvoImage img = cfgIfc.GetImage(imageName);
                Console.WriteLine("Image details: {0}", img.ToString());
                _log.Trace(m => m("Image details: {0}", img.ToString()));
                img.Picture.Save(imageSaveToName);
                _log.Trace(m => m("Image saved to: {0}", imageSaveToName));
            }
            catch (Exception exc)
            {
                _log.Fatal(m => m("Exception getting image '{0}': {1}", imageName, exc));
            }

        }

    }
}
