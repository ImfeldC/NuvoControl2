/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
 ***************************************************************************************************
 *
 *   Project:        NuvoControl
 *   SubProject:     NuvoControl.Client.Viewer
 *   Author:         Bernhard Limacher based on code from Juval Löwy, Author of Programming WCF
 *   Creation Date:  12.07.2009
 *   File Name:      WsDualProxyHelper.cs
 * 
 ***************************************************************************************************
 * 
 * 
 **************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;

namespace NuvoControl.Client.ServiceAccess
{
    public static class WsDualProxyHelper
    {
        public static void SetClientBaseAddress<T>(this DuplexClientBase<T> proxy, int port, string machineIpOrName) where T : class
        {
            WSDualHttpBinding binding = proxy.Endpoint.Binding as WSDualHttpBinding;
            Debug.Assert(binding != null);
            binding.ClientBaseAddress = new Uri("http://" + machineIpOrName  + ":" + port + "/");
        }

        public static void SetClientBaseAddress<T>(this DuplexClientBase<T> proxy, int port) where T : class
        {
            SetClientBaseAddress(proxy, port, "localhost");
        }

        public static void SetClientBaseAddress<T>(this DuplexClientBase<T> proxy, string machineIpOrName) where T : class
        {
            lock (typeof(WsDualProxyHelper))
            {
                int portNumber = FindPort();
                SetClientBaseAddress(proxy, portNumber, machineIpOrName);
                proxy.Open();
            }
        }

        public static void SetClientBaseAddress<T>(this DuplexClientBase<T> proxy) where T : class
        {
            SetClientBaseAddress(proxy, "localhost");
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
}

/**************************************************************************************************
 * 
 *   Copyright (C) B. Limacher, C. Imfeld. All Rights Reserved. Confidential
 * 
**************************************************************************************************/