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
            Console.WriteLine("**** Console client started. *******");

            ConfigureClient cfgIfc = null;
            try
            {
                cfgIfc = new ConfigureClient();
                Zone zone = cfgIfc.GetZoneKonfiguration(new Address(100, 4));

                Console.WriteLine("Zone name: {0}", zone.Name);
                Console.WriteLine("Picture type: {0}", zone.PictureType);
                
                Graphic graphic = cfgIfc.GetGraphicConfiguration();

                cfgIfc.Close();
            }
            catch (FaultException<ArgumentException>)
            {
            }
            catch (Exception)
            {
                Graphic graphic = cfgIfc.GetGraphicConfiguration();
                cfgIfc.Abort();
            }

            IMonitorAndControlCallback serverCallback = new ServerCallback();
            MonitorAndControlClient mcProxy = new MonitorAndControlClient(new InstanceContext(serverCallback));
            mcProxy.SetClientBaseAddress();
            mcProxy.Connect();
            mcProxy.Monitor(new Address(100, 1));
            System.Threading.Thread.Sleep(10000);
            mcProxy.RemoveMonitor(new Address(100, 1));

            Console.WriteLine(">>> Press <Enter> to stop the services.");
            Console.ReadLine();

        }
    }
}
